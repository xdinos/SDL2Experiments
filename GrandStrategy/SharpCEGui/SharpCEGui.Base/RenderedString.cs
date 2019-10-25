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
using System.Linq;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class representing a rendered string of entities.
    /// 
    /// Here 'string' does not refer solely to a text string, rather a string of
    /// any renderable items.
    /// </summary>
    public class RenderedString
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public RenderedString()
        {
            // set up initial line info
            AppendLineBreak();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        public RenderedString(RenderedString other)
        {
            CloneComponentList(other.d_components);
            CloneTrackInfoList(other.d_lines);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public RenderedString Clone()
        {
            var rs = new RenderedString();
            rs.CloneComponentList(d_components);
            rs.CloneTrackInfoList(d_lines);
            return rs;
        }

        // TODO: Copy constructor.
        //public RenderedString(const RenderedString& other)
        //{
        //    cloneComponentList(other.d_components);
        //    d_lines = other.d_lines;
        //}

        // TODO: Assignment.
        //RenderedString operator=(RenderedString rhs)
        //{
        //    cloneComponentList(rhs.d_components);
        //    d_lines = rhs.d_lines;
        //    return *this;
        //}

        // TODO: Destructor.
        //virtual ~RenderedString();

        /// <summary>
        /// Draw the string to a GeometryBuffer.
        /// </summary>
        /// <param name="refWnd">
        /// A pointer to a reference Window used to retrieve certain attributes if needed.
        /// </param>
        /// <param name="line">
        /// The line of the RenderedString to draw.
        /// </param>
        /// <param name="position">
        /// Vector2 describing the position where the RenderedString is to be drawn.
        /// Note that this is not the final onscreen position, but the position as
        /// offset from the top-left corner of the entity represented by the
        /// GeometryBuffer.
        /// </param>
        /// <param name="modColours">
        /// Pointer to a ColourRect describing colour values that are to be
        /// modulated with the any stored colour values to calculate the final
        /// colour values to be used.  This may be 0 if no modulated colours are
        /// required.  NB: Each specific component will decide if and how it will
        /// apply the modulated colours.
        /// </param>
        /// <param name="clipRect">
        /// Pointer to a Rect object that describes a clipping rectangle that should
        /// be used when drawing the RenderedString.  This may be 0 if no clipping
        /// is required.
        /// </param>
        /// <param name="spaceExtra">
        /// float value indicating additional padding value to be applied to space
        /// characters in the string.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a line is out of range.
        /// </exception>
        public List<GeometryBuffer> CreateRenderGeometry(Window refWnd, int line, Lunatics.Mathematics.Vector2 position, ColourRect modColours, Rectf? clipRect, float spaceExtra)
        {
            if (line >= GetLineCount())
                throw new InvalidRequestException("line number specified is invalid.");

            var renderHeight = GetPixelSize(refWnd, line).Height;
            var compPos = position;
            var geomBuffers = new List<GeometryBuffer>();

            var endComponent = d_lines[line].First + d_lines[line].Second;
            for (var i = d_lines[line].First; i < endComponent; ++i)
            {
                var currentRenderGeometry = d_components[i].CreateRenderGeometry(refWnd, compPos, modColours, clipRect, renderHeight, spaceExtra);
                geomBuffers.AddRange(currentRenderGeometry);
                compPos.X += d_components[i].GetPixelSize(refWnd).Width;
            }

            return geomBuffers;
        }

        /// <summary>
        /// Return the pixel size of a specified line for the RenderedString.
        /// </summary>
        /// <param name="refWnd">
        /// The line number whose size is to be returned.</param>
        /// <param name="line"></param>
        /// <returns>
        /// Size object describing the size of the rendered output of the specified
        /// line of this RenderedString, in pixels.
        /// </returns>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a line is out of range.
        /// </exception>
        public Sizef GetPixelSize(Window refWnd, int line)
        {
            if (line >= GetLineCount())
                throw new InvalidRequestException("line number specified is invalid.");

            var sz = Sizef.Zero;

            var endComponent = d_lines[line].First + d_lines[line].Second;
            for (var i = d_lines[line].First; i < endComponent; ++i)
            {
                var compSz = d_components[i].GetPixelSize(refWnd);
                sz.Width += compSz.Width;

                if (compSz.Height > sz.Height)
                    sz.Height = compSz.Height;
            }

            return sz;
        }

        /// <summary>
        /// Return the maximum horizontal extent of all lines, in pixels.
        /// </summary>
        /// <param name="refWnd"></param>
        /// <returns></returns>
        public float GetHorizontalExtent(Window refWnd)
        {
            var w = 0.0f;
            for (var i = 0; i < d_lines.Count; ++i)
            {
                var thisWidth = GetPixelSize(refWnd, i).Width;
                if (thisWidth > w)
                    w = thisWidth;
            }

            return w;
        }

        /// <summary>
        /// Return the sum vertical extent of all lines, in pixels.
        /// </summary>
        /// <param name="refWnd"></param>
        /// <returns></returns>
        public float GetVerticalExtent(Window refWnd)
        {
            var h = 0.0f;
            for (var i = 0; i < d_lines.Count; ++i)
                h += GetPixelSize(refWnd, i).Height;

            return h;
        }

        /// <summary>
        /// append \a component to the list of components drawn for this string.
        /// </summary>
        /// <param name="component"></param>
        public void AppendComponent(RenderedStringComponent component)
        {
            d_components.Add(component.Clone());
            d_lines[d_lines.Count - 1] = new TrackInfo
                                             {
                                                 First = d_lines[d_lines.Count - 1].First,
                                                 Second = d_lines[d_lines.Count - 1].Second + 1
                                             };
        }

        /// <summary>
        /// clear the list of components drawn for this string.
        /// </summary>
        public void ClearComponents()
        {
            ClearComponentList(d_components);
            d_lines.Clear();
        }

        /// <summary>
        /// return the number of components that make up this string.
        /// </summary>
        /// <returns></returns>
        public int GetComponentCount()
        {
            return d_components.Count;
        }

        /// <summary>
        /// split the string in line \a line as close to \a split_point as possible.
        /// <para>
        /// The RenderedString \a left will receive the left portion of the split,
        /// while the right portion of the split will remain in this RenderedString.
        /// </para>
        /// </summary>
        /// <param name="refWnd"></param>
        /// <param name="line">
        /// The line number on which the split is to occur.
        /// </param>
        /// <param name="splitPoint">
        /// float value specifying the pixel location where the split should occur.
        /// The actual split will occur as close to this point as possible, though
        /// preferring a shorter 'left' portion when the split can not be made
        /// exactly at the requested point.
        /// </param>
        /// <param name="left">
        /// RenderedString object that will receieve the left portion of the split.
        /// Any existing content in the RenderedString is replaced.
        /// </param>
        /// <exception cref="InvalidRequestException">
        /// thrown if \a line is out of range.
        /// </exception>
        public void Split(Window refWnd, int line, float splitPoint, RenderedString left)
        {
            // FIXME: This function is big and nasty; it breaks all the rules for a
            // 'good' function and desperately needs some refactoring work done to it.
            // On the plus side, it does seem to work though ;)

            if (line >= GetLineCount())
                throw new InvalidRequestException("line number specified is invalid.");

            left.ClearComponents();

            if (d_components.Count==0)
                return;

            // move all components in lines prior to the line being split to the left
            if (line > 0)
            {
                // calculate size of range
                var sz = d_lines[line - 1].First + d_lines[line - 1].Second;

                //// range start
                //ComponentList::iterator cb = d_components.begin();
                //// range end (exclusive)
                //ComponentList::iterator ce = cb + sz;
                //// copy components to left side
                //left.d_components.assign(cb, ce);
                //// erase components from this side.
                //d_components.erase(cb, ce);
                for (int i = 0; i < sz; i++)
                {
                    left.d_components.Add(d_components[0]);
                    d_components.RemoveAt(0);
                }

                //LineList::iterator lb = d_lines.begin();
                //LineList::iterator le = lb + line;
                //// copy lines to left side
                //left.d_lines.assign(lb, le);
                //// erase lines from this side
                //d_lines.erase(lb, le);
                for (int i = 0; i < line; i++)
                {
                    left.d_lines.Add(d_lines[0]);
                    d_lines.RemoveAt(0);
                }
            }

            // find the component where the requested split point lies.
            var partialExtent = 0f;

            var idx = 0;
            var lastComponent = d_lines[0].Second;
            for (; idx < lastComponent; ++idx)
            {
                partialExtent += d_components[idx].GetPixelSize(refWnd).Width;

                if (splitPoint <= partialExtent)
                    break;
            }

            // case where split point is past the end
            if (idx >= lastComponent)
            {
                // transfer this line's components to the 'left' string.
                //
                // calculate size of range
                var sz = d_lines[0].Second;
                //// range start
                //ComponentList::iterator cb = d_components.begin();
                //// range end (exclusive)
                //ComponentList::iterator ce = cb + sz;
                //// copy components to left side
                //left.d_components.insert(left.d_components.end(), cb, ce);
                //// erase components from this side.
                //d_components.erase(cb, ce);
                for (int i = 0; i < sz; i++)
                {
                    left.d_components.Add(d_components[0]);
                    d_components.RemoveAt(0);
                }

                // copy line info to left side
                left.d_lines.Add(d_lines[0]);
                // erase line from this side
                d_lines.RemoveAt(0);

                // fix up lines in this object
                for (int comp = 0, i = 0; i < d_lines.Count; ++i)
                {
                    d_lines[i].First = comp;
                    comp += d_lines[i].Second;
                }

                return;
            }

            left.AppendLineBreak();
            var leftLine = left.GetLineCount() - 1;
            // Everything up to 'idx' is xfered to 'left'
            for (var i = 0; i < idx; ++i)
            {
                left.d_components.Add(d_components[0]);
                d_components.RemoveAt(0);
                ++left.d_lines[leftLine].Second;
                --d_lines[0].Second;
            }

            // now to split item 'idx' putting half in left and leaving half in this.
            RenderedStringComponent c = d_components[0];
            if (c.CanSplit())
            {
                var lc = c.Split(refWnd, splitPoint - (partialExtent - c.GetPixelSize(refWnd).Width), idx == 0);

                if (lc != null)
                {
                    left.d_components.Add(lc);
                    ++left.d_lines[leftLine].Second;
                }
            }
            // can't split, if component width is >= split_point xfer the whole
            // component to it's own line in the left part (FIX #306)
            else if (c.GetPixelSize(refWnd).Width >= splitPoint)
            {
                left.AppendLineBreak();
                left.d_components.Add(d_components[0]);
                d_components.RemoveAt(0);
                ++left.d_lines[leftLine + 1].Second;
                --d_lines[0].Second;
            }

            // fix up lines in this object
            for (int comp = 0, i = 0; i < d_lines.Count; ++i)
            {
                d_lines[i].First = comp;
                comp += d_lines[i].Second;
            }
        }

        /// <summary>
        /// return the total number of spacing characters in the specified line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public int GetSpaceCount(int line)
        {
            if (line >= GetLineCount())
                throw new InvalidRequestException("line number specified is invalid.");

            var spaceCount = 0;

            var endComponent = d_lines[line].First + d_lines[line].Second;
            for (var i = d_lines[line].First; i < endComponent; ++i)
                spaceCount += d_components[i].GetSpaceCount();

            return spaceCount;
        }

        /// <summary>
        /// linebreak the rendered string at the present position.
        /// </summary>
        public void AppendLineBreak()
        {
            var firstComponent = d_lines.Count == 0
                                     ? 0
                                     : d_lines[d_lines.Count-1].First + d_lines[d_lines.Count-1].Second;

            d_lines.Add(new TrackInfo
                            {
                                First = firstComponent,
                                Second = 0
                            });
        }

        /// <summary>
        /// return number of lines in this string.
        /// </summary>
        /// <returns></returns>
        public int GetLineCount()
        {
            return d_lines.Count;
        }

        /// <summary>
        /// set selection highlight
        /// </summary>
        /// <param name="ref_wnd"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetSelection(Window ref_wnd, float start, float end)
        {
            var lastComponent = d_lines[0].Second;
            float partialExtent = 0;
            var idx = 0;

            // clear last selection from all components
            for (var i = 0; i < d_components.Count; i++)
                d_components[i].SetSelection(ref_wnd, 0, 0);

            for (; idx < lastComponent; ++idx)
            {
                if (start <= partialExtent + d_components[idx].GetPixelSize(ref_wnd).Width)
                    break;

                partialExtent += d_components[idx].GetPixelSize(ref_wnd).Width;
            }

            start -= partialExtent;
            end -= partialExtent;

            while (end > 0.0f)
            {
                var compExtent = d_components[idx].GetPixelSize(ref_wnd).Width;
                d_components[idx].SetSelection(ref_wnd, start, (end >= compExtent) ? compExtent : end);
                start = 0;
                end -= compExtent;
                ++idx;
            }
        }

        /// <summary>
        /// RenderedStringComponent objects that comprise this RenderedString.
        /// </summary>
        protected List<RenderedStringComponent> d_components = new List<RenderedStringComponent>();


        // track info for a line.  first is componetn idx, second is component count.
        // Collection type used to hold details about the lines.
        
        protected class TrackInfo
        {
            public int /*Index*/First;
            public int /*Count*/Second;
        }
        
        //! lines that make up this string.
        protected List<TrackInfo> d_lines = new List<TrackInfo>();

        /// <summary>
        /// Make this object's component list a clone of \a list.
        /// </summary>
        /// <param name="list"></param>
        protected void CloneComponentList(List<RenderedStringComponent> list)
        {
            ClearComponentList(d_components);

            foreach (var item in list)
                d_components.Add(item.Clone());
        }

        /// <summary>
        /// Free components in the given ComponentList and clear the list.
        /// </summary>
        /// <param name="list"></param>
        protected static void ClearComponentList(List<RenderedStringComponent> list)
        {
            // TODO: ...
            //for (var i = 0; i < list.Count; i++)
            //    list[i].Dispose() = null;

            list.Clear();
        }

        private void CloneTrackInfoList(IEnumerable<TrackInfo> list)
        {
            d_lines.Clear();
            d_lines.AddRange(list.Select(x => new TrackInfo { First = x.First, Second = x.Second }));
        }
    }
}