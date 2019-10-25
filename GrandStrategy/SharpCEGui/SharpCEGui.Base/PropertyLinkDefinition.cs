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
    /// <summary>
    /// Class representing a property that links to another property defined on
    /// an attached child widget.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyLinkDefinition<T> : FalagardPropertyBase<T>
    {
        public PropertyLinkDefinition(string propertyName, string widgetName,
                                      string targetProperty, string initialValue,
                                      string origin,
                                      bool redrawOnWrite, bool layoutOnWrite,
                                      string fireEvent, string eventNamespace) :
            base(propertyName,
                                               "Falagard property link definition - links a " +
                                               "property on this window to properties " +
                                               "defined on one or more child windows, or " +
                                               "the parent window.",
                                               initialValue, origin,
                                               redrawOnWrite, layoutOnWrite,
                                               fireEvent, eventNamespace)
        {
            // add initial target if it was specified via constructor
            // (typically meaning it came via XML attributes)
            if (!String.IsNullOrEmpty(widgetName) || !String.IsNullOrEmpty(targetProperty))
                AddLinkTarget(widgetName, targetProperty);
        }

        // TODO: ~PropertyLinkDefinition() {}

        /// <summary>
        /// add a new link target to \a property on \a widget (name).
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="property"></param>
        public void AddLinkTarget(string widget, string property)
        {
            d_targets.Add(new Tuple<string, string>(widget, property));
        }

        //! clear all link targets from this link definition.
        public void ClearLinkTargets()
        {
            d_targets.Clear();
        }

        //------------------------------------------------------------------------//
        // return whether a the given widget / property pair is a target of this
        // property link.
        public bool IsTargetProperty(string widget, string property)
        {
            foreach (var i in d_targets)
            {
                if (property == i.Item2 && widget == i.Item1)
                    return true;
            }

            return false;
        }

        //------------------------------------------------------------------------//
        public override void InitialisePropertyReceiver(PropertyReceiver receiver)
        {
            UpdateLinkTargets(receiver, PropertyHelper.FromString<T>(this.d_default));
        }

        //------------------------------------------------------------------------//
        public override Property Clone()
        {
            throw new NotImplementedException();
            //return new PropertyLinkDefinition<T>(*this);
        }

        // override members from FalagardPropertyBase
        //------------------------------------------------------------------------//
        protected override T GetNativeImpl(PropertyReceiver receiver)
        {
            var i = d_targets[0];
            var target_wnd = GetTargetWindow(receiver, i.Item1);

            // if no target, or target (currently) invalid, return the default value
            if (d_targets.Count==0 || target_wnd==null)
                return PropertyHelper.FromString<T>(d_default);

            // otherwise return the value of the property for first target, since
            // this is considered the 'master' target for get operations.
            return PropertyHelper.FromString<T>(target_wnd.GetProperty(String.IsNullOrEmpty(i.Item2) ? d_name : i.Item2));
        }

        protected override void SetNativeImpl(PropertyReceiver receiver, T value)
        {
            UpdateLinkTargets(receiver, value);

            // base handles things like ensuring redraws and such happen
            base.SetNativeImpl(receiver, value);
        }

        protected void UpdateLinkTargets(PropertyReceiver receiver, T value)
        {
            foreach (var i in d_targets)
            {
                var targetWnd = GetTargetWindow(receiver, i.Item1);

                // only try to set property if target is currently valid.
                if (targetWnd != null)
                    targetWnd.SetProperty(String.IsNullOrEmpty(i.Item2)
                                              ? d_name
                                              : i.Item2, PropertyHelper.ToString(value));
            }
        }

        public override void WriteDefinitionXMLElementType(XMLSerializer xml_stream)
        {
            xml_stream.OpenTag("PropertyLinkDefinition");
        }

        protected void WriteFalagardXMLAttributes(XMLSerializer xml_stream)
        {
            base.WriteDefinitionXMLAttributes(xml_stream);

            // HACK: Here we abuse some intimate knowledge in that we know it's
            // safe to write our sub-elements out although the function is named
            // for writing attributes.  The alternative was to repeat code from the
            // base class, also demonstrating intimate knowledge ;)

            // if there is one target only, write it out as attributes
            if (d_targets.Count == 1)
            {
                var i = d_targets[0];
                if (!String.IsNullOrEmpty(i.Item1))
                    xml_stream.Attribute("widget", i.Item1);

                if (!String.IsNullOrEmpty(i.Item2))
                    xml_stream.Attribute("targetProperty", i.Item2);
            }
            // we have multiple targets, so write them as PropertyLinkTarget tags
            else
            {
                foreach (var i in d_targets)
                {
                    xml_stream.OpenTag("PropertyLinkTarget");

                    if (!String.IsNullOrEmpty(i.Item1))
                        xml_stream.Attribute("widget", i.Item1);

                    if (!String.IsNullOrEmpty(i.Item2))
                        xml_stream.Attribute("property", i.Item2);

                    xml_stream.CloseTag();
                }
            }
        }

        /// <summary>
        /// Return a pointer to the target window with the given name.
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Window GetTargetWindow(PropertyReceiver receiver, string name)
        {
            if (String.IsNullOrEmpty(name))
                return (Window)receiver;

            // handle link back to parent.  Return receiver if no parent.
            if (name== S_parentIdentifier)
                return ((Window)receiver).GetParent();

            return ((Window)receiver).GetChild(name);
        }

        //! collection of targets for this PropertyLinkDefinition.
        protected List<Tuple<string, string>> d_targets = new List<Tuple<string, string>>();

        // TODO: ...
        private const string S_parentIdentifier = "";
    }
}