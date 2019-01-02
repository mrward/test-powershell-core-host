﻿//
// TestUIHost.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using Messages;

namespace PowerShellHostTest
{
	public class TestUIHost : PSHostUserInterface
	{
		TestRawUI rawUI = new TestRawUI ();

		public TestUIHost ()
		{
		}

		public bool UseConsole {
			get { return Server == null; }
		}

		internal PowerShellServer Server { get; set; }

		public override PSHostRawUserInterface RawUI => rawUI;

		public override Dictionary<string, PSObject> Prompt (string caption, string message, Collection<FieldDescription> descriptions)
		{
			throw new NotImplementedException ();
		}

		public override int PromptForChoice (string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
		{
			throw new NotImplementedException ();
		}

		public override PSCredential PromptForCredential (string caption, string message, string userName, string targetName)
		{
			throw new NotImplementedException ();
		}

		public override PSCredential PromptForCredential (string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
		{
			throw new NotImplementedException ();
		}

		public override string ReadLine ()
		{
			return null;
		}

		public override SecureString ReadLineAsSecureString ()
		{
			return null;
		}

		public override void Write (ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
		{
			Write (value);
		}

		public override void Write (string value)
		{
			if (UseConsole) {
				Console.Write (value);
			} else {
				Server.Log (LogLevel.Info, value);
			}
		}

		public override void WriteDebugLine (string message)
		{
			if (UseConsole) {
				Console.WriteLine ("DEBUG: {0}", message);
			} else {
				Server.Log (LogLevel.Debug, message);
			}
		}

		public override void WriteErrorLine (string value)
		{
			if (UseConsole) {
				Console.WriteLine ("ERROR: {0}", value);
			} else {
				Server.Log (LogLevel.Error, value);
			}
		}

		public override void WriteLine (string value)
		{
			if (UseConsole) {
				Console.WriteLine (value);
			} else {
				Server.Log (LogLevel.Info, value);
			}
		}

		public override void WriteProgress (long sourceId, ProgressRecord record)
		{
		}

		public override void WriteVerboseLine (string message)
		{
			if (UseConsole) {
				Console.WriteLine ("VERBOSE: {0}", message);
			} else {
				Server.Log (LogLevel.Verbose, message);
			}
		}

		public override void WriteWarningLine (string message)
		{
			if (UseConsole) {
				Console.WriteLine ("WARNING: {0}", message);
			} else {
				Server.Log (LogLevel.Warning, message);
			}
		}
	}
}
