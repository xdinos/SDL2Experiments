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
    /// Class the encapsulates imagery for a given widget state.
    /// </summary>
    public class StateImagery
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StateImagery()
        {
            d_clipToDisplay = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">
        /// Name of the state
        /// </param>
        public StateImagery(string name)
        {
            d_stateName = name;
            d_clipToDisplay = false;
        }

        /// <summary>
        /// Render imagery for this state.
        /// </summary>
        /// <param name="srcWindow">
        /// Window to use when convering BaseDim values to pixels.
        /// </param>
        /// <param name="modcols"></param>
        /// <param name="clipper"></param>
        public void Render(Window srcWindow, ColourRect modcols = null, Rectf? clipper = null)
        {
            // render all layers defined for this state
            foreach (var curr in d_layers)
                curr.Render(srcWindow, modcols, clipper, d_clipToDisplay);
        }

        /// <summary>
        /// Render imagery for this state.
        /// </summary>
        /// <param name="srcWindow">
        /// Window to use when convering BaseDim values to pixels.
        /// </param>
        /// <param name="baseRect">
        /// Rect to use when convering BaseDim values to pixels.
        /// </param>
        /// <param name="modcols"></param>
        /// <param name="clipper"></param>
        public void Render(Window srcWindow, Rectf baseRect, ColourRect modcols = null, Rectf? clipper = null)
        {
            // render all layers defined for this state
            foreach (var curr in d_layers)
                curr.Render(srcWindow, baseRect, modcols, clipper, d_clipToDisplay);
        }

        /// <summary>
        /// Add an imagery LayerSpecification to this state.
        /// </summary>
        /// <param name="layer">
        /// LayerSpecification to be added to this state (will be copied)
        /// </param>
        public void AddLayer(LayerSpecification layer)
        {
            d_layers.Add(layer);
            Sort();
        }

        /// <summary>
        /// Sorts the LayerSpecifications after their priority. Whenever a LayerSpecification, which has been added
        /// to this StateImagery, is changed or an element is added to or removed from the list, this sort function
        /// has to be called.
        /// </summary>
        public void Sort()
        {
            d_layers.Sort((lhs, rhs) => lhs.GetLayerPriority().CompareTo(rhs.GetLayerPriority()));
        }

        /// <summary>
        /// Removed all LayerSpecifications from this state.
        /// </summary>
        public void ClearLayers()
        {
            d_layers.Clear();
        }

        /// <summary>
        /// Return the name of this state.
        /// </summary>
        /// <returns>
        /// String object holding the name of the StateImagery object.
        /// </returns>
        public string GetName()
        {
            return d_stateName;
        }

        /// <summary>
        /// Set the name of this state.
        /// </summary>
        /// <param name="value">
        /// String object holding the name of the StateImagery object.
        /// </param>
        public void SetName(string value)
        {
            d_stateName = value;
        }

        /// <summary>
        /// Return whether this state imagery should be clipped to the display rather than the target window.
        /// 
        /// Clipping to the display effectively implies that the imagery should be rendered unclipped.
        /// </summary>
        /// <returns>
        /// - true if the imagery will be clipped to the display area.
        /// - false if the imagery will be clipped to the target window area.
        /// </returns>
        public bool IsClippedToDisplay()
        {
            return d_clipToDisplay;
        }

        /// <summary>
        /// Set whether this state imagery should be clipped to the display rather than the target window.
        /// 
        /// Clipping to the display effectively implies that the imagery should be rendered unclipped.
        /// </summary>
        /// <param name="value">
        /// - true if the imagery should be clipped to the display area.
        /// - false if the imagery should be clipped to the target window area.
        /// </param>
        public void SetClippedToDisplay(bool value)
        {
            d_clipToDisplay = value;
        }

        /// <summary>
        /// Writes an xml representation of this StateImagery to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        #region Fields

        private string d_stateName;    //!< Name of this state.
        
        // TODO: ...
        //typedef std::multiset<LayerSpecification> LayersList;
        //LayersList      d_layers;       //!< Collection of LayerSpecification objects to be drawn for this state.
        private List<LayerSpecification> d_layers=new List<LayerSpecification>();

        private bool d_clipToDisplay; //!< true if Imagery for this state should be clipped to the display instead of winodw (effectively, not clipped).
    
        // TODO: public
        //typedef ConstVectorIterator<LayersList> LayerIterator;
        //LayerIterator getLayerIterator() const;

        #endregion
    }
}