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
using Messages;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;

namespace HostApp
{
	public class Server
	{
		[JsonRpcMethod (Methods.LogMessage)]
		public void OnLogMessage (JToken arg)
		{
			try {
				var logMessage = arg.ToObject<LogMessage> ();
				switch (logMessage.Level) {
					case LogLevel.Error:
						WriteError (logMessage.Message);
						break;
					case LogLevel.Warning:
						WriteWarning (logMessage.Message);
						break;
					case LogLevel.Verbose:
						WriteVerbose (logMessage.Message);
						break;
					case LogLevel.Debug:
						WriteDebug (logMessage.Message);
						break;
					default:
						Console.WriteLine (logMessage.Message);
						break;
				}
			} catch (Exception ex) {
				Console.WriteLine ("OnLogMessage error: {0}", ex);
			}
		}

		void WriteError (string message)
		{
			WriteLine ("91", message);
		}

		void WriteWarning (string message)
		{
			WriteLine ("93", message);
		}

		void WriteDebug (string message)
		{
			WriteLine ("94", message);
		}

		void WriteVerbose (string message)
		{
			WriteLine ("96", message);
		}

		void WriteLine (string code, string message)
		{
			Console.Write ("\x1B[{0}m", code);
			Console.Write (message);
			Console.WriteLine ("\x1B[0m");
		}
	}
}