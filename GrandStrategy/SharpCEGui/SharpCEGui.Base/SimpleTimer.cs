﻿#region Copyright
// Copyright (C) 2004 - 2013 Paul D Turner & The CEGUI Development Team
// 
// C# Port developed by The SharpCEGui Development Team
// Copyright (C) 2012 - 2013
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System.Diagnostics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Simple timer class.
    /// </summary>
    public class SimpleTimer
    {
        private double d_baseTime;
        private readonly global::System.Diagnostics.Stopwatch _stopwatch = new Stopwatch();

        //! returns time in seconds
        public static double CurrentTime()
        {
            return global::System.Diagnostics.Stopwatch.GetTimestamp()/1000.0;
            //return DateTime.Now.Ticks/1000.0;
        }

        public SimpleTimer()
        {
            d_baseTime = CurrentTime();
        }

        public void Restart()
        {
            //d_baseTime = CurrentTime();
            _stopwatch.Restart();
        }
        public double Elapsed()
        {
            return _stopwatch.Elapsed.Milliseconds/1000.0;
            //return CurrentTime() - d_baseTime;
        }
    };
}