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
    /// <summary>
    /// Class that represents a simple 'link' to an ImagerySection.
    /// 
    /// This class enables sections to be easily re-used, by different states and/or layers, by allowing
    /// sections to be specified by name rather than having mutiple copies of the same thing all over the place.
    /// </summary>
    public class SectionSpecification
    {
        public SectionSpecification()
        {
            d_usingColourOverride = false;
        }

        /*!
        \brief
            Constructor

        \param owner
            String holding the name of the WidgetLookFeel object that contains the target section.

        \param sectionName
            String holding the name of the target section.

        \param controlPropertySource
            String holding the name of a property that will control whether
            rendering for this secion will actually occur or not.

        \param controlPropertyValue
            String holding the value to be tested for from the property named in
            controlPropertySource.  If this is empty, then controlPropertySource
            will be accessed as a boolean property, otherwise rendering will
            only occur when the value returned via controlPropertySource matches
            the value specified here.

        \param controlPropertyWidget
            String holding either a child widget name or the special value of
            '__parent__' indicating the window upon which the property named
            in controlPropertySource should be accessed.  If this is empty then
            the window itself is used as the source, rather than a child or the
            parent.
        */

        public SectionSpecification(string owner,
                                    string sectionName,
                                    string controlPropertySource,
                                    string controlPropertyValue,
                                    string controlPropertyWidget)
        {
            d_owner = owner;
            d_sectionName = sectionName;
            d_usingColourOverride = false;
            d_renderControlProperty = controlPropertySource;
            d_renderControlValue = controlPropertyValue;
            d_renderControlWidget = controlPropertyWidget;
        }

        /*!
        \brief
            Constructor

        \param owner
            String holding the name of the WidgetLookFeel object that contains the target section.

        \param sectionName
            String holding the name of the target section.

        \param controlPropertySource
            String holding the name of a property that will control whether
            rendering for this secion will actually occur or not.

        \param controlPropertyValue
            String holding the value to be tested for from the property named in
            controlPropertySource.  If this is empty, then controlPropertySource
            will be accessed as a boolean property, otherwise rendering will
            only occur when the value returned via controlPropertySource matches
            the value specified here.

        \param controlPropertyWidget
            String holding either a child widget name or the special value of
            '__parent__' indicating the window upon which the property named
            in controlPropertySource should be accessed.  If this is empty then
            the window itself is used as the source, rather than a child or the
            parent.

        \param cols
            Override colours to be used (modulates sections master colours).
        */
        public SectionSpecification(string owner, string sectionName,
                                string controlPropertySource,
                                string controlPropertyValue,
                                string controlPropertyWidget,
                                ColourRect cols)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Render the section specified by this SectionSpecification.
        /// </summary>
        /// <param name="srcWindow">
        /// Window object to be used when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="modcols"></param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void Render(Window srcWindow, ColourRect modcols = null, Rectf? clipper = null, bool clipToDisplay = false)
        {
            // see if we need to bother rendering
            if (!ShouldBeDrawn(srcWindow))
                return;

            try
            {
                // get the imagery section object with the name we're set up to use
                var sect = WidgetLookManager.GetSingleton().GetWidgetLook(d_owner).GetImagerySection(d_sectionName);

                // decide what colours are to be used
                var finalColours =new ColourRect();
                InitColourRectForOverride(srcWindow, ref finalColours);
                finalColours.ModulateAlpha(srcWindow.GetEffectiveAlpha());

                if (modcols!=null)
                    finalColours *= modcols;

                // render the imagery section
                sect.Render(srcWindow, finalColours, clipper, clipToDisplay);
            }
            catch (Exception)
            {
                // do nothing here, errors are non-faltal and are logged for debugging purposes.
            }
        }

        /// <summary>
        /// Render the section specified by this SectionSpecification.
        /// </summary>
        /// <param name="srcWindow">
        /// Window object to be used when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="baseRect">
        /// Rect object to be used when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="modcols"></param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void Render(Window srcWindow, Rectf baseRect, ColourRect modcols = null, Rectf? clipper = null, bool clipToDisplay = false)
        {
            // see if we need to bother rendering
            if (!ShouldBeDrawn(srcWindow))
                return;

            try
            {
                // get the imagery section object with the name we're set up to use
                var sect = WidgetLookManager.GetSingleton().GetWidgetLook(d_owner).GetImagerySection(d_sectionName);

                // decide what colours are to be used
                var finalColours = new ColourRect();
                InitColourRectForOverride(srcWindow, ref finalColours);
                finalColours.ModulateAlpha(srcWindow.GetEffectiveAlpha());

                if (modcols != null)
                    finalColours *= modcols;

                // render the imagery section
                sect.Render(srcWindow, baseRect, finalColours, clipper, clipToDisplay);
            }
            catch (Exception)
            {
                // do nothing here, errors are non-faltal and are logged for debugging purposes.
            }
        }

        /*!
        \brief
            Return the name of the WidgetLookFeel object containing the target section.

        \return
            String object holding the name of the WidgetLookFeel that contains the target ImagerySection.
        */
        public string GetOwnerWidgetLookFeel()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Return the name of the WidgetLookFeel object containing the target section.

        \param name
            String object holding the name of the WidgetLookFeel that contains the target ImagerySection.

        \return
            Nothing.
        */
        public void SetOwnerWidgetLookFeel(string owner)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the name of the target ImagerySection.
        /// </summary>
        /// <returns>
        /// String object holding the name of the target ImagerySection.
        /// </returns>
        public string GetSectionName()
        {
            return d_sectionName;
        }
        /*!
        \brief
            Return the name of the target ImagerySection.

        \param name
            String object holding the name of the target ImagerySection.

        \return
            Nothing.
        */
        public void SetSectionName(string name)
        {
            d_sectionName = name;
        }

        /*!
        \brief
            Return the current override colours.

        \return
            ColourRect holding the colours that will be modulated with the sections master colours if
            colour override is enabled on this SectionSpecification.
        */
        public ColourRect GetOverrideColours()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the override colours to be used by this SectionSpecification.
        /// </summary>
        /// <param name="cols">
        /// ColourRect describing the override colours to set for this SectionSpecification.
        /// </param>
        public void SetOverrideColours(ColourRect cols)
        {
            d_coloursOverride = cols;
        }

        /*!
        \brief
            return whether the use of override colours is enabled on this SectionSpecification.

        \return
            - true if override colours will be used for this SectionSpecification.
            - false if override colours will not be used for this SectionSpecification.
        */
        public bool IsUsingOverrideColours()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enable or disable the use of override colours for this section.
        /// </summary>
        /// <param name="value">
        /// - true if override colours should be used for this SectionSpecification.
        /// - false if override colours should not be used for this SectionSpecification.
        /// </param>
        public void SetUsingOverrideColours(bool value = true)
        {
            d_usingColourOverride = value;
        }

        /*!
        \brief
            Get the name of the property where override colour values can be obtained.

        \return
            String containing the name of the property.
        */
        public string GetOverrideColoursPropertySource()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the name of the property where override colour values can be obtained.
        /// </summary>
        /// <param name="value">
        /// String containing the name of the property.
        /// </param>
        public void SetOverrideColoursPropertySource(string value)
        {
            d_colourPropertyName = value;
        }

        /*!
        \brief
            Get the name of the property that controls whether to actually
            render this section.

        \return
            String containing the name of the property.
        */
        public string GetRenderControlPropertySource()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Set the name of the property that controls whether to actually
            render this section.

        \param property
            String containing the name of the property.

        \return
            Nothing.
        */
        public void SetRenderControlPropertySource(string property)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Get the test value used when determining whether to render this
            section.

            The value set here will be compared to the current value of the
            property named as the render control property, if they match the
            secion will be drawn, otherwise the section will not be drawn.  If
            this value is set to the empty string, the control property will
            instead be treated as a boolean property.
        */
        public string GetRenderControlValue()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Set the test value used when determining whether to render this
            section.
            
            The value set here will be compared to the current value of the
            property named as the render control property, if they match the
            secion will be drawn, otherwise the section will not be drawn.  If
            this value is set to the empty string, the control property will
            instead be treated as a boolean property.
        */
        public void SetRenderControlValue(string value)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Get the widget what will be used as the source of the property
            named as the control property.

            The value of this setting will be interpreted as follows:
            - empty string: The target widget being drawn will be the source of
                the property value.
            - '__parent__': The parent of the widget being drawn will be the
                source of the property value.
            - any other value: The value will be taken as a name and
                a child window with the specified name will be the source of the
                property value.
        */
        public string GetRenderControlWidget()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Set the widget what will be used as the source of the property
            named as the control property.

            The value of this setting will be interpreted as follows:
            - empty string: The target widget being drawn will be the source of
                the property value.
            - '__parent__': The parent of the widget being drawn will be the
                source of the property value.
            - any other value: The value will be taken as a name and
                a child window with the specified name will be the source of the
                property value.
        */
        public void SetRenderControlWidget(string widget)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Writes an xml representation of this SectionSpecification to \a out_stream.

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
        /// Helper method to initialise a ColourRect with appropriate values according to the way the
        /// section sepcification is set up.
        /// 
        /// This will try and get values from multiple places:
        ///     - a property attached to \a wnd
        ///     - the integral d_coloursOverride values.
        ///     - or default to colour(1,1,1,1);
        /// </summary>
        /// <param name="wnd"></param>
        /// <param name="cr"></param>
        protected void InitColourRectForOverride(Window wnd, ref ColourRect cr)
        {
            // if no override set
            if (!d_usingColourOverride)
            {
                var val = new Colour(1, 1, 1, 1);
                cr.d_top_left = val;
                cr.d_top_right = val;
                cr.d_bottom_left = val;
                cr.d_bottom_right = val;
            }
            // if override comes via a colour property
            else if (!String.IsNullOrEmpty(d_colourPropertyName))
            {
                // if property accesses a ColourRect or a colour
                cr = wnd.GetProperty<ColourRect>(d_colourPropertyName);
            }
                // override is an explicitly defined ColourRect.
            else
            {
                cr = d_coloursOverride;
            }
        }


        /// <summary>
        /// return whether the section should be drawn, based upon the
        /// render control property and associated items.
        /// </summary>
        /// <param name="wnd"></param>
        /// <returns></returns>
        protected bool ShouldBeDrawn(Window wnd)
        {
            // test the simple case first.
            if (String.IsNullOrEmpty(d_renderControlProperty))
                return true;

            Window propertySource;

            // work out which window the property should be accessed for.
            if (String.IsNullOrEmpty(d_renderControlWidget))
                propertySource = wnd;
            else if (d_renderControlWidget == S_parentIdentifier)
                propertySource = wnd.GetParent();
            else
                propertySource = wnd.GetChild(d_renderControlWidget);

            // if no source window, we can't access the property, so never draw
            if (propertySource == null)
                return false;

            // return whether to draw based on property value.
            if (String.IsNullOrEmpty(d_renderControlValue))
                return propertySource.GetProperty<bool>(d_renderControlProperty);

            return propertySource.GetProperty(d_renderControlProperty) == d_renderControlValue;
        }

        private string          d_owner;                //!< Name of the WidgetLookFeel containing the required section.
        private string          d_sectionName;          //!< Name of the required section within the specified WidgetLookFeel.
        private ColourRect      d_coloursOverride;      //!< Colours to use when override is enabled.
        private bool            d_usingColourOverride;  //!< true if colour override is enabled.
        private string          d_colourPropertyName;   //!< name of property to fetch colours from.
        //! Name of a property to control whether to draw this section.
        private string d_renderControlProperty;
        //! Comparison value to test against d_renderControlProperty.
        private string d_renderControlValue;
        //! Widget upon which d_renderControlProperty is to be accessed.
        private string d_renderControlWidget;

        private const string S_parentIdentifier = "__parent__";
    }
}