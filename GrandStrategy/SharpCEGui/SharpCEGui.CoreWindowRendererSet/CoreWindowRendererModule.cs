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

using SharpCEGui.Base;

namespace SharpCEGui.CoreWindowRendererSet
{
    /// <summary>
    /// Implementation of WindowRendererModule for the Falagard window renderers
    /// </summary>
    public class CoreWindowRendererModule : FactoryModule
    {
        /// <summary>
        /// 
        /// </summary>
        public CoreWindowRendererModule()
        {
            Registry.Add(new TplWRFactoryRegisterer<FalagardButton>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardDefault>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardEditbox>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardFrameWindow>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardItemEntry>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardListHeader>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardListHeaderSegment>());
            //Registry.Add(new TplWRFactoryRegisterer<FalagardListbox>()); // TODO: remove this
            Registry.Add(new TplWRFactoryRegisterer<FalagardListView>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardMenubar>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardMenuItem>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardMultiColumnList>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardMultiLineEditbox>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardPopupMenu>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardProgressBar>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardScrollablePane>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardScrollbar>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardSlider>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardStatic>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardStaticImage>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardStaticText>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardTabButton>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardTabControl>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardTitlebar>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardToggleButton>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardTooltip>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardItemListbox>());
            Registry.Add(new TplWRFactoryRegisterer<FalagardTree>()); // TODO: remove this
            // TODO: Registry.Add(new TplWRFactoryRegisterer<FalagardTreeView>());
        }

        // TODO: Destructor
        //~CoreWindowRendererModule()
        //{
        //    FactoryRegistry::iterator i = d_registry.begin();
        //    for (; i != d_registry.end(); ++i)
        //        delete(*i);
        //}

    }
}