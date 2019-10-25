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
using System.Linq;
using SharpCEGui.Base.Collections;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that encapsulates look & feel information for a particular widget
    /// type.
    /// </summary>
    public class WidgetLookFeel
    {
        public WidgetLookFeel(string name, string inheritedLookName)
        {
            d_lookName = name;
            d_inheritedLookName = inheritedLookName;
        }

        public WidgetLookFeel()
        {
            throw new NotImplementedException();
        }

        // TODO: ...
        //WidgetLookFeel(const WidgetLookFeel& other);
        //WidgetLookFeel& operator=(const WidgetLookFeel& other);

        // TODO: virtual ~WidgetLookFeel();

        /// <summary>
        /// Return a const reference to the StateImagery object for the specified
        /// state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>
        /// StateImagery object for the requested state.
        /// </returns>
        public StateImagery GetStateImagery(string state)
        {
            if (d_stateImagery.ContainsKey(state))
                return d_stateImagery[state];

            if (String.IsNullOrEmpty(d_inheritedLookName))
                throw new UnknownObjectException("unknown state '" + state + "' in look '" + d_lookName + "'.");

            return WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).GetStateImagery(state);
        }

        /// <summary>
        /// Return a const reference to the ImagerySection object with the
        /// specified name.
        /// </summary>
        /// <param name="section">
        /// ImagerySection object with the specified name.
        /// </param>
        /// <returns></returns>
        public ImagerySection GetImagerySection(string section)
        {
            if (d_imagerySections.ContainsKey(section))
                return d_imagerySections[section];


            if (String.IsNullOrEmpty(d_inheritedLookName))
                throw new UnknownObjectException("unknown imagery section '" + section + "' in look '" + d_lookName +
                                                 "'.");

            return WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).GetImagerySection(section);
        }

        /// <summary>
        /// Return the name of the widget look.
        /// </summary>
        /// <returns>
        /// String object holding the name of the WidgetLookFeel.
        /// </returns>
        public string GetName()
        {
            return d_lookName;
        }

        /// <summary>
        /// Add an ImagerySection to the WidgetLookFeel.
        /// </summary>
        /// <param name="section">
        /// ImagerySection object to be added.
        /// </param>
        public void AddImagerySection(ImagerySection section)
        {
            if (d_imagerySections.ContainsKey(section.GetName()))
            {
                System.GetSingleton().Logger
                      .LogEvent("WidgetLookFeel.AddImagerySection - Defintion for imagery section '" + section.GetName() +
                                "' already exists. Replacing previous definition.");
            }

            d_imagerySections[section.GetName()] = section;
        }

        public void RenameImagerySection(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a WidgetComponent to the WidgetLookFeel.
        /// </summary>
        /// <param name="widget">
        /// WidgetComponent object to be added.
        /// </param>
        public void AddWidgetComponent(WidgetComponent widget)
        {
            d_childWidgets.Add(widget);
        }

        /*!
        \brief
            Add a state specification (StateImagery object) to the WidgetLookFeel.

        \param section
            StateImagery object to be added.

        \return
            Nothing.
        */

        public void AddStateSpecification(StateImagery state)
        {
            if (d_stateImagery.ContainsKey(state.GetName()))
            {
                System.GetSingleton().Logger
                      .LogEvent("WidgetLookFeel::addStateSpecification - Defintion for state '" + state.GetName() +
                                "' already exists.  Replacing previous definition.");
            }

            d_stateImagery[state.GetName()] = state;
        }

        /*!
        \brief
            Add a property initialiser to the WidgetLookFeel.

        \param initialiser
            PropertyInitialiser object to be added.

        \return
            Nothing.
        */

        public void AddPropertyInitialiser(PropertyInitialiser initialiser)
        {
            d_properties.Add(initialiser);
        }

        /*!
        \brief
            Clear all ImagerySections from the WidgetLookFeel.

        \return
            Nothing.
        */

        public void ClearImagerySections()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Clear all WidgetComponents from the WidgetLookFeel.

        \return
            Nothing.
        */

        public void ClearWidgetComponents()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Clear all StateImagery objects from the WidgetLookFeel.

        \return
            Nothing.
        */

        public void ClearStateSpecifications()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Clear all PropertyInitialiser objects from the WidgetLookFeel.

        \return
            Nothing.
        */

        public void ClearPropertyInitialisers()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Initialise the given window using PropertyInitialsers and component
        /// widgets specified for this WidgetLookFeel.
        /// </summary>
        /// <param name="widget">
        /// Window based object to be initialised.
        /// </param>
        public void InitialiseWidget(Window widget)
        {
            // add new property definitions
            var pdc = new NamedDefinitionCollator<string, IPropertyDefinition>();
            AppendPropertyDefinitions(pdc);
            foreach (var pdi in pdc)
            {
                // add the property to the window
                widget.AddProperty((Property) pdi);
            }

            // add required child widgets
            var wcc = new NamedDefinitionCollator<string, WidgetComponent>();
            AppendChildWidgetComponents(wcc);
            foreach (var wci in wcc)
            {
                wci.Create(widget);
            }

            // add new property link definitions
            var pldc = new NamedDefinitionCollator<string, IPropertyDefinition>();
            AppendPropertyLinkDefinitions(pldc);
            foreach (var pldi in pldc)
            {
                // add the property to the window
                widget.AddProperty((Property) pldi);
            }
            // apply properties to the parent window
            var pic = new NamedDefinitionCollator<string, PropertyInitialiser>();
            AppendPropertyInitialisers(pic);
            foreach (var pi in pic)
            {
                pi.Apply(widget);
            }

            // setup linked events
            var eldc = new NamedDefinitionCollator<string, EventLinkDefinition>();
            AppendEventLinkDefinitions(eldc);
            foreach (var eldi in eldc)
            {
                eldi.InitialiseWidget(widget);
            }

            // create animation instances
            var ans = new HashSet<string>();
            AppendAnimationNames(ans);
            foreach (var ani in ans)
            {
                var instance = AnimationManager.GetSingleton().InstantiateAnimation(ani);

                d_animationInstances.Add(widget, instance);
                instance.SetTargetWindow(widget);
            }
        }

        /// <summary>
        /// Clean up the given window from all properties and component widgets
        /// created by this WidgetLookFeel
        /// </summary>
        /// <param name="widget">
        /// Window based object to be cleaned up.
        /// </param>
        public void CleanUpWidget(Window widget)
        {
            if (widget.GetLookNFeel() != GetName())
            {
                throw new InvalidRequestException("The window '" + widget.GetNamePath() +
                                                  "' does not have this WidgetLook assigned");
            }

            // remove added child widgets
            var wcc = new NamedDefinitionCollator<string, WidgetComponent>();
            AppendChildWidgetComponents(wcc);
            foreach (var wci in wcc)
                wci.Cleanup(widget);

            // delete added named Events
            var eldc = new NamedDefinitionCollator<string, EventLinkDefinition>();
            AppendEventLinkDefinitions(eldc);
            foreach (var eldi in eldc)
                eldi.CleanUpWidget(widget);

            // remove added property definitions
            var pdc = new NamedDefinitionCollator<string, IPropertyDefinition>();
            AppendPropertyDefinitions(pdc);
            foreach (var pdi in pdc)
            {
                // remove the property from the window
                widget.RemoveProperty(pdi.GetPropertyName());
            }

            // remove added property link definitions
            var pldc = new NamedDefinitionCollator<string, IPropertyDefinition>();
            AppendPropertyLinkDefinitions(pldc);
            foreach (var pldi in pldc)
            {
                // remove the property from the window
                widget.RemoveProperty(pldi.GetPropertyName());
            }

            // TODO: clean up animation instances assoicated wit the window.
            //AnimationInstanceMap::iterator anim;
            //while ((anim = d_animationInstances.find(&widget)) != d_animationInstances.end())
            //{
            //    AnimationManager::getSingleton().destroyAnimationInstance(anim->second);
            //    d_animationInstances.erase(anim);
            //}
        }

        /// <summary>
        /// Returns if a StateImagery with the given name is present in this look.
        /// </summary>
        /// <param name="name">
        /// The name of the StateImagery to look for.
        /// </param>
        /// <param name="includeInheritedLook">
        /// If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
        /// </param>
        /// <returns>
        /// - true, if the element with the given name is present,
        /// - false, if no such element is present.
        /// </returns>
        public bool IsStateImageryPresent(string name, bool includeInheritedLook = true)
        {
            if (d_stateImagery.ContainsKey(name))
                return true;

            if (String.IsNullOrEmpty(d_inheritedLookName) || !includeInheritedLook)
                return false;

            return WidgetLookManager.GetSingleton().
                                     GetWidgetLook(d_inheritedLookName).
                                     IsStateImageryPresent(name, true);
        }

        /// <summary>
        /// Returns if a ImagerySection with the given name is present in this look.
        /// </summary>
        /// <param name="name">
        /// The name of the ImagerySection to look for.
        /// </param>
        /// <param name="includeInheritedLook">
        /// If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
        /// </param>
        /// <returns>
        /// - true, if the element with the given name is present,
        /// - false, if no such element is present.
        /// </returns>
        public bool IsImagerySectionPresent(string name, bool includeInheritedLook = true)
        {
            if (d_imagerySections.ContainsKey(name))
                return true;

            if (String.IsNullOrEmpty(d_inheritedLookName) || !includeInheritedLook)
                return false;

            return WidgetLookManager.GetSingleton()
                                    .GetWidgetLook(d_inheritedLookName)
                                    .IsImagerySectionPresent(name, true);
        }

        /// <summary>
        /// Returns if a NamedArea with the given name is present in this look.
        /// </summary>
        /// <param name="name">
        /// The name of the NamedArea to look for.
        /// </param>
        /// <param name="includeInheritedLook">
        /// If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
        /// </param>
        /// <returns>
        /// - true, if the element with the given name is present,
        /// - false, if no such element is present.
        /// </returns>
        public bool IsNamedAreaPresent(string name, bool includeInheritedLook = true)
        {
            if (d_namedAreas.ContainsKey(name))
                return true;

            if (String.IsNullOrEmpty(d_inheritedLookName) || !includeInheritedLook)
                return false;

            return WidgetLookManager.GetSingleton()
                                    .GetWidgetLook(d_inheritedLookName)
                                    .IsNamedAreaPresent(name, true);
        }

        /// <summary>
        /// Returns if a WidgetComponent with the given name is present in this look.
        /// </summary>
        /// <param name="name">
        /// The name of the WidgetComponent to look for.
        /// </param>
        /// <param name="includeInheritedLook">
        /// If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
        /// </param>
        /// <returns>
        /// - true, if the element with the given name is present,
        /// - false, if no such element is present.
        /// </returns>
        public bool IsWidgetComponentPresent(string name, bool includeInheritedLook = true)
        {
            throw new NotImplementedException();
            //WidgetComponentMap::const_iterator widgetComponentIter = d_widgetComponentMap.find(name);
            //if (widgetComponentIter != d_widgetComponentMap.end())
            //    return true;

            //if (d_namedAreas.ContainsKey(name))
            //    return true;

            if (String.IsNullOrEmpty(d_inheritedLookName) || !includeInheritedLook)
                return false;

            return WidgetLookManager.GetSingleton()
                                    .GetWidgetLook(d_inheritedLookName)
                                    .IsWidgetComponentPresent(name, true);
        }

        /*!
+    \brief
+        Returns if a PropertyInitialiser with the given name is present in this look.
+
+    \param name
+        The name of the PropertyInitialiser to look for.
+
+    \param includeInheritedLook
+       If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
+
+    \return
+        - true, if the element with the given name is present,
+        - false, if no such element is present.
+    */

        public bool IsPropertyInitialiserPresent(string name, bool includeInheritedLook = true)
        {
//            PropertyInitialiserMap::const_iterator propInitialiserIter = d_propertyInitialiserMap.find(name);
//+
//+    if (propInitialiserIter != d_propertyInitialiserMap.end())
//+        return true;
//+
//+    if (d_inheritedLookName.empty() || !includeInheritedLook)
//+        return false;
//+
//+    return WidgetLookManager::getSingleton().getWidgetLook(d_inheritedLookName).isPropertyInitialiserPresent(name, true);
            throw new NotImplementedException();
        }

        /*!
+    \brief
+        Returns if a PropertyDefinition with the given name is present in this look.
+
+    \param name
+        The name of the PropertyDefinition to look for.
+
+    \param includeInheritedLook
+       If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
+
+    \return
+        - true, if the element with the given name is present,
+        - false, if no such element is present.
+    */

        public bool IsPropertyDefinitionPresent(string name, bool includeInheritedLook = true)
        {
//            PropertyDefinitionMap::const_iterator propDefIter = d_propertyDefinitionMap.find(name);
//+
//+    if (propDefIter != d_propertyDefinitionMap.end())
//+        return true;
//+
//+    if (d_inheritedLookName.empty() || !includeInheritedLook)
//+        return false;
//+
//+    return WidgetLookManager::getSingleton().getWidgetLook(d_inheritedLookName).isPropertyDefinitionPresent(name, true);
            throw new NotImplementedException();
        }

        /*!
+    \brief
+        Returns if a PropertyLinkDefinition with the given name is present in this look.
+
+    \param name
+        The name of the PropertyLinkDefinition to look for.
+
+    \param includeInheritedLook
+       If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
+
+    \return
+        - true, if the element with the given name is present,
+        - false, if no such element is present.
+    */

        public bool IsPropertyLinkDefinitionPresent(string name, bool includeInheritedLook = true)
        {
//+    PropertyLinkDefinitionMap::const_iterator propLinkDefIter = d_propertyLinkDefinitionMap.find(name);
//+
//+    if (propLinkDefIter != d_propertyLinkDefinitionMap.end())
//+        return true;
//+
//+    if (d_inheritedLookName.empty() || !includeInheritedLook)
//+        return false;
//+
//+    return WidgetLookManager::getSingleton().getWidgetLook(d_inheritedLookName).isPropertyLinkDefinitionPresent(name, true);
            throw new NotImplementedException();
        }

        /*!
+    \brief
+        Returns if a EventLinkDefinition with the given name is present in this look.
+
+    \param name
+        The name of the EventLinkDefinition to look for.
+
+    \param includeInheritedLook
+       If set to true, this function will try to also include elements from the inherited WidgetLookFeel.
+
+    \return
+        - true, if the element with the given name is present,
+        - false, if no such element is present.
+    */

        public bool IsEventLinkDefinitionPresent(string name, bool includeInheritedLook = true)
        {
//+    EventLinkDefinitionMap::const_iterator eventLinkDefIter = d_eventLinkDefinitionMap.find(name);
//+
//+    if (eventLinkDefIter != d_eventLinkDefinitionMap.end())
//+        return true;
//+
//+    if (d_inheritedLookName.empty() || !includeInheritedLook)
//+        return false;
//+
//+    return WidgetLookManager::getSingleton().getWidgetLook(d_inheritedLookName).isEventLinkDefinitionPresent(name, true);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds a named area to the WidgetLookFeel.
        /// </summary>
        /// <param name="area">
        /// NamedArea to be added.
        /// </param>
        public void AddNamedArea(NamedArea area)
        {
            if (d_namedAreas.ContainsKey(area.GetName()))
            {
                System.GetSingleton().Logger
                      .LogEvent("WidgetLookFeel.AddNamedArea - Defintion for area '" +
                                area.GetName() + "' already exists.  Replacing previous definition.");
            }
            d_namedAreas[area.GetName()] = area;
        }

        /*!
        \brief
            Clear all defined named areas from the WidgetLookFeel

        \return
            Nothing.
        */

        public void ClearNamedAreas()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the NamedArea with the specified name.
        /// </summary>
        /// <param name="name">
        /// String object holding the name of the NamedArea to be returned.
        /// </param>
        /// <returns>
        /// The requested NamedArea object.
        /// </returns>
        public NamedArea GetNamedArea(string name)
        {
            if (d_namedAreas.ContainsKey(name))
                return d_namedAreas[name];

            if (String.IsNullOrEmpty(d_inheritedLookName))
                throw new UnknownObjectException("unknown named area: '" + name + "' in look '" + d_lookName + "'.");

            return WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).GetNamedArea(name);
        }

        public void RenameNamedArea(string oldName, string newName)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// return whether a NamedArea object with the specified name exists for
        ///// this WidgetLookFeel.
        ///// </summary>
        ///// <param name="name">
        ///// String holding the name of the NamedArea to check for.
        ///// </param>
        ///// <returns>
        ///// - true if a named area with the requested name is defined for this
        /////   WidgetLookFeel.
        ///// - false if no such named area is defined for this WidgetLookFeel.
        ///// </returns>
        //public bool IsNamedAreaDefined(string name)
        //{
        //    if (d_namedAreas.ContainsKey(name))
        //        return true;

        //    if (String.IsNullOrEmpty(d_inheritedLookName))
        //        return false;

        //    return WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).IsNamedAreaDefined(name);
        //}

        /// <summary>
        /// Layout the child widgets defined for this WidgetLookFeel which are
        /// attached to the given window.
        /// </summary>
        /// <param name="owner">
        /// Window object that has the child widgets that require laying out.
        /// </param>
        public void LayoutChildWidgets(Window owner)
        {
            var wcc = new NamedDefinitionCollator<string, WidgetComponent>();
            AppendChildWidgetComponents(wcc);

            foreach (var wci in wcc)
                wci.Layout(owner);
        }

        /*!
        \brief
            Adds a property definition to the WidgetLookFeel.

        \param propdef
            PropertyDefinition object to be added.

        \return
            Nothing.
        */

        public void AddPropertyDefinition(IPropertyDefinition /*PropertyDefinitionBase*/ propdef)
        {
            d_propertyDefinitions.Add(propdef);
        }

        /// <summary>
        /// Adds a property link definition to the WidgetLookFeel.
        /// </summary>
        /// <param name="propdef">
        /// PropertyLinkDefinition object to be added.
        /// </param>
        public void AddPropertyLinkDefinition(IPropertyDefinition propdef)
        {
            d_propertyLinkDefinitions.Add(propdef);
        }

        /// <summary>
        /// Clears the map of added PropertyDefinitions of this WidgetLookFeel and destroys the PropertyDefinitions.
        /// </summary>
        public void ClearPropertyDefinitions()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Clears the map of added PropertyLinkDefinitions of this WidgetLookFeel and destroys the PropertyLinkDefinitions.
        /// </summary>
        public void ClearPropertyLinkDefinitions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add the name of an animation that is associated with the
        /// WidgetLookFeel.
        /// </summary>
        /// <param name="anim_name">
        /// Reference to a String object that contains the name of the animation
        /// to be associated with this WidgetLookFeel.
        /// </param>
        public void AddAnimationName(string anim_name)
        {
            if (!d_animations.Contains(anim_name))
                d_animations.Add(anim_name);

        }

        //! adds an event link definition to the WidgetLookFeel.
        public void AddEventLinkDefinition(EventLinkDefinition evtdef)
        {
            throw new NotImplementedException();
        }

        //! clear all defined event link definitions from the WidgetLookFeel.
        public void ClearEventLinkDefinitions()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Writes an xml representation of this WidgetLookFeel to \a out_stream.

        \param xml_stream
            Stream where xml data should be output.

        \return
            Nothing.
        */

        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Takes the name of a property and returns a pointer to the last
        /// PropertyInitialiser for this property or 0 if the is no
        /// PropertyInitialiser for this property in the WidgetLookFeel
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property to look for.
        /// </param>
        /// <returns></returns>
        public PropertyInitialiser FindPropertyInitialiser(string propertyName)
        {
            var pic = new NamedDefinitionCollator<string, PropertyInitialiser>();
            AppendPropertyInitialisers(pic);

            return pic.SingleOrDefault(x => x.GetTargetPropertyName() == propertyName);
            //PropertyInitialiserCollator::const_iterator i = pic.find(propertyName);

            //if (i == pic.end())
            //    return 0;

            //return *i;
        }

        /// <summary>
        /// Takes the name for a widget component and returns a pointer to
        /// it if it exists or 0 if it does'nt.
        /// </summary>
        /// <param name="name">
        /// The name of the Child component to look for.
        /// </param>
        /// <returns></returns>
        public WidgetComponent FindWidgetComponent(string name)
        {
            var wcc = new NamedDefinitionCollator<string, WidgetComponent>();
            AppendChildWidgetComponents(wcc);

            return wcc.SingleOrDefault(x => x.GetWidgetName() == name);
            //WidgetComponentCollator::const_iterator wci = wcc.find(name);

            //if (wci == wcc.end())
            //    return 0;

            //return *wci;
        }

        /** Typedefs for property related lists. */
        //typedef std::vector<PropertyInitialiserCEGUI_VECTOR_ALLOC(PropertyInitialiser)> PropertyList;
        //typedef std::vector<PropertyDefinitionBase*CEGUI_VECTOR_ALLOC(PropertyDefinitionBase*)> PropertyDefinitionList;
        //typedef std::vector<PropertyDefinitionBase*CEGUI_VECTOR_ALLOC(PropertyDefinitionBase*)> PropertyLinkDefinitionList;

        /** Obtains list of properties definitions.
         * @access public
         * @return CEGUI::WidgetLookFeel::PropertyDefinitionList List of properties
         * definitions
         */

        public List<IPropertyDefinition /*PropertyDefinitionBase*/> GetPropertyDefinitions()
        {
            return d_propertyDefinitions;
        }

        /** Obtains list of properties link definitions.
         * @access public
         * @return CEGUI::WidgetLookFeel::PropertyLinkDefinitionList List of
         * properties link definitions
         */

        public List<IPropertyDefinition /*PropertyDefinitionBase*/> GetPropertyLinkDefinitions()
        {
            return d_propertyLinkDefinitions;
        }

        /// <summary>
        /// Obtains list of properties.
        /// </summary>
        /// <returns></returns>
        public List<PropertyInitialiser> GetProperties()
        {
            return d_properties;
        }

        /// <summary>
        /// perform any processing required due to the given font having changed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public bool HandleFontRenderSizeChange(Window window, Font font)
        {
            var result = false;

            foreach (var i in d_imagerySections)
                result |= i.Value.HandleFontRenderSizeChange(window, font);

            foreach (var i in d_childWidgets)
                result |= i.HandleFontRenderSizeChange(window, font);

            if (!String.IsNullOrEmpty(d_inheritedLookName))
            {
                result |= WidgetLookManager.GetSingleton().
                                            GetWidgetLook(d_inheritedLookName).
                                            HandleFontRenderSizeChange(window, font);
            }

            return result;
        }

        #region Fields

        // TODO: remove this...
        //typedef std::map<String, StateImagery, StringFastLessCompare
        //    CEGUI_MAP_ALLOC(String, StateImagery)> StateList;
        //typedef std::map<String, ImagerySection, StringFastLessCompare
        //    CEGUI_MAP_ALLOC(String, ImagerySection)> ImageryList;
        //typedef std::map<String, NamedArea, StringFastLessCompare
        //    CEGUI_MAP_ALLOC(String, NamedArea)> NamedAreaList;
        //typedef std::vector<WidgetComponent
        //    CEGUI_VECTOR_ALLOC(WidgetComponent)> WidgetList;
        //typedef std::vector<String
        //    CEGUI_VECTOR_ALLOC(String)> AnimationList;
        //typedef std::multimap<Window*, AnimationInstance*
        //    /*CEGUI_MULTIMAP_ALLOC(Window*, AnimationInstance*)*/> AnimationInstanceMap;
        //typedef std::vector<EventLinkDefinition
        //    CEGUI_VECTOR_ALLOC(EventLinkDefinition)> EventLinkDefinitionList;

        /// <summary>
        /// Name of this WidgetLookFeel.
        /// </summary>
        private string d_lookName;

        /// <summary>
        /// Name of a WidgetLookFeel inherited by this WidgetLookFeel.
        /// </summary>
        private string d_inheritedLookName;

        /// <summary>
        /// Collection of ImagerySection objects.
        /// </summary>
        private Dictionary<string, ImagerySection> d_imagerySections = new Dictionary<string, ImagerySection>();

        /// <summary>
        /// Collection of WidgetComponent objects.
        /// </summary>
        private List<WidgetComponent> d_childWidgets = new List<WidgetComponent>();

        /// <summary>
        /// Collection of StateImagery objects.
        /// </summary>
        private Dictionary<string, StateImagery> d_stateImagery = new Dictionary<string, StateImagery>();

        /// <summary>
        /// Collection of PropertyInitialser objects.
        /// </summary>
        private List<PropertyInitialiser> d_properties = new List<PropertyInitialiser>();

        /// <summary>
        /// Collection of NamedArea objects.
        /// </summary>
        private Dictionary<string, NamedArea> d_namedAreas = new Dictionary<string, NamedArea>();

        /// <summary>
        /// Collection of PropertyDefinition objects.
        /// </summary>
        private List<IPropertyDefinition /*PropertyDefinitionBase*/> d_propertyDefinitions =
                new List<IPropertyDefinition>();

        /// <summary>
        /// Collection of PropertyLinkDefinition objects.
        /// </summary>
        private List<IPropertyDefinition /*PropertyDefinitionBase*/> d_propertyLinkDefinitions =
                new List<IPropertyDefinition>();

        /// <summary>
        /// Collection of animation names associated with this WidgetLookFeel.
        /// </summary>
        private List<string> d_animations = new List<string>();

        //! map of windows and their associated animation instances
        private MultiValueDictionary<Window, AnimationInstance> d_animationInstances =
                new MultiValueDictionary<Window, AnimationInstance>();

        //! Collection of EventLinkDefinition objects.
        private List<EventLinkDefinition> d_eventLinkDefinitions = new List<EventLinkDefinition>();

        #endregion

        // these are container types used when composing final collections of
        // objects that come via inheritence.
        //typedef NamedDefinitionCollator<String, const WidgetComponent*> WidgetComponentCollator;
        //typedef NamedDefinitionCollator<String, PropertyDefinitionBase*> PropertyDefinitionCollator;
        //typedef NamedDefinitionCollator<String, PropertyDefinitionBase*> PropertyLinkDefinitionCollator;
        //typedef NamedDefinitionCollator<String, const PropertyInitialiser*> PropertyInitialiserCollator;
        //typedef NamedDefinitionCollator<String, const EventLinkDefinition*> EventLinkDefinitionCollator;
        //typedef std::set<String, StringFastLessCompare, CEGUI_SET_ALLOC(String)> AnimationNameSet;

        // functions to populate containers with collections of objects that we
        // gain through inheritence.
        private void AppendChildWidgetComponents(NamedDefinitionCollator<string, WidgetComponent> col,
                                                 bool inherits = true)
        {
            if (!String.IsNullOrEmpty(d_inheritedLookName) && inherits)
                WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).AppendChildWidgetComponents(col);

            foreach (var i in d_childWidgets)
                col.Set(i.GetWidgetName(), i);
        }

        private void AppendPropertyDefinitions(NamedDefinitionCollator<string, IPropertyDefinition> col,
                                               bool inherits = true)
        {
            if (!String.IsNullOrEmpty(d_inheritedLookName) && inherits)
                WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).AppendPropertyDefinitions(col);

            foreach (var i in d_propertyDefinitions)
                col.Set(i.GetPropertyName(), i);
        }

        private void AppendPropertyLinkDefinitions(NamedDefinitionCollator<string, IPropertyDefinition> col,
                                                   bool inherits = true)
        {
            if (!String.IsNullOrEmpty(d_inheritedLookName) && inherits)
                WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).AppendPropertyLinkDefinitions(col);

            foreach (var i in d_propertyLinkDefinitions)
                col.Set(i.GetPropertyName(), i);
        }

        private void AppendPropertyInitialisers(NamedDefinitionCollator<string, PropertyInitialiser> col,
                                                bool inherits = true)
        {
            if (!String.IsNullOrEmpty(d_inheritedLookName) && inherits)
                WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).AppendPropertyInitialisers(col);

            foreach (var i in d_properties)
                col.Set(i.GetTargetPropertyName(), i);
        }

        private void AppendEventLinkDefinitions(NamedDefinitionCollator<string, EventLinkDefinition> col,
                                                bool inherits = true)
        {
            if (!String.IsNullOrEmpty(d_inheritedLookName) && inherits)
                WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).AppendEventLinkDefinitions(col);

            foreach (var i in d_eventLinkDefinitions)
                col.Set(i.GetName(), i);
        }

        private void AppendAnimationNames(ISet<string> set, bool inherits = true)
        {
            foreach (var animation in d_animations)
                set.Add(animation);

            if (!String.IsNullOrEmpty(d_inheritedLookName) && inherits)
                WidgetLookManager.GetSingleton().GetWidgetLook(d_inheritedLookName).AppendAnimationNames(set);
        }

        private void Swap(WidgetLookFeel other)
        {
            throw new NotImplementedException();
        }

        /*************************************************************************
            Iterator stuff
        *************************************************************************/
        //typedef std::set<String, StringFastLessCompare
        //        CEGUI_SET_ALLOC(String)> StringSet;

        //typedef ConstMapIterator<StateList> StateIterator;
        //typedef ConstMapIterator<ImageryList> ImageryIterator;
        //typedef ConstMapIterator<NamedAreaList> NamedAreaIterator;
        //typedef ConstVectorIterator<WidgetComponentCollator> WidgetComponentIterator;
        //typedef ConstVectorIterator<PropertyDefinitionCollator> PropertyDefinitionIterator;
        //typedef ConstVectorIterator<PropertyLinkDefinitionCollator> PropertyLinkDefinitionIterator;
        //typedef ConstVectorIterator<PropertyInitialiserCollator> PropertyInitialiserIterator;
        //typedef ConstVectorIterator<EventLinkDefinitionCollator> EventLinkDefinitionIterator;
        //typedef ConstVectorIterator<AnimationNameSet> AnimationNameIterator;

        public ISet<string> GetStateNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetImageryNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetNamedAreaNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetWidgetNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetPropertyDefinitionNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetPropertyLinkDefinitionNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetPropertyInitialiserNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetEventLinkDefinitionNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetAnimationNames(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StateImagery> GetStateIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ImagerySection> GetImageryIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NamedArea> GetNamedAreaIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WidgetComponent> GetWidgetComponentIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PropertyDefinitionBase> GetPropertyDefinitionIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PropertyDefinitionBase> GetPropertyLinkDefinitionIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PropertyInitialiser> GetPropertyInitialiserIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventLinkDefinition> GetEventLinkDefinitionIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAnimationNameIterator(bool inherits = false)
        {
            throw new NotImplementedException();
        }
    }
}