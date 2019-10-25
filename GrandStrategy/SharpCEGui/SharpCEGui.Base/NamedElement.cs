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
    /// EventArgs based class that is used for objects passed to handlers triggered
    /// for events concerning some NamedElement object.
    /// </summary>
    public class NamedElementEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public NamedElementEventArgs(NamedElement element)
        {
            Element = element;
        }

        /// <summary>
        /// pointer to an Element object of relevance to the event.
        /// </summary>
        public NamedElement Element { get; set; }
    }

    /// <summary>
    /// Adds name to the Element class, including name path traversal.
    /// \par Name path
    /// A name path is a string that describes a path down the element
    /// hierarchy using names and the forward slash '/' as a separator.
    /// For example, if this element has a child attached to it named "Panel"
    /// which has its own children attached named "Okay" and "Cancel",
    /// you can check for the element "Okay" from this element by using the
    /// name path "Panel/Okay".  To check for "Panel", you would simply pass
    /// the name "Panel".
    /// </summary>
    /// <seealso cref="Element"/>
    public abstract class NamedElement : Element
    {
        #region Events

        public new const string EventNamespace = "NamedElement";

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<NamedElementEventArgs> NameChanged;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        protected NamedElement(string name = "")
        {
            d_name = name;
            AddNamedElementProperties();
        }

        /// <summary>
        /// Renames the element. 
        /// </summary>
        /// <param name="name">
        /// String object holding the new name for the element.
        /// </param>
        /// <exception cref="AlreadyExistsException">
        /// thrown if an element named \a name already exists in the parent of this element.
        /// </exception>
        public virtual void SetName(String name)
        {
            ThrowIfDisposed();

            if (d_name == name)
                return;
            
            if (GetParentElement()!=null)
            {
                var parent = GetParentElement() as NamedElement;
                if (parent!=null && parent.IsChild(name))
                {
                    throw new AlreadyExistsException("Failed to rename " +
                                                       "NamedElement at: " + GetNamePath() + " as: " + name +
                                                       ". A Window " +
                                                       "with that name is already attached as a sibling.");
                }
            }
            
            // log this under informative level
            System.GetSingleton().Logger.LogEvent("Renamed element at: " + GetNamePath() + " as: " + name,
                                           LoggingLevel.Informative);
            
            d_name = name;
            
            OnNameChanged(new NamedElementEventArgs(this));
        }

        /// <summary>
        /// Return a String object holding the name of this Element.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            ThrowIfDisposed();

            return d_name;
        }

        /// <summary>
        /// Return a String object that describes the name path for this Element.
        /// </summary>
        /// <returns></returns>
        public string GetNamePath()
        {
            ThrowIfDisposed();

            var path = String.Empty;

            var parentElement = GetParentElement();
            var parentNamedElement = parentElement as NamedElement;

            if (parentElement != null)
            {
                if (parentNamedElement != null)
                    path = parentNamedElement.GetNamePath() + '/';
                else
                    path = "<not a named element>/";
            }

            path += GetName();
            return path;
        }

        /// <summary>
        /// Checks whether given name path references a NamedElement that is attached to this Element. 
        /// </summary>
        /// <param name="namePath">
        /// String object holding the name path of the child element to test.
        /// </param>
        /// <returns>
        /// - true if the element referenced by \a name_path is attached.
        /// - false if the element referenced by \a name_path is not attached.
        /// </returns>
        public bool IsChild(string namePath)
        {
            ThrowIfDisposed();

            return GetChildByNamePathImpl(namePath) != null;
        }

        /// <summary>
        /// returns whether at least one window with the given name is attached
        /// to this Window or any of it's children as a child.
        /// </summary>
        /// <remarks>
        /// WARNING! This function can be very expensive and should only be used
        /// when you have no other option available. If you decide to use it anyway,
        /// make sure the window hierarchy from the entry point is small.
        /// </remarks>
        /// <param name="name">
        /// name to look for.
        /// </param>
        /// <returns>
        /// - true if at least one child window was found with the name \a name
        /// - false if no child window was found with the name \a name.
        /// </returns>
        public bool IsChildRecursive(string name)
        {
            ThrowIfDisposed();

            return GetChildByNameRecursiveImpl(name) != null;
        }

        /// <summary>
        /// Return true if the specified element name is a name of some ancestor of this Element
        /// </summary>
        /// <param name="name">
        /// String object holding the name to check for.
        /// </param>
        /// <returns>
        /// - true if an element named \a name is an ancestor (parent, or parent of
        ///   parent, etc) of this element.
        /// - false if an element named \a name is in no way an ancestor of this
        ///   element.
        /// </returns>
        public bool IsAncestor(string name)
        {
            Element current = this;
            while (true)
            {
                var parent = current.GetParentElement();

                if (parent == null)
                    return false;

                var namedParent = parent as NamedElement;
                if (namedParent != null && namedParent.GetName() == name)
                    return true;

                current = parent;
            }
        }

        /// <summary>
        /// Return the attached child element that the given name path references. 
        /// </summary>
        /// <param name="namePath">
        /// String object holding the name path of the child element to return.
        /// </param>
        /// <returns>
        /// the NamedElement object referenced by \a name_path.
        /// </returns>
        /// <exception cref="UnknownObjectException">
        /// thrown if \a name_path does not reference an Element attached to this Element.
        /// </exception>
        public NamedElement GetChildElement(string namePath)
        {
            NamedElement e = GetChildByNamePathImpl(namePath);
            
            if (e!=null)
                return e;

            throw new UnknownObjectException("The Element object " +
                                             "referenced by '" + namePath + "' is not attached to Element at '"
                                             + GetNamePath() + "'.");
        }

        /// <summary>
        /// Find the first child with the given name, recursively and breadth-first.
        /// </summary>
        /// <param name="namePath">
        /// String object holding the name of the child element to find.
        /// </param>
        /// <returns>
        /// Pointer to the (first) Element object attached to this Element that has
        /// the name \a name
        /// </returns>
        public NamedElement GetChildElementRecursive(string namePath)
        {
            ThrowIfDisposed();

            return GetChildByNameRecursiveImpl(namePath);
        }
        
        /// <summary>
        /// Remove the Element referenced by the given name path from this Element's
        /// child list.
        /// </summary>
        /// <param name="namePath">
        /// String the name path that references the the Element to be removed.
        /// If the Element specified is not attached to this Window,
        /// UnknownObjectException is thrown
        /// </param>
        public void RemoveChild(string namePath)
        {
            ThrowIfDisposed();

            var e = GetChildByNamePathImpl(namePath);

            if (e != null)
                RemoveChild(e);
            else
                throw new UnknownObjectException("The Element object referenced by '" + namePath +
                                                 "' is not attached to Element at '" + GetNamePath() + "'.");
        }

        protected override void AddChildImpl(Element element)
        {
            var namedElement = element as NamedElement;

            if (namedElement != null)
            {
                var existing = GetChildByNamePathImpl(namedElement.GetName());

                if (existing != null && namedElement != existing)
                    throw new AlreadyExistsException("Failed to add Element named: " + namedElement.GetName() +
                                                     " to element at: " + GetNamePath() +
                                                     " since an Element with that name is already attached.");
            }

            base.AddChildImpl(element);
        }

        /// <summary>
        /// Retrieves a child at \a name_path or 0 if none such exists
        /// </summary>
        /// <param name="namePath"></param>
        /// <returns></returns>
        protected virtual NamedElement GetChildByNamePathImpl(string namePath)
        {
            var sep = namePath.IndexOf('/');
            var baseChild = namePath.CEGuiSubstring(0, sep);

            var childCount = d_children.Count;
            
            for (var i = 0; i < childCount; ++i)
            {
                var child = GetChildElementAtIdx(i);
                var namedChild = child as NamedElement;
                
                if (namedChild==null)
                    continue;
                
                if (namedChild.GetName() == baseChild)
                {
                    if (sep != -1 && sep < namePath.Length - 1)
                        return namedChild.GetChildByNamePathImpl(namePath.Substring(sep + 1));

                    return namedChild;
                }
            }
            
            return null;
        }


        /// <summary>
        /// Finds a child by \a name or null if none such exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual NamedElement GetChildByNameRecursiveImpl(string name)
        {
            var elementsToSearch = new Queue<Element>();
            var childCount = d_children.Count;

            for (var i = 0; i < childCount; ++i) // load all children into the queue
            {
                var child = GetChildElementAtIdx(i);
                elementsToSearch.Enqueue(child);
            }

            while (elementsToSearch.Count != 0) // breadth-first search for the child to find
            {
                var child = elementsToSearch.Dequeue();
                var namedChild = child as NamedElement;
                if (namedChild != null)
                {
                    if (namedChild.GetName() == name)
                    {
                        return namedChild;
                    }
                }

                var elementChildCount = child.GetChildCount();
                for (var i = 0; i < elementChildCount; ++i)
                {
                    elementsToSearch.Enqueue(child.GetChildElementAtIdx(i));
                }
            }

            return null;
        }

        /// <summary>
        /// Add standard CEGUI::NamedElement properties.
        /// </summary>
        protected void AddNamedElementProperties()
        {
            // "Name" is already stored in <Window> element
            AddPropertyNoXml("Name",
                             "Property to get/set the name of the Element. Make sure it's unique in its parent if any.",
                             (e, v) => e.SetName(v), e => e.GetName(), String.Empty);

            AddPropertyNoXml("NamePath",
                             "Property to get the absolute name path of this Element.",
                             null, e => e.GetNamePath(), String.Empty);
        }

        private void AddPropertyNoXml<T>(string name, string help, Action<NamedElement,T> setter, Func<NamedElement, T> getter, T defaultValue)
        {
            const string propertyOrigin = "NamedElement";
            AddProperty(new TplWindowProperty<NamedElement, T>(
                            name,
                            help,
                            setter, getter, propertyOrigin, defaultValue, false));
        }

        /// <summary>
        /// Handler called when the element's name changes.
        /// </summary>
        /// <param name="e">
        /// NamedElementEventArgs object whose 'element' pointer field is set to the element
        /// that triggered the event. For this event the trigger element is always 'this'.
        /// </param>
        protected virtual void OnNameChanged(NamedElementEventArgs e)
        {
            var handler = NameChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// The name of the element, unique in the parent of this element
        /// </summary>
        protected string d_name;
    }
}