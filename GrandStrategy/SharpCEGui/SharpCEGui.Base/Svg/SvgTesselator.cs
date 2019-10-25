using System;
using System.Collections.Generic;
using Lunatics.Mathematics;

namespace SharpCEGui.Base.Svg
{
    /// <summary>
    ///  Defines a static class that provides helper functions for the tessellation of SVGBasicShapes.
    /// </summary>
    internal static class SvgTessellator
    {
        /// <summary>
        /// Object containing the data needed for rendering segments
        /// </summary>
        internal class StrokeSegmentData
        {
            public StrokeSegmentData(GeometryBuffer geometryBuffer, float strokeHalfWidth, SvgPaintStyle paintStyle, float maxScale)
            {
                d_geometryBuffer = geometryBuffer;
                d_strokeHalfWidth = strokeHalfWidth;
                d_paintStyle = paintStyle;
                d_maxScale = maxScale;

                d_prevPoint = Vector2.Zero;
                d_curPoint = Vector2.Zero;
                d_nextPoint = Vector2.Zero;

                // Get and add the stroke colour
                d_strokeVertex.Colour = GetStrokeColour(paintStyle);
                // Set the z coordinate
                d_strokeVertex.Position.Z = 0.0f;
                //Create the fade stroke colour from the normal colour and set the alpha to 0
                d_strokeFadeVertex = d_strokeVertex;
                d_strokeFadeVertex.Colour = Vector4.Zero;
            }

            /// <summary>
            /// Sets the pointers to the line points of this segment
            /// </summary>
            /// <param name="prevPoint"></param>
            /// <param name="curPoint"></param>
            /// <param name="nextPoint"></param>
            public void SetPoints(Vector2 prevPoint, Vector2 curPoint, Vector2 nextPoint)
            {
                d_prevPoint = prevPoint;
                d_curPoint = curPoint;
                d_nextPoint = nextPoint;
            }

            //! Half of the width of the stroke
            public readonly float d_strokeHalfWidth;

            //! The geometry buffer we will draw into
            public GeometryBuffer d_geometryBuffer;

            //! The paint style
            public readonly SvgPaintStyle d_paintStyle;

            //! Last left stroke point, lying in anti-clockwise direction away from the stroke direction.
            public Vector2 d_lastPointLeft;
            //! Last right stroke point, lying in clockwise direction away from the stroke direction.
            public Vector2 d_lastPointRight;
            //! Last left stroke fade point, lying in anti-clockwise direction away from the stroke direction.
            public Vector2 d_lastFadePointLeft;
            //! Last right stroke fade point, lying in clockwise direction away from the stroke direction.
            public Vector2 d_lastFadePointRight;

            //! Current left stroke point, lying in anti-clockwise direction away from the stroke direction.
            public Vector2 d_currentPointLeft;
            //! Current right stroke point, lying in clockwise direction away from the stroke direction.
            public Vector2 d_currentPointRight;
            //! Current left stroke fade point, lying in anti-clockwise direction away from the stroke direction.
            public Vector2 d_currentFadePointLeft;
            //! Current right stroke fade point, lying in clockwise direction away from the stroke direction.
            public Vector2 d_currentFadePointRight;

            //! Last left stroke point, lying in anti-clockwise direction away from the stroke direction.
            public Vector2 d_subsequentPointLeft;
            //! Last right stroke point, lying in clockwise direction away from the stroke direction.
            public Vector2 d_subsequentPointRight;
            //! Last left stroke fade point, lying in anti-clockwise direction away from the stroke direction.
            public Vector2 d_subsequentFadePointLeft;
            //! Last right stroke fade point, lying in clockwise direction away from the stroke direction.
            public Vector2 d_subsequentFadePointRight;

            //! The vertex we will modify with positions and append to the GeometryBuffer
            public ColouredVertex d_strokeVertex;
            //! The vertex we will modify with positions and append to the GeometryBuffer
            public ColouredVertex d_strokeFadeVertex;

            //! The maximum of the scalings (either vert or horz). We need this to determine the degree of tesselation
            // of curved elements of the stroke
            public float d_maxScale;

            //! Anti-aliasing offsets, the first element represents the offset of the solid stroke, the second is for the
            // offset of the alpha-fade
            public Vector2 d_antiAliasingOffsets;

            // TODO: ...
            //! Pointer to the previous line point of this segment
            public Vector2 d_prevPoint;
            //private int d_prevPoint;
            //const glm::vec2* d_prevPoint;

            //! Pointer to the current line point of this segment
            public Vector2 d_curPoint;
            //private int d_curPoint;
            //const glm::vec2* d_curPoint;

            //! Pointer to the subsequent line point of this segment
            public Vector2 d_nextPoint;
            //private int d_nextPoint;
            //const glm::vec2* d_nextPoint;
        }

        /// <summary>
        /// Tesselates an SVGRect and adds the created geometry to the GeometryBuffer list.
        /// </summary>
        /// <param name="rect">
        /// The SVGRect object that contains the data.
        /// </param>
        /// <param name="renderSettings">
        /// The ImageRenderSettings for the geometry that will be created.
        /// </param>
        public static List<GeometryBuffer> TesselateRect(SvgRect rect, SvgImage.SvgImageRenderSettings renderSettings)
        {
            var transformation = new Matrix3x3(1.0f, 0.0f, rect.d_x,
                                               0.0f, 1.0f, rect.d_y,
                                               0.0f, 0.0f, 1.0f) * rect.Transformation;

            //Setup the required Geometry-buffers
            GeometryBuffer fillGeometryBuffer;
            GeometryBuffer strokeGeometryBuffer;
            var geomBuffers = SetupGeometryBuffers(out fillGeometryBuffer, out strokeGeometryBuffer, renderSettings, transformation, false);

            //The shape's paint styles
            var paintStyle = rect.PaintStyle;

            //Get the final scale by extracting the scale from the matrix and combining it with the image scale
            var scaleFactors = DetermineScaleFactors(rect.Transformation, renderSettings);

            //Make a list of rectangle (corner) points
            var rectanglePoints = new List<Vector2>
                                  {
                                          new Vector2(0.0f, 0.0f),
                                          new Vector2(0.0f, rect.d_height),
                                          new Vector2(rect.d_width, rect.d_height),
                                          new Vector2(rect.d_width, 0.0f)
                                  };

            //Create and append the rectangle's fill geometry
            CreateTriangleStripFillGeometry(rectanglePoints, fillGeometryBuffer, paintStyle);

            //Create and append the rectangle's stroke geometry
            CreateStroke(rectanglePoints, strokeGeometryBuffer, paintStyle, renderSettings, scaleFactors, true);

            return geomBuffers;
        }

        /// <summary>
        /// Tesselates an SVGCircle and adds the created geometry to the GeometryBuffer list.
        /// </summary>
        /// <param name="circle">
        /// The SVGCircle object that contains the data.
        /// </param>
        /// <param name="renderSettings">
        /// The ImageRenderSettings for the geometry that will be created.
        /// </param>
        public static List<GeometryBuffer> TesselateCircle(SvgCircle circle, SvgImage.SvgImageRenderSettings renderSettings)
        {
            var transformation = new Matrix3x3(1.0f, 0.0f, circle.d_cx,
                                               0.0f, 1.0f, circle.d_cy,
                                               0.0f, 0.0f, 1.0f)*circle.Transformation;

            //Setup the required Geometrybuffers
            GeometryBuffer fillGeometryBuffer;
            GeometryBuffer strokeGeometryBuffer;
            var geomBuffers = SetupGeometryBuffers(out fillGeometryBuffer, out strokeGeometryBuffer, renderSettings, transformation, false);

            //The shape's paint styles
            var paintStyle = circle.PaintStyle;

            //Get the final scale by extracting the scale from the matrix and combining it with the image scale
            var scaleFactors = DetermineScaleFactors(circle.Transformation, renderSettings);

            //We need this to determine the degree of tesselation required for the curved elements
            var maxScale = Math.Max(renderSettings.d_scaleFactor.X, renderSettings.d_scaleFactor.Y);

            //Get the radius
            var radius = circle.d_r;

            //Precalculate values needed for the circle tesselation
            float numSegments, cosValue, sinValue;
            CalculateCircleTesselationParameters(radius, maxScale, out numSegments, out cosValue, out sinValue);

            //Create circle points
            var circlePoints=new List<Vector2>();
            CreateCirclePoints(radius, numSegments, cosValue, sinValue, circlePoints);

            if (circlePoints.Count < 3)
                return geomBuffers;
  
            //Create and append the circle's fill geometry
            CreateCircleFill(circlePoints, maxScale, paintStyle, fillGeometryBuffer, renderSettings, scaleFactors);

            //Create and append the circle's stroke geometry
            CreateCircleStroke(circlePoints, maxScale, paintStyle, strokeGeometryBuffer, renderSettings, scaleFactors);

            return geomBuffers;
        }

        /*!
    \brief
        Tesselates an SVGEllipse and adds the created geometry to the GeometryBuffer
        list.
    
    \param rect
            The SVGEllipse object that contains the data.
    \param geometry_buffers
            The GeometryBuffer list to which the created geometry will be added.
    \param render_settings
            The ImageRenderSettings for the geometry that will be created.
    */

        public static List<GeometryBuffer> tesselateEllipse(SvgEllipse ellipse, SvgImage.SvgImageRenderSettings render_settings)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Tesselates an SVGLine and adds the created geometry to the GeometryBuffer list.
        /// </summary>
        /// <param name="line">
        /// The SVGLine object that contains the data.
        /// </param>
        /// <param name="renderSettings">
        /// The ImageRenderSettings for the geometry that will be created.
        /// </param>
        public static List<GeometryBuffer> TesselateLine(SvgLine line, SvgImage.SvgImageRenderSettings renderSettings)
        {
            //Setup the required Geometrybuffers
            GeometryBuffer fillGeometryBuffer;
            GeometryBuffer strokeGeometryBuffer;
            var geomBuffers = SetupGeometryBuffers(out fillGeometryBuffer, out strokeGeometryBuffer, renderSettings, line.Transformation, false);

            //The shape's paint styles
            var paintStyle = line.PaintStyle;

            //Get the final scale by extracting the scale from the matrix and combining it with the image scale
            var scaleFactors = DetermineScaleFactors(line.Transformation, renderSettings);

            //Create the line points and add them to the stroke points list
            var points=new List<Vector2> {new Vector2(line.d_x1, line.d_y1), new Vector2(line.d_x2, line.d_y2)};

            //Create and append the polyline's stroke geometry
            CreateStroke(points, strokeGeometryBuffer, paintStyle, renderSettings, scaleFactors, false);

            return geomBuffers;
        }
        
