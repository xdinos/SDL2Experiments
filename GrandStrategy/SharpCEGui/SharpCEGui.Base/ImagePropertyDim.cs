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
    /// ImageDimBase subclass that accesses an image fetched via a property.
    /// </summary>
    public class ImagePropertyDim : ImageDimBase
    {
        public ImagePropertyDim() {}

        /*!
        \brief
            Constructor.

        \param property_name
            String holding the name of the property on the target that will be
            accessed to retrieve the name of the image to be accessed by the
            ImageDim.

        \param dim
            DimensionType value indicating which dimension of an Image that
            this ImageDim is to represent.
        */
        public ImagePropertyDim(string property_name, DimensionType dim)
        {
            throw new NotImplementedException();
        }

        //! return the name of the property accessed by this ImagePropertyDim.
        public string GetSourceProperty()
        {
            throw new NotImplementedException();
        }

        //! set the name of the property accessed by this ImagePropertyDim.
        public void SetSourceProperty(string property_name)
        {
            throw new NotImplementedException();
        }

        // Implementation of the base class interface
        public override BaseDim Clone()
        {
            throw new NotImplementedException();
        }

        // Implementation / overrides of functions in superclasses
        protected override Image GetSourceImage(Window wnd)
        {
            throw new NotImplementedException();
        }

        protected override void WriteXmlElementNameImpl(XMLSerializer xmlStream)
        {
            throw new NotImplementedException();
        }

        protected override void WriteXmlElementAttributesImpl(XMLSerializer xmlStream)
        {
            throw new NotImplementedException();
        }

        //! name of the property from which to fetch the image name.
        protected string d_propertyName;
    };
}