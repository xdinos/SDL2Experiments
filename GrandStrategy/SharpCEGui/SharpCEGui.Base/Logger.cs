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
using System.Diagnostics;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Enumeration of logging levels
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// Only actual error conditions will be logged.
        /// </summary>
        Errors,

        /// <summary>
        /// Warnings will be logged as well.
        /// </summary>
        Warnings,

        /// <summary>
        /// Basic events will be logged (default level).
        /// </summary>
        Standard,

        /// <summary>
        /// Useful tracing (object creations etc) information will be logged.
        /// </summary>
        Informative,

        /// <summary>
        /// Mostly everything gets logged (use for heavy tracing only, log WILL be big).
        /// </summary>
        Insane
    }

    /// <summary>
    /// Abstract class that defines the interface of a logger object for the GUI system.
    /// The default implementation of this interface is the DefaultLogger class; if you
    /// want to perform special logging, derive your own class from Logger and initialize
    /// an object of that type before you create the CEGUI::System singleton.
    /// </summary>
    public abstract class Logger : IDisposable
    {
        public static Logger Instance { get; private set; }

        /// <summary>
        /// Constructor for Logger object.
        /// </summary>
        protected Logger()
        {
            if (Instance == null)
                Instance = this;
        }

        #region Implementation of IDisposable

        ~Logger()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        #endregion

        /// <summary>
        /// Set the level of logging information that will get out to the log file
        /// </summary>
        /// <param name="value">
        /// One of the LoggingLevel enumerated values that specified the level of logging information required.
        /// </param>
        public void SetLoggingLevel(LoggingLevel value)
        {
            d_level = value;
        }

        /// <summary>
        /// return the current logging level setting
        /// </summary>
        /// <returns>
        /// One of the LoggingLevel enumerated values specifying the current level of logging
        /// </returns>
        public LoggingLevel GetLoggingLevel()
        {
            return d_level;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        [Conditional("DEBUG")]
        public static void LogInsane(string message)
        {
            System.GetSingleton().Logger.LogEvent(message, LoggingLevel.Insane);
        }

        /// <summary>
        /// Add an event to the log.
        /// </summary>
        /// <param name="message">
        /// String object containing the message to be added to the event log.
        /// </param>
        /// <param name="level">
        /// LoggingLevel for this message.  
        /// If \a level is greater than the current set logging level, the message is not logged.
        /// </param>
        public abstract void LogEvent(string message, LoggingLevel level = LoggingLevel.Standard);

        /// <summary>
        /// Set the name of the log file where all subsequent log entries should be written.
        /// The interpretation of file name may differ depending on the concrete logger
        /// implementation.
        /// </summary>
        /// <remarks>
        /// When this is called, and the log file is created, any cached log entries are
        /// flushed to the log file.
        /// </remarks>
        /// <param name="filename">
        /// Name of the file to put log messages.
        /// </param>
        /// <param name="append">
        /// - true if events should be added to the end of the current file.
        /// - false if the current contents of the file should be discarded.
        /// </param>
        public abstract void SetLogFilename(string filename, bool append = false);

        /// <summary>
        /// Holds current logging level.
        /// </summary>
        protected LoggingLevel d_level = LoggingLevel.Standard;
    }
}