        /// <summary>
        /// Tesselates an SVGPolyline and adds the created geometry to the GeometryBuffer list.
        /// </summary>
        /// <param name="polyline">
        /// The SVGPolyline object that contains the data.
        /// <param name="renderSettings">
        /// The ImageRenderSettings for the geometry that will be created.
        /// </param>
        public static List<GeometryBuffer> TesselatePolyline(SvgPolyline polyline, SvgImage.SvgImageRenderSettings renderSettings)
        {
            //Setup the required Geometrybuffers
            GeometryBuffer fillGeometryBuffer;
            GeometryBuffer strokeGeometryBuffer;
            var geomBuffers = SetupGeometryBuffers(out fillGeometryBuffer, out strokeGeometryBuffer, renderSettings, polyline.Transformation, true);

            //The shape's paint styles
            var paintStyle = polyline.PaintStyle;

            //Getting the points defining the polyline
            var points = polyline.d_points;

            //Get the final scale by extracting the scale from the matrix and combining it with the image scale
            var scaleFactors = DetermineScaleFactors(polyline.Transformation, renderSettings);

            //Create and append the polyline's fill geometry
            CreateFill(points, fillGeometryBuffer, paintStyle, renderSettings, scaleFactors);

            //Create and append the polyline's stroke geometry
            CreateStroke(points, strokeGeometryBuffer, paintStyle, renderSettings, scaleFactors, false);

            return geomBuffers;
        }

        /*!
    \brief
        Tesselates an SVGPolygon and adds the created geometry to the GeometryBuffer
        list.

    \param polyline
            The SVGPolygon object that contains the data.
    \param geometry_buffers
            The GeometryBuffer list to which the created geometry will be added.
    \param render_settings
            The ImageRenderSettings for the geometry that will be created.
    */

        public static List<GeometryBuffer> tesselatePolygon(SvgPolygon polyline, SvgImage.SvgImageRenderSettings render_settings)
        {
            throw new NotImplementedException();
        }

        private enum LineIntersectResult
        {
            LIR_PARALLEL,
            LIR_COINCIDENT,
            LIR_NOT_INTERSECTING,
            LIR_INTERESECTING,

            LIR_COUNT
        }
        
        /// <summary>
        /// Helper function for creating a fill based on a list of polygon points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="geometryBuffer"></param>
        /// <param name="paintStyle"></param>
        /// <param name="renderSettings"></param>
        /// <param name="scaleFactors"></param>
        private static void CreateFill(List<Vector2> points,
                                       GeometryBuffer geometryBuffer,
                                       SvgPaintStyle paintStyle,
                                       SvgImage.SvgImageRenderSettings renderSettings,
                                       Vector2 scaleFactors)
        {
            if (points.Count < 3 || paintStyle.d_fill.d_none)
                return;

            //Create the rectangle fill vertex
            var fillVertex = new ColouredVertex {Position = Vector3.Zero, Colour = GetFillColour(paintStyle)};

            //Switches the stencil mode on
            geometryBuffer.SetStencilRenderingActive(paintStyle.d_fillRule);

            addTriangleFanGeometry(points, geometryBuffer, fillVertex);
            //Set the vertex count to the quad's vertex count
            geometryBuffer.SetStencilPostRenderingVertexCount(6);

            //Calculate the axis-aligned bounding box for the vertices that we will use to create a minimally sized quad
            var min = Vector2.Zero;
            var max = Vector2.Zero;
            calculateMinMax(points, ref min, ref max);

            //Add the quad
            addFillQuad(min, new Vector2(min.X, max.Y), new Vector2(max.X, min.Y), max, geometryBuffer, fillVertex);
        }

        /// <summary>
        /// Helper function for creating a stroke based on a list of subsequent points forming the stroke
        /// </summary>
        /// <param name="points"></param>
        /// <param name="geometryBuffer"></param>
        /// <param name="paintStyle"></param>
        /// <param name="renderSettings"></param>
        /// <param name="scaleFactors"></param>
        /// <param name="isShapeClosed"></param>
        private static void CreateStroke(IReadOnlyList<Vector2> points,
                                         GeometryBuffer geometryBuffer,
                                         SvgPaintStyle paintStyle,
                                         SvgImage.SvgImageRenderSettings renderSettings,
                                         Vector2 scaleFactors,
                                         bool isShapeClosed)
        {
            if(points.Count < 2 || paintStyle.d_stroke.d_none || paintStyle.d_strokeWidth == 0.0f)
                return;

            //We need this to determine the degree of tesselation required for the curved elements
            var maxScale = Math.Max(renderSettings.d_scaleFactor.X, renderSettings.d_scaleFactor.Y);

            // Create an object containing all the data we need for our segment processing
            var strokeData = new StrokeSegmentData(geometryBuffer, paintStyle.d_strokeWidth * 0.5f, paintStyle, maxScale);

            // If doing anti-aliasing, get the anti-aliasing offsets
            if(renderSettings.d_antiAliasing)
                DetermineAntiAliasingOffsets(strokeData.d_paintStyle.d_strokeWidth, ref strokeData.d_antiAliasingOffsets);

            var pointsCount = points.Count;
            var i = 0;
   
            // Handle the beginning of the stroke considering that the shape might be open and therefore needs linecaps
            if(!isShapeClosed)
            {
                //Create the starting linecap
                strokeData.SetPoints(points[0], points[0], points[1]);
                CreateStrokeLinecap(strokeData, renderSettings, scaleFactors, true);

                ++i;
            }
            else
            {
                //Get the first two stroke points without drawing anything
                strokeData.SetPoints(points[pointsCount - 2], points[pointsCount - 1], points[0]);
                createStrokeLinejoin(strokeData, renderSettings, scaleFactors, false);

                if(!renderSettings.d_antiAliasing)
                    SetStrokeDataSubsequentPointsAsLastPoints(strokeData);
                else
                    setStrokeDataSubsequentPointsAsLastPointsAA(strokeData);


                //Add the segment connected via the first point
                strokeData.SetPoints(points[pointsCount - 1], points[i], points[i + 1]);
                createStrokeLinejoin(strokeData, renderSettings, scaleFactors, true);

                if(!renderSettings.d_antiAliasing)
                {
                    createStrokeSegmentConnection(strokeData);
                    SetStrokeDataSubsequentPointsAsLastPoints(strokeData);
                }
                else
                {
                    CreateStrokeSegmentConnectionAA(strokeData);
                    setStrokeDataSubsequentPointsAsLastPointsAA(strokeData);
                }

                ++i;
            }

            // Handle segments between start and end of the stroke
            for(; i < pointsCount - 1; ++i)
            {       
                strokeData.SetPoints(points[i - 1], points[i], points[i + 1]);
                createStrokeLinejoin(strokeData, renderSettings, scaleFactors, true);

                if(!renderSettings.d_antiAliasing)
                {
                    createStrokeSegmentConnection(strokeData);
                    SetStrokeDataSubsequentPointsAsLastPoints(strokeData);
                }
                else
                {
                    CreateStrokeSegmentConnectionAA(strokeData);
                    setStrokeDataSubsequentPointsAsLastPointsAA(strokeData);
                }
            }

            // Handle the end of the stroke considering that the shape might be open and therefore needs linecaps
            if(!isShapeClosed)
            {
                //Set out last points as current points so we do not override them with he linecap creation
                if(!renderSettings.d_antiAliasing)
                    SetStrokeDataLastPointsAsCurrentPoints(strokeData);
                else
                    SetStrokeDataLastPointsAsCurrentPointsAA(strokeData);

                //Create linecap
                strokeData.SetPoints(points[pointsCount - 2], points[pointsCount - 1], points[pointsCount - 1]);
                CreateStrokeLinecap(strokeData, renderSettings, scaleFactors, false);

                //Connect to the linecap
                if(!renderSettings.d_antiAliasing)
                    createStrokeSegmentConnection(strokeData);
                else
                    CreateStrokeSegmentConnectionAA(strokeData);
            }
            else
            {
                //Add the segment connected via the last point
                strokeData.SetPoints(points[pointsCount - 2], points[pointsCount - 1], points[0]);
                createStrokeLinejoin(strokeData, renderSettings, scaleFactors, true);

                if(!renderSettings.d_antiAliasing)
                    createStrokeSegmentConnection(strokeData);
                else
                    CreateStrokeSegmentConnectionAA(strokeData);
            }
        }

        //! Stroke helper function that determines vertices of a stroke segment and adds them to the geometry buffer
        private static void createStrokeLinejoin(StrokeSegmentData stroke_data,
                                                 SvgImage.SvgImageRenderSettings render_settings,
                                                 Vector2 scale_factors,
                                                 bool draw = true)
        {
            var linejoin = stroke_data.d_paintStyle.d_strokeLinejoin;
            var prev_point = stroke_data.d_prevPoint;
            var cur_point = stroke_data.d_curPoint;
            var next_point = stroke_data.d_nextPoint;

            // Check if our corner points form a clockwise or anticlockwise polygon
            bool polygon_is_clockwise = IsPolygonClockwise(prev_point, cur_point, next_point);
            float direction_sign = polygon_is_clockwise ? 1.0f : -1.0f;

            var prev_to_cur = Vector2.Normalize(cur_point - prev_point);
            var prev_dir_to_inside = direction_sign*new Vector2(prev_to_cur.Y, -prev_to_cur.X);
            var prev_vec_to_inside = stroke_data.d_strokeHalfWidth * prev_dir_to_inside;

            var cur_to_next = Vector2.Normalize(next_point - cur_point);
            var next_dir_to_inside =  direction_sign * new Vector2(cur_to_next.Y, -cur_to_next.X);
            var next_vec_to_inside = stroke_data.d_strokeHalfWidth * next_dir_to_inside;

            // We calculate the intersection of the inner lines along the stroke
            var innerIntersection = Vector2.Zero;
            IntersectLines(prev_point + prev_vec_to_inside, cur_point + prev_vec_to_inside,
                           next_point + next_vec_to_inside, cur_point + next_vec_to_inside,
                           ref innerIntersection);

            // The outer connection point of the stroke
            var outerPoint = Vector2.Zero;
            // Reference to the end-points of our stroke segment
            var segment_end_left = polygon_is_clockwise ? outerPoint : innerIntersection;
            var segment_end_right = polygon_is_clockwise ? innerIntersection : outerPoint;

            //If the stroke miter is exceeded we fall back to bevel
            if(linejoin == SvgPaintStyle.SVGLinejoin.SLJ_MITER)
                handleStrokeMiterExceedance(stroke_data, cur_point, innerIntersection, linejoin);

            //Switch through the types and render them if required 
            if(linejoin == SvgPaintStyle.SVGLinejoin.SLJ_MITER)
            {
                //We calculate the connection point of the outer lines
                outerPoint = cur_point + cur_point - innerIntersection;

                if(!render_settings.d_antiAliasing)
                {
                    SetStrokeDataCurrentPoints(stroke_data, segment_end_left, segment_end_right);
                    SetStrokeDataSubsequentPoints(stroke_data, segment_end_left, segment_end_right);
                }
                else
                    CalculateAAMiterAndSetConnectionPoints(stroke_data, segment_end_left, segment_end_right, polygon_is_clockwise,
                                                           prev_to_cur, cur_to_next, prev_dir_to_inside, next_dir_to_inside, scale_factors);
            }
            else if(linejoin == SvgPaintStyle.SVGLinejoin.SLJ_BEVEL || linejoin == SvgPaintStyle.SVGLinejoin.SLJ_ROUND)
            {
                //Is the first bevel corner point
                outerPoint = cur_point - prev_vec_to_inside;
                //The second bevel corner point
                var second_bevel_point = cur_point - next_vec_to_inside;

                if(!render_settings.d_antiAliasing)
                    createStrokeLinejoinBevelOrRound(stroke_data, cur_point, prev_dir_to_inside, next_dir_to_inside, segment_end_left, segment_end_right,
                                                     second_bevel_point, linejoin, polygon_is_clockwise, draw);
                else
                    createStrokeLinejoinBevelOrRoundAA(stroke_data, render_settings, scale_factors, cur_point, second_bevel_point,
                                                       segment_end_left, segment_end_right, prev_to_cur, cur_to_next,
                                                       prev_dir_to_inside, next_dir_to_inside, linejoin, polygon_is_clockwise, draw);
            }
        }

