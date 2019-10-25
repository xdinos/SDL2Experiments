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
using System.Diagnostics;
using System.Globalization;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Handler class used to parse look & feel XML files used by the Falagard system.
    /// </summary>
    public class Falagard_xmlHandler : ChainedXmlHandler
    {
        /// <summary>
        /// Constructor for Falagard_xmlHandler objects
        /// </summary>
        /// <param name="mgr"></param>
        public Falagard_xmlHandler(WidgetLookManager mgr)
        {
            _manager = mgr;
            _widgetlook = null;
            _childcomponent = null;
            _imagerysection = null;
            _stateimagery = null;
            _layer = null;
            _section = null;
            _imagerycomponent = null;
            _area = null;
            _textcomponent = null;
            _namedArea = null;
            _framecomponent = null;
            _propertyLink = null;
            d_eventLink = null;

            // register element start handlers
            registerElementStartHandler(FalagardElement, elementFalagardStart);
            registerElementStartHandler(WidgetLookElement, ElementWidgetLookStart);
            registerElementStartHandler(ChildElement, ElementChildStart);
            registerElementStartHandler(ImagerySectionElement, ElementImagerySectionStart);
            registerElementStartHandler(StateImageryElement, ElementStateImageryStart);
            registerElementStartHandler(LayerElement, ElementLayerStart);
            registerElementStartHandler(SectionElement, ElementSectionStart);
            registerElementStartHandler(ImageryComponentElement, ElementImageryComponentStart);
            registerElementStartHandler(TextComponentElement, ElementTextComponentStart);
            registerElementStartHandler(FrameComponentElement, ElementFrameComponentStart);
            registerElementStartHandler(AreaElement, ElementAreaStart);
            registerElementStartHandler(ImageElement, ElementImageStart);
            registerElementStartHandler(ColoursElement, ElementColoursStart);
            registerElementStartHandler(VertFormatElement, ElementVertFormatStart);
            registerElementStartHandler(HorzFormatElement, ElementHorzFormatStart);
            registerElementStartHandler(VertAlignmentElement, ElementVertAlignmentStart);
            registerElementStartHandler(HorzAlignmentElement, ElementHorzAlignmentStart);
            registerElementStartHandler(PropertyElement, ElementPropertyStart);
            registerElementStartHandler(DimElement, ElementDimStart);
            registerElementStartHandler(UnifiedDimElement, ElementUnifiedDimStart);
            registerElementStartHandler(AbsoluteDimElement, ElementAbsoluteDimStart);
            registerElementStartHandler(ImageDimElement, ElementImageDimStart);
            registerElementStartHandler(ImagePropertyDimElement, elementImagePropertyDimStart);
            registerElementStartHandler(WidgetDimElement, ElementWidgetDimStart);
            registerElementStartHandler(FontDimElement, ElementFontDimStart);
            registerElementStartHandler(PropertyDimElement, ElementPropertyDimStart);
            registerElementStartHandler(TextElement, ElementTextStart);
            registerElementStartHandler(ColourPropertyElement, ElementColourRectPropertyStart);
            registerElementStartHandler(ColourRectPropertyElement, ElementColourRectPropertyStart);
            registerElementStartHandler(NamedAreaElement, ElementNamedAreaStart);
            registerElementStartHandler(PropertyDefinitionElement, ElementPropertyDefinitionStart);
            registerElementStartHandler(PropertyLinkDefinitionElement, ElementPropertyLinkDefinitionStart);
            registerElementStartHandler(OperatorDimElement, ElementOperatorDimStart);
            registerElementStartHandler(VertFormatPropertyElement, ElementVertFormatPropertyStart);
            registerElementStartHandler(HorzFormatPropertyElement, ElementHorzFormatPropertyStart);
            registerElementStartHandler(AreaPropertyElement, ElementAreaPropertyStart);
            registerElementStartHandler(ImagePropertyElement, ElementImagePropertyStart);
            registerElementStartHandler(TextPropertyElement, ElementTextPropertyStart);
            registerElementStartHandler(FontPropertyElement, elementFontPropertyStart);
            registerElementStartHandler(ColourElement, ElementColourStart);
            registerElementStartHandler(PropertyLinkTargetElement, elementPropertyLinkTargetStart);
            registerElementStartHandler(AnimationDefinitionHandler.ElementName, ElementAnimationDefinitionStart);
            registerElementStartHandler(EventLinkDefinitionElement, elementEventLinkDefinitionStart);
            registerElementStartHandler(EventLinkTargetElement, elementEventLinkTargetStart);
            registerElementStartHandler(NamedAreaSourceElement, elementNamedAreaSourceStart);
            registerElementStartHandler(EventActionElement, elementEventActionStart);

            // register element end handlers
            registerElementEndHandler(FalagardElement, elementFalagardEnd);
            registerElementEndHandler(WidgetLookElement, ElementWidgetLookEnd);
            registerElementEndHandler(ChildElement, ElementChildEnd);
            registerElementEndHandler(ImagerySectionElement, ElementImagerySectionEnd);
            registerElementEndHandler(StateImageryElement, ElementStateImageryEnd);
            registerElementEndHandler(LayerElement, ElementLayerEnd);
            registerElementEndHandler(SectionElement, ElementSectionEnd);
            registerElementEndHandler(ImageryComponentElement, ElementImageryComponentEnd);
            registerElementEndHandler(TextComponentElement, ElementTextComponentEnd);
            registerElementEndHandler(FrameComponentElement, ElementFrameComponentEnd);
            registerElementEndHandler(AreaElement, ElementAreaEnd);
            registerElementEndHandler(UnifiedDimElement, ElementAnyDimEnd);
            registerElementEndHandler(AbsoluteDimElement, ElementAnyDimEnd);
            registerElementEndHandler(ImageDimElement, ElementAnyDimEnd);
            registerElementEndHandler(ImagePropertyDimElement, ElementAnyDimEnd);
            registerElementEndHandler(WidgetDimElement, ElementAnyDimEnd);
            registerElementEndHandler(FontDimElement, ElementAnyDimEnd);
            registerElementEndHandler(PropertyDimElement, ElementAnyDimEnd);
            registerElementEndHandler(OperatorDimElement, ElementAnyDimEnd);
            registerElementEndHandler(NamedAreaElement, ElementNamedAreaEnd);
            registerElementEndHandler(PropertyLinkDefinitionElement, ElementPropertyLinkDefinitionEnd);
            registerElementEndHandler(EventLinkDefinitionElement, elementEventLinkDefinitionEnd);
        }

        /*!
        \brief
            Destructor for Falagard_xmlHandler objects
        */
        // TODO: ~Falagard_xmlHandler();

        /// <summary>
        /// Stores the native version, the only version we are supposed to load
        /// </summary>
        /// <remarks>
        /// The assets' versions aren't usually the same as CEGUI version, 
        /// they are versioned from version 1 onwards!
        /// </remarks>
        public const string NativeVersion = "7";

        #region Overrides of ChainedXMLHandler

        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            // find registered handler for this element.
            if (_startHandlersMap.ContainsKey(element))
            {
                // if a handler existed
                // call the handler for this element
                _startHandlersMap[element](attributes);
            }
            else
            {
                // no handler existed
                System.GetSingleton().Logger
                      .LogEvent(
                          "Falagard::xmlHandler::elementStart - The unknown XML element '" + element +
                          "' was encountered while processing the look and feel file.", LoggingLevel.Errors);
            }
        }

        protected override void ElementEndLocal(string element)
        {
            // find registered handler for this element.
            if (_endHandlersMap.ContainsKey(element))
            {
                // if a handler existed
                // call the handler for this element
                _endHandlersMap[element]();
            }
        }

        #endregion

        /*************************************************************************
            Typedefs
        *************************************************************************/

        //! Type for handlers of an opening xml element.
        //typedef void (Falagard_xmlHandler::*ElementStartHandler)(const XMLAttributes& attributes);
        private delegate void ElementStartHandler(XMLAttributes attributes);

        //! Type for handlers of a closing xml element.
        //typedef void (Falagard_xmlHandler::*ElementEndHandler)();
        private delegate void ElementEndHandler();

        //! Map of handlers for opening xml elements.
        //typedef std::map<String, ElementStartHandler, StringFastLessCompare> ElementStartHandlerMap;

        //! Map of handlers for closing xml elements.
        //typedef std::map<String, ElementEndHandler, StringFastLessCompare> ElementEndHandlerMap;

        /*************************************************************************
            Implementation Constants
        *************************************************************************/
        // element names
        private const string FalagardElement = "Falagard"; //!< Tag name for root Falagard elements.
        private const string WidgetLookElement = "WidgetLook"; //!< Tag name for WidgetLook elements.
        private const string ChildElement = "Child"; //!< Tag name for Child elements.
        private const string ImagerySectionElement = "ImagerySection"; //!< Tag name for ImagerySection elements.
        private const string StateImageryElement = "StateImagery"; //!< Tag name for StateImagery elements.
        private const string LayerElement = "Layer"; //!< Tag name for Layer elements.
        private const string SectionElement = "Section"; //!< Tag name for Section elements.
        private const string ImageryComponentElement = "ImageryComponent"; //!< Tag name for ImageryComponent elements.
        private const string TextComponentElement = "TextComponent"; //!< Tag name for TextComponent elements.
        private const string FrameComponentElement = "FrameComponent"; //!< Tag name for FrameComponent elements.
        private const string AreaElement = "Area"; //!< Tag name for Area elements.
        private const string ImageElement = "Image"; //!< Tag name for Image elements.
        private const string ColoursElement = "Colours"; //!< Tag name for Colours elements.
        private const string VertFormatElement = "VertFormat"; //!< Tag name for VertFormat elements.
        private const string HorzFormatElement = "HorzFormat"; //!< Tag name for HorzFormat elements.
        private const string VertAlignmentElement = "VertAlignment"; //!< Tag name for VertAlignment elements.
        private const string HorzAlignmentElement = "HorzAlignment"; //!< Tag name for HorzAlignment elements.
        private const string PropertyElement = "Property"; //!< Tag name for Property elements.
        private const string DimElement = "Dim"; //!< Tag name for dimension container elements.
        private const string UnifiedDimElement = "UnifiedDim"; //!< Tag name for unified dimension elements.
        private const string AbsoluteDimElement = "AbsoluteDim"; //!< Tag name for absolute dimension elements.
        private const string ImageDimElement = "ImageDim"; //!< Tag name for image dimension elements.

        private const string ImagePropertyDimElement = "ImagePropertyDim";
        //!< Tag name for image property dimension elements.

        private const string WidgetDimElement = "WidgetDim"; //!< Tag name for widget dimension elements.
        private const string FontDimElement = "FontDim"; //!< Tag name for font dimension elements.
        private const string PropertyDimElement = "PropertyDim"; //!< Tag name for property dimension elements.
        private const string TextElement = "Text"; //!< Tag name for text component text elements

        private const string ColourPropertyElement = "ColourProperty";
        //!< Tag name for property colour elements (fetches cols from a colour property)

        private const string ColourRectPropertyElement = "ColourRectProperty";
        //!< Tag name for property colour elements (fetches cols from a ColourRect property)

        private const string NamedAreaElement = "NamedArea"; //!< Tag name for named area elements.

        private const string PropertyDefinitionElement = "PropertyDefinition";
        //!< Tag name for property definition elements.

        private const string PropertyLinkDefinitionElement = "PropertyLinkDefinition";
        //!< Tag name for property link elements.

        private const string PropertyLinkTargetElement = "PropertyLinkTarget";
        //!< Tag name for property link target elements.

        private const string OperatorDimElement = "OperatorDim"; //!< Tag name for dimension operator elements.

        private const string VertFormatPropertyElement = "VertFormatProperty";
        //!< Tag name for element that specifies a vertical formatting property.

        private const string HorzFormatPropertyElement = "HorzFormatProperty";
        //!< Tag name for element that specifies a horizontal formatting property..

        private const string AreaPropertyElement = "AreaProperty";
        //!< Tag name for element that specifies a URect property..

        private const string ImagePropertyElement = "ImageProperty";
        //!< Tag name for element that specifies an Image property..

        private const string TextPropertyElement = "TextProperty";
        //!< Tag name for element that specifies an Text property.

        private const string FontPropertyElement = "FontProperty";
        //!< Tag name for element that specifies an Font property.

        private const string ColourElement = "Colour"; //!< Tag name for Colour elements.
        private const string EventLinkDefinitionElement = "EventLinkDefinition"; //!< Tag name for event link elements.
        private const string EventLinkTargetElement = "EventLinkTarget"; //!< Tag name for event link target elements.
        //! Tag name for specifying a source named area for a component
        private const string NamedAreaSourceElement = "NamedAreaSource";
        //! Tag name for specifying event / action responses for Child components
        private const string EventActionElement = "EventAction";

        // attribute names
        private const string TopLeftAttribute = "topLeft"; //!< Attribute name that stores colour for top-left corner.

        private const string TopRightAttribute = "topRight";
        //!< Attribute name that stores colour for top-right corner.

        private const string BottomLeftAttribute = "bottomLeft";
        //!< Attribute name that stores colour for bottom-left corner.

        private const string BottomRightAttribute = "bottomRight";
        //!< Attribute name that stores colour for bottom-right corner.

        private const string TypeAttribute = "type"; //!< Attribute name that stores a type string.
        private const string NameAttribute = "name"; //!< Attribute name that stores name string
        private const string PriorityAttribute = "priority"; //!< Attribute name that stores an integer priority.
        private const string SectionNameAttribute = "section"; //!< Attribute name that stores an imagery section name.
        private const string NameSuffixAttribute = "nameSuffix"; //!< Attribute name that stores a widget name suffix.

        private const string RendererAttribute = "renderer";
        //!< Attribute name that stores the name of a window renderer factory.

        private const string LookAttribute = "look"; //!< Attribute name that stores the name of a widget look.
        private const string ScaleAttribute = "scale"; //!< Attribute name that stores a UDim scale value.
        private const string OffsetAttribute = "offset"; //!< Attribute name that stores a UDim offset value.
        private const string ValueAttribute = "value"; //!< Attribute name that stores a property value string.
        private const string DimensionAttribute = "dimension"; //!< Attribute name that stores a dimension type.
        private const string WidgetAttribute = "widget"; //!< Attribute name that stores the name of a widget (suffix).
        private const string StringAttribute = "string"; //!< Attribute name that stores a string of text.
        private const string FontAttribute = "font"; //!< Attribute name that stores the name of a font.

        private const string InitialValueAttribute = "initialValue";
        //!< Attribute name that stores the initial default value for a property definition.

        private const string ClippedAttribute = "clipped";
        //!< Attribute name that stores whether some component will be clipped.

        private const string OperatorAttribute = "op"; //!< Attribute name that stores the name of an operator.
        private const string PaddingAttribute = "padding"; //!< Attribute name that stores some padding value..

        private const string LayoutOnWriteAttribute = "layoutOnWrite";
        //!< Attribute name that stores whether to layout on write of a property.

        private const string RedrawOnWriteAttribute = "redrawOnWrite";
        //!< Attribute name that stores whether to redraw on write of a property.

        private const string TargetPropertyAttribute = "targetProperty";
        //!< Attribute name that stores a name of a target property.

        private const string ControlPropertyAttribute = "controlProperty";
        //!< Attribute name that stores a name of a property to control rendering of a section.

        private const string ColourAttribute = "colour"; //!< Attribute name that stores colour for all corners.
        private const string PropertyAttribute = "property"; //!< Attribute name that stores the name of a property.

        private const string ControlValueAttribute = "controlValue";
        //!< Attribute name that stores a test value to control rendering of a section.

        private const string ControlWidgetAttribute = "controlWidget";
        //!< Attribute name that stores a widget identifier used to control rendering of a section.

        //! Attribute name that stores a help string.
        private const string HelpStringAttribute = "help";
        //! Attribute name that stores an Event name string.
        private const string EventAttribute = "event";
        //! Attribute name that stores the name of an inherited WidgetLook
        private const string InheritsAttribute = "inherits";
        //! Attribute name that stores auto-window preference
        private const string AutoWindowAttribute = "autoWindow";
        //! Attribute name that stores name of event to fire for property defs
        private const string FireEventAttribute = "fireEvent";
        //! Attribute name that stores the name of an action to be taken
        private const string ActionAttribute = "action";
        //! Attribute name that stores some component enum value
        private const string ComponentAttribute = "component";

        /*************************************************************************
            helper methods
        **************************************************************************/

        private static int HexStringToArgb(string str)
        {
            return Int32.Parse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        private void AssignAreaDimension(Dimension dim)
        {
            if (_area != null)
            {
                switch (dim.GetDimensionType())
                {
                    case DimensionType.LeftEdge:
                    case DimensionType.XPosition:
                        _area.d_left = dim;
                        break;
                    case DimensionType.TopEdge:
                    case DimensionType.YPosition:
                        _area.d_top = dim;
                        break;
                    case DimensionType.RightEdge:
                    case DimensionType.Width:
                        _area.d_right_or_width = dim;
                        break;
                    case DimensionType.BottomEdge:
                    case DimensionType.Height:
                        _area.d_bottom_or_height = dim;
                        break;
                    default:
                        throw new InvalidRequestException("Invalid DimensionType specified for area component.");
                }
            }
        }

        private void AssignColours(ColourRect cols)
        {
            // need to decide what to apply colours to
            if (_framecomponent != null)
            {
                _framecomponent.SetColours(cols);
            }
            else if (_imagerycomponent != null)
            {
                _imagerycomponent.SetColours(cols);
            }
            else if (_textcomponent != null)
            {
                _textcomponent.SetColours(cols);
            }
            else if (_imagerysection != null)
            {
                _imagerysection.SetMasterColours(cols);
            }
            else if (_section != null)
            {
                _section.SetOverrideColours(cols);
                _section.SetUsingOverrideColours();
            }
        }

        /// <summary>
        /// Method that performs common handling for all *Dim elements.
        /// </summary>
        /// <param name="dim"></param>
        private void DoBaseDimStart(BaseDim dim)
        {
            var cloned = dim.Clone();
            _dimStack.Push(cloned);
        }

        /// <summary>
        /// Method that handles the opening Falagard XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void elementFalagardStart(XMLAttributes attributes)
        {
            System.GetSingleton().Logger
                  .LogEvent("===== Falagard 'root' element: look and feel parsing begins =====");

            var version = attributes.GetValueAsString("version", "unknown");

            if (version != NativeVersion)
            {
                throw new InvalidRequestException("You are attempting to load a looknfeel of version '" + version +
                                                  "' but this CEGUI version is only meant to load looknfeels of " +
                                                  "version '" + NativeVersion + "'. Consider using the migrate.py " +
                                                  "script bundled with CEGUI Unified Editor to migrate your data.");
            }
        }

        /// <summary>
        /// Method that handles the opening WidgetLook XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementWidgetLookStart(XMLAttributes attributes)
        {
            Debug.Assert(_widgetlook == null);

            _widgetlook = new WidgetLookFeel(attributes.GetValueAsString(NameAttribute),
                                              attributes.GetValueAsString(InheritsAttribute));

            System.GetSingleton().Logger
                  .LogEvent("---> Start of definition for widget look '" + _widgetlook.GetName() + "'.",
                            LoggingLevel.Informative);
        }

        /// <summary>
        /// Method that handles the opening Child XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementChildStart(XMLAttributes attributes)
        {
            Debug.Assert(_childcomponent == null);

            _childcomponent = new WidgetComponent(
                attributes.GetValueAsString(TypeAttribute),
                attributes.GetValueAsString(LookAttribute),
                attributes.GetValueAsString(NameSuffixAttribute),
                attributes.GetValueAsString(RendererAttribute),
                attributes.GetValueAsBool(AutoWindowAttribute, true));

            Logger.LogInsane("-----> Start of definition for child widget." +
                             " Type: " + _childcomponent.GetBaseWidgetType() +
                             " Name: " + _childcomponent.GetWidgetName() +
                             " Look: " + _childcomponent.GetWidgetLookName() +
                             " Auto: " + (_childcomponent.IsAutoWindow() ? "Yes" : "No"));
        }

        /// <summary>
        /// Method that handles the opening ImagerySection XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementImagerySectionStart(XMLAttributes attributes)
        {
            Debug.Assert(_imagerysection == null);

            _imagerysection = new ImagerySection(attributes.GetValueAsString(NameAttribute));

            Logger.LogInsane("-----> Start of definition for imagery section '" + _imagerysection.GetName() + "'.");
        }

        /// <summary>
        /// Method that handles the opening StateImagery XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementStateImageryStart(XMLAttributes attributes)
        {
            Debug.Assert(_stateimagery == null);
            _stateimagery = new StateImagery(attributes.GetValueAsString(NameAttribute));
            _stateimagery.SetClippedToDisplay(!attributes.GetValueAsBool(ClippedAttribute, true));

            Logger.LogInsane("-----> Start of definition for imagery for state '" + _stateimagery.GetName() + "'.");
        }

        /// <summary>
        /// Method that handles the opening Layer XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementLayerStart(XMLAttributes attributes)
        {
            Debug.Assert(_layer == null);
            _layer = new LayerSpecification(attributes.GetValueAsInteger(PriorityAttribute));

            Logger.LogInsane("-------> Start of definition of new imagery layer, priority: " +
                             attributes.GetValueAsString(PriorityAttribute, "0"));
        }

        /// <summary>
        /// Method that handles the opening Section XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementSectionStart(XMLAttributes attributes)
        {
            Debug.Assert(_section == null);
            Debug.Assert(_widgetlook != null);
            var owner = attributes.GetValueAsString(LookAttribute);

            _section = new SectionSpecification(
                String.IsNullOrEmpty(owner) ? _widgetlook.GetName() : owner,
                attributes.GetValueAsString(SectionNameAttribute),
                attributes.GetValueAsString(ControlPropertyAttribute),
                attributes.GetValueAsString(ControlValueAttribute),
                attributes.GetValueAsString(ControlWidgetAttribute));

            Logger.LogInsane("---------> Layer references imagery section '" + _section.GetSectionName() + "'.");
        }

        /// <summary>
        /// Method that handles the opening ImageryComponent XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementImageryComponentStart(XMLAttributes attributes)
        {
            Debug.Assert(_imagerycomponent == null);
            _imagerycomponent = new ImageryComponent();

            Logger.LogInsane("-------> Image component definition...");
        }

        /// <summary>
        /// Method that handles the opening TextComponent XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementTextComponentStart(XMLAttributes attributes)
        {
            Debug.Assert(_textcomponent == null);
            _textcomponent = new TextComponent();

            Logger.LogInsane("-------> Text component definition...");
        }

        /// <summary>
        /// Method that handles the opening FrameComponent XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementFrameComponentStart(XMLAttributes attributes)
        {
            Debug.Assert(_framecomponent == null);
            _framecomponent = new FrameComponent();

            Logger.LogInsane("-------> Frame component definition...");
        }

        /// <summary>
        /// Method that handles the opening Area XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementAreaStart(XMLAttributes attributes)
        {
            Debug.Assert(_area == null);
            _area = new ComponentArea();
        }

        /// <summary>
        /// Method that handles the opening Image XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementImageStart(XMLAttributes attributes)
        {
            if (_imagerycomponent != null)
            {
                _imagerycomponent.SetImage(attributes.GetValueAsString(NameAttribute));
                Logger.LogInsane("---------> Using image: " + attributes.GetValueAsString(NameAttribute));
            }
            else if (_framecomponent != null)
            {
                _framecomponent.SetImage( /*FalagardXMLHelper*/
                    PropertyHelper.FromString<FrameImageComponent>(
                        attributes.GetValueAsString(ComponentAttribute)),
                    attributes.GetValueAsString(NameAttribute));

                Logger.LogInsane("---------> Using image: " +
                                 attributes.GetValueAsString(NameAttribute) + " for: " +
                                 attributes.GetValueAsString(ComponentAttribute));
            }
        }

        /// <summary>
        /// Method that handles the opening Colours XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementColoursStart(XMLAttributes attributes)
        {
            var cols = new ColourRect(HexStringToArgb(attributes.GetValueAsString(TopLeftAttribute)),
                                      HexStringToArgb(attributes.GetValueAsString(TopRightAttribute)),
                                      HexStringToArgb(attributes.GetValueAsString(BottomLeftAttribute)),
                                      HexStringToArgb(attributes.GetValueAsString(BottomRightAttribute)));

            AssignColours(cols);
        }

        /// <summary>
        /// Method that handles the opening VertFormat XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementVertFormatStart(XMLAttributes attributes)
        {
            if (_framecomponent!=null)
            {
                var what = /*FalagardXMLHelper*/PropertyHelper.FromString<FrameImageComponent>(
                        attributes.GetValueAsString(ComponentAttribute, "Background"));
                var fmt = /*FalagardXMLHelper*/PropertyHelper.FromString<VerticalFormatting>(
                        attributes.GetValueAsString(TypeAttribute));

                switch(what)
                {
                    case FrameImageComponent.LeftEdge:
                        _framecomponent.SetLeftEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.RightEdge:
                        _framecomponent.SetRightEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.Background:
                        _framecomponent.SetBackgroundVerticalFormatting(fmt);
                        break;
                    default:
                        throw new InvalidRequestException(
                            VertFormatElement + " within " +
                            FrameComponentElement +
                            " may only be used for LeftEdge, RightEdge or Background components. Received: " +
                            attributes.GetValueAsString(ComponentAttribute));
                }
            }
            else if (_imagerycomponent!=null)
            {
                _imagerycomponent.SetVerticalFormatting(
                    /*FalagardXMLHelper*/PropertyHelper.FromString<VerticalFormatting>(
                        attributes.GetValueAsString(TypeAttribute)));
            }
            else if (_textcomponent!=null)
            {
                _textcomponent.SetVerticalFormatting(
                    /*FalagardXMLHelper*/PropertyHelper.FromString<VerticalTextFormatting>(
                        attributes.GetValueAsString(TypeAttribute)));
            }
        }

        /// <summary>
        /// Method that handles the opening HorzFormat XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementHorzFormatStart(XMLAttributes attributes)
        {
            if (_framecomponent!=null)
            {
                var what = /*FalagardXMLHelper*/PropertyHelper.FromString<FrameImageComponent>(
                        attributes.GetValueAsString(ComponentAttribute, "Background"));
                var fmt = /*FalagardXMLHelper*/PropertyHelper.FromString<HorizontalFormatting>(
                        attributes.GetValueAsString(TypeAttribute));

                switch(what)
                {
                    case FrameImageComponent.TopEdge:
                        _framecomponent.SetTopEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.BottomEdge:
                        _framecomponent.SetBottomEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.Background:
                        _framecomponent.SetBackgroundHorizontalFormatting(fmt);
                        break;
                    default:
                        throw new InvalidRequestException(
                            HorzFormatElement + " within " +
                            FrameComponentElement + " may only be used for " +
                            "TopEdge, BottomEdge or Background components. " +
                            "Received: " +
                            attributes.GetValueAsString(ComponentAttribute));
                }
            }
            else if (_imagerycomponent!=null)
            {
                _imagerycomponent.SetHorizontalFormatting(
                    /*FalagardXMLHelper*/PropertyHelper.FromString<HorizontalFormatting>(
                        attributes.GetValueAsString(TypeAttribute)));
            }
            else if (_textcomponent!=null)
            {
                _textcomponent.SetHorizontalFormatting(
                    /*FalagardXMLHelper*/PropertyHelper.FromString<HorizontalTextFormatting>(
                        attributes.GetValueAsString(TypeAttribute)));
            }
        }

        /// <summary>
        /// Method that handles the opening VertAlignment XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementVertAlignmentStart(XMLAttributes attributes)
        {
            Debug.Assert(_childcomponent != null);
            _childcomponent.SetVerticalWidgetAlignment(
            /*FalagardXMLHelper*/PropertyHelper.FromString<VerticalAlignment>(
                attributes.GetValueAsString(TypeAttribute)));
        }

        /// <summary>
        /// Method that handles the opening HorzAlignment XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementHorzAlignmentStart(XMLAttributes attributes)
        {
            Debug.Assert(_childcomponent != null);
            _childcomponent.SetHorizontalWidgetAlignment(
            /*FalagardXMLHelper*/PropertyHelper.FromString<HorizontalAlignment>(
                attributes.GetValueAsString(TypeAttribute)));
        }

        /// <summary>
        /// Method that handles the opening Property XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementPropertyStart(XMLAttributes attributes)
        {
            Debug.Assert(_widgetlook != null);

            var prop = new PropertyInitialiser(attributes.GetValueAsString(NameAttribute),
                                               attributes.GetValueAsString(ValueAttribute));

            if (_childcomponent != null)
            {
                _childcomponent.AddPropertyInitialiser(prop);
                Logger.LogInsane("-------> Added property initialiser for property: " + prop.GetTargetPropertyName() +
                                 " with value: " + prop.GetInitialiserValue());
            }
            else
            {
                _widgetlook.AddPropertyInitialiser(prop);
                Logger.LogInsane("---> Added property initialiser for property: " + prop.GetTargetPropertyName() +
                                 " with value: " + prop.GetInitialiserValue());
            }
        }

        /// <summary>
        /// Method that handles the opening Dim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementDimStart(XMLAttributes attributes)
        {

            _dimension.SetDimensionType(
                /*FalagardXMLHelper*/
                PropertyHelper.FromString<DimensionType>(attributes.GetValueAsString(TypeAttribute)));
        }

        /// <summary>
        /// Method that handles the opening UnifiedDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementUnifiedDimStart(XMLAttributes attributes)
        {
            var @base = new UnifiedDim(
                new UDim(attributes.GetValueAsFloat(ScaleAttribute), attributes.GetValueAsFloat(OffsetAttribute)),
                PropertyHelper.FromString<DimensionType>(attributes.GetValueAsString(TypeAttribute)));

            DoBaseDimStart(@base);
        }

        /// <summary>
        /// Method that handles the opening AbsoluteDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementAbsoluteDimStart(XMLAttributes attributes)
        {
            var @base = new AbsoluteDim(attributes.GetValueAsFloat(ValueAttribute));
            DoBaseDimStart(@base);
        }

        /// <summary>
        /// Method that handles the opening ImageDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementImageDimStart(XMLAttributes attributes)
        {
            var @base = new ImageDim(attributes.GetValueAsString(NameAttribute),
                                     PropertyHelper.FromString<DimensionType>(attributes.GetValueAsString(DimensionAttribute)));
            DoBaseDimStart(@base);
        }

        /// <summary>
        /// Method that handles the opening ImagePropertyDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void elementImagePropertyDimStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that handles the opening WidgetDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementWidgetDimStart(XMLAttributes attributes)
        {
            var @base = new WidgetDim(attributes.GetValueAsString(WidgetAttribute),
                                      /*FalagardXMLHelper*/PropertyHelper.FromString<DimensionType>(
                                          attributes.GetValueAsString(DimensionAttribute)));

            DoBaseDimStart(@base);
        }

        /// <summary>
        /// Method that handles the opening FontDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementFontDimStart(XMLAttributes attributes)
        {
            var @base = new FontDim(
                attributes.GetValueAsString(WidgetAttribute),
                attributes.GetValueAsString(FontAttribute),
                attributes.GetValueAsString(StringAttribute),
                PropertyHelper.FromString<FontMetricType>(attributes.GetValueAsString(TypeAttribute)),
                attributes.GetValueAsFloat(PaddingAttribute));

            DoBaseDimStart(@base);
        }

        /// <summary>
        /// Method that handles the opening PropertyDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementPropertyDimStart(XMLAttributes attributes)
        {
            var strType = attributes.GetValueAsString(TypeAttribute);
            var type = DimensionType.Invalid;
            if (!String.IsNullOrEmpty(strType))
                type = PropertyHelper.FromString<DimensionType>(strType);

            var @base = new PropertyDim(attributes.GetValueAsString(WidgetAttribute),
                                        attributes.GetValueAsString(NameAttribute),
                                        type);

            DoBaseDimStart(@base);
        }

        /// <summary>
        /// Method that handles the opening Text XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementTextStart(XMLAttributes attributes)
        {
            Debug.Assert(_textcomponent != null);

            _textcomponent.SetText(attributes.GetValueAsString(StringAttribute));
            _textcomponent.SetFont(attributes.GetValueAsString(FontAttribute));
        }

        /// <summary>
        /// Method that handles the opening ColourRectProperty XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementColourRectPropertyStart(XMLAttributes attributes)
        {
            // need to decide what to apply colours to
            if (_framecomponent != null)
            {
                _framecomponent.SetColoursPropertySource(attributes.GetValueAsString(NameAttribute));
            }
            else if (_imagerycomponent != null)
            {
                _imagerycomponent.SetColoursPropertySource(attributes.GetValueAsString(NameAttribute));
            }
            else if (_textcomponent != null)
            {
                _textcomponent.SetColoursPropertySource(attributes.GetValueAsString(NameAttribute));
            }
            else if (_imagerysection != null)
            {
                _imagerysection.SetMasterColoursPropertySource(attributes.GetValueAsString(NameAttribute));
            }
            else if (_section != null)
            {
                _section.SetOverrideColoursPropertySource(attributes.GetValueAsString(NameAttribute));
                _section.SetUsingOverrideColours();
            }
        }

        /// <summary>
        /// Method that handles the opening NamedArea XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementNamedAreaStart(XMLAttributes attributes)
        {
            Debug.Assert(_namedArea == null);
            
            _namedArea = new NamedArea(attributes.GetValueAsString(NameAttribute));
            
            Logger.LogInsane("-----> Creating named area: " + _namedArea.GetName());
        }

        /// <summary>
        /// Method that handles the opening PropertyDefinition XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementPropertyDefinitionStart(XMLAttributes attributes)
        {
            Debug.Assert(_widgetlook != null);

            var name = attributes.GetValueAsString(NameAttribute);
            var init = attributes.GetValueAsString(InitialValueAttribute);
            var help = attributes.GetValueAsString(HelpStringAttribute,
                                                   "Falagard custom property definition - gets/sets a named user string.");
            var type = attributes.GetValueAsString(TypeAttribute, "Generic");
            var redraw = attributes.GetValueAsBool(RedrawOnWriteAttribute);
            var layout = attributes.GetValueAsBool(LayoutOnWriteAttribute);
            var eventName = attributes.GetValueAsString(FireEventAttribute);

            IPropertyDefinition prop;

            if (type == "Colour")
                prop = new PropertyDefinition<Colour>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                      eventName, _widgetlook.GetName());
            else if (type == "ColourRect")
                prop = new PropertyDefinition<ColourRect>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                          eventName, _widgetlook.GetName());
            else if (type == "UBox")
                prop = new PropertyDefinition<UBox>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                    _widgetlook.GetName());
            else if (type == "URect")
                prop = new PropertyDefinition<URect>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                     _widgetlook.GetName());
            else if (type == "USize")
                prop = new PropertyDefinition<USize>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                     _widgetlook.GetName());
            else if (type == "UDim")
                prop = new PropertyDefinition<UDim>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                    _widgetlook.GetName());
            else if (type == "UVector2")
                prop = new PropertyDefinition<UVector2>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                        eventName, _widgetlook.GetName());
            else if (type == "Sizef")
                prop = new PropertyDefinition<Sizef>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                     _widgetlook.GetName());
            else if (type == "Vector2")
                prop = new PropertyDefinition<Lunatics.Mathematics.Vector2>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                        eventName, _widgetlook.GetName());
            else if (type == "Vector3")
                prop = new PropertyDefinition<Lunatics.Mathematics.Vector3>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                        eventName, _widgetlook.GetName());
            else if (type == "Rectf")
                prop = new PropertyDefinition<Rectf>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                     _widgetlook.GetName());
            else if (type == "Font")
                prop = new PropertyDefinition<Font>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                    _widgetlook.GetName());
            else if (type == "Image")
                prop = new PropertyDefinition<Image>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                     _widgetlook.GetName());
            else if (type == "Quaternion")
                prop = new PropertyDefinition<Lunatics.Mathematics.Quaternion>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                          eventName, _widgetlook.GetName());
            else if (type == "AspectMode")
                prop = new PropertyDefinition<AspectMode>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                          eventName, _widgetlook.GetName());
            else if (type == "HorizontalAlignment")
                prop = new PropertyDefinition<HorizontalAlignment>(name, init, help, _widgetlook.GetName(), redraw,
                                                                   layout, eventName, _widgetlook.GetName());
            else if (type == "VerticalAlignment")
                prop = new PropertyDefinition<VerticalAlignment>(name, init, help, _widgetlook.GetName(), redraw,
                                                                 layout, eventName, _widgetlook.GetName());
            else if (type == "HorizontalTextFormatting")
                prop = new PropertyDefinition<HorizontalTextFormatting>(name, init, help, _widgetlook.GetName(), redraw,
                                                                        layout, eventName, _widgetlook.GetName());
            else if (type == "VerticalTextFormatting")
                prop = new PropertyDefinition<VerticalTextFormatting>(name, init, help, _widgetlook.GetName(), redraw,
                                                                      layout, eventName, _widgetlook.GetName());
            else if (type == "WindowUpdateMode")
                prop = new PropertyDefinition<WindowUpdateMode>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                                eventName, _widgetlook.GetName());
            else if (type == "bool")
                prop = new PropertyDefinition<bool>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                    _widgetlook.GetName());
            else if (type == "uint")
                prop = new PropertyDefinition<uint>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                    _widgetlook.GetName());
            else if (type == "unsigned long")
                prop = new PropertyDefinition<ulong>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                     _widgetlook.GetName());
            else if (type == "int")
                prop = new PropertyDefinition<int>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                   _widgetlook.GetName());
            else if (type == "float")
                prop = new PropertyDefinition<float>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName,
                                                     _widgetlook.GetName());
            else if (type == "double")
                prop = new PropertyDefinition<double>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                      eventName, _widgetlook.GetName());
            else if(type == "TabControl::TabPanePosition")
                prop = new PropertyDefinition<Widgets.TabControl.TabPanePosition>(name, init, help, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            // TODO: ...
            //else if(type == "Spinner::TextInputMode")
            //    prop = new PropertyDefinition<Spinner::TextInputMode>(name, init, help, d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            //else if(type == "ItemListBase::SortMode")
            //    prop = new PropertyDefinition<ItemListBase::SortMode>(name, init, help, d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            //else if(type == "ListHeaderSegment::SortDirection")
            //    prop = new PropertyDefinition<ListHeaderSegment::SortDirection>(name, init, help, d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            //else if(type == "MultiColumnList::SelectionMode")
            //    prop = new PropertyDefinition<MultiColumnList::SelectionMode>(name, init, help, d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            else if (type == "VerticalFormatting")
                prop = new PropertyDefinition<VerticalFormatting>(name, init, help, _widgetlook.GetName(), redraw,
                                                                  layout, eventName, _widgetlook.GetName());
            else if (type == "HorizontalFormatting")
                prop = new PropertyDefinition<HorizontalFormatting>(name, init, help, _widgetlook.GetName(), redraw,
                                                                    layout, eventName, _widgetlook.GetName());
            else if (type == "Range")
                // typedef std::pair<float, float> Range;
                prop = new PropertyDefinition<Tuple<float, float> /*Range*/>(name, init, help, _widgetlook.GetName(),
                                                                             redraw, layout, eventName,
                                                                             _widgetlook.GetName());
            else
            {
                if (type != "Generic" && type != "String")
                {
                    // type was specified but wasn't recognised
                    System.GetSingleton().Logger
                          .LogEvent(
                              "Type '" + type + "' wasn't recognized in property definition (name: '" + name + "').",
                              LoggingLevel.Warnings);
                }

                prop = new PropertyDefinition<String>(name, init, help, _widgetlook.GetName(), redraw, layout,
                                                      eventName, _widgetlook.GetName());
            }

            Logger.LogInsane("-----> Adding PropertyDefiniton. Name: " + name + " Default Value: " + init);

            _widgetlook.AddPropertyDefinition(prop);
        }
        
        /// <summary>
        /// Method that handles the opening PropertyLinkDefinition XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementPropertyLinkDefinitionStart(XMLAttributes attributes)
        {
            Debug.Assert(_widgetlook!=null);
            Debug.Assert(_propertyLink == null);

            var widget = attributes.GetValueAsString(WidgetAttribute);
            var target = attributes.GetValueAsString(TargetPropertyAttribute);
            var name = attributes.GetValueAsString(NameAttribute);
            var init = attributes.GetValueAsString(InitialValueAttribute);
            var type = attributes.GetValueAsString(TypeAttribute, "Generic");
            var redraw = attributes.GetValueAsBool(RedrawOnWriteAttribute);
            var layout = attributes.GetValueAsBool(LayoutOnWriteAttribute);
            var eventName = attributes.GetValueAsString(FireEventAttribute);
            
            if (type == "Colour")
                _propertyLink = new PropertyLinkDefinition<Colour>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "ColourRect")
                _propertyLink = new PropertyLinkDefinition<ColourRect>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "UBox")
                _propertyLink = new PropertyLinkDefinition<UBox>(name, widget,
                        target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "URect")
                _propertyLink = new PropertyLinkDefinition<URect>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "USize")
                _propertyLink = new PropertyLinkDefinition<USize>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "UDim")
                _propertyLink = new PropertyLinkDefinition<UDim>(name, widget,
                        target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "UVector2")
                _propertyLink = new PropertyLinkDefinition<UVector2>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "Sizef")
                _propertyLink = new PropertyLinkDefinition<Sizef>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "Vector2")
                _propertyLink = new PropertyLinkDefinition<Lunatics.Mathematics.Vector2>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "Vector3")
                _propertyLink = new PropertyLinkDefinition<Lunatics.Mathematics.Vector3>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "Rectf")
                _propertyLink = new PropertyLinkDefinition<Rectf>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "Font")
                _propertyLink = new PropertyLinkDefinition<Font>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "Image")
                _propertyLink = new PropertyLinkDefinition<Image>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "Quaternion")
                _propertyLink = new PropertyLinkDefinition<Lunatics.Mathematics.Quaternion>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "AspectMode")
                _propertyLink = new PropertyLinkDefinition<AspectMode>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "HorizontalAlignment")
                _propertyLink =new PropertyLinkDefinition<HorizontalAlignment>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName()
                );
            else if (type == "VerticalAlignment")
                _propertyLink = new PropertyLinkDefinition<VerticalAlignment>(
                        name, widget, target, init, _widgetlook.GetName(), redraw,
                        layout, eventName, _widgetlook.GetName());
            else if (type == "HorizontalTextFormatting")
                _propertyLink = new PropertyLinkDefinition<
                        HorizontalTextFormatting>(name, widget, target, init,
                        _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "VerticalTextFormatting")
                _propertyLink = new PropertyLinkDefinition<
                        VerticalTextFormatting>(name, widget, target, init,
                        _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "WindowUpdateMode")
                _propertyLink = new PropertyLinkDefinition<WindowUpdateMode>(
                        name, widget, target, init, _widgetlook.GetName(), redraw,
                        layout, eventName, _widgetlook.GetName());
            else if (type == "bool")
                _propertyLink = new PropertyLinkDefinition<bool>(name, widget,
                        target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "uint")
                _propertyLink = new PropertyLinkDefinition<uint>(name, widget,
                        target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "unsigned long")
                _propertyLink = new PropertyLinkDefinition<ulong>(
                        name, widget, target, init, _widgetlook.GetName(), redraw,
                        layout, eventName, _widgetlook.GetName());
            else if (type == "int")
                _propertyLink = new PropertyLinkDefinition<int>(name, widget,
                        target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "float")
                _propertyLink = new PropertyLinkDefinition<float>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "double")
                _propertyLink = new PropertyLinkDefinition<double>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else if (type == "TabControl::TabPanePosition")
                _propertyLink = new PropertyLinkDefinition<
                        Widgets.TabControl.TabPanePosition>(name, widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            // TODO: ...
            //else if (type == "Spinner::TextInputMode")
            //    d_propertyLink = new PropertyLinkDefinition<
            //            Spinner::TextInputMode>(name, widget, target, init,
            //            d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            //else if (type == "ItemListBase::SortMode")
            //    d_propertyLink = new PropertyLinkDefinition<
            //            ItemListBase::SortMode>(name, widget, target, init,
            //            d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            //else if (type == "ListHeaderSegment::SortDirection")
            //    d_propertyLink = new PropertyLinkDefinition<
            //            ListHeaderSegment::SortDirection>(name, widget, target, init,
            //            d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            //else if (type == "MultiColumnList::SelectionMode")
            //    d_propertyLink = new PropertyLinkDefinition<
            //            MultiColumnList::SelectionMode>(name, widget, target, init,
            //            d_widgetlook.GetName(), redraw, layout, eventName, d_widgetlook.GetName());
            else if (type == "VerticalFormatting")
                _propertyLink = new PropertyLinkDefinition<VerticalFormatting>(
                        name, widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName()
                );
            else if (type == "HorizontalFormatting")
                _propertyLink = new PropertyLinkDefinition<HorizontalFormatting>(
                        name, widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName()
                );
            // TODO: typedef std::pair<float, float> Range;
            else if (type == "Range")
                _propertyLink = new PropertyLinkDefinition<Tuple<float,float>>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            else
            {
                if (type != "Generic" && type != "String")
                {
                    // type was specified but wasn't recognised
                    System.GetSingleton().Logger
                          .LogEvent(
                              "Type '" + type + "' wasn't recognized in property link definition (name: '" + name +
                              "').", LoggingLevel.Warnings);
                }

                _propertyLink = new PropertyLinkDefinition<String>(name,
                        widget, target, init, _widgetlook.GetName(), redraw, layout, eventName, _widgetlook.GetName());
            }

            Logger.LogInsane("-----> Adding PropertyLinkDefiniton. Name: " + name);

            if (!String.IsNullOrEmpty(widget) || !String.IsNullOrEmpty(target))
            {
                Logger.LogInsane("-------> Adding link target to property: " + target + " on widget: " + widget);
            }
        }

        /// <summary>
        /// Method that handles the opening OperatorDim XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementOperatorDimStart(XMLAttributes attributes)
        {
            var @base = new OperatorDim( /*FalagardXMLHelper*/PropertyHelper.FromString<DimensionOperator>(
                attributes.GetValueAsString(OperatorAttribute)));

            DoBaseDimStart(@base);
        }

        /// <summary>
        /// Method that handles the opening VertFormatProperty XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementVertFormatPropertyStart(XMLAttributes attributes)
        {
            if (_framecomponent != null)
            {
                var what = /*FalagardXMLHelper*/
                    PropertyHelper.FromString<FrameImageComponent>(attributes.GetValueAsString(ComponentAttribute,
                                                                                               "Background"));
                var fmt = /*FalagardXMLHelper*/
                    PropertyHelper.FromString<VerticalFormatting>(attributes.GetValueAsString(TypeAttribute));

                switch (what)
                {
                    case FrameImageComponent.LeftEdge:
                        _framecomponent.SetLeftEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.RightEdge:
                        _framecomponent.SetRightEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.Background:
                        _framecomponent.SetBackgroundVerticalFormatting(fmt);
                        break;
                    default:
                        throw new InvalidRequestException(VertFormatPropertyElement + " within " + FrameComponentElement +
                                                          " may only be used for LeftEdge, RightEdge or Background components. Received: " +
                                                          attributes.GetValueAsString(ComponentAttribute));
                }
            }
            else if (_imagerycomponent != null)
                _imagerycomponent.SetVerticalFormattingPropertySource(attributes.GetValueAsString(NameAttribute));
            else if (_textcomponent != null)
                _textcomponent.SetVerticalFormattingPropertySource(attributes.GetValueAsString(NameAttribute));
        }

        /// <summary>
        /// Method that handles the opening HorzFormatProperty XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementHorzFormatPropertyStart(XMLAttributes attributes)
        {
            if (_framecomponent != null)
            {
                var what = /*FalagardXMLHelper*/
                    PropertyHelper.FromString<FrameImageComponent>(attributes.GetValueAsString(ComponentAttribute,
                                                                                               "Background"));
                var fmt = /*FalagardXMLHelper*/
                    PropertyHelper.FromString<HorizontalFormatting>(attributes.GetValueAsString(TypeAttribute));

                switch (what)
                {
                    case FrameImageComponent.TopEdge:
                        _framecomponent.SetTopEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.BottomEdge:
                        _framecomponent.SetBottomEdgeFormatting(fmt);
                        break;
                    case FrameImageComponent.Background:
                        _framecomponent.SetBackgroundHorizontalFormatting(fmt);
                        break;
                    default:
                        throw new InvalidRequestException(
                            HorzFormatPropertyElement + " within " +
                            FrameComponentElement +
                            " may only be used for TopEdge, BottomEdge or Background components. Received: " +
                            attributes.GetValueAsString(ComponentAttribute));
                }
            }
            else if (_imagerycomponent != null)
                _imagerycomponent.SetHorizontalFormattingPropertySource(attributes.GetValueAsString(NameAttribute));
            else if (_textcomponent != null)
                _textcomponent.SetHorizontalFormattingPropertySource(attributes.GetValueAsString(NameAttribute));
        }

        /// <summary>
        /// Method that handles the opening AreaProperty XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementAreaPropertyStart(XMLAttributes attributes)
        {
            Debug.Assert(_area!=null);

            _area.SetAreaPropertySource(attributes.GetValueAsString(NameAttribute));
        }

        /// <summary>
        /// Method that handles the opening ImageProperty XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementImagePropertyStart(XMLAttributes attributes)
        {
            Debug.Assert(_imagerycomponent != null || _framecomponent != null);

            if (_imagerycomponent!=null)
            {
                _imagerycomponent.SetImagePropertySource(attributes.GetValueAsString(NameAttribute));

                Logger.LogInsane("---------> Using image via property: " +
                                 attributes.GetValueAsString(NameAttribute));
            }
            else if (_framecomponent!=null)
            {
                _framecomponent.SetImagePropertySource(
                    /*FalagardXMLHelper*/PropertyHelper.FromString<FrameImageComponent>(
                        attributes.GetValueAsString(ComponentAttribute)),
                                         attributes.GetValueAsString(NameAttribute));
                

                Logger.LogInsane("---------> Using image via property: " +
                    attributes.GetValueAsString(NameAttribute) + " for: " +
                    attributes.GetValueAsString(ComponentAttribute));
            }
        }

        /// <summary>
        /// Method that handles the opening TextProperty XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementTextPropertyStart(XMLAttributes attributes)
        {
            Debug.Assert(_textcomponent != null);

            _textcomponent.SetTextPropertySource(attributes.GetValueAsString(NameAttribute));
        }

        /*!
        \brief
            Method that handles the opening FontProperty XML element.
        */

        private void elementFontPropertyStart(XMLAttributes attributes)
        {
            Debug.Assert(_textcomponent != null);

            _textcomponent.SetFontPropertySource(attributes.GetValueAsString(NameAttribute));
        }

        /// <summary>
        /// Method that handles the opening Colour XML element.
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementColourStart(XMLAttributes attributes)
        {
            AssignColours(new ColourRect(Colour.Parse(attributes.GetValueAsString(ColourAttribute))));
        }

        //! Function to handle PropertyLinkTarget elements.
        private void elementPropertyLinkTargetStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Function to handle AnimationDefinition elements
        /// </summary>
        /// <param name="attributes"></param>
        private void ElementAnimationDefinitionStart(XMLAttributes attributes)
        {
            Debug.Assert(_widgetlook != null);

            var animNamePrefix = _widgetlook.GetName();
            animNamePrefix += "/";

            ChainedHandler = new AnimationDefinitionHandler(
                attributes, animNamePrefix);

            // This is a little bit of abuse here, ideally we would get the name
            // somewhere else.
            _widgetlook.AddAnimationName(
                animNamePrefix +
                attributes.GetValueAsString("name"));
        }

        //! Function to handle EventLinkDefinition elements.
        private void elementEventLinkDefinitionStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        //! Function to handle EventLinkTarget elements.
        private void elementEventLinkTargetStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Function to handle NamedAreaSource elements.
        /// </summary>
        /// <param name="attributes"></param>
        private void elementNamedAreaSourceStart(XMLAttributes attributes)
        {
            Debug.Assert(_area != null);

            var look = attributes.GetValueAsString(LookAttribute);

            _area.SetNamedAreaSouce(String.IsNullOrEmpty(look)
                                        ? _widgetlook.GetName()
                                        : look,
                                    attributes.GetValueAsString(NameAttribute));
        }

        //! Function to handle EventAction elements.
        private void elementEventActionStart(XMLAttributes attributes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method that handles the closing Falagard XML element.
        /// </summary>
        private void elementFalagardEnd()
        {
            System.GetSingleton().Logger.LogEvent("===== Look and feel parsing completed =====");
        }

        /// <summary>
        /// Method that handles the closing WidgetLook XML element.
        /// </summary>
        private void ElementWidgetLookEnd()
        {
            if (_widgetlook != null)
            {
                System.GetSingleton()
                      .Logger.LogEvent("---< End of definition for widget look '" + _widgetlook.GetName() + "'.",
                                       LoggingLevel.Informative);
                _manager.AddWidgetLook(_widgetlook);
                // TODO: CEGUI_DELETE_AO d_widgetlook;
                _widgetlook = null;
            }
        }

        /// <summary>
        /// Method that handles the closing Child XML element.
        /// </summary>
        private void ElementChildEnd()
        {
            Debug.Assert(_widgetlook != null);

            if (_childcomponent!=null)
            {
                Logger.LogInsane("-----< End of definition for child widget. Type: " + _childcomponent.GetBaseWidgetType() + ".");
                _widgetlook.AddWidgetComponent(_childcomponent);
                // TODO: CEGUI_DELETE_AO d_childcomponent;
                _childcomponent = null;
            }
        }

        /// <summary>
        /// Method that handles the closing ImagerySection XML element.
        /// </summary>
        private void ElementImagerySectionEnd()
        {
            Debug.Assert(_widgetlook != null);

            if (_imagerysection != null)
            {
                Logger.LogInsane("-----< End of definition for imagery section '" + _imagerysection.GetName() + "'.");
                _widgetlook.AddImagerySection(_imagerysection);
                // TODO: CEGUI_DELETE_AO d_imagerysection;
                _imagerysection = null;
            }
        }

        /// <summary>
        /// Method that handles the closing StateImagery XML element.
        /// </summary>
        private void ElementStateImageryEnd()
        {
            Debug.Assert(_widgetlook != null);

            if (_stateimagery!=null)
            {
                Logger.LogInsane("-----< End of definition for imagery for state '" + _stateimagery.GetName() + "'.");
                _widgetlook.AddStateSpecification(_stateimagery);
                // TODO: CEGUI_DELETE_AO d_stateimagery;
                _stateimagery = null;
            }
        }

        /// <summary>
        /// Method that handles the closing Layer XML element.
        /// </summary>
        private void ElementLayerEnd()
        {
            Debug.Assert(_stateimagery != null);

            if (_layer!=null)
            {
                Logger.LogInsane("-------< End of definition of imagery layer.");
                _stateimagery.AddLayer(_layer);
                // TODO: CEGUI_DELETE_AO d_layer;
                _layer = null;
            }
        }

        /// <summary>
        /// Method that handles the closing Section XML element.
        /// </summary>
        private void ElementSectionEnd()
        {
            Debug.Assert(_layer != null);

            if (_section!=null)
            {
                _layer.AddSectionSpecification(_section);
                // TODO: CEGUI_DELETE_AO d_section;
                _section = null;
            }
        }

        /// <summary>
        /// Method that handles the closing ImageryComponent XML element.
        /// </summary>
        private void ElementImageryComponentEnd()
        {
            Debug.Assert(_imagerysection != null);

            if (_imagerycomponent!=null)
            {
                _imagerysection.AddImageryComponent(_imagerycomponent);
                // TODO: CEGUI_DELETE_AO d_imagerycomponent;
                _imagerycomponent = null;
            }
        }

        /// <summary>
        /// Method that handles the closing TextComponent XML element.
        /// </summary>
        private void ElementTextComponentEnd()
        {
            Debug.Assert(_imagerysection != null);

            if (_textcomponent!=null)
            {
                _imagerysection.AddTextComponent(_textcomponent);
                // TODO: CEGUI_DELETE_AO d_textcomponent;
                _textcomponent = null;
            }
        }

        /// <summary>
        /// Method that handles the closing FrameComponent XML element.
        /// </summary>
        private void ElementFrameComponentEnd()
        {
            Debug.Assert(_imagerysection != null);

            if (_framecomponent!=null)
            {
                _imagerysection.AddFrameComponent(_framecomponent);
                // TODO: CEGUI_DELETE_AO d_framecomponent;
                _framecomponent = null;
            }
        }

        /// <summary>
        /// Method that handles the closing Area XML element.
        /// </summary>
        private void ElementAreaEnd()
        {
            Debug.Assert(((_childcomponent != null) || (_imagerycomponent != null) || (_textcomponent != null) ||
                          _namedArea != null || _framecomponent != null));
            Debug.Assert(_area != null);

            if (_childcomponent != null)
            {
                _childcomponent.SetComponentArea(_area);
            }
            else if (_framecomponent != null)
            {
                _framecomponent.SetComponentArea(_area);
            }
            else if (_imagerycomponent != null)
            {
                _imagerycomponent.SetComponentArea(_area);
            }
            else if (_textcomponent != null)
            {
                _textcomponent.SetComponentArea(_area);
            }
            else if (_namedArea != null)
            {
                global::System.Diagnostics.Debug.Assert(_area!=null);
                _namedArea.SetArea(_area);
            }

            // TODO: CEGUI_DELETE_AO d_area;
            _area = null;
        }

        /// <summary>
        /// Method that handles the closing NamedArea XML element.
        /// </summary>
        private void ElementNamedAreaEnd()
        {
            Debug.Assert(_widgetlook != null);

            if (_namedArea!=null)
            {
                _widgetlook.AddNamedArea(_namedArea);
                // TODO: CEGUI_DELETE_AO d_namedArea;
                _namedArea = null;
            }
        }

        /// <summary>
        /// Method that handles the closing XML for all *Dim elements.
        /// </summary>
        private void ElementAnyDimEnd()
        {
            if (_dimStack.Count!=0)
            {
                var currDim = _dimStack.Pop();

                if (_dimStack.Count != 0)
                {
                    var op = _dimStack.Peek() as OperatorDim;
                    if (op!=null)
                        op.SetNextOperand(currDim);
                }
                else
                {
                    _dimension.SetBaseDimension(currDim);
                    AssignAreaDimension(new Dimension(_dimension.GetBaseDimension(),_dimension.GetDimensionType()));
                }

                // release the dim we popped.
                // TODO: CEGUI_DELETE_AO currDim;

            }
        }

        /// <summary>
        /// Function to handle closing PropertyLinkDefinition XML element.
        /// </summary>
        private void ElementPropertyLinkDefinitionEnd()
        {
            Debug.Assert(_propertyLink!=null);
            _widgetlook.AddPropertyLinkDefinition(_propertyLink);

            Logger.LogInsane("<----- End of PropertyLinkDefiniton. Name: " + _propertyLink.GetPropertyName());
            _propertyLink = null;
        }

        //! Function to handle closing EventLinkDefinition XML element.
        private void elementEventLinkDefinitionEnd()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Register a handler for the opening tag of an XML element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="handler"></param>
        private void registerElementStartHandler(string element, ElementStartHandler handler)
        {
            _startHandlersMap[element] = handler;
        }

        /// <summary>
        /// Register a handler for the closing tag of an XML element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="handler"></param>
        private void registerElementEndHandler(string element, ElementEndHandler handler)
        {
            _endHandlersMap[element] = handler;
        }

        /// <summary>
        /// helper to add an event link target as dictated by the input strings.
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="event"></param>
        private void ProcessEventLinkTarget(string widget, string @event)
        {
            throw new NotImplementedException();
        }

        /*************************************************************************
            Implementation Data
        *************************************************************************/
        private readonly WidgetLookManager _manager;

        // these are used to implement the handler without using a huge
        // if / else if /else construct, we just register the element name, and
        // handler member function, and everything else is done using those
        // mappings.
        private readonly Dictionary<string, ElementStartHandler> _startHandlersMap = new Dictionary<string, ElementStartHandler>();

        private readonly Dictionary<string, ElementEndHandler> _endHandlersMap = new Dictionary<string, ElementEndHandler>();

        // these hold pointers to various objects under construction.
        private WidgetLookFeel _widgetlook;
        private WidgetComponent _childcomponent;
        private ImagerySection _imagerysection;
        private StateImagery _stateimagery;
        private LayerSpecification _layer;
        private SectionSpecification _section;
        private ImageryComponent _imagerycomponent;
        private ComponentArea _area;
        private Dimension _dimension = new Dimension(); // ???
        private TextComponent _textcomponent;
        private NamedArea _namedArea;
        private FrameComponent _framecomponent;

        private readonly Stack<BaseDim> _dimStack = new Stack<BaseDim>();

        private IPropertyDefinition _propertyLink;
        private EventLinkDefinition d_eventLink;
    }
}