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

namespace SharpCEGui.Base
{
    public class EventLinkDefinition
    {
        public EventLinkDefinition(string event_name)
        {
            throw new NotImplementedException();
        }

        // TODO: ~EventLinkDefinition();

        //! add a new link target to \a event on \a widget (name).
        public void AddLinkTarget(string widget, string @event)
        {
            throw new NotImplementedException();
        }

        //! clear all link targets from this link definition.
        public void ClearLinkTargets()
        {
            throw new NotImplementedException();
        }

        //! initialise \a window with an event link as specified here.
        public void InitialiseWidget(Window window)
        {
            throw new NotImplementedException();
        }

        //! clean this event from \a window.
        public void CleanUpWidget(Window window)
        {
            throw new NotImplementedException();
        }


        public void SetName(string name)
        {
            throw new NotImplementedException();
        }

        //! return the name of the Event defined here.
        public string GetName()
        {
            throw new NotImplementedException();
        }

        //! Return a pointer to the target window with the given name.
        protected Window GetTargetWindow(Window start_wnd, string name)
        {
            throw new NotImplementedException();
        }

        //! String holding the name of the event being defined
        protected string d_eventName;

        //typedef std::pair<String,String> StringPair;
        ////! type used for the collection of target events.
        //typedef std::vector<StringPair CEGUI_VECTOR_ALLOC(StringPair)> LinkTargetCollection;

        //! collection of targets for this EventLinkDefinition.
        //LinkTargetCollection d_targets;
        protected List<Tuple<string, string>> d_targets;
        //public:
        //typedef ConstVectorIterator<LinkTargetCollection> LinkTargetIterator;
        //LinkTargetIterator getLinkTargetIterator() const;
    }
}