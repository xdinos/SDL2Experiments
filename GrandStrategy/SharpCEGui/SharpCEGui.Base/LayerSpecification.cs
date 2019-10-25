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
    /// Class that encapsulates a single layer of imagery.
    /// </summary>
    public class LayerSpecification
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="priority">
        /// Specifies the priority of the layer.  Layers with higher priorities will be drawn on top
        /// of layers with lower priorities.
        /// </param>
        public LayerSpecification(int priority = 0)
        {
            d_layerPriority = priority;
        }

        /// <summary>
        /// Render this layer.
        /// </summary>
        /// <param name="srcWindow">
        /// Window to use when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="modcols"></param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void Render(Window srcWindow, ColourRect modcols = null, Rectf? clipper = null, bool clipToDisplay = false)
        {
            // render all sections in this layer
            foreach (var curr in d_sections)
                curr.Render(srcWindow, modcols, clipper, clipToDisplay);
        }

        /// <summary>
        /// Render this layer.
        /// </summary>
        /// <param name="srcWindow">
        /// Window to use when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="baseRect">
        /// Rect to use when calculating pixel values from BaseDim values.
        /// </param>
        /// <param name="modcols"></param>
        /// <param name="clipper"></param>
        /// <param name="clipToDisplay"></param>
        public void Render(Window srcWindow, Rectf baseRect, ColourRect modcols = null, Rectf? clipper = null, bool clipToDisplay = false)
        {
            // render all sections in this layer
            foreach (var curr in d_sections)
                curr.Render(srcWindow, baseRect, modcols, clipper, clipToDisplay);
        }
        
        /// <summary>
        /// Add a section specification to the layer.
        /// 
        /// A section specification is a reference to a named ImagerySection within the WidgetLook.
        /// </summary>
        /// <param name="section"></param>
        public void AddSectionSpecification(SectionSpecification section)
        {
            d_sections.Add(section);
        }

        /// <summary>
        /// Clear all section specifications from this layer,
        /// </summary>
        public void ClearSectionSpecifications()
        {
            d_sections.Clear();
        }

        /// <summary>
        /// Return the priority of this layer.
        /// </summary>
        /// <returns>
        /// uint value descibing the priority of this LayerSpecification.
        /// </returns>
        public int GetLayerPriority()
        {
            return d_layerPriority;
        }
        
        /// <summary>
        /// Sets the priority of this layer.
        /// </summary>
        /// <param name="priority">
        /// uint value descibing the priority of this LayerSpecification.
        /// </param>
        public void SetLayerPriority(int priority)
        {
            d_layerPriority = priority;
        }

        // required to sort layers according to priority
        // TODO: bool operator<(const LayerSpecification& other) const; { return d_layerPriority < otherLayerSpec.getLayerPriority(); }

        /// <summary>
        /// Writes an xml representation of this Layer to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            //xml_stream.openTag(Falagard_xmlHandler::LayerElement);

            //if (d_layerPriority != 0)
            //    xml_stream.attribute(Falagard_xmlHandler::PriorityAttribute, PropertyHelper<uint>::toString(d_layerPriority));

            //// ouput all sections in this layer
            //for(SectionSpecificationList::const_iterator curr = d_sections.begin(); curr != d_sections.end(); ++curr)
            //{
            //    (*curr).writeXMLToStream(xml_stream);
            //}

            //xml_stream.closeTag();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Collection of SectionSpecification objects descibing the sections to be drawn for this layer.
        /// </summary>
        private readonly List<SectionSpecification> d_sections = new List<SectionSpecification>();

        /// <summary>
        /// Priority of the layer.
        /// </summary>
        private int d_layerPriority;

        //public:
        //typedef ConstVectorIterator<SectionList> SectionIterator;
        //SectionIterator getSectionIterator() const;

        public IEnumerator<SectionSpecification> GetSectionIterator()
        {
            //LayerSpecification::SectionSpecificationPointerList pointerList;

            //SectionSpecificationList::iterator sectionSpecificationIter = d_sections.begin();
            //SectionSpecificationList::iterator sectionSpecificationEnd = d_sections.end();
            //while( sectionSpecificationIter != sectionSpecificationEnd )
            //{
            //    pointerList.push_back(&(*sectionSpecificationIter));
            //    ++sectionSpecificationIter;
            //}

            //return pointerList;
            throw new NotImplementedException();
        }
        
    }
}