        //! Stroke helper function that determines and adds the anti-aliased geometry of a bevel- or rounded-linejoin
        private static void createStrokeLinejoinBevelOrRoundAA(StrokeSegmentData strokeData,
                                                               SvgImage.SvgImageRenderSettings renderSettings,
                                                               Vector2 scaleFactors,
                                                               Vector2 curPoint,
                                                               Vector2 secondBevelPoint,
                                                               Vector2 segmentEndLeft,
                                                               Vector2 segmentEndRight,
                                                               Vector2 prevToCur,
                                                               Vector2 curToNext,
                                                               Vector2 prevDirToInside,
                                                               Vector2 nextDirToInside,
                                                               SvgPaintStyle.SVGLinejoin linejoin,
                                                               bool polygonIsClockwise,
                                                               bool draw)
        {
            var strokeVertex = strokeData.d_strokeVertex;
            var strokeFadeVertex = strokeData.d_strokeFadeVertex;
            var geometryBuffer = strokeData.d_geometryBuffer;

            var innerPoint = polygonIsClockwise ? segmentEndRight : segmentEndLeft;
            var outerPoint = polygonIsClockwise ? segmentEndLeft : segmentEndRight;

            // Get the scaled vector for the inner AA points
            var innerScaledVec = calculateScaledCombinedVector(scaleFactors, prevToCur, curToNext, prevDirToInside, nextDirToInside, false);
            // Calculate the offset vectors for the inner points from the original point
            var coreOffsetVecInner = strokeData.d_antiAliasingOffsets.X * innerScaledVec;
            var fadeOffsetVecInner = strokeData.d_antiAliasingOffsets.Y * innerScaledVec;
            // Calculate the inner positions of our bevel
            var inner_AA = innerPoint + coreOffsetVecInner;
            var inner_fade_AA = innerPoint + fadeOffsetVecInner;

            // Get the dir to the edge between the two outer corner points and its orthogonal direction
            Vector2 edgeDir;
            if (!(secondBevelPoint == outerPoint))
                edgeDir = Vector2.Normalize(secondBevelPoint - outerPoint);
            else //TODO Ident: Check if this is a sufficient workaround
                edgeDir = prevToCur;
            var edgePerpendicularDir = new Vector2(polygonIsClockwise ? edgeDir.Y : -edgeDir.Y,
                                                     polygonIsClockwise ? -edgeDir.X : edgeDir.X);

            // Get the scaled-widths of the perpendicular directions of the edges
            var lengthScale1 = CalculateLengthScale(prevDirToInside, scaleFactors);
            var lengthScale2 = CalculateLengthScale(edgePerpendicularDir, scaleFactors);
            var lengthScale3 = CalculateLengthScale(nextDirToInside, scaleFactors);

            // Calculate scale-distorted direction
            var fadecornerVec1 = prevToCur * lengthScale2 - edgeDir * lengthScale1;
            var fadecornerVec2 = edgeDir * lengthScale3 - curToNext * lengthScale2;          

            //We do not need to normalize at this point, as our result after the dot product division gives us the same result in either case
            //Apply dot product scales
            fadecornerVec1 /= Vector2.Dot(-prevDirToInside, fadecornerVec1);
            fadecornerVec2 /= Vector2.Dot(-edgePerpendicularDir, fadecornerVec2);
            //Apply length factors
            fadecornerVec1 *= lengthScale1;
            fadecornerVec2 *= lengthScale2;

            // Calculate the segment positions
            var outer_AA = outerPoint + strokeData.d_antiAliasingOffsets.X * fadecornerVec1;
            var outer_fade_AA = outerPoint + strokeData.d_antiAliasingOffsets.Y * fadecornerVec1;
            var outer2_AA = secondBevelPoint + strokeData.d_antiAliasingOffsets.X * fadecornerVec2;
            var outer2_fade_AA = secondBevelPoint + strokeData.d_antiAliasingOffsets.Y * fadecornerVec2;

            //The lines to the points can overlap in case the vectors point to different directions, which happens in extreme scale cases.
            // Normally the 2 points can be merged into 1 point here, so we need to consider this case
            var couldVectorsOverlap = !isVectorLeftOfOtherVector(polygonIsClockwise ? fadecornerVec1 : fadecornerVec2,
                                                                    polygonIsClockwise ? fadecornerVec2 : fadecornerVec1);
            var areLinesOverlapping = false;
            var intersectionPoint = Vector2.Zero;
            if (couldVectorsOverlap)
                areLinesOverlapping =
                        (IntersectLines(outerPoint, outer_fade_AA, secondBevelPoint, outer2_fade_AA,
                                        ref intersectionPoint) ==
                         LineIntersectResult.LIR_INTERESECTING);

   
            if(areLinesOverlapping)
            {
                //In case of an overlap we fall back to just using single vertex, similar to the miter linejoin
                var outerPointMiter = curPoint + curPoint - innerPoint;

                // Calculate the corrected outer positions
                var outer_AA_corrected = outerPointMiter - coreOffsetVecInner;
                var outer_AA_fade_corrected = outerPointMiter - fadeOffsetVecInner;

                // Set the connection
                if(polygonIsClockwise)
                {
                    setStrokeDataCurrentPointsAA(strokeData, outer_AA_corrected, inner_AA,
                                                 outer_AA_fade_corrected, inner_fade_AA);
                    setStrokeDataSubsequentPointsAA(strokeData, outer_AA_corrected, inner_AA,
                                                    outer_AA_fade_corrected, inner_fade_AA);
                }
                else
                {
                    setStrokeDataCurrentPointsAA(strokeData, inner_AA, outer_AA_corrected,
                                                 inner_fade_AA, outer_AA_fade_corrected);
                    setStrokeDataSubsequentPointsAA(strokeData, inner_AA, outer_AA_corrected,
                                                 inner_fade_AA, outer_AA_fade_corrected);
                }
            }
            else 
            { 
                if(draw && linejoin == SvgPaintStyle.SVGLinejoin.SLJ_BEVEL)
                {
                    //Add the geometry for bevel
                    AddTriangleGeometry(outer2_AA, inner_AA, outer_AA, geometryBuffer, ref strokeVertex);

                    addStrokeQuadAA(outer_AA, outer2_AA, outer_fade_AA, outer2_fade_AA,
                                    geometryBuffer, ref strokeVertex, ref strokeFadeVertex);
                }
                else if(draw && linejoin == SvgPaintStyle.SVGLinejoin.SLJ_ROUND)
                {
                    //Add the geometry for rounded linejoin
                    var arcAngle = (float)Math.Acos(Vector2.Dot(prevDirToInside, nextDirToInside));

                    //Get the parameters
                    float numSegments, tangentialFactor, radialFactor;
                    CalculateArcTesselationParameters(strokeData.d_strokeHalfWidth, arcAngle, strokeData.d_maxScale,
                                                      out numSegments, out tangentialFactor, out radialFactor);

                    //Get the arc points
                    var arcPoints=new List<Vector2>();
                    CreateArcPoints(curPoint, outerPoint, secondBevelPoint, numSegments,
                                    polygonIsClockwise ? -tangentialFactor : tangentialFactor, radialFactor, arcPoints);

                    createArcStrokeAAGeometry(arcPoints, curPoint, inner_AA, strokeData, scaleFactors,
                                              !polygonIsClockwise, ref outer_AA, ref outer2_AA, ref outer_fade_AA, ref outer2_fade_AA);
                }


                // We add the geometry of the segment that connects to the last linecap/linejoin
                if(polygonIsClockwise)
                {
                    setStrokeDataCurrentPointsAA(strokeData, outer_AA, inner_AA, outer_fade_AA, inner_fade_AA);
                    setStrokeDataSubsequentPointsAA(strokeData, outer2_AA, inner_AA, outer2_fade_AA, inner_fade_AA);
                }
                else
                {
                    setStrokeDataCurrentPointsAA(strokeData, inner_AA, outer_AA, inner_fade_AA, outer_fade_AA);
                    setStrokeDataSubsequentPointsAA(strokeData, inner_AA, outer2_AA, inner_fade_AA, outer2_fade_AA);
                }
            }
        }

        //! Stroke helper function that determines and adds the geometry of a bevel- or rounded-linejoin
        private static void createStrokeLinejoinBevelOrRound(StrokeSegmentData stroke_data,
                                                             Vector2 cur_point,
                                                             Vector2 prev_dir_to_inside,
                                                             Vector2 next_dir_to_inside,
                                                             Vector2 segment_end_left,
                                                             Vector2 segment_end_right,
                                                             Vector2 second_bevel_point,
                                                             SvgPaintStyle.SVGLinejoin linejoin,
                                                             bool polygon_is_clockwise,
                                                             bool draw)
        {
            if(draw)
            {
                SetStrokeDataCurrentPoints(stroke_data, segment_end_left, segment_end_right);

                if(linejoin == SvgPaintStyle.SVGLinejoin.SLJ_BEVEL)
                {
                    //Simply add a triangle for the bevel
                    AddTriangleGeometry(segment_end_left, segment_end_right, second_bevel_point,
                                        stroke_data.d_geometryBuffer, ref stroke_data.d_strokeVertex);
                }
                else if(linejoin == SvgPaintStyle.SVGLinejoin.SLJ_ROUND)
                {
                    //Determine the linejoin angle
                    var arc_angle = (float)Math.Acos( Vector2.Dot(prev_dir_to_inside, next_dir_to_inside) );

                    //Get the parameters
                    float num_segments, tangential_factor, radial_factor;
                    CalculateArcTesselationParameters(stroke_data.d_strokeHalfWidth, arc_angle, stroke_data.d_maxScale,
                                                      out num_segments, out tangential_factor, out radial_factor);

                    //Get the arc points and add them to the geometrybuffer
                    var arc_points=new List<Vector2>();
                    arc_points.Add(polygon_is_clockwise ? segment_end_right : segment_end_left);

                    CreateArcPoints(cur_point, polygon_is_clockwise ? second_bevel_point : segment_end_right,
                                    polygon_is_clockwise ? segment_end_left : second_bevel_point, num_segments,
                                    tangential_factor, radial_factor, arc_points);

                    CreateArcStrokeGeometry(arc_points, stroke_data.d_geometryBuffer, ref stroke_data.d_strokeVertex);
                }
            }

            SetStrokeDataSubsequentPoints(stroke_data, polygon_is_clockwise ? second_bevel_point : segment_end_left, polygon_is_clockwise ? segment_end_right : second_bevel_point);
        }

