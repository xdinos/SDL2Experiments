using System.Collections.Generic;
using Lunatics.Mathematics;

namespace SharpCEGui.Base.Svg
{
    /// <summary>
    /// Defines the 'paint' type based on how it is used in SVG. Main purpose is for defining how
    /// 'fill' and 'stroke' are to be drawn.
    /// </summary>
    public class SvgPaint
    {
        public SvgPaint()
        {
            d_none = false;
            d_colour = Vector3.Zero;
        }

        //! Defines if the paint is to be drawn at all.
        public bool d_none;

        //! Defines the colour of the paint.
        public Vector3 d_colour;
    }

    /// <summary>
    /// Defines the SVGShapeStyle class, which describes the overall shape style of an SVG shape.
    /// </summary>
    public class SvgPaintStyle
    {
        /// <summary>
        /// Specifies the shape which shall be used at the end of open subpaths when they are stroked.
        /// </summary>
        public enum SVGLinecap
        {
            /// <summary>
            /// A simple linear cap through the endpoint.
            /// </summary>
            SLC_BUTT,

            /// <summary>
            /// A rounded cap with the endpoint as center.
            /// </summary>
            SLC_ROUND,
            
            /// <summary>
            /// A simple linear cap that is offset from the endpoint by the stroke width.
            /// </summary>
            SLC_SQUARE,

            SLC_COUNT
        }

        /// <summary>
        /// Specifies the shape which shall be used at the corners of shapes when they are stroked. 
        /// </summary>
        public enum SVGLinejoin
        {
            /// <summary>
            /// Makes two lines join at the intersection points of their outlines. This can be influenced
            /// by the value set for stroke-miterlimit. In the case the miter is exceedingly long a bevel
            /// linejoin will then be used for that corner.
            /// </summary>
            SLJ_MITER,
            
            /// <summary>
            /// A rounded linejoin.
            /// </summary>
            SLJ_ROUND,
            
            /// <summary>
            /// A linejoin with two corners.
            /// </summary>
            SLJ_BEVEL,

            SLJ_COUNT
        };

        public SvgPaintStyle()
        {
            d_fill = new SvgPaint();
            d_fillRule = PolygonFillRule.NonZero;
            d_fillOpacity = 1.0f;
            d_stroke = new SvgPaint();
            d_strokeWidth = 1.0f;
            d_strokeLinecap = SVGLinecap.SLC_BUTT;
            d_strokeLinejoin = SVGLinejoin.SLJ_MITER;
            d_strokeMiterlimit = 4.0f;
            d_strokeDashArray = new List<float>();
            d_strokeDashOffset = 0.0f;
            d_strokeOpacity = 1.0f;
        }

        //! The 'fill' property defines how the interior of a graphical element must be painted.
        public SvgPaint d_fill;

        /*!
    \brief
        The 'fill-rule' property indicates the algorithm which must be used to determine what parts
        of the canvas are included inside the shape.
    */
        public PolygonFillRule d_fillRule;

        /*!
    \brief
        The opacity setting that will be applied to the fill. Values must be inside range 0.0 
        (fully transparent) to 1.0 (fully opaque). Default is 1.0.
    */
        public float d_fillOpacity;

        //! The 'stroke' property defines how the stroke of a graphical element must be painted.
        public SvgPaint d_stroke;

        /*!
    \brief
        The width of the stroke which shall be used on the current object.
        No stroke shall be painted for a zero value. A negative value is unsupported and must be
        treated as if the stroke had not been specified. Default is 1.0.
    */
        public float d_strokeWidth;

        /*!
    \brief
        Specifies the shape which shall be used at the end of open subpaths when they are stroked.
    */
        public SVGLinecap d_strokeLinecap;

        /*!
    \brief
        Specifies the shape which shall be used at the corners of shapes when they are stroked.
    */
        public SVGLinejoin d_strokeLinejoin;

        /*!
    \brief
        When two line segments meet at a sharp angle and miter joins have been specified for 'stroke-linejoin',
        it is possible for the miter to extend far beyond the thickness of the line stroking the path. The 
        'stroke-miterlimit' imposes a limit on the ratio of the miter length to the 'stroke-width'. When the
        limit is exceeded, the join must be converted from a miter to a bevel.
        The limit on the ratio of the miter length to the 'stroke-width'. The value of <miterlimit> must be a
        number greater than or equal to 1. Any other value shall be treated as unsupported and processed as if
        the property had not been specified. 
    */
        public float d_strokeMiterlimit;

        /*!
    \brief
        Indicates if dashing shall be used. If stroked and dash array is set to none, the line must be drawn solid. 
    */
        public bool d_strokeDashArrayNone;

        /*!
    \brief
        Specifies the pattern of dashes and gaps that shall be used to stroke paths.
    */
        public List<float> d_strokeDashArray;

        /*!
    \brief
        Specifies the distance into the dash pattern that must be used to start the dash.
    */
        public float d_strokeDashOffset;

        /*!
    \brief
        The opacity setting that will be applied to the stroke. Values must be inside range 0.0 
        (fully transparent) to 1.0 (fully opaque). Default is 1.0.
    */
        public float d_strokeOpacity;
    }
}