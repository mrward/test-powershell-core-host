﻿//
// PowerShellServer.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2019 Microsoft
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace PowerShellHostTest
{
	class PowerShellServer
	{
		JsonRpc rpc;
		Runspace runspace;
		TestHost host;
		Dte dte = new Dte ();

		internal PowerShellServer (Stream sender, Stream reader)
		{
			Logger.Log ("PowerShellServer starting...");

			host = new TestHost ();
			host.Server = this;

			var initialSessionState = CreateInitialSessionState ();
			runspace = RunspaceFactory.CreateRunspace (host, initialSessionState);
			runspace.Open ();

			rpc = JsonRpc.Attach (sender, reader, this);
			rpc.Disconnected += OnRpcDisconnected;
		}

		public void Run ()
		{
			Logger.Log ("PowerShellServer running...");
			rpc.Completion.Wait ();
		}

		void OnRpcDisconnected (object sender, JsonRpcDisconnectedEventArgs e)
		{
			Logger.Log ("PowerShellServer disconnected: {0}", e.Description);
		}

		[JsonRpcMethod (Methods.Invoke)]
		public void InvokeTest (string line)
		{
			Logger.Log ("PowerShellServer.Invoke: {0}", line);
			try {
				//string line = "get-alias";
				using (var pipeline = CreatePipeline (runspace, line)) {
					pipeline.Invoke ();
				}
			} catch (Exception ex) {
				Logger.Log ("PowerShellServer.Invoke error: {0}", ex);
			}
		}

		InitialSessionState CreateInitialSessionState ()
		{
			var initialSessionState = InitialSessionState.CreateDefault ();
			SessionStateVariableEntry variable = CreateDTESessionVariable ();
			initialSessionState.Variables.Add (variable);
			return initialSessionState;
		}

		SessionStateVariableEntry CreateDTESessionVariable ()
		{
			var options = ScopedItemOptions.AllScope | ScopedItemOptions.Constant;
			return new SessionStateVariableEntry ("DTE", dte, "SharpDevelop DTE object", options);
		}

		static Pipeline CreatePipeline (Runspace runspace, string command)
		{
			Pipeline pipeline = runspace.CreatePipeline ();
			pipeline.Commands.AddScript (command, false);
			pipeline.Commands.Add ("out-default");
			pipeline.Commands[0].MergeMyResults (PipelineResultTypes.Error, PipelineResultTypes.Output);
			return pipeline;
		}

		public void Log (string value)
		{
			var task = rpc.NotifyAsync (Methods.LogMessage, value);
			Ignore (task);
		}

		static void Ignore (Task task)
		{
			task.ContinueWith (t => {
				if (t.IsFaulted)
					Logger.Log ("Async operation failed {0}", t.Exception);
			});
		}
	}
}