        //! Checks if the vector is left (meaning that the angle is smaller in clockwise direction) of the other vector
        private static bool isVectorLeftOfOtherVector(Vector2 vector, Vector2 vector_other)
        {
            return (vector.X*-vector_other.Y) + (vector.Y*vector_other.X) > 0.0f;
        }

        //! Checks if the stroke miter is exceeding the maximum set for it, and if this is the case switches the linejoin to bevel
        private static void handleStrokeMiterExceedance(StrokeSegmentData stroke_data,
                                                        Vector2 cur_point,
                                                        Vector2 inner_intersection,
                                                        SvgPaintStyle.SVGLinejoin linejoin)
        {
            //If the miter length we exceeds the limit we will use a regular bevel instead
            var miterlimit = stroke_data.d_paintStyle.d_strokeMiterlimit;

            var half_miter_extension = (cur_point - inner_intersection).Length();
            if(half_miter_extension > (miterlimit * stroke_data.d_strokeHalfWidth))
                linejoin = SvgPaintStyle.SVGLinejoin.SLJ_BEVEL;
        }

        //! Calculates the anti-aliased miter-linejoin points and sets the points necessary to form the connection
        private static void CalculateAAMiterAndSetConnectionPoints(StrokeSegmentData strokeData,
                                                                   Vector2 segmentEndLeftOrig,
                                                                   Vector2 segmentEndRightOrig,
                                                                   bool polygonIsClockwise,
                                                                   Vector2 prevToCur,
                                                                   Vector2 curToNext,
                                                                   Vector2 prevDirToInside,
                                                                   Vector2 nextDirToInside,
                                                                   Vector2 scaleFactors)
        {
            var vec_to_corner = calculateScaledCombinedVector(scaleFactors, prevToCur, curToNext,
                                                              prevDirToInside, nextDirToInside,
                                                              polygonIsClockwise);

            // Calculate the offset vectors from the original area
            var coreOffsetVec = strokeData.d_antiAliasingOffsets.X*vec_to_corner;
            var fadeOffsetVec = strokeData.d_antiAliasingOffsets.Y*vec_to_corner;
            // Calculate the segment positions
            var segmentFadeLeftEnd = segmentEndLeftOrig + fadeOffsetVec;
            var segmentLeftEnd = segmentEndLeftOrig + coreOffsetVec;
            var segmentRightEnd = segmentEndRightOrig - coreOffsetVec;
            var segmentFadeRightEnd = segmentEndRightOrig - fadeOffsetVec;

            // If we want to draw we have to combine the vertices
            setStrokeDataCurrentPointsAA(strokeData, segmentLeftEnd, segmentRightEnd, segmentFadeLeftEnd, segmentFadeRightEnd);
            setStrokeDataSubsequentPointsAA(strokeData, segmentLeftEnd, segmentRightEnd, segmentFadeLeftEnd, segmentFadeRightEnd);
        }

        //! Calculates the scaled vector based on two direction vectors and their perpendicular vectors
        private static Vector2 calculateScaledCombinedVector(Vector2 scale_factors,
                                                             Vector2 prev_to_cur,
                                                             Vector2 cur_to_next,
                                                             Vector2 prev_dir_to_inside,
                                                             Vector2 next_dir_to_inside,
                                                             bool polygon_is_clockwise)
        {
            var almost_parallel = Vector2.Dot(prev_dir_to_inside, next_dir_to_inside)/
                                  (prev_dir_to_inside.Length()*next_dir_to_inside.Length()) > 0.9999f;

            if(!almost_parallel)
            {
                // Get the scaled-widths of the incoming and outgoing line segments
                var length_scale1 = CalculateLengthScale(prev_dir_to_inside, scale_factors);
                var length_scale2 = CalculateLengthScale(next_dir_to_inside, scale_factors);

                var vec_to_outer_corner = Vector2.Normalize(prev_to_cur * length_scale2 - cur_to_next * length_scale1);

                // Calculate scale-distorted direction
                // glm::vec2 vec_to_corner = glm::normalize(prev_to_cur * length_scale2 - cur_to_next * length_scale1);

                // Calculate how much we need to offset along the direction depending on the angle
                /* We get the distance to our new corner using a factor. The factor would, in case we didn't prepare for non-uniform scaling, just consist
                of a simple vector projection. However, here we also need to multiply the local stroke width's scale factor (in the direction of the stroke)
                to get currect results. We have two alternative calculations available for this with the same results.
                Alternative version:  length_scale2 / glm::dot( (polygon_is_clockwise ? -next_dir_to_inside : next_dir_to_inside) , vec_to_corner );
                */
                float length_to_corner = length_scale1/
                                         Vector2.Dot(polygon_is_clockwise ? -prev_dir_to_inside : prev_dir_to_inside,
                                                     vec_to_outer_corner);
 
                return vec_to_outer_corner * length_to_corner;
            }
            //TODO Ident: Check if this fix is valid in all cases
            else
            {
                //If we are near-parallel we need to calculate the corner vector differently
                float length_scale1 = CalculateLengthScale(prev_dir_to_inside, scale_factors);

                var vec_to_outer_corner = 0.5f * ( next_dir_to_inside + prev_dir_to_inside );

                float length_to_corner = length_scale1/
                                         Vector2.Dot(polygon_is_clockwise ? -prev_dir_to_inside : prev_dir_to_inside,
                                                     vec_to_outer_corner);
 
        
                return vec_to_outer_corner * length_to_corner;
            }
        }

        /// <summary>
        /// Returns the inverse of the factor by which the length of the given unit factor would be increased, 
        /// when it gets scaled by the given scale factors along the x- and y-axis.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="scaleFactors"></param>
        /// <returns></returns>
        private static float CalculateLengthScale(Vector2 direction, Vector2 scaleFactors)
        {
            const float halfPi = MathUtils.PiOverTwo;
            const float oneOverPiHalf = 1.0f / halfPi;

            var angleToXAxis = (float)Math.Acos(Math.Abs(direction.X));
            var angleToYAxis = halfPi - angleToXAxis;
            return oneOverPiHalf*(scaleFactors.X*angleToYAxis + scaleFactors.Y*angleToXAxis);
        }

        //! Stroke draw helper function that adds geometry to connect linejoins and linecaps with each other. Creates a connection consisting of 1 quad.
        private static void createStrokeSegmentConnection(StrokeSegmentData stroke_data)
        {
            // Add the geometry
            var stroke_vertex = stroke_data.d_strokeVertex;
            var geometry_buffer = stroke_data.d_geometryBuffer;

            stroke_vertex.Position.X = stroke_data.d_currentPointLeft.X;
            stroke_vertex.Position.Y = stroke_data.d_currentPointLeft.Y;
            geometry_buffer.AppendVertex(stroke_vertex);

            stroke_vertex.Position.X = stroke_data.d_currentPointRight.X;
            stroke_vertex.Position.Y = stroke_data.d_currentPointRight.Y;
            geometry_buffer.AppendVertex(stroke_vertex);

            stroke_vertex.Position.X = stroke_data.d_lastPointLeft.X;
            stroke_vertex.Position.Y = stroke_data.d_lastPointLeft.Y;
            geometry_buffer.AppendVertex(stroke_vertex);

            stroke_vertex.Position.X = stroke_data.d_lastPointLeft.X;
            stroke_vertex.Position.Y = stroke_data.d_lastPointLeft.Y;
            geometry_buffer.AppendVertex(stroke_vertex);

            stroke_vertex.Position.X = stroke_data.d_currentPointRight.X;
            stroke_vertex.Position.Y = stroke_data.d_currentPointRight.Y;
            geometry_buffer.AppendVertex(stroke_vertex);

            stroke_vertex.Position.X = stroke_data.d_lastPointRight.X;
            stroke_vertex.Position.Y = stroke_data.d_lastPointRight.Y;
            geometry_buffer.AppendVertex(stroke_vertex);
        }

        //! Stroke draw helper function that adds geometry to connect anti-aliased linejoins and linecaps with each other. Creates a connection consisting of 3 quads.
        private static void CreateStrokeSegmentConnectionAA(StrokeSegmentData strokeData)
        {
            var strokeVertex = strokeData.d_strokeVertex;
            var strokeFadeVertex = strokeData.d_strokeFadeVertex;
            var geometryBuffer = strokeData.d_geometryBuffer;

            //Fade1
            strokeFadeVertex.Position.X = strokeData.d_currentFadePointLeft.X;
            strokeFadeVertex.Position.Y = strokeData.d_currentFadePointLeft.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeFadeVertex.Position.X = strokeData.d_lastFadePointLeft.X;
            strokeFadeVertex.Position.Y = strokeData.d_lastFadePointLeft.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeVertex.Position.X = strokeData.d_currentPointLeft.X;
            strokeVertex.Position.Y = strokeData.d_currentPointLeft.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = strokeData.d_currentPointLeft.X;
            strokeVertex.Position.Y = strokeData.d_currentPointLeft.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeFadeVertex.Position.X = strokeData.d_lastFadePointLeft.X;
            strokeFadeVertex.Position.Y = strokeData.d_lastFadePointLeft.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeVertex.Position.X = strokeData.d_lastPointLeft.X;
            strokeVertex.Position.Y = strokeData.d_lastPointLeft.Y;
            geometryBuffer.AppendVertex(strokeVertex);


            //Core
            strokeVertex.Position.X = strokeData.d_currentPointLeft.X;
            strokeVertex.Position.Y = strokeData.d_currentPointLeft.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = strokeData.d_lastPointLeft.X;
            strokeVertex.Position.Y = strokeData.d_lastPointLeft.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = strokeData.d_currentPointRight.X;
            strokeVertex.Position.Y = strokeData.d_currentPointRight.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = strokeData.d_currentPointRight.X;
            strokeVertex.Position.Y = strokeData.d_currentPointRight.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = strokeData.d_lastPointLeft.X;
            strokeVertex.Position.Y = strokeData.d_lastPointLeft.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = strokeData.d_lastPointRight.X;
            strokeVertex.Position.Y = strokeData.d_lastPointRight.Y;
            geometryBuffer.AppendVertex(strokeVertex);


            //Fade1
            strokeVertex.Position.X = strokeData.d_currentPointRight.X;
            strokeVertex.Position.Y = strokeData.d_currentPointRight.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = strokeData.d_lastPointRight.X;
            strokeVertex.Position.Y = strokeData.d_lastPointRight.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeFadeVertex.Position.X = strokeData.d_currentFadePointRight.X;
            strokeFadeVertex.Position.Y = strokeData.d_currentFadePointRight.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeFadeVertex.Position.X = strokeData.d_currentFadePointRight.X;
            strokeFadeVertex.Position.Y = strokeData.d_currentFadePointRight.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeVertex.Position.X = strokeData.d_lastPointRight.X;
            strokeVertex.Position.Y = strokeData.d_lastPointRight.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeFadeVertex.Position.X = strokeData.d_lastFadePointRight.X;
            strokeFadeVertex.Position.Y = strokeData.d_lastFadePointRight.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);
        }

