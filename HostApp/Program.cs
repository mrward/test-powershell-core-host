//
// Program.cs
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
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using PowerShellHostTest;
using StreamJsonRpc;

namespace HostApp
{
	class MainClass
	{
		static Server server;
		static JsonRpc jsonRpc;

		public static void Main (string[] args)
		{
			try {
				Run ();
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}
		}

		static void Run ()
		{
			Process process = StartPowerShellHost ();

			server = new Server ();
			jsonRpc = new JsonRpc (process.StandardInput.BaseStream, process.StandardOutput.BaseStream, server);
			jsonRpc.Disconnected += JsonRpc_Disconnected;
			jsonRpc.StartListening ();
			jsonRpc.JsonSerializer.NullValueHandling = NullValueHandling.Ignore;

			while (true) {
				Console.Write ("PS> ");
				string line = Console.ReadLine ();
				try {
					jsonRpc.InvokeAsync (Methods.Invoke, line).Wait ();
				} catch (Exception ex) {
					ex = ex.GetBaseException ();
					Console.WriteLine (ex.Message);
				}
			}
		}

		static Process StartPowerShellHost ()
		{
			var info = new ProcessStartInfo ();
			var programPath = Path.Combine (Path.GetDirectoryName (typeof (MainClass).Assembly.Location), "..", "PowerShellHost", "bin", "Debug", "netcoreapp2.1", "PowerShellHostTest.dll");
			programPath = Path.GetFullPath (programPath);
			info.FileName = "dotnet";
			info.Arguments = programPath;
			info.WorkingDirectory = Path.GetDirectoryName (programPath);
			info.UseShellExecute = false;
			info.RedirectStandardInput = true;
			info.RedirectStandardOutput = true;

			var process = new Process ();
			process.StartInfo = info;
			process.Start ();
			process.Exited += Process_Exited;

			return process;
		}

		static void JsonRpc_Disconnected (object sender, JsonRpcDisconnectedEventArgs e)
		{
			Console.WriteLine ("Disconnected {0} {1}", e.Reason, e.Description);
		}

		static void Process_Exited (object sender, EventArgs e)
		{
			Console.WriteLine ("PowerShell host exited");
		}
	}
}
