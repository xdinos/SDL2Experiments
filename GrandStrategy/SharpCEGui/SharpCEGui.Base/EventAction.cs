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

namespace SharpCEGui.Base
{
    public class EventAction
    {
        public EventAction(string event_name, ChildEventAction action)
        {
            throw new NotImplementedException();
        }

        // TODO: ~EventAction();

        public void SetEventName(string event_name)
        {
            throw new NotImplementedException();
        }
        
        public string GetEventName()
        {
            throw new NotImplementedException();
        }

        public void SetAction(ChildEventAction action)
        {
            throw new NotImplementedException();
        }

        public ChildEventAction GetAction()
        {
            throw new NotImplementedException();
        }

        public void InitialiseWidget(Window widget)
        {
            throw new NotImplementedException();
        }

        public void CleanupWidget(Window widget)
        {
            throw new NotImplementedException();
        }

        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        protected string d_eventName;
        protected ChildEventAction d_action;

        protected string MakeConnectionKeyName(Window widget)
        {
            throw new NotImplementedException();
        }

        //typedef std::multimap<String, Event::ScopedConnection> ConnectionMap;
        //mutable ConnectionMap d_connections;
    }
}