        //! Add the stroke linecap AA geometry
        private static void addStrokeLinecapAAGeometryVertices(StrokeSegmentData stroke_data,
                                                               Vector2 linecap_left,
                                                               Vector2 linecap_right,
                                                               Vector2 linecap_fade_left,
                                                               Vector2 linecap_fade_right)
        {
            var stroke_vertex = stroke_data.d_strokeVertex;
            var stroke_fade_vertex = stroke_data.d_strokeFadeVertex;
            var geometry_buffer = stroke_data.d_geometryBuffer;

            stroke_fade_vertex.Position.X = linecap_fade_left.X;
            stroke_fade_vertex.Position.Y = linecap_fade_left.Y;
            geometry_buffer.AppendVertex(stroke_fade_vertex);
            stroke_fade_vertex.Position.X = linecap_fade_right.X;
            stroke_fade_vertex.Position.Y = linecap_fade_right.Y;
            geometry_buffer.AppendVertex(stroke_fade_vertex);
            stroke_vertex.Position.X = linecap_left.X;
            stroke_vertex.Position.Y = linecap_left.Y;
            geometry_buffer.AppendVertex(stroke_vertex);
            stroke_vertex.Position.X = linecap_left.X;
            stroke_vertex.Position.Y = linecap_left.Y;
            geometry_buffer.AppendVertex(stroke_vertex);
            stroke_fade_vertex.Position.X = linecap_fade_right.X;
            stroke_fade_vertex.Position.Y = linecap_fade_right.Y;
            geometry_buffer.AppendVertex(stroke_fade_vertex);
            stroke_vertex.Position.X = linecap_right.X;
            stroke_vertex.Position.Y = linecap_right.Y;
            geometry_buffer.AppendVertex(stroke_vertex);
        }

        /// <summary>
        /// Stroke draw helper function that adds the linecap depending on linecap type and beginning/end
        /// </summary>
        /// <param name="strokeData"></param>
        /// <param name="renderSettings"></param>
        /// <param name="scaleFactors"></param>
        /// <param name="isStart"></param>
        private static void CreateStrokeLinecap(StrokeSegmentData strokeData,
                                                SvgImage.SvgImageRenderSettings renderSettings,
                                                Vector2 scaleFactors,
                                                bool isStart)
        {
            var linecap = strokeData.d_paintStyle.d_strokeLinecap;
            var point1 = isStart ? strokeData.d_curPoint : strokeData.d_prevPoint;
            var point2 = isStart ? strokeData.d_nextPoint : strokeData.d_curPoint;

            // Direction towards the linecap
            var linecapDir = Vector2.Normalize(point2 - point1);
            // Direction to the left side in linecap direction
            var dirToOutside = new Vector2(-linecapDir.Y, linecapDir.X);
            var vecToOutside = strokeData.d_strokeHalfWidth * dirToOutside;
            //Turn around direction to the linecap if we are at the starting cap 
            if(isStart)
                linecapDir *= -1.0f;

            var linecapLeftFade = Vector2.Zero;
            var linecapRightFade = Vector2.Zero;
            var  linecapCenterPoint = strokeData.d_curPoint;

            //We offset the linecap points in case we want a squared cap
            if(linecap == SvgPaintStyle.SVGLinecap.SLC_SQUARE)
                linecapCenterPoint += strokeData.d_strokeHalfWidth * linecapDir;

            //We get the lincap points
            var linecapLeft = linecapCenterPoint + vecToOutside;
            var linecapRight = linecapCenterPoint - vecToOutside;
            var linecap_left_AA=Vector2.Zero;
            var linecap_right_AA = Vector2.Zero;

            //We determine our linecap points for anti-aliasing if required. Also we call the draw commands for squared and butt linecaps.
            if(renderSettings.d_antiAliasing)
            {
                //We calculate the stretch factors in directions of our offsets and then offset the vertices
                var lengthSideScale = CalculateLengthScale(dirToOutside, scaleFactors);
                var linesideOffsetVec = lengthSideScale * dirToOutside;

                linecapLeftFade = linecapLeft + strokeData.d_antiAliasingOffsets.Y * linesideOffsetVec;
                linecapRightFade = linecapRight + strokeData.d_antiAliasingOffsets.Y * -linesideOffsetVec;
                linecap_left_AA = linecapLeft + strokeData.d_antiAliasingOffsets.X * linesideOffsetVec;
                linecap_right_AA = linecapRight + strokeData.d_antiAliasingOffsets.X * -linesideOffsetVec;
            }

            if( linecap == SvgPaintStyle.SVGLinecap.SLC_BUTT || linecap == SvgPaintStyle.SVGLinecap.SLC_SQUARE )
            {
                if(renderSettings.d_antiAliasing)
                {
                    var lengthCapScale = CalculateLengthScale(linecapDir, scaleFactors);
                    var linecapOffsetVec = lengthCapScale * linecapDir;
                    linecap_left_AA += strokeData.d_antiAliasingOffsets.X * linecapOffsetVec;
                    linecap_right_AA += strokeData.d_antiAliasingOffsets.X * linecapOffsetVec;
                    linecapLeftFade += strokeData.d_antiAliasingOffsets.Y * linecapOffsetVec;
                    linecapRightFade += strokeData.d_antiAliasingOffsets.Y * linecapOffsetVec;

                    //Create the outer AA quad of the butt or square linecap
                    addStrokeLinecapAAGeometryVertices(strokeData, linecap_left_AA, linecap_right_AA, linecapLeftFade, linecapRightFade);
                    //Add the anti-aliased connection points to the stroke data
                    setStrokeDataLastPointsAA(strokeData, linecap_left_AA, linecap_right_AA, linecapLeftFade, linecapRightFade);
                }
                else
                    //Add the connection points to the stroke data
                    SetStrokeDataLastPoints(strokeData, linecapLeft, linecapRight);
            }

            //In case we got rounded linecaps we want to determine our points first and draw then
            if(linecap == SvgPaintStyle.SVGLinecap.SLC_ROUND)
            {
                const float halfCircleAngle = MathUtils.Pi;

                //Get the parameters
                float numSegments, tangentialFactor, radialFactor;
                CalculateArcTesselationParameters(strokeData.d_strokeHalfWidth, halfCircleAngle, strokeData.d_maxScale,
                                                  out numSegments, out tangentialFactor, out radialFactor);

                //Get the arc points
                var arcPoints = new List<Vector2>();
                CreateArcPoints(linecapCenterPoint, linecapLeft, linecapRight, numSegments,
                                isStart ? tangentialFactor : -tangentialFactor, radialFactor, arcPoints);
        
                if(!renderSettings.d_antiAliasing)
                {
                    //Calculate the arc points
                    CreateArcStrokeGeometry(arcPoints, strokeData.d_geometryBuffer, ref strokeData.d_strokeVertex);
            
                    SetStrokeDataLastPoints(strokeData, linecapLeft, linecapRight);
                }
                else
                {
                    //Calculate the anti-aliased arc points
                    createArcStrokeAAGeometry(arcPoints, linecapCenterPoint, linecapCenterPoint, strokeData, scaleFactors,
                                              isStart, ref linecap_left_AA, ref linecap_right_AA, ref linecapLeftFade, ref linecapRightFade);

                    setStrokeDataLastPointsAA(strokeData, linecap_left_AA, linecap_right_AA,
                                              linecapLeftFade, linecapRightFade);
                }
            }
        }

        /// <summary>
        /// Helper function to set the stroke-data's last point values
        /// </summary>
        /// <param name="strokeData"></param>
        /// <param name="lastPointLeft"></param>
        /// <param name="lastPointRight"></param>
        private static void SetStrokeDataLastPoints(StrokeSegmentData strokeData, Vector2 lastPointLeft, Vector2 lastPointRight)
        {
            //We set our lastPoint values
            strokeData.d_lastPointLeft = lastPointLeft;
            strokeData.d_lastPointRight = lastPointRight;
        }

        /// <summary>
        /// Helper function to set the stroke-data's last anti-aliased point values
        /// </summary>
        /// <param name="strokeData"></param>
        /// <param name="lastPointLeft"></param>
        /// <param name="lastPointRight"></param>
        /// <param name="lastPointLeftFade"></param>
        /// <param name="lastPointRightFade"></param>
        private static void setStrokeDataLastPointsAA(StrokeSegmentData strokeData,
                                                      Vector2 lastPointLeft,
                                                      Vector2 lastPointRight,
                                                      Vector2 lastPointLeftFade,
                                                      Vector2 lastPointRightFade)
        {
            // We set our lastPoint values
            strokeData.d_lastPointLeft = lastPointLeft;
            strokeData.d_lastPointRight = lastPointRight;
            strokeData.d_lastFadePointLeft = lastPointLeftFade;
            strokeData.d_lastFadePointRight = lastPointRightFade;
        }

        /// <summary>
        /// Helper function to set the stroke-data's current point values
        /// </summary>
        /// <param name="strokeData"></param>
        /// <param name="currentPointLeft"></param>
        /// <param name="currentPointRight"></param>
        private static void SetStrokeDataCurrentPoints(StrokeSegmentData strokeData,
                                                       Vector2 currentPointLeft,
                                                       Vector2 currentPointRight)
        {
            //We set our currentPoint values
            strokeData.d_currentPointLeft = currentPointLeft;
            strokeData.d_currentPointRight = currentPointRight;
        }

