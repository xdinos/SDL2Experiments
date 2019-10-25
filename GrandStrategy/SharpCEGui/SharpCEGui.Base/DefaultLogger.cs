#region Copyright
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Default implementation for the Logger class.
    /// If you want to redirect CEGUI logs to some place other than a text file,
    /// implement your own Logger implementation and create a object of the
    /// Logger type before creating the CEGUI::System singleton.
    /// </summary>
    public sealed class DefaultLogger : Logger
    {
        /// <summary>
        /// 
        /// </summary>
        public DefaultLogger()
        {
            // create log header
            LogEvent("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
            LogEvent("+                     Crazy Eddie's GUI System - Event log                    +");
            LogEvent("+                          (http://www.cegui.org.uk/)                         +");
            LogEvent("+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+\n");
            LogEvent(String.Format("CEGUI::Logger singleton created. 0x{0:X8}", GetHashCode()));
        }

        // TODO: ~DefaultLogger()
        //{
        //    if (d_ostream.is_open())
        //    {
        //        char addr_buff[32];
        //        sprintf(addr_buff, "(%p)", static_cast<void*>(this));
        //        logEvent("CEGUI::Logger singleton destroyed. " + String(addr_buff));
        //        d_ostream.close();
        //    }
        //}

        #region Overrides of Logger

        public override void LogEvent(string message, LoggingLevel level = LoggingLevel.Standard)
        {
            // clear sting stream
            d_workstream.Clear();

            var dateTime = DateTime.Now;

            // write date
            d_workstream.AppendFormat("{0:dd/MM/yyyy} ", dateTime);

            // write time
            d_workstream.AppendFormat("{0:HH:mm:ss} ", dateTime);

            // write event type code
            switch (level)
            {
                case LoggingLevel.Errors:
                    d_workstream.Append("(Error)\t");
                    break;

                case LoggingLevel.Warnings:
                    d_workstream.Append("(Warn) \t");
                    break;

                case LoggingLevel.Standard:
                    d_workstream.Append("(Std)  \t");
                    break;

                case LoggingLevel.Informative:
                    d_workstream.Append("(Info) \t");
                    break;

                case LoggingLevel.Insane:
                    d_workstream.Append("(Insan)\t");
                    break;

                default:
                    d_workstream.Append("(Unkwn)\t");
                    break;
            }

            d_workstream.AppendLine(message);

            if (d_caching)
            {
                d_cache.Add(new Tuple<string, LoggingLevel>(d_workstream.ToString(), level));
            }
            else if (d_level >= level)
            {
                // write message
                d_ostream.Write(d_workstream.ToString());
                // ensure new event is written to the file, rather than just being
                // buffered.
                d_ostream.Flush();
            }
        }

        public override void SetLogFilename(string filename, bool append = false)
        {
            // close current log file (if any)
            if (d_ostream!=null)
                d_ostream.Close();


            d_ostream = new StreamWriter(filename, append, Encoding.UTF8);
            //if (!d_ostream)
            //    CEGUI_THROW(FileIOException(
            //        "Failed to open file '" + filename + "' for writing"));

            // initialise width for date & time alignment.
            //d_ostream.width(2);

            // write out cached log strings.
            if (d_caching)
            {
                d_caching = false;

                foreach (var item in d_cache)
                {
                    if (d_level>=item.Item2)
                    {
                        // write message
                        d_ostream.Write(item.Item1);
                        // ensure new event is written to the file, rather than just
                        // being buffered.
                        d_ostream.Flush();
                    }
                }

                d_cache.Clear();
            }
        }

        #endregion

        /// <summary>
        /// Stream used to implement the logger
        /// </summary>
        private StreamWriter d_ostream;

        //! Used to build log entry strings. 
        private StringBuilder d_workstream = new StringBuilder();

        /// <summary>
        /// Used to cache log entries before log file is created. 
        /// </summary>
        private readonly List<Tuple<string, LoggingLevel>>  d_cache = new List<Tuple<string, LoggingLevel>>();

        /// <summary>
        /// true while log entries are beign cached (prior to logfile creation)
        /// </summary>
        private bool d_caching = true;
    }
}