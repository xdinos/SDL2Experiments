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

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class AnimationXmlHandler : ChainedXmlHandler
    {
        /// <summary>
        /// String holding the element handled by this class.
        /// </summary>
        public const string ElementName = "Animations";

        public override string GetSchemaName()
        {
            return AnimationManager.XMLSchemaName;
        }
        public override string GetDefaultResourceGroup()
        {
            return AnimationManager.GetDefaultResourceGroup();
        }

        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            if (element == ElementName)
            {
                Logger.LogInsane("===== Begin Animations parsing =====");
            }
            else if (element == AnimationDefinitionHandler.ElementName)
            {
                ChainedHandler = new AnimationDefinitionHandler(attributes, "");
            }
            else
            {
                System.GetSingleton().Logger
                      .LogEvent("AnimationXmlHandler.ElementStart: <" + element + "> is invalid at this location.",
                                LoggingLevel.Errors);
            }
        }

        protected override void ElementEndLocal(string element)
        {
            if (element == ElementName)
            {
                System.GetSingleton().Logger.LogEvent("===== End Animations parsing =====", LoggingLevel.Insane);
            }
            else
            {
                System.GetSingleton().Logger
                      .LogEvent(
                          "AnimationXmlHandler.ElementEnd: </" + element + "> is invalid at this location.",
                          LoggingLevel.Errors);
            }
        }
    }
}