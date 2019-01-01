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
using System.Management.Automation.Host;

namespace PowerShellHostTest
{
	public class TestRawUI : PSHostRawUserInterface
	{
		public const int MinimumColumns = 80;
		public static readonly ConsoleColor NoConsoleColor = (ConsoleColor)(-1);

		public override Size BufferSize {
			get {
				int columns = MinimumColumns;
				return new Size (columns, 0);
			}
			set { throw new NotImplementedException (); }
		}

		public override Coordinates CursorPosition { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }
		public override int CursorSize { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }

		public override ConsoleColor ForegroundColor {
			get { return NoConsoleColor; }
			set { }
		}

		public override ConsoleColor BackgroundColor {
			get { return NoConsoleColor; }
			set { }
		}

		public override bool KeyAvailable => throw new NotImplementedException ();

		public override Size MaxPhysicalWindowSize => throw new NotImplementedException ();

		public override Size MaxWindowSize => throw new NotImplementedException ();

		public override Coordinates WindowPosition { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }
		public override Size WindowSize { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }

		public override string WindowTitle { get; set; }

		public override void FlushInputBuffer ()
		{
			throw new NotImplementedException ();
		}

		public override BufferCell[,] GetBufferContents (Rectangle rectangle)
		{
			throw new NotImplementedException ();
		}

		public override KeyInfo ReadKey (ReadKeyOptions options)
		{
			throw new NotImplementedException ();
		}

		public override void ScrollBufferContents (Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
		{
			throw new NotImplementedException ();
		}

		public override void SetBufferContents (Coordinates origin, BufferCell[,] contents)
		{
			throw new NotImplementedException ();
		}

		public override void SetBufferContents (Rectangle rectangle, BufferCell fill)
		{
			throw new NotImplementedException ();
		}
	}
}