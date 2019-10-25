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

using System.Collections.Generic;
using Lunatics.Mathematics;

namespace SharpCEGui.Base.Svg
{
    /// <summary>
    /// Defines a class that acts as interface for several classes that store the data of SVG BasicShapes.
    /// </summary>
    public abstract class SvgBasicShape
    {
        //protected SvgBasicShape()
        //{
        //}

        protected SvgBasicShape(SvgPaintStyle paintStyle, Matrix3x3 transformation)
        {
            PaintStyle = paintStyle;
            Transformation = transformation;
        }

        /// <summary>
        /// Renders the shape into a new geometry buffer and adds it to the list.
        /// </summary>
        /// <param name="renderSettings">
        /// The ImageRenderSettings that contain render settings for new GeometryBuffers.
        /// </param>
        public abstract List<GeometryBuffer> CreateRenderGeometry(SvgImage.SvgImageRenderSettings renderSettings);

        /// <summary>
        /// The BasicShape's style, which describes the filling and stroke of the graphical element.
        /// </summary>
        public SvgPaintStyle PaintStyle;

        /// <summary>
        /// The matrix transformation to apply to the element.
        /// </summary>
        public Matrix3x3 Transformation;
    }

    /// <summary>
    /// Defines a class for storing the data of the SVG 'rect' element based on how it is defined in the SVG standard.
    /// 
    /// The 'rect' element defines a rectangle which is axis-aligned with the current user coordinate system.
    /// Rounded rectangles can be achieved by setting appropriate values for attributes 'rx' and 'ry'.
    /// http://www.w3.org/TR/SVGTiny12/shapes.html#RectElement
    /// </summary>
    public class SvgRect : SvgBasicShape
    {
        //public SvgRect()
        //{
        //}

        public SvgRect(SvgPaintStyle paintStyle, Matrix3x3 transformation,
                       float x, float y, float width, float height, float rx = 0.0f, float ry = 0.0f)
                : base(paintStyle, transformation)
        {
            d_x = x;
            d_y = y;
            d_width = width;
            d_height = height;
            d_rx = rx;
            d_ry = ry;
        }

        //! Implementation of SVGBasicShape interface
        public override List<GeometryBuffer> CreateRenderGeometry(SvgImage.SvgImageRenderSettings renderSettings)
        {
            return SvgTessellator.TesselateRect(this, renderSettings);
        }

        //! The x-axis coordinate of the side of the rectangle which has the smaller x-axis coordinate value in the current user coordinate system
        public float d_x;

        //! The y-axis coordinate of the side of the rectangle which has the smaller y-axis coordinate value in the current user coordinate system.
        public float d_y;
        
        //! The width of the rectangle. A negative value is unsupported. A value of zero disables rendering of the element.
        public float d_width;
        
        //! The height of the rectangle. A negative value is unsupported. A value of zero disables rendering of the element.
        public float d_height;
        
        //! For rounded rectangles, the x-axis radius of the ellipse used to round off the corners of the rectangle. A negative value is unsupported.
        public float d_rx;
        
        //! For rounded rectangles, the y-axis radius of the ellipse used to round off the corners of the rectangle. A negative value is unsupported.
        public float d_ry;
    }

    /// <summary>
    /// Defines a class for storing the data of the SVG 'circle' element based on how it is defined in the SVG standard.
    /// 
    /// The 'circle' element defines a circle based on a center point and a radius.
    /// http://www.w3.org/TR/SVGTiny12/shapes.html#CircleElement
    /// </summary>
    public class SvgCircle : SvgBasicShape
    {
        //public SvgCircle()
        //{
        //}

        public SvgCircle(SvgPaintStyle paintStyle,
                         Matrix3x3 transformation,
                         float cx,
                         float cy,
                         float r) : base(paintStyle, transformation)
        {
            d_cx = cx;
            d_cy = cy;
            d_r = r;
        }

        //! Implementation of SVGBasicShape interface
        public override List<GeometryBuffer> CreateRenderGeometry(SvgImage.SvgImageRenderSettings renderSettings)
        {
            return SvgTessellator.TesselateCircle(this, renderSettings);
        }

        //! The x-axis coordinate of the center of the circle. 
        public float d_cx;

        //! The y-axis coordinate of the center of the circle. 
        public float d_cy;

        //! The radius of the circle. A negative value is unsupported. Default = 0.
        public float d_r;
    }

    /// <summary>
    /// Defines a class for storing the data of the SVG 'ellipse' element based on how it is defined in the SVG standard.
    /// 
    /// The 'ellipse' element defines an ellipse which is axis-aligned with the current user coordinate system based on a center point and two radii.
    /// http://www.w3.org/TR/SVGTiny12/shapes.html#EllipseElement
    /// </summary>
    public class SvgEllipse : SvgBasicShape
    {
        //public SvgEllipse()
        //{
        //}

        public SvgEllipse(SvgPaintStyle paintStyle,
                          Matrix3x3 transformation,
                          float cx,
                          float cy,
                          float rx,
                          float ry) : base(paintStyle, transformation)
        {
            d_cx = cx;
            d_cy = cy;
            d_rx = rx;
            d_ry = ry;
        }

        //! Implementation of SVGBasicShape interface
        public override List<GeometryBuffer> CreateRenderGeometry(SvgImage.SvgImageRenderSettings renderSettings)
        {
            return SvgTessellator.tesselateEllipse(this, renderSettings);
        }

        //! The x-axis coordinate of the center of the ellipse. 
        public float d_cx;
        
        //! The y-axis coordinate of the center of the ellipse. 
        public float d_cy;
        
        //! The x-axis radius of the ellipse. A negative value is unsupported. Default = 0.
        public float d_rx;
        
        //! The y-axis radius of the ellipse. A negative value is unsupported. Default = 0.
        public float d_ry;
    }

    /// <summary>
    /// Defines a class for storing the data of the SVG 'line' element based on how it is defined in the SVG standard.
    /// 
    /// The 'line' element defines a line segment that starts at one point and ends at another.
    /// http://www.w3.org/TR/SVGTiny12/shapes.html#LineElement
    /// </summary>
    public class SvgLine : SvgBasicShape
    {
        //public SvgLine()
        //{
        //}

        //! Constructors
        public SvgLine(SvgPaintStyle paintStyle, Matrix3x3 transformation, float x1, float y1, float x2, float y2)
                : base(paintStyle, transformation)
        {
            d_x1 = x1;
            d_y1 = y1;
            d_x2 = x2;
            d_y2 = y2;
        }

        public SvgLine(SvgPaintStyle paintStyle, Matrix3x3 transformation, Vector2 lineStart, Vector2 lineEnd)
                : this(paintStyle, transformation, lineStart.X, lineStart.Y, lineEnd.X, lineEnd.Y)
        {
        }

        //! Implementation of SVGBasicShape interface
        public override List<GeometryBuffer> CreateRenderGeometry(SvgImage.SvgImageRenderSettings renderSettings)
        {
            return SvgTessellator.TesselateLine(this, renderSettings);
        }

        //! The x-axis coordinate of the start of the line
        public float d_x1;
        
        //! The y-axis coordinate of the start of the line
        public float d_y1;
        
        //! The x-axis coordinate of the end of the line
        public float d_x2;
        
        //! The y-axis coordinate of the end of the line
        public float d_y2;
    }

    /// <summary>
    /// Defines a class for storing the data of the SVG 'polyline' element based on how it is defined in the SVG standard.
    /// 
    /// The 'polyline' element defines a set of connected straight line segments.
    /// Typically, 'polyline' elements define open shapes.
    /// http://www.w3.org/TR/SVGTiny12/shapes.html#PolylineElement
    /// </summary>
    public class SvgPolyline : SvgBasicShape
    {
        //public SvgPolyline()
        //{
        //}

        //! Constructor
        public SvgPolyline(SvgPaintStyle paintStyle, Matrix3x3 transformation, List<Vector2> points)
                : base(paintStyle, transformation)
        {
            d_points = points;
        }

        //! Implementation of SVGBasicShape interface
        public override List<GeometryBuffer> CreateRenderGeometry(SvgImage.SvgImageRenderSettings renderSettings)
        {
            return SvgTessellator.TesselatePolyline(this, renderSettings);
        }

        //! The points defining the line
        public List<Vector2> d_points;
    }

    /// <summary>
    /// Defines a class for storing the data of the SVG 'polygon' element based on how it is defined in the SVG standard.
    /// 
    /// The 'polygon' element defines a closed shape consisting of a set of connected straight line segments.
    /// http://www.w3.org/TR/SVGTiny12/shapes.html#PolygonElement
    /// </summary>
    public class SvgPolygon : SvgBasicShape
    {
        //public SvgPolygon()
        //{
        //}

        //! Constructor
        public SvgPolygon(SvgPaintStyle paintStyle, Matrix3x3 transformation, List<Vector2> points)
                : base(paintStyle, transformation)
        {
            d_points = points;
        }

        //! Implementation of SVGBasicShape interface
        public override List<GeometryBuffer> CreateRenderGeometry(SvgImage.SvgImageRenderSettings renderSettings)
        {
            return SvgTessellator.tesselatePolygon(this, renderSettings);
        }

        //! The points defining the line
        public List<Vector2> d_points;
    }
}