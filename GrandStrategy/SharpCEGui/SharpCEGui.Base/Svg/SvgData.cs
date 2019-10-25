using System;
using System.Collections.Generic;
using System.Globalization;
using Lunatics.Mathematics;

namespace SharpCEGui.Base.Svg
{
    /// <summary>
    /// Defines a class for the SVG data.
    /// 
    /// The SVGData class stores data of a vector graphics image based on the SVG Tiny 1.2
    /// file standard. The SVGData is utilised by the SVGImage class.
    /// 
    /// The data for this class can be either be created by parsing it from an SVG file
    /// or be added manually by a user to draw custom geometry.
    /// </summary>
    public class SvgData : ChainedXmlHandler
    {
        //! Enumerator describing the available unit types in the SVG standard for the length type
        public enum SvgUnit
        {
            SLU_UNDEFINED,
            SLU_IN,
            SLU_CM,
            SLU_MM,
            SLU_PT,
            SLU_PC,
            SLU_PX,
            SLU_PERCENT,

            SLU_COUNT
        };

        public class /*struct*/ SvgLength
        {
            public SvgLength()
            {
                d_value = 0f;
                d_unit = SvgUnit.SLU_UNDEFINED;
            }

            public float d_value;
            public SvgUnit d_unit;
        };

        public SvgData(string name)
        {
            d_name = name;
        }

        public SvgData(string name, string filename, string resourceGroup)
                : this(name)
        {
            loadFromFile(filename, resourceGroup);
        }

        // TODO: ~SvgData() { destroyShapes(); }

        /// <summary>
        /// Returns the name given to the SVGData when it was created.
        /// </summary>
        /// <returns>
        /// Reference to a String object that holds the name of the SVGData.
        /// </returns>
        public string getName()
        {
            return d_name;
        }

        /*!
        \brief
            Loads and parses the specified SVG file into this SVGData object.

        \param file_name
            The filename of the SVG file that is to be loaded.

        \param resource_group
            Resource group identifier to be passed to the resource provider when
            loading the image file.

        \return
            Nothing.
        */

        public void loadFromFile(string file_name, string resource_group)
        {
            System.GetSingleton().GetXMLParser().ParseXmlFile(this, file_name, "", resource_group, false);
        }

        /*!
        \brief
            Adds a SVGBasicShape to the list of shapes of this class. This class takes ownership
            of the passed object and will free the memory itself.
        \param svg_shape
            The SVGBasicShape that will be added.
        */

        public void addShape(SvgBasicShape svg_shape)
        {
            d_svgBasicShapes.Add(svg_shape);
        }

        /*!
        \brief
            Deletes all shapes in the list and clears the list.
        */

        public void destroyShapes()
        {
            //const unsigned int shape_count = d_svgBasicShapes.size();
            //for (unsigned int i = 0; i < shape_count; ++i)
            //    delete d_svgBasicShapes[i];

            d_svgBasicShapes.Clear();
        }

        /*!
        \brief
            Returns the list of shapes of this class.
        \return
            The list of shapes of this class.
        */

        public List<SvgBasicShape> getShapes()
        {
            return d_svgBasicShapes;
        }

        /*!
        \brief
            Returns the SVGData's width in pixels.
        \return
            The SVGData's width in pixels.
        */

        public float getWidth()
        {
            return d_width;
        }

        /*!
        \brief
            Sets the SVGData's width in pixels.
        \param width
            The width in pixels.
        */

        public void setWidth(float width)
        {
            d_width = width;
        }

        /*!
        \brief
            Returns the SVGData's height in pixels.
        \return
            The SVGData's height in pixels.
        */

        public float getHeight()
        {
            return d_height;
        }

        /*!
        \brief
            Sets the SVGData's height in pixels.
        \param width
            The height in pixels.
        */

        public void setHeight(float height)
        {
            d_height = height;
        }

        // implement chained xml handler abstract interface
        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            // handle SVG document fragment element
            if (element == SVGElement)
            {
                ElementSvgStart(attributes);
            }

