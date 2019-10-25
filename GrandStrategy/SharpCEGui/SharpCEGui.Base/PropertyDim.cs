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
    /// Dimension type that represents the value of a Window property.
    /// Implements BaseDim interface.
    /// </summary>
    public class PropertyDim : BaseDim
    {
        public PropertyDim()
        {
            
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">
        /// String holding the name suffix of the window on which the property
        /// is to be accessed.
        /// </param>
        /// <param name="property">
        /// String object holding the name of the property this PropertyDim
        /// represents the value of.  The property named should represent either
        /// a UDim value or a simple float value - dependning upon what \a type
        /// is specified as.
        /// </param>
        /// <param name="type">
        /// DimensionType value indicating what dimension named property
        /// represents.  The possible DimensionType values are as follows:
        /// - DT_INVALID the property should represent a simple float value.
        /// - DT_WIDTH the property should represent a UDim value where the
        ///   scale is relative to the targetted Window's width.
        /// - DT_HEIGHT the property should represent a UDim value where the
        ///   scale is relative to the targetted Window's height.
        /// - All other values will cause an InvalidRequestException exception
        ///   to be thrown.</param>
        public PropertyDim(string name, string property, DimensionType type)
        {
            d_property = property;
            d_childName = name;
            d_type = type;
        }

        /// <summary>
        /// Get the name suffix to use for this WidgetDim.
        /// </summary>
        /// <returns>
        /// String object holding the name suffix for a window/widget.
        /// </returns>
        public string GetWidgetName()
        {
            return d_childName;
        }

        /// <summary>
        /// Set the name suffix to use for this WidgetDim.
        /// </summary>
        /// <param name="name">
        /// String object holding the name suffix for a window/widget.
        /// </param>
        public void SetWidgetName(string name)
        {
            d_childName = name;
        }

        /*!
        \brief
            Get the name of the property to use for this WidgetDim.

        \return
            String object holding the name of the property.
        */
        public string GetPropertyName()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Set the name of the property to use for this WidgetDim.

        \param property
            String object holding the name of the property.

        \return
            Nothing.
        */
        public void SetPropertyName(string property)
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Gets the source dimension type for this WidgetDim.

        \return
            DimensionType value indicating which dimension of the target window to
            use as the reference / base value when accessing a property that
            represents a unified dimension:
                - DT_INVALID if the property does not represent a unified dim.
                - DT_WIDTH to use target width as reference value.
                - DT_HEIGHT to use target hight as reference value.
        */
        public DimensionType GetSourceDimension()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Sets the source dimension type for this WidgetDim.

        \param dim
            DimensionType value indicating which dimension of the target window to
            use as the reference / base value when accessing a property that
            represents a unified dimension:
                - DT_INVALID if the property does not represent a unified dim.
                - DT_WIDTH to use target width as reference value.
                - DT_HEIGHT to use target hight as reference value.
        */
        public void SetSourceDimension(DimensionType dim)
        {
            throw new NotImplementedException();
        }

        public override float GetValue(Window wnd)
        {
            // get window to use.
            var sourceWindow = String.IsNullOrEmpty(d_childName) ? wnd : wnd.GetChild(d_childName);

            if (d_type == DimensionType.Invalid)
            {
                // check property data type and convert to float if necessary
                var pi = sourceWindow.GetPropertyInstance(d_property);
                if (pi.GetDataType() == typeof(bool).Name)
                    return sourceWindow.GetProperty<bool>(d_property) ? 1.0f : 0.0f;

                // return float property value.
                return sourceWindow.GetProperty<float>(d_property);
            }

            var d = sourceWindow.GetProperty<UDim>(d_property);
            var s = sourceWindow.GetPixelSize();

            switch (d_type)
            {
                case DimensionType.Width:
                    return CoordConverter.AsAbsolute(d, s.Width);

                case DimensionType.Height:
                    return CoordConverter.AsAbsolute(d, s.Height);

                default:
                    throw new InvalidRequestException("unknown or unsupported DimensionType encountered.");
            }
        }

        public override float GetValue(Window wnd, Rectf container)
        {
            throw new NotImplementedException();
        }
        
        public override BaseDim Clone()
        {
            return new PropertyDim
                       {
                           d_childName = d_childName,
                           d_property = d_property,
                           d_type = d_type
                       };
        }

        // Implementation of the base class interface
        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream)
        {
            throw new NotImplementedException();
        }
        
        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            throw new NotImplementedException();
        }

        //! Propery that this object represents.
        private string d_property;
        //! String to hold the name of the child to access the property form.
        private string d_childName;
        //! String to hold the type of dimension
        private DimensionType d_type;
    };
}