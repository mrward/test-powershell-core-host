//
// Program.cs
//
// Author:
//       Matt Ward <matt.ward@microsoft.com>
//
// Copyright (c) 2018 Microsoft
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
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace PowerShellHostTest
{
	class Program
	{
		static PowerShellServer server;
		static Dte dte = new Dte ();

		static int Main (string[] args)
		{
			try {
				RunJsonRpc ();
				//RunInteractive ();
				//RunWithCreatedRunspace ();
				//Run ();
				return 0;
			} catch (Exception ex) {
				Logger.Log ("Error: {0}", ex);
				return -1;
			}
		}

		static void RunJsonRpc ()
		{
			Logger.Clear ();
			try {
				server = new PowerShellServer (Console.OpenStandardOutput (), Console.OpenStandardInput ());
				server.Run ();
			} catch (Exception ex) {
				Logger.Log ("PowerShellServer error: {0}", ex.Message);
				throw;
			}
		}

		static int RunInteractive ()
		{
			var host = new TestHost ();
			var initialSessionState = PowerShellServer.CreateInitialSessionState (dte);
			var runspace = RunspaceFactory.CreateRunspace (host, initialSessionState);
			runspace.Open ();

			while (true) {
				Console.Write ("PS> ");
				string line = Console.ReadLine ();

				if (line == "exit")
					break;

				using (var pipeline = PowerShellServer.CreatePipeline (runspace, line)) {
					pipeline.Invoke ();
				}
			}

			return 0;
		}

		static int RunWithCreatedRunspace ()
		{
			var host = new TestHost ();
			var initialSessionState = PowerShellServer.CreateInitialSessionState (dte);
			var runspace = RunspaceFactory.CreateRunspace (host, initialSessionState);
			runspace.Open ();

			var pipeline = PowerShellServer.CreatePipeline (runspace, "get-verb");
			pipeline.Invoke ();

			return 0;
		}

		static int Run ()
		{
			var state = PowerShellServer.CreateInitialSessionState (dte);
			using (PowerShell ps = PowerShell.Create (state)) {
				Console.WriteLine ("\nEvaluating 'Get-Command Write-Output' in PS Core Runspace\n");
				var results = ps.AddScript ("Get-Command Write-Output").Invoke ();
				Console.WriteLine (results[0].ToString ());

				Console.WriteLine ("\nTab expansion test");
				string line = "Get-";
				string script = "$__pc_args=@(); $input|%{$__pc_args+=$_}; (TabExpansion2 $__pc_args[0] $__pc_args[0].length).CompletionMatches | % {$_.CompletionText}; Remove-Variable __pc_args -Scope 0;";
				results = ps.AddScript (script)
					.Invoke (new object[] { line, line.Length });
				ps.Commands.Clear ();

				Console.WriteLine ();
				Console.WriteLine (results.Count);
				foreach (var result in results) {
					Console.WriteLine (result);
				}

				Console.WriteLine ("\nDTE evaluation");
				results = ps.AddScript ("$dte")
					.Invoke ();
				foreach (var result in results) {
					Console.WriteLine (result);
				}
			}
			return 0;
		}
	}
}