            // handle SVG 'rect' element
            else if (element == SVGRectElement)
            {
                ElementSvgRect(attributes);
            }
            // handle SVG 'circle' fragment element
            else if (element == SVGCircleElement)
            {
                ElementSvgCircle(attributes);
            }
            // handle SVG 'ellipse' element
            else if (element == SVGEllipseElement)
            {
                ElementSvgEllipse(attributes);
            }
            // handle SVG 'line' element
            else if (element == SVGLineElement)
            {
                ElementSvgLine(attributes);
            }
            // handle SVG 'polyline' element
            else if (element == SVGPolylineElement)
            {
                ElementSvgPolyline(attributes);
            }
            // handle SVG document fragment element
            else if (element == SVGPolygonElement)
            {
                ElementSvgPolygon(attributes);
            }
        }

        protected override void ElementEndLocal(string element)
        {
        }

        /// <summary>
        /// Function that handles the opening SVG element.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This function processes the SVG document fragment which contains
        /// the SVG graphics elements, container elements, etc. ...
        /// </remarks>
        protected void ElementSvgStart(XMLAttributes attributes)
        {
            // Get the SVG version
            var version = attributes.GetValueAsString(SVGElementAttributeVersion);

            // TODO: ...
            // Currently we only support pixels as units and interpret everything as
            // such, probably some default conversion from inch/mm to pixels should happen
            var width = attributes.GetValueAsString(SVGElementAttributeWidth);
            d_width = parseLengthDataType(width).d_value;

            var height = attributes.GetValueAsString(SVGElementAttributeHeight);
            d_height = parseLengthDataType(height).d_value;
        }

        /// <summary>
        /// Function that handles opening SVG 'rect' elements.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This function processes the SVG 'rect' element.
        /// </remarks>
        protected void ElementSvgRect(XMLAttributes attributes)
        {
            var paintStyle = parsePaintStyle(attributes);
            var transform = parseTransform(attributes);

            var xString = attributes.GetValueAsString(SVGRectAttributeXPos, "0");
            var x = parseLengthDataType(xString).d_value;

            var yString = attributes.GetValueAsString(SVGRectAttributeYPos, "0");
            var y = parseLengthDataType(yString).d_value;

            var widthString = attributes.GetValueAsString(SVGRectAttributeWidth, "0");
            var width = parseLengthDataType(widthString).d_value;

            var heightString = attributes.GetValueAsString(SVGRectAttributeHeight, "0");
            var height = parseLengthDataType(heightString).d_value;

            var rxString = attributes.GetValueAsString(SVGRectAttributeRoundedX, "0");
            var rx = parseLengthDataType(rxString).d_value;

            var ryString = attributes.GetValueAsString(SVGRectAttributeRoundedY, "0");
            var ry = parseLengthDataType(ryString).d_value;

            var rect = new SvgRect(paintStyle, transform, x, y, width, height, rx, ry);
            addShape(rect);
        }

        /// <summary>
        /// Function that handles opening SVG 'circle' elements.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This function processes the SVG 'circle' element.
        /// </remarks>
        protected void ElementSvgCircle(XMLAttributes attributes)
        {
            var paintStyle = parsePaintStyle(attributes);
            var transform = parseTransform(attributes);

            var cxString = attributes.GetValueAsString(SVGCircleAttributeCX, "0");
            var cx = parseLengthDataType(cxString).d_value;

            var cyString = attributes.GetValueAsString(SVGCircleAttributeCY, "0");
            var cy = parseLengthDataType(cyString).d_value;

            var radiusString = attributes.GetValueAsString(SVGCircleAttributeRadius, "0");
            var radius = parseLengthDataType(radiusString).d_value;

            var circle = new SvgCircle(paintStyle, transform, cx, cy, radius);
            addShape(circle);
        }

        /// <summary>
        /// Function that handles opening SVG 'ellipse' elements.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This function processes the SVG 'ellipse' element.
        /// </remarks>
        protected void ElementSvgEllipse(XMLAttributes attributes)
        {
            var paintStyle = parsePaintStyle(attributes);
            var transform = parseTransform(attributes);

            var cxString = attributes.GetValueAsString(SVGEllipseAttributeCX, "0");
            var cx = parseLengthDataType(cxString).d_value;

            var cyString = attributes.GetValueAsString(SVGEllipseAttributeCY, "0");
            var cy = parseLengthDataType(cyString).d_value;

            var rxString = attributes.GetValueAsString(SVGEllipseAttributeRX, "0");
            var rx = parseLengthDataType(rxString).d_value;

            var ryString = attributes.GetValueAsString(SVGEllipseAttributeRY, "0");
            var ry = parseLengthDataType(ryString).d_value;

            var ellipse = new SvgEllipse(paintStyle, transform, cx, cy, rx, ry);
            addShape(ellipse);  
        }