        /// <summary>
        /// Helper function to set the stroke-data's current anti-aliased point values
        /// </summary>
        /// <param name="strokeData"></param>
        /// <param name="currentPointLeft"></param>
        /// <param name="currentPointRight"></param>
        /// <param name="currentPointLeftFade"></param>
        /// <param name="currentPointRightFade"></param>
        private static void setStrokeDataCurrentPointsAA(StrokeSegmentData strokeData,
                                                         Vector2 currentPointLeft,
                                                         Vector2 currentPointRight,
                                                         Vector2 currentPointLeftFade,
                                                         Vector2 currentPointRightFade)
        {
            // We set our currentPoint values
            strokeData.d_currentPointLeft = currentPointLeft;
            strokeData.d_currentPointRight = currentPointRight;
            strokeData.d_currentFadePointLeft = currentPointLeftFade;
            strokeData.d_currentFadePointRight = currentPointRightFade;
        }

        /// <summary>
        /// Helper function to set the stroke-data's subsequent point values
        /// </summary>
        /// <param name="strokeData"></param>
        /// <param name="subsequentPointLeft"></param>
        /// <param name="subsequentPointRight"></param>
        private static void SetStrokeDataSubsequentPoints(StrokeSegmentData strokeData,
                                                          Vector2 subsequentPointLeft,
                                                          Vector2 subsequentPointRight)
        {
            //We set our subsequentPoint values
            strokeData.d_subsequentPointLeft = subsequentPointLeft;
            strokeData.d_subsequentPointRight = subsequentPointRight;
        }

        //! Helper function to set the stroke-data's subsequent anti-aliased point values
        private static void setStrokeDataSubsequentPointsAA(StrokeSegmentData strokeData,
                                                            Vector2 subsequentPointLeft,
                                                            Vector2 subsequentPointRight,
                                                            Vector2 subsequentPointLeftFade,
                                                            Vector2 subsequentPointRightFade)
        {
            // We set our subsequentPoint values
            strokeData.d_subsequentPointLeft = subsequentPointLeft;
            strokeData.d_subsequentPointRight = subsequentPointRight;
            strokeData.d_subsequentFadePointLeft = subsequentPointLeftFade;
            strokeData.d_subsequentFadePointRight = subsequentPointRightFade;
        }

        /// <summary>
        /// Helper function that sets the subsequentPoints as the new lastPoints
        /// </summary>
        /// <param name="strokeData"></param>
        private static void SetStrokeDataSubsequentPointsAsLastPoints(StrokeSegmentData strokeData)
        {
            strokeData.d_lastPointLeft = strokeData.d_subsequentPointLeft;
            strokeData.d_lastPointRight = strokeData.d_subsequentPointRight;
        }

        //! Helper function that sets the anti-aliased subsequentPoints as the new lastPoints
        private static void setStrokeDataSubsequentPointsAsLastPointsAA(StrokeSegmentData strokeData)
        {
            strokeData.d_lastPointLeft = strokeData.d_subsequentPointLeft;
            strokeData.d_lastPointRight = strokeData.d_subsequentPointRight;
            strokeData.d_lastFadePointLeft = strokeData.d_subsequentFadePointLeft;
            strokeData.d_lastFadePointRight = strokeData.d_subsequentFadePointRight;
        }

        //! Helper function that sets the lastPoints as the new currentPoints
        private static void SetStrokeDataLastPointsAsCurrentPoints(StrokeSegmentData strokeData)
        {
            strokeData.d_currentPointLeft = strokeData.d_lastPointLeft;
            strokeData.d_currentPointRight = strokeData.d_lastPointRight;
        }

        /// <summary>
        /// Helper function that sets the lastPoints as the new currentPoints
        /// </summary>
        /// <param name="strokeData"></param>
        private static void SetStrokeDataLastPointsAsCurrentPointsAA(StrokeSegmentData strokeData)
        {
            strokeData.d_currentPointLeft = strokeData.d_lastPointLeft;
            strokeData.d_currentPointRight = strokeData.d_lastPointRight;
            strokeData.d_currentFadePointLeft = strokeData.d_lastFadePointLeft;
            strokeData.d_currentFadePointRight = strokeData.d_lastFadePointRight;
        }

        /// <summary>
        /// Stroke helper function that determines if the polygon encompassed by the points is clockwise
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <returns></returns>
        private static bool IsPolygonClockwise(Vector2 point1, Vector2 point2, Vector2 point3)
        {
            return ((point2.X - point1.X)*(point3.Y - point1.Y) - (point3.X - point1.X)*(point2.Y - point1.Y)) < 0.0f;
        }

        /// <summary>
        /// Helper function that creates and sets the parameters for a coloured geometry buffer
        /// </summary>
        /// <param name="fillGeometryBuffer"></param>
        /// <param name="strokeGeometryBuffer"></param>
        /// <param name="renderSettings"></param>
        /// <param name="svgTransformation"></param>
        /// <param name="isFillNeedingStencil"></param>
        private static List<GeometryBuffer> SetupGeometryBuffers(out GeometryBuffer fillGeometryBuffer,
                                                                 out GeometryBuffer strokeGeometryBuffer,
                                                                 SvgImage.SvgImageRenderSettings renderSettings,
                                                                 Matrix3x3 svgTransformation,
                                                                 bool isFillNeedingStencil)
        {
            var geomBuffers = new List<GeometryBuffer>();

            //Calculate the transformation matrix for the CEGUI rendering system based on the SVG transformation matrix
            var ceguiTransformationMatrix = CreateRenderableMatrixFromSvgMatrix(svgTransformation);

            fillGeometryBuffer = System.GetSingleton().GetRenderer().CreateGeometryBufferColoured();

            SetupGeometryBufferSettings(fillGeometryBuffer, renderSettings, ceguiTransformationMatrix);
            geomBuffers.Add(fillGeometryBuffer);

            //TODO Ident: For gradients ability we will also need to perform a check for the DS_type here to see if we need a seperate buffer or not
            if (!isFillNeedingStencil)
            {
                //We can use the GeometryBuffer of the fill also for the stroke
                strokeGeometryBuffer = fillGeometryBuffer;
            }
            else
            {
                strokeGeometryBuffer = System.GetSingleton().GetRenderer().CreateGeometryBufferColoured();
                SetupGeometryBufferSettings(strokeGeometryBuffer, renderSettings, ceguiTransformationMatrix);
                geomBuffers.Add(strokeGeometryBuffer);
            }

            return geomBuffers;
        }

        /// <summary>
        /// Helper function for setting an SVG GeometryBuffer's render settings and transformation matrix
        /// </summary>
        /// <param name="geometryBuffer"></param>
        /// <param name="renderSettings"></param>
        /// <param name="ceguiTransformationMatrix"></param>
        private static void SetupGeometryBufferSettings(GeometryBuffer geometryBuffer,
                                                        SvgImage.SvgImageRenderSettings renderSettings,
                                                        Matrix ceguiTransformationMatrix)
        {
            if(renderSettings.ClipArea.HasValue)
            {
                geometryBuffer.SetClippingActive(true);
                geometryBuffer.SetClippingRegion(renderSettings.ClipArea.Value);
            }
            else
                geometryBuffer.SetClippingActive(false);

            geometryBuffer.SetScale(new Vector2(renderSettings.d_scaleFactor.X, renderSettings.d_scaleFactor.Y));
            geometryBuffer.SetCustomTransform(ceguiTransformationMatrix);
            geometryBuffer.SetAlpha(renderSettings.Alpha);
        }

        /// <summary>
        /// Turns a matrix as defined by SVG into a matrix that can be used internally by the CEGUI Renderers
        /// </summary>
        /// <param name="svgMatrix"></param>
        /// <returns></returns>
        private static Matrix CreateRenderableMatrixFromSvgMatrix(Matrix3x3 svgMatrix)
        {
            return new Matrix(svgMatrix.Row1.X, svgMatrix.Row2.X, 0.0f, 0.0f,
                              svgMatrix.Row1.Y, svgMatrix.Row2.Y, 0.0f, 0.0f,
                              0.0f, 0.0f, 1.0f, 0.0f,
                              svgMatrix.Row1.Z, svgMatrix.Row2.Z, 0.0f, 1.0f);
        }

        /// <summary>
        /// Helper function for getting the fill Colour from an SVGPaintStyle
        /// </summary>
        /// <param name="paintStyle"></param>
        /// <returns></returns>
        private static Vector4 GetFillColour(SvgPaintStyle paintStyle)
        {
            var fillColourValues = paintStyle.d_fill.d_colour;
            return new Vector4(fillColourValues.X, fillColourValues.Y, fillColourValues.Z, paintStyle.d_fillOpacity);
        }

        /// <summary>
        /// Helper function for getting the stroke Colour from an SVGPaintStyle
        /// </summary>
        /// <param name="paintStyle"></param>
        /// <returns></returns>
        private static Vector4 GetStrokeColour(SvgPaintStyle paintStyle)
        {
            var strokeColourValues = paintStyle.d_stroke.d_colour;
            return new Vector4(strokeColourValues.X, strokeColourValues.Y, strokeColourValues.Z, paintStyle.d_strokeOpacity);

        }

        /// <summary>
        /// Create the circle's fill
        /// </summary>
        /// <param name="circlePoints"></param>
        /// <param name="maxScale"></param>
        /// <param name="paintStyle"></param>
        /// <param name="geometryBuffer"></param>
        /// <param name="renderSettings"></param>
        /// <param name="scaleFactors"></param>
        private static void CreateCircleFill(IReadOnlyList<Vector2> circlePoints,
                                             float maxScale,
                                             SvgPaintStyle paintStyle,
                                             GeometryBuffer geometryBuffer,
                                             SvgImage.SvgImageRenderSettings renderSettings,
                                             Vector2 scaleFactors)
        {
            if(paintStyle.d_fill.d_none)
                return;

            //Append the geometry based on the the circle points
            if (!renderSettings.d_antiAliasing)
                CreateTriangleStripFillGeometry(circlePoints, geometryBuffer, paintStyle);
            else
            {
                //We calculate the anti-aliasing offsets based on an arbitrary width
                var antiAliasingOffsets = Vector2.Zero;
                DetermineAntiAliasingOffsets(10.0f, ref antiAliasingOffsets);

                var circleModifiedPoints = new List<Vector2>();
                var circleFadePoints = new List<Vector2>();
                CreateCircleOrEllipseFillPointsAntiAliased(circlePoints, antiAliasingOffsets, scaleFactors,
                                                           circleModifiedPoints, circleFadePoints);

                CreateTriangleStripFillGeometry(circleModifiedPoints, geometryBuffer, paintStyle);
                createFillGeometryAAFadeOnly(circleModifiedPoints, circleFadePoints, paintStyle, geometryBuffer, true);
            }
        }