        /// <summary>
        /// Function that handles opening SVG 'line' elements.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This function processes the SVG 'line' element.
        /// </remarks>
        protected void ElementSvgLine(XMLAttributes attributes)
        {
            var paintStyle = parsePaintStyle(attributes);
            var transform = parseTransform(attributes);

            var x1String = attributes.GetValueAsString(SVGLineAttributeX1, "0");
            var x1 = parseLengthDataType(x1String).d_value;

            var y1String = attributes.GetValueAsString(SVGLineAttributeY1, "0");
            var y1 = parseLengthDataType(y1String).d_value;

            var x2String= attributes.GetValueAsString(SVGLineAttributeX2, "0");
            var x2 = parseLengthDataType(x2String).d_value;

            var y2String = attributes.GetValueAsString(SVGLineAttributeY2, "0");
            var y2 = parseLengthDataType(y2String).d_value;

            var line = new SvgLine(paintStyle, transform, x1, y1, x2, y2);
            addShape(line);
        }

        /// <summary>
        /// Function that handles opening SVG 'polyline' elements.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This function processes the SVG 'polyline' element.
        /// </remarks>
        protected void ElementSvgPolyline(XMLAttributes attributes)
        {
            var paintStyle = parsePaintStyle(attributes);
            var transform = parseTransform(attributes);

            var pointsString = attributes.GetValueAsString(SVGPolylineAttributePoints, "");

            var points =new List<Vector2>();
            parsePointsString(pointsString, points);

            var polyline = new SvgPolyline(paintStyle, transform, points);
            addShape(polyline);
        }

        /// <summary>
        /// Function that handles opening SVG 'polygon' elements.
        /// </summary>
        /// <param name="attributes"></param>
        /// <remarks>
        /// This function processes the SVG 'polygon' element.
        /// </remarks>
        protected void ElementSvgPolygon(XMLAttributes attributes)
        {
            var paintStyle = parsePaintStyle(attributes);
            var transform = parseTransform(attributes);

            var pointsString = attributes.GetValueAsString(SVGPolylineAttributePoints, "");

            var points=new List<Vector2>();
            parsePointsString(pointsString, points);

            var polygon = new SvgPolygon(paintStyle, transform, points);
            addShape(polygon);
        }

        /// <summary>
        /// Name of this SVGData objects
        /// </summary>
        protected string d_name;

        /// <summary>
        /// The SVGData's width in pixels.
        /// 
        /// This is the value representing the intrinsic width of the 'SVG document fragment'. It is used in CEGUI
        /// to determine the clipping area of the SVG image and to scale the image elements in case the Image is
        /// rendered with horizontal stretching.
        /// </summary>
        private float d_width;

        /// <summary>
        /// The SVGData's height in pixels.
        /// 
        /// This is the value representing the intrinsic height of the 'SVG document fragment'. It is used in CEGUI
        /// to determine the clipping area of the SVG image and to scale the image elements in case the Image is
        /// rendered with vertical stretching.
        /// </summary>
        private float d_height;

        /// <summary>
        /// The basic shapes that were added to the SVGData
        /// </summary>
        private List<SvgBasicShape> d_svgBasicShapes = new List<SvgBasicShape>();

        /// <summary>
        /// Function that parses the paint style (fill and stroke parameters) from an SVG graphics element.
        /// </summary>
        /// <param name="attributes">
        /// The XML attributes from which the values will be parsed.
        /// </param>
        /// <returns></returns>
        private static SvgPaintStyle parsePaintStyle(XMLAttributes attributes)
        {
            var paintStyle = new SvgPaintStyle();

            // TODO: ...
            // unsupported/unspecified values should be inherited from the parents if possible, this
            // however would require adding an additional bool to every attribute member variable to check if
            // it is to be inherited or not
            var fillString = attributes.GetValueAsString(SVGGraphicsElementAttributeFill);
            parsePaintStyleFill(fillString, paintStyle);

            var fillRuleString = attributes.GetValueAsString(SVGGraphicsElementAttributeFillRule);
            parsePaintStyleFillRule(fillRuleString, paintStyle);

            var fillOpacityString = attributes.GetValueAsString(SVGGraphicsElementAttributeFillOpacity);
            parsePaintStyleFillOpacity(fillOpacityString, paintStyle);

            var strokeString = attributes.GetValueAsString(SVGGraphicsElementAttributeStroke);
            parsePaintStyleStroke(strokeString, paintStyle);

            var strokeWidthString = attributes.GetValueAsString(SVGGraphicsElementAttributeStrokeWidth, "1");
            parsePaintStyleStrokeWidth(strokeWidthString, paintStyle);

            var strokeLinecapString = attributes.GetValueAsString(SVGGraphicsElementAttributeStrokeLinecap, "butt");
            parsePaintStyleStrokeLinecap(strokeLinecapString, paintStyle);

            var strokeLinejoinString = attributes.GetValueAsString(SVGGraphicsElementAttributeStrokeLinejoin, "miter");
            parsePaintStyleStrokeLinejoin(strokeLinejoinString, paintStyle);

            var strokeMiterLimitString= attributes.GetValueAsString(SVGGraphicsElementAttributeStrokeMiterLimit, "4");
            parsePaintStyleMiterlimitString(strokeMiterLimitString, paintStyle);

            var strokeDashArrayString = attributes.GetValueAsString(SVGGraphicsElementAttributeStrokeDashArray);
            parsePaintStyleStrokeDashArray(strokeDashArrayString, paintStyle);

            var strokeDashOffsetString = attributes.GetValueAsString(SVGGraphicsElementAttributeStrokeDashOffset);
            parsePaintStyleStrokeDashOffset(strokeDashOffsetString, paintStyle);

            var strokeOpacityString = attributes.GetValueAsString(SVGGraphicsElementAttributeStrokeOpacity);
            parsePaintStyleStrokeOpacity(strokeOpacityString, paintStyle);

            return paintStyle;
        }

        //! Parses the String value of a 'fill' property 
        private static void parsePaintStyleFill(string fillString, SvgPaintStyle paintStyle)
        {
            if (fillString.Equals("none"))
                paintStyle.d_fill.d_none = true;
            else if (String.IsNullOrEmpty(fillString))
            {
                // Inherit value or use default
                paintStyle.d_fill.d_none = false;
                paintStyle.d_fill.d_colour = parseColour("black");
            }
            else
            {
                paintStyle.d_fill.d_none = false;
                paintStyle.d_fill.d_colour = parseColour(fillString);
            }
        }

        //! Parses the String value of a 'fill-rule' property 
        private static void parsePaintStyleFillRule(string fillRuleString, SvgPaintStyle paintStyle)
        {
            if (String.IsNullOrEmpty(fillRuleString))
                // Inherit value or use default
                paintStyle.d_fillRule = PolygonFillRule.NonZero;
            else if (fillRuleString.Equals("nonzero"))
                paintStyle.d_fillRule = PolygonFillRule.NonZero;
            else if (fillRuleString.Equals("evenodd"))
                paintStyle.d_fillRule = PolygonFillRule.EvenOdd;
        }

        //! Parses the String value of a 'fill-opacity' property
        private static void parsePaintStyleFillOpacity(string fillOpacityString, SvgPaintStyle paintStyle)
        {
            if (String.IsNullOrEmpty(fillOpacityString))
            {
                // Inherit value or use default
                paintStyle.d_fillOpacity = 1.0f;
            }
            else
            {
                paintStyle.d_fillOpacity = Single.Parse(fillOpacityString);
                //! Clamp value in each case without throwing a warning if the values are below 0 or above 1
                paintStyle.d_fillOpacity = Math.Min(Math.Max(0.0f, paintStyle.d_fillOpacity), 1.0f);
            }
        }

        //! Parses the String value of a 'stroke' property 
        private static void parsePaintStyleStroke(string strokeString, SvgPaintStyle paintStyle)
        {
            if (String.IsNullOrEmpty(strokeString) || strokeString.Equals("none"))
                paintStyle.d_stroke.d_none = true;
            else
            {
                paintStyle.d_stroke.d_none = false;
                paintStyle.d_stroke.d_colour = parseColour(strokeString);
            }
        }

        //! Parses the String value of a 'stroke-width' property 
        private static void parsePaintStyleStrokeWidth(string strokeWidthString, SvgPaintStyle paintStyle)
        {
            paintStyle.d_strokeWidth = Single.Parse(strokeWidthString);
            if(paintStyle.d_strokeWidth < 0.0f)
            {
                paintStyle.d_strokeWidth = 1.0f;
                System.GetSingleton()
                      .Logger.LogEvent(
                              "SVGData::parsePaintStyle - An unsupported value for stroke-width was specified in the SVG file. The value is set to the initial value '1'.",
                              LoggingLevel.Errors);
            }
        }

        //! Parses the String value of a 'stroke-linecap' property 
        private static void parsePaintStyleStrokeLinecap(string strokeLinecapString, SvgPaintStyle paintStyle)
        {
            if(strokeLinecapString.Equals("butt"))
                paintStyle.d_strokeLinecap = SvgPaintStyle.SVGLinecap.SLC_BUTT;
            else if(strokeLinecapString.Equals("round"))
                paintStyle.d_strokeLinecap = SvgPaintStyle.SVGLinecap.SLC_ROUND;
            else if (strokeLinecapString.Equals("square"))
                paintStyle.d_strokeLinecap = SvgPaintStyle.SVGLinecap.SLC_SQUARE;
            else
                throw new Exception(
                        "SVG file parsing was aborted because of an invalid value for the SVG 'linecap' type");
        }

        //! Parses the String value of a 'stroke-linejoin' property 
        private static void parsePaintStyleStrokeLinejoin(string strokeLinejoinString, SvgPaintStyle paintStyle)
        {
            if(strokeLinejoinString.Equals("miter"))
                paintStyle.d_strokeLinejoin = SvgPaintStyle.SVGLinejoin.SLJ_MITER;
            else if (strokeLinejoinString.Equals("round"))
                paintStyle.d_strokeLinejoin = SvgPaintStyle.SVGLinejoin.SLJ_ROUND;
            else if (strokeLinejoinString.Equals("bevel"))
                paintStyle.d_strokeLinejoin = SvgPaintStyle.SVGLinejoin.SLJ_BEVEL;
            else
                throw new Exception("SVG file parsing was aborted because of an invalid value for the SVG 'linejoin' type");
        }

        //! Parses the String value of a 'stroke-miterlimit' property 
        private static void parsePaintStyleMiterlimitString(string strokeMiterLimitString, SvgPaintStyle paintStyle)
        {
            paintStyle.d_strokeMiterlimit = Single.Parse(strokeMiterLimitString);
            if (paintStyle.d_strokeMiterlimit < 1.0f)
            {
                paintStyle.d_strokeMiterlimit = 4.0f;
                System.GetSingleton()
                      .Logger.LogEvent(
                              "SVGData::parsePaintStyle - An unsupported value for stroke-miterlimit was specified in the SVG file. The value is set to the initial value '4'.",
                              LoggingLevel.Errors);
            }
        }

        //! Parses the String value of a 'stroke-dasharray' property 
        private static void parsePaintStyleStrokeDashArray(string strokeDashArrayString, SvgPaintStyle paintStyle)
        {
            if (strokeDashArrayString.Equals("none"))
            {
                paintStyle.d_strokeDashArrayNone = true;
                paintStyle.d_strokeDashArray.Clear();
            }
            else
            {
                paintStyle.d_strokeDashArray = parseListOfLengths(strokeDashArrayString);

                var dashArraySize = paintStyle.d_strokeDashArray.Count;
                paintStyle.d_strokeDashArrayNone = dashArraySize != 0;
                //! If an odd number of values is provided, then the list of values shall be repeated to yield an even number of values
                if (paintStyle.d_strokeDashArrayNone == false && (dashArraySize%2) == 1)
                    paintStyle.d_strokeDashArray.AddRange(paintStyle.d_strokeDashArray);
            }
        }

        //! Parses the String value of a 'stroke-dashoffset' property 
        private static void parsePaintStyleStrokeDashOffset(string strokeDashOffsetString, SvgPaintStyle paint_style)
        {
            if (String.IsNullOrEmpty(strokeDashOffsetString))
                paint_style.d_strokeDashOffset = 0.0f;
            else
                paint_style.d_strokeDashOffset = Single.Parse(strokeDashOffsetString);
        }

        //! Parses the String value of a 'stroke-opacity' property 
        private static void parsePaintStyleStrokeOpacity(string strokeOpacityString, SvgPaintStyle paint_style)
        {
            if(String.IsNullOrEmpty(strokeOpacityString))
                paint_style.d_strokeOpacity = 1.0f;
            else
            {
                paint_style.d_strokeOpacity = Single.Parse(strokeOpacityString);
                //! Clamp value without ever throwing a warning
                paint_style.d_strokeOpacity = Math.Min(Math.Max(0.0f, paint_style.d_strokeOpacity), 1.0f);
            }
        }

        /*!
        \brief
            Function that parses a String into an SVG length object and returns it.

        \param length_string
            The String containing the characters that should be parsed into an SVG length.

        \return
            The SVGLength object.

        \exception SVGParsingException          thrown if there was some problem parsing the String.
        */

        private static SvgLength parseLengthDataType(string length_string)
        {
            throw new NotImplementedException();
            //var length = new SvgLength();
            //char lengthEnding[3] = "";
            //String unitString;

            //sscanf(length_string.c_str(), "%f%2s", &length.d_value, lengthEnding);
            //unitString = lengthEnding;

            //if(unitString.empty())
            //    return length;
            //else if(unitString.length() == 2)
            //{
            //    if(unitString.compare("in") == 0)
            //        length.d_unit = SLU_IN;
            //    else if(unitString.compare("cm") == 0)
            //        length.d_unit = SLU_CM;
            //    else if(unitString.compare("mm") == 0)
            //        length.d_unit = SLU_MM;
            //    else if(unitString.compare("pt") == 0)
            //        length.d_unit = SLU_PT;
            //    else if(unitString.compare("pc") == 0)
            //        length.d_unit = SLU_PC;
            //    else if(unitString.compare("px") == 0)
            //        length.d_unit = SLU_PX;
            //}
            //else if (unitString.length() == 1)
            //{
            //    if (unitString.compare("%") == 0)
            //        length.d_unit = SLU_PERCENT;
            //}
            //else
            //{
            //    // Parse error
            //    throw new global::System.Exception("SVG file parsing was aborted because of an invalid value of an SVG 'length' type");
            //}

            //return length;
        }

        //! Parses the String value of a 'points' property 
        private static void parsePointsString(string pointsString, List<Vector2> points)
        {
            throw new NotImplementedException();
            //const char* currentStringSegment = pointsString.c_str();
            //int offset;
            //glm::vec2 currentPoint;

            //while (true)
            //{
            //    int successful_args = sscanf(currentStringSegment, " %f , %f%n", &currentPoint.x, &currentPoint.y, &offset);

            //    if (successful_args == 2)
            //    {
            //        points.Add(currentPoint);
            //        currentStringSegment += offset;
            //    }
            //    else if (successful_args == 1)
            //    {
            //        points.Clear();
            //        currentStringSegment += offset;
            //    }
            //    else
            //        break;
            //}
        }

        /// <summary>
        /// Function that parses a 'transform' attribute and creates a mat3x3 from it.
        /// </summary>
        /// <param name="attributes">
        /// The XML attributes from which the values will be parsed.
        /// </param>
        /// <returns></returns>
        private static Matrix3x3 parseTransform(XMLAttributes attributes)
        {
            var transformString = attributes.GetValueAsString(SVGTransformAttribute);
            throw new NotImplementedException();
            //const char* transformStringSegment = transformString.c_str();
            //int offset = 0;
            //// Unity matrix is our default/basis
            //var currentMatrix = Matrix3x3.Identity;

            //if(sscanf(transformStringSegment, " matrix( %n", &offset) == 0 && offset != 0)
            //{
            //    transformStringSegment += offset;
            //    float matrixValues[6];

            //    int i = 0;
            //    while( (i < 6) && ( sscanf(transformStringSegment, " %f %n", &matrixValues[i],  &offset) == 1 || 
            //                        sscanf(transformStringSegment, " , %f %n", &matrixValues[i],  &offset) == 1 ) )
            //    {
            //        transformStringSegment += offset;
            //        ++i;
            //    }
        
            //    //If we parsed the expected amount of matrix elements we will multiply the matrix to our transformation
            //    if (i == 6)
            //        currentMatrix *= new Matrix3x3(matrixValues[0], matrixValues[2], matrixValues[4],
            //                                       matrixValues[1], matrixValues[3], matrixValues[5],
            //                                       0.0f, 0.0f, 1.0f);
            //}

            //return currentMatrix;
        }

        /*!
        \brief
            Function that parses the an SVG list of lengths from a String.
        \param list_of_lengths_string
            The String containing the list of lengths.
        */

        public static List<float> parseListOfLengths(string list_of_lengths_string)
        {
            throw new NotImplementedException();
            //std::vector<float> list_of_lengths;
            //const char* lengths_array_pointer = list_of_lengths_string.c_str();
            //float value;
            //int offset;

            //while (1 == sscanf(lengths_array_pointer, " %f ,%n", &value, &offset))
            //{
            //    list_of_lengths.push_back(value);
            //    lengths_array_pointer += offset;
            //}

            //return list_of_lengths;
        }

        /// <summary>
        /// Function that parses an SVG paint colour and returns the CEGUI Colour created from it.
        /// </summary>
        /// <param name="colourString">
        /// The colour String value from which the colour values will be parsed;
        /// </param>
        /// <returns></returns>
        private static Vector3 parseColour(string colourString)
        {
            if(colourString.StartsWith("#"))
            {
                var colour = new Vector3();

                if(colourString.Length == 7)
                {
                    var value = Int32.Parse(colourString.Substring(1, 2), NumberStyles.AllowHexSpecifier);
                    colour.X = value / 255.0f;
                    value = Int32.Parse(colourString.Substring(3, 2), NumberStyles.AllowHexSpecifier);
                    colour.Y = value / 255.0f;
                    value = Int32.Parse(colourString.Substring(5, 2), NumberStyles.AllowHexSpecifier);
                    colour.Z = value / 255.0f;

                    return colour;
                }
                
                if(colourString.Length == 4)
                {
                    var modColourString = "" +
                                            colourString[1] + colourString[1] +
                                            colourString[2] + colourString[2] +
                                            colourString[3] + colourString[3];

                    var value = Int32.Parse(modColourString.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                    colour.X = value / 255.0f;
                    value = Int32.Parse(modColourString.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                    colour.Y = value / 255.0f;
                    value = Int32.Parse(modColourString.Substring(4, 2), NumberStyles.AllowHexSpecifier);
                    colour.Z = value / 255.0f;

                    return colour;
                }
            }
            else if (colourString.StartsWith("rgb("))
            {
                var modColourString = colourString.Substring(4, colourString.Length - 5);
                var colors = modColourString.Split(',');
                int r = Int32.Parse(colors[0]);
                int g = Int32.Parse(colors[1]);
                int b = Int32.Parse(colors[2]);

                return new Vector3(r/255.0f, g/255.0f, b/255.0f);
            }
                    // SVG's default colours
            else if (colourString.Equals("black"))
                return new Vector3(0.0f, 0.0f, 0.0f);
            else if (colourString.Equals("green"))
                return new Vector3(0.0f, 128.0f/255.0f, 0.0f);
            else if (colourString.Equals("silver"))
                return new Vector3(192.0f/255.0f, 192.0f/255.0f, 192.0f/255.0f);
            else if (colourString.Equals("lime"))
                return new Vector3(0.0f, 1.0f, 0.0f);
            else if (colourString.Equals("gray"))
                return new Vector3(128.0f/255.0f, 128.0f/255.0f, 128.0f/255.0f);
            else if (colourString.Equals("olive"))
                return new Vector3(128.0f/255.0f, 128.0f/255.0f, 0f);
            else if (colourString.Equals("white"))
                return new Vector3(1.0f, 1.0f, 1.0f);
            else if (colourString.Equals("yellow"))
                return new Vector3(1.0f, 1.0f, 0.0f);
            else if (colourString.Equals("maroon"))
                return new Vector3(128.0f/255.0f, 0.0f, 0.0f);
            else if (colourString.Equals("navy"))
                return new Vector3(0.0f, 0.0f, 128.0f/255.0f);
            else if (colourString.Equals("red"))
                return new Vector3(1.0f, 0.0f, 0.0f);
            else if (colourString.Equals("blue"))
                return new Vector3(0.0f, 0.0f, 1.0f);
            else if (colourString.Equals("purple"))
                return new Vector3(128.0f/255.0f, 0.0f, 128.0f/255.0f);
            else if (colourString.Equals("teal"))
                return new Vector3(0.0f, 128.0f/255.0f, 128.0f/255.0f);
            else if (colourString.Equals("fuchsia"))
                return new Vector3(1.0f, 0.0f, 1.0f);
            else if (colourString.Equals("aqua"))
                return new Vector3(0.0f, 1.0f, 1.0f);

            // Parse error
            throw new Exception("SVG file parsing was aborted because of an invalid colour value");
        }

        #region  Internal Strings holding XML element and attribute names

        private const string ImagesetSchemaName = "Imageset.xsd";
        //SVG elements and attributes
        private const string SVGElement = "svg";
        private const string SVGElementAttributeVersion = "version";
        private const string SVGElementAttributeWidth = "width";
        private const string SVGElementAttributeHeight = "height";

        //SVG Basic Shapes
        private const string SVGRectElement = "rect";
        private const string SVGCircleElement = "circle";
        private const string SVGEllipseElement = "ellipse";
        private const string SVGLineElement = "line";
        private const string SVGPolylineElement = "polyline";
        private const string SVGPolygonElement = "polygon";

        // SVG graphics elements paint attributes
        private const string SVGGraphicsElementAttributeFill = "fill";
        private const string SVGGraphicsElementAttributeFillRule = "fill-rule";
        private const string SVGGraphicsElementAttributeFillOpacity = "fill-opacity";
        private const string SVGGraphicsElementAttributeStroke = "stroke";
        private const string SVGGraphicsElementAttributeStrokeWidth = "stroke-width";
        private const string SVGGraphicsElementAttributeStrokeLinecap = "stroke-linecap";
        private const string SVGGraphicsElementAttributeStrokeLinejoin = "stroke-linejoin";
        private const string SVGGraphicsElementAttributeStrokeMiterLimit = "stroke-miterlimit";
        private const string SVGGraphicsElementAttributeStrokeDashArray = "stroke-dasharray";
        private const string SVGGraphicsElementAttributeStrokeDashOffset = "stroke-dashoffset";
        private const string SVGGraphicsElementAttributeStrokeOpacity = "stroke-opacity";

        // SVG transform attribute
        private const string SVGTransformAttribute = "transform";

        // SVG 'rect' element attributes
        private const string SVGRectAttributeXPos = "x";
        private const string SVGRectAttributeYPos = "y";
        private const string SVGRectAttributeWidth = "width";
        private const string SVGRectAttributeHeight = "height";
        private const string SVGRectAttributeRoundedX = "rx";
        private const string SVGRectAttributeRoundedY = "ry";
        // SVG 'circle' element attributes
        private const string SVGCircleAttributeCX = "cx";
        private const string SVGCircleAttributeCY = "cy";
        private const string SVGCircleAttributeRadius = "r";
        // SVG 'ellipse' element attributes
        private const string SVGEllipseAttributeCX = "cx";
        private const string SVGEllipseAttributeCY = "cy";
        private const string SVGEllipseAttributeRX = "rx";
        private const string SVGEllipseAttributeRY = "ry";
        // SVG 'polyline' element attributes
        private const string SVGPolylineAttributePoints = "points";
        // SVG 'polyline' element attributes
        private const string SVGLineAttributeX1 = "x1";
        private const string SVGLineAttributeY1 = "y1";
        private const string SVGLineAttributeX2 = "x2";
        private const string SVGLineAttributeY2 = "y2";

        #endregion
    }
}