        /// <summary>
        /// Calculate the tesselation parameters necessary to calculate the circle points
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="maxScale"></param>
        /// <param name="numSegments"></param>
        /// <param name="cosValue"></param>
        /// <param name="sinValue"></param>
        private static void CalculateCircleTesselationParameters(float radius,
                                                                 float maxScale,
                                                                 out float numSegments,
                                                                 out float cosValue,
                                                                 out float sinValue)
        {
	        // Adapt the tessellation to the scale
	        var segmentLength = CircleRoundnessValue / maxScale;
	        var theta = (float) Math.Acos(1.0f - (segmentLength / radius));

	        const float twoPi = 2.0f * MathUtils.Pi;
	        // Calculate the number of segments using 360° as angle and using theta
	        numSegments = twoPi / theta;

	        // Pre-calculate values we will need for our circle tessellation
	        cosValue = (float) Math.Cos(theta);
	        sinValue = (float) Math.Sin(theta);
        }

        /// <summary>
        /// Calculate the circle points
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="numSegments"></param>
        /// <param name="cosValue"></param>
        /// <param name="sinValue"></param>
        /// <param name="circlePoints"></param>
        private static void CreateCirclePoints(float radius,
                                               float numSegments,
                                               float cosValue,
                                               float sinValue,
                                               ICollection<Vector2> circlePoints)
        {
            //Create the circle points
            //We start at angle = 0 
            var currentPos = new Vector2(radius, 0.0f);
            for (int i = 0; i < numSegments; i++)
            {
                //Rotate
                var tempValue = currentPos.X;
                currentPos.X = cosValue*currentPos.X - sinValue*currentPos.Y;
                currentPos.Y = sinValue*tempValue + cosValue*currentPos.Y;

                circlePoints.Add(currentPos);
            }
        }

        /// <summary>
        /// Create the circle's stroke
        /// </summary>
        /// <param name="circlePoints"></param>
        /// <param name="maxScale"></param>
        /// <param name="paintStyle"></param>
        /// <param name="geometryBuffer"></param>
        /// <param name="renderSettings"></param>
        /// <param name="scaleFactors"></param>
        private static void CreateCircleStroke(List<Vector2> circlePoints,
                                               float maxScale,
                                               SvgPaintStyle paintStyle,
                                               GeometryBuffer geometryBuffer,
                                               SvgImage.SvgImageRenderSettings renderSettings,
                                               Vector2 scaleFactors)
        {
            if(paintStyle.d_stroke.d_none || paintStyle.d_strokeWidth == 0.0f)
                return;

                var strokeData=new StrokeSegmentData(geometryBuffer, paintStyle.d_strokeWidth * 0.5f, paintStyle, maxScale);

                if(!renderSettings.d_antiAliasing)
                {
                    //Calculate stroke points
                    var outerCirclePoints=new List<Vector2>();
                    var innerCirclePoints=new List<Vector2>();
                    createCircleOrEllipseStrokePoints(circlePoints, strokeData, outerCirclePoints, innerCirclePoints);

                    //Create the geometry from the points
                    createStrokeGeometry(outerCirclePoints, innerCirclePoints, strokeData, true);
                }
                else
                {
                    DetermineAntiAliasingOffsets(strokeData.d_paintStyle.d_strokeWidth, ref strokeData.d_antiAliasingOffsets);

                    //Calculate stroke points
                    var outerCirclePoints = new List<Vector2>();
                    var outerCirclePointsFade = new List<Vector2>();
                    var innerCirclePoints = new List<Vector2>();
                    var innerCirclePointsFade = new List<Vector2>();
                    createCircleOrEllipseStrokePointsAA(circlePoints, strokeData, scaleFactors, outerCirclePoints,
                                                        outerCirclePointsFade, innerCirclePoints, innerCirclePointsFade);

                    //Create the geometry from the points
                    createStrokeGeometryAA(outerCirclePoints, outerCirclePointsFade, innerCirclePoints, innerCirclePointsFade,
                                           strokeData, true);
    }
        }

        /// <summary>
        /// Helper function for creating a triangle strip with filling
        /// </summary>
        /// <param name="points"></param>
        /// <param name="geometryBuffer"></param>
        /// <param name="paintStyle"></param>
        private static void CreateTriangleStripFillGeometry(IReadOnlyList<Vector2> points, GeometryBuffer geometryBuffer, SvgPaintStyle paintStyle)
        {
            if(points.Count < 3 || paintStyle.d_fill.d_none)
                return;

            //Create the rectangle fill vertex
            var fillVertex = new ColouredVertex {Position = Vector3.Zero, Colour = GetFillColour(paintStyle)};

            //Fixed triangle fan point
            var point1 = points[0];

            var maximumIndex = points.Count - 1;
            for (var i = 1; i < maximumIndex; ++i)
                AddTriangleGeometry(point1, points[i], points[i + 1], geometryBuffer, ref fillVertex);
        }

        /// <summary>
        /// Helper function for creating an arc's stroke
        /// </summary>
        /// <param name="points"></param>
        /// <param name="geometryBuffer"></param>
        /// <param name="strokeVertex"></param>
        private static void CreateArcStrokeGeometry(IReadOnlyList<Vector2> points, GeometryBuffer geometryBuffer, ref ColouredVertex strokeVertex)
        {
            //Fixed triangle fan point
            var point1 = points[0];

            var maximumIndex = points.Count - 1;
            for(var i = 1; i < maximumIndex; ++i)
                AddTriangleGeometry(point1, points[i], points[i + 1], geometryBuffer, ref strokeVertex);
        }

        //! Helper function for calculating and creating an anti-aliased stroke for an arc
        private static void createArcStrokeAAGeometry(IReadOnlyList<Vector2> points,
                                                      Vector2 arcCenterPoint,
                                                      Vector2 arcDrawOriginPoint,
                                                      StrokeSegmentData strokeData,
                                                      Vector2 scaleFactors,
                                                      bool polygonIsClockwise,
                                                      ref Vector2 linecapLeftAa,
                                                      ref Vector2 linecapRightAa,
                                                      ref Vector2 linecapLeftFade,
                                                      ref Vector2 linecapRightFade)
        {
            if(points.Count==0)
                return;

            var previousNormalPoint = Vector2.Zero;
            var previousFadePoint = Vector2.Zero;

            //Draw all arc parts
            var indexLimit = points.Count;
            for(var i = 0; i < indexLimit; ++i)
            {
                var isFirst = (i == 0);
                var isLast = (i == indexLimit-1);

                //Calculate the scale vector for our offset
                var vecToOutside = Vector2.Normalize(points[i] - arcCenterPoint);
                float lengthScale = CalculateLengthScale(vecToOutside, scaleFactors);
                vecToOutside *= lengthScale;

                // Calculate the positions
                var currentNormalPoint = points[i] + vecToOutside * strokeData.d_antiAliasingOffsets.X; 
                var currentFadePoint = points[i] + vecToOutside * strokeData.d_antiAliasingOffsets.Y; 


                //Once we determined the first AA points we need to set the references to them
                if(isFirst)
                {
                    linecapLeftAa = currentNormalPoint;
                    linecapLeftFade = currentFadePoint;
                }
                else
                {
                    addStrokeQuadAA(currentNormalPoint, previousNormalPoint, currentFadePoint, previousFadePoint,
                                    strokeData.d_geometryBuffer, ref strokeData.d_strokeVertex, ref strokeData.d_strokeFadeVertex);

                    AddTriangleGeometry(currentNormalPoint, previousNormalPoint, arcDrawOriginPoint,
                        strokeData.d_geometryBuffer, ref strokeData.d_strokeVertex);
                }

                if(isLast)
                {
                    linecapRightAa = currentNormalPoint;
                    linecapRightFade = currentFadePoint;
                }

                previousNormalPoint = currentNormalPoint;
                previousFadePoint = currentFadePoint;
            }
        }

        //! Creates the basic ellipse points
        private static void createEllipsePoints(float radiusX,
                                                float radiusY,
                                                float max_scale,
                                                List<Vector2> ellipse_points)
        {
            throw new NotImplementedException();
        }

        //! Create the ellipse's fill
        private static void createEllipseFill(List<Vector2> ellipse_points,
                                              float max_scale,
                                              SvgPaintStyle paint_style,
                                              GeometryBuffer geometry_buffer,
                                              SvgImage.SvgImageRenderSettings render_settings,
                                              Vector2 scale_factors)
        {
            throw new NotImplementedException();
        }

        //! Create the circle or ellipse's anti-aliased fill points
        private static void CreateCircleOrEllipseFillPointsAntiAliased(IReadOnlyList<Vector2> points,
                                                              Vector2 antiAliasingOffsets,
                                                              Vector2 scaleFactors,
                                                              ICollection<Vector2> modifiedPoints,
                                                              ICollection<Vector2> fadePoints)
        {
            var pointsCount = points.Count;
            for (var i = 0; i < pointsCount; ++i)
            {
                var index1 = (i == pointsCount - 1) ? 0 : (i + 1);
                var index2 = (i == 0) ? (pointsCount - 1) : (i - 1);

                var direction = Vector2.Normalize(points[index1] - points[index2]);
                direction = new Vector2(direction.Y, -direction.X);

                var lengthScale = CalculateLengthScale(direction, scaleFactors);
                direction *= lengthScale;

                modifiedPoints.Add(points[i] + direction*antiAliasingOffsets.X);
                fadePoints.Add(points[i] + direction*antiAliasingOffsets.Y);
            }
        }

        //! Create the ellipse's stroke
        private static void createEllipseStroke(List<Vector2> ellipse_points,
                                                float max_scale,
                                                SvgPaintStyle paint_style,
                                                GeometryBuffer geometry_buffer,
                                                SvgImage.SvgImageRenderSettings render_settings,
                                                Vector2 scale_factors)
        {
            throw new NotImplementedException();
        }

        //! Create circle or ellipse stroke points
        private static void createCircleOrEllipseStrokePoints(List<Vector2> points,
                                                              StrokeSegmentData stroke_data,
                                                              List<Vector2> outer_points,
                                                              List<Vector2> inner_points)
        {
            throw new NotImplementedException();
        }

        //! Create anti-aliased circle or ellipse stroke points
        private static void createCircleOrEllipseStrokePointsAA(List<Vector2> points,
                                                                StrokeSegmentData stroke_data,
                                                                Vector2 scale_factors,
                                                                List<Vector2> outer_points,
                                                                List<Vector2> outer_points_fade,
                                                                List<Vector2> inner_points,
                                                                List<Vector2> inner_points_fade)
        {
            throw new NotImplementedException();
        }

        //! Scales the points of an ellipse (originally circle points) so that they match the scaling
        private static void scaleEllipsePoints(List<Vector2> circle_points,
                                               bool isRadiusXBigger,
                                               float radiusRatio)
        {
            throw new NotImplementedException();
        }

        //! Helper function for adding an anti-aliasing quad of a stroke to the GeometryBuffer
        private static void addStrokeQuadAA(Vector2 point1,
                                            Vector2 point2,
                                            Vector2 fadePoint1,
                                            Vector2 fadePoint2,
                                            GeometryBuffer geometryBuffer,
                                            ref ColouredVertex strokeVertex,
                                            ref ColouredVertex strokeFadeVertex)
        {
            strokeFadeVertex.Position.X = fadePoint1.X;
            strokeFadeVertex.Position.Y = fadePoint1.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeFadeVertex.Position.X = fadePoint2.X;
            strokeFadeVertex.Position.Y = fadePoint2.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeVertex.Position.X = point1.X;
            strokeVertex.Position.Y = point1.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeVertex.Position.X = point1.X;
            strokeVertex.Position.Y = point1.Y;
            geometryBuffer.AppendVertex(strokeVertex);

            strokeFadeVertex.Position.X = fadePoint2.X;
            strokeFadeVertex.Position.Y = fadePoint2.Y;
            geometryBuffer.AppendVertex(strokeFadeVertex);

            strokeVertex.Position.X = point2.X;
            strokeVertex.Position.Y = point2.Y;
            geometryBuffer.AppendVertex(strokeVertex);
        }

        /// <summary>
        /// Helper function for appending a circle fill triangle to a GeometryBuffer
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="geometryBuffer"></param>
        /// <param name="vertex"></param>
        private static void AddTriangleGeometry(Vector2 point1,
                                                Vector2 point2,
                                                Vector2 point3,
                                                GeometryBuffer geometryBuffer,
                                                ref ColouredVertex vertex)
        {
            vertex.Position.X = point1.X;
            vertex.Position.Y = point1.Y;
            geometryBuffer.AppendVertex(vertex);

            vertex.Position.X = point2.X;
            vertex.Position.Y = point2.Y;
            geometryBuffer.AppendVertex(vertex);

            vertex.Position.X = point3.X;
            vertex.Position.Y = point3.Y;
            geometryBuffer.AppendVertex(vertex);
        }

        //! Creates the stroke geometry of an arbitrary stroke based on the outer and inner points
        private static void createStrokeGeometry(List<Vector2> outer_circle_points,
                                                 List<Vector2> inner_circle_points,
                                                 StrokeSegmentData stroke_data,
                                                 bool is_surface_closed)
        {
            throw new NotImplementedException();
        }

        //! Creates the stroke geometry of an arbitrary anti-aliased stroke, based on the outer and inner points of it
        private static void createStrokeGeometryAA(List<Vector2> uter_points,
                                                   List<Vector2> outer_points_fade,
                                                   List<Vector2> inner_points,
                                                   List<Vector2> inner_points_fade,
                                                   StrokeSegmentData stroke_data,
                                                   bool is_surface_closed)
        {
            throw new NotImplementedException();
        }

        //! Creates the anti-aliasing fade fill geometry for an arbitrary anti-aliased fill, based on normal points and fade points
        private static void createFillGeometryAAFadeOnly(IReadOnlyList<Vector2> points,
                                                         IReadOnlyList<Vector2> pointsFade,
                                                         SvgPaintStyle paintStyle,
                                                         GeometryBuffer geometryBuffer,
                                                         bool isSurfaceClosed)
        {
            // Get and add the fill colour
            var fillVertex=new ColouredVertex();
            fillVertex.Colour = GetFillColour(paintStyle);
            // Set the z coordinate
            fillVertex.Position.Z = 0.0f;
            //Create the fade fill vertex from the fill vertex and set its alpha to 0
            var  fillFadeVertex = fillVertex;
            fillFadeVertex.Colour.W = 0.0f;

            var pointsCount = points.Count;
            for(var i = 0; i < pointsCount - 1; ++i)
                addStrokeQuadAA(points[i], points[i + 1], pointsFade[i], pointsFade[i + 1],
                                geometryBuffer, ref fillVertex, ref fillFadeVertex);

            if(isSurfaceClosed)
                addStrokeQuadAA(points[pointsCount - 1], points[0], pointsFade[pointsCount - 1], pointsFade[0],
                                geometryBuffer, ref fillVertex, ref fillFadeVertex);
        }

        /// <summary>
        /// Calculates the parameters necessary to calculate the arc points
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="arcAngle"></param>
        /// <param name="maxScale"></param>
        /// <param name="numSegments"></param>
        /// <param name="tangentialFactor"></param>
        /// <param name="radialFactor"></param>
        private static void CalculateArcTesselationParameters(float radius,
                                                              float arcAngle,
                                                              float maxScale,
                                                              out float numSegments,
                                                              out float tangentialFactor,
                                                              out float radialFactor)
        {
            //Adapt the tesselation to the scale
            var segmentLength  = CircleRoundnessValue / maxScale;
            var theta = (float)Math.Acos(1.0f - (segmentLength/radius));

            //Calculate the number of segments from the arc angle and theta
	        numSegments = arcAngle / theta;

            //Precalculate values we will need for our arc tesselation
	        tangentialFactor = (float)Math.Tan(theta);
	        radialFactor = (float)Math.Cos(theta);
        }

        /// <summary>
        /// Calculates the points of an arc.
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="numSegments"></param>
        /// <param name="tangentialFactor"></param>
        /// <param name="radialFactor"></param>
        /// <param name="arcPoints"></param>
        private static void CreateArcPoints(Vector2 centerPoint,
                                            Vector2 startPoint,
                                            Vector2 endPoint,
                                            float numSegments,
                                            float tangentialFactor,
                                            float radialFactor,
                                            ICollection<Vector2> arcPoints)
        {
            //Add the start points of the arc to our list and set the current position to be the 
            arcPoints.Add(startPoint);
            // arc's start point in object coordinates
            var currentPos = startPoint - centerPoint;

            //Calculate the arc points, skip last segment because that will be our endpoint
            for (var i = 0; i < numSegments - 1.0f; ++i)
            {
                var temp = new Vector2(-currentPos.Y, currentPos.X);

                currentPos += temp*tangentialFactor;
                currentPos *= radialFactor;

                arcPoints.Add(centerPoint + currentPos);
            }

            //Add the end point of the arc to our list
            arcPoints.Add(endPoint);
        }

        /// <summary>
        /// Function to determine the geometry offsets needed in anti-aliasing, depending on the width
        /// </summary>
        /// <param name="width"></param>
        /// <param name="antialiasingOffsets"></param>
        private static void DetermineAntiAliasingOffsets(float width, ref Vector2 antialiasingOffsets)
        {
            var remainder = width - (int) width;

            //float core_offset = antialiasing_offsets.X;
            //float fade_offset = antialiasing_offsets.Y;

            //core_offset = -0.5f;
            //fade_offset = 0.5f;

            antialiasingOffsets.X =-0.5f;
            antialiasingOffsets.Y = 0.5f;
        }

        /// <summary>
        /// Helper function to determine the scale factors in x and y-direction based on the transformation matrix and the image scale
        /// </summary>
        /// <param name="transformation"></param>
        /// <param name="renderSettings"></param>
        /// <returns></returns>
        private static Vector2 DetermineScaleFactors(Matrix3x3 transformation, SvgImage.SvgImageRenderSettings renderSettings)
        {
            var scale = new Vector2(
                    new Vector3(transformation.Row1.X, transformation.Row2.X, transformation.Row3.X).Length(),
                    new Vector3(transformation.Row1.Y, transformation.Row2.Y, transformation.Row3.Y).Length());
            scale *= new Vector2(renderSettings.d_scaleFactor.X, renderSettings.d_scaleFactor.Y);
            scale = 1.0f/scale;
            return scale;
        }

        /// <summary>
        /// Intersects two lines and returns the result. Also the intersection point will be given if there is an intersection..
        /// </summary>
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <param name="intersection"></param>
        /// <returns></returns>
        private static LineIntersectResult IntersectLines(Vector2 line1Start, Vector2 line1End,
                                                          Vector2 line2Start, Vector2 line2End,
                                                          ref Vector2 intersection)
        {
            var denom = ((line2End.Y - line2Start.Y)*(line1End.X - line1Start.X)) -
                        ((line2End.X - line2Start.X)*(line1End.Y - line1Start.Y));

            var nume_a = ((line2End.X - line2Start.X)*(line1Start.Y - line2Start.Y)) -
                         ((line2End.Y - line2Start.Y)*(line1Start.X - line2Start.X));

            var nume_b = ((line1End.X - line1Start.X)*(line1Start.Y - line2Start.Y)) -
                         ((line1End.Y - line1Start.Y)*(line1Start.X - line2Start.X));

            if (denom == 0.0f)
            {
                if (nume_a == 0.0f && nume_b == 0.0f)
                    return LineIntersectResult.LIR_COINCIDENT;
                else
                    return LineIntersectResult.LIR_PARALLEL;
            }

            float ua = nume_a/denom;
            float ub = nume_b/denom;

            if (ua >= 0.0f && ua <= 1.0f && ub >= 0.0f && ub <= 1.0f)
            {
                //Get the intersection point
                intersection.X = line1Start.X + ua*(line1End.X - line1Start.X);
                intersection.Y = line1Start.Y + ua*(line1End.Y - line1Start.Y);

                return LineIntersectResult.LIR_INTERESECTING;
            }

            return LineIntersectResult.LIR_NOT_INTERSECTING;
        }

        //! Create the rectangles fill
        private static void createRectangleFill(SvgPaintStyle paint_style, List<Vector2> rectangle_points,
                                                GeometryBuffer geometry_buffer)
        {
            throw new NotImplementedException();
        }


        //! Helper function to get the min and max x and y coordinates of a list of points
        private static void calculateMinMax(List<Vector2> points, ref Vector2 min, ref Vector2 max)
        {
            throw new NotImplementedException();
        }

        //! Helper function to append a fill-quad based on its 4 corner points to the Geometrybuffer
        private static void addFillQuad(Vector2 point1,
                                        Vector2 point2,
                                        Vector2 point3,
                                        Vector2 point4,
                                        GeometryBuffer geometry_buffer,
                                        ColouredVertex fill_vertex)
        {
            throw new NotImplementedException();
        }

        //! Helper function to append a stroke-quad based on its 4 corner points to the Geometrybuffer
        private static void addStrokeQuad(Vector2 point1,
                                          Vector2 point2,
                                          Vector2 point3,
                                          Vector2 point4,
                                          GeometryBuffer geometry_buffer,
                                          ColouredVertex stroke_vertex)
        {
            throw new NotImplementedException();
        }

        //! Helper function to append a triangle fan, which is based on a list of points, to the Geometrybuffer
        private static void addTriangleFanGeometry(List<Vector2> points,
                                                   GeometryBuffer geometry_buffer,
                                                   ColouredVertex coloured_vertex)
        {
            throw new NotImplementedException();
        }

        //Internal numeric value for  circle roundness. The lower, the better tesselated the
        //circle will be. We will set it to an, for our needs, appropriate fixed value.
        const float CircleRoundnessValue = 0.8f;
    }
}