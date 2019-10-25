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
using System.Text;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Basic RenderedStringParser class that offers support for the following tags:
    /// - 'colour' value is a CEGUI colour property value.
    /// - 'font' value is the name of a font.
    /// - 'image' value is a CEGUI image property value.
    /// - 'window' value is the name of a window.
    /// - 'vert-alignment' value is either top, bottom, centre or stretch.
    /// - 'padding' value is a CEGUI Rect property value.
    /// - 'top-padding' value is a float.
    /// - 'bottom-padding' value is a float.
    /// - 'left-padding' value is a float.
    /// - 'right-padding' value is a float.
    /// - 'image-size' value is a CEGUI size property value.
    /// - 'image-width' value is a float.
    /// - 'image-height' value is a float.
    /// - 'aspect-lock' value is a boolean (NB: this currently has no effect).
    /// </summary>
    public sealed class BasicRenderedStringParser : RenderedStringParser
    {
        // Strings for supported tags
        public const string ColourTagName = "colour";
        public const string FontTagName = "font";
        public const string ImageTagName = "image";
        public const string WindowTagName = "window";
        public const string VertAlignmentTagName = "vert-alignment";
        public const string PaddingTagName = "padding";
        public const string TopPaddingTagName = "top-padding";
        public const string BottomPaddingTagName = "bottom-padding";
        public const string LeftPaddingTagName = "left-padding";
        public const string RightPaddingTagName = "right-padding";
        public const string AspectLockTagName = "aspect-lock";
        public const string ImageSizeTagName = "image-size";
        public const string ImageWidthTagName = "image-width";
        public const string ImageHeightTagName = "image-height";
        public const string TopAlignedValueName = "top";
        public const string BottomAlignedValueName = "bottom";
        public const string CentreAlignedValueName = "centre";
        public const string StretchAlignedValueName = "stretch";

        /// <summary>
        /// Constructor.
        /// </summary>
        public BasicRenderedStringParser()
        {
            d_initialColours = new ColourRect();
            d_vertAlignment = VerticalFormatting.BottomAligned;
            d_imageSize = Sizef.Zero;
            d_aspectLock = false;
            d_initialised = false;

            InitialiseDefaultState();
        }

        /// <summary>
        /// Initialising constructor.
        /// </summary>
        /// <param name="initialFont">
        /// Reference to a String holding the name of the initial font to be used.
        /// </param>
        /// <param name="initialColours">
        /// Reference to a ColourRect describing the initial colours to be used.
        /// </param>
        public BasicRenderedStringParser(string initialFont, ColourRect initialColours)
        {
            d_initialFontName = initialFont;
            d_initialColours = new ColourRect(initialColours);
            d_vertAlignment = VerticalFormatting.BottomAligned;
            d_imageSize = Sizef.Zero;
            d_aspectLock = false;
            d_initialised = false;

            InitialiseDefaultState();
        }

        /// <summary>
        /// set the initial font name to be used on subsequent calls to parse.
        /// </summary>
        /// <param name="fontName">
        /// String object holding the name of the font.
        /// </param>
        public void SetInitialFontName(string fontName)
        {
            d_initialFontName = fontName;
        }

        /// <summary>
        /// Set the initial colours to be used on subsequent calls to parse.
        /// </summary>
        /// <param name="colours">
        /// ColourRect object holding the colours.
        /// </param>
        public void SetInitialColours(ColourRect colours)
        {
            d_initialColours = new ColourRect(colours);
        }

        /// <summary>
        /// Return the name of the initial font used in each parse.
        /// </summary>
        /// <returns></returns>
        public string GetInitialFontName()
        {
            return d_initialFontName;
        }

        /// <summary>
        /// Return a ColourRect describing the initial colours used in each parse.
        /// </summary>
        /// <returns></returns>
        public ColourRect GetInitialColours()
        {
            return d_initialColours;
        }

        #region Implementation of RenderedStringParser

        public RenderedString Parse(string inputString, Font initialFont, ColourRect initialColours)
        {
            if (inputString == null)
                inputString = String.Empty;

            // first-time initialisation (due to issues with static creation order)
            if (!d_initialised)
                InitialiseTagHandlers();

            InitialiseDefaultState();

            // maybe override initial font.
            if (initialFont!=null)
                d_fontName = initialFont.GetName();

            // maybe override initial colours.
            if (initialColours!=null)
                d_colours = new ColourRect(initialColours);

            var rs=new RenderedString();
            var currSection=new StringBuilder();
            var tagString = new StringBuilder();

            for (int inputIter = 0; inputIter < inputString.Length;)
            {
                var foundTag = ParseSection(inputString, ref inputIter, inputString.Length, '[', currSection);
                AppendRenderedText(rs, currSection.ToString());

                if (!foundTag)
                    return rs;

                if (!ParseSection(inputString, ref inputIter, inputString.Length, ']', tagString))
                {
                    System.GetSingleton().Logger.LogEvent(
                        "BasicRenderedStringParser.Parse: Ignoring unterminated tag : [" + tagString);

                    return rs;
                }

                ProcessControlString(rs, tagString.ToString());
            }

            //for (String::const_iterator input_iter(input_string.begin());
            //     input_iter != input_string.end();
            //     /* no-op*/)
            //{
            //    const bool found_tag = parse_section(input_iter, input_string.end(), '[', curr_section);
            //    appendRenderedText(rs, curr_section);

            //    if (!found_tag)
            //        return rs;

            //    if (!parse_section(input_iter, input_string.end(), ']', tag_string))
            //    {
            //        Logger::getSingleton().logEvent(
            //            "BasicRenderedStringParser::parse: Ignoring unterminated tag : [" +
            //            tag_string);

            //        return rs;
            //    }

            //    processControlString(rs, tag_string);
            //}
            
            return rs;
        }

        #endregion

        /// <summary>
        /// append the text string \a text to the RenderedString \a rs.
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="text"></param>
        private void AppendRenderedText(RenderedString rs, string text)
        {
            var  cpos = 0;
            // split the given string into lines based upon the newline character
            while (text.Length > cpos)
            {
                // find next newline
                var nlpos = text.IndexOf("\n", cpos);
                // calculate length of this substring
                var len = ((nlpos != -1) ? nlpos : text.Length) - cpos;

                // construct new text component and append it.
                var rtc = new RenderedStringTextComponent(text.Substring(cpos, len), d_fontName);
                rtc.SetPadding(d_padding);
                rtc.SetColours(new ColourRect(d_colours));
                rtc.SetVerticalFormatting(d_vertAlignment);
                rtc.SetAspectLock(d_aspectLock);
                rs.AppendComponent(rtc);

                // break line if needed
                if (nlpos != -1)
                    rs.AppendLineBreak();

                // advance current position.  +1 to skip the \n char
                cpos += len + 1;
            }
        }

        /// <summary>
        /// Process the control string \a ctrl_str.
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="ctrlStr"></param>
        private void ProcessControlString(RenderedString rs, string ctrlStr)
        {
            // all our default strings are of the form <var> = <val>
            // so do a quick check for the = char and abort if it's not there.
            if (-1 == ctrlStr.IndexOf("="))
            {
                System.GetSingleton().Logger.LogEvent(
                    "BasicRenderedStringParser.ProcessControlString: unable to make " +
                    "sense of control string '" + ctrlStr + "'.  Ignoring!");
        
                return;
            }

            //char var_buf[128];
            //char val_buf[128];
            //sscanf(ctrl_str.c_str(), " %127[^ =] = '%127[^']", var_buf, val_buf);

            //const String var_str(var_buf);
            //const String val_str(val_buf);

            var values = ctrlStr.Split('=');
            var varStr = values[0];
            var valStr = values[1].Trim('\'');

            // look up handler function and
            // despatch handler, or log error
            if(_tagHandlers.ContainsKey(varStr))
                _tagHandlers[varStr](rs, valStr);
            else
                System.GetSingleton().Logger.LogEvent("BasicRenderedStringParser::processControlString: unknown " +
                                                      "control variable '" + varStr + "'.  Ignoring!");
        }

        //! initialise the default state
        private void InitialiseDefaultState()
        {
            d_padding = Rectf.Zero;
            d_colours = new ColourRect(d_initialColours);
            d_fontName = d_initialFontName;
            d_imageSize.Width = d_imageSize.Height = 0.0f;
            d_vertAlignment = VerticalFormatting.BottomAligned;
            d_aspectLock = false;
        }

        //! initialise tag handlers
        private void InitialiseTagHandlers()
        {
            _tagHandlers[ColourTagName] = HandleColour;
            _tagHandlers[FontTagName] = HandleFont;
            _tagHandlers[ImageTagName] = HandleImage;
            _tagHandlers[WindowTagName] = HandleWindow;
            _tagHandlers[VertAlignmentTagName] = HandleVertAlignment;
            _tagHandlers[PaddingTagName] = HandlePadding;
            _tagHandlers[TopPaddingTagName] = HandleTopPadding;
            _tagHandlers[BottomPaddingTagName] = HandleBottomPadding;
            _tagHandlers[LeftPaddingTagName] = HandleLeftPadding;
            _tagHandlers[RightPaddingTagName] = HandleRightPadding;
            _tagHandlers[AspectLockTagName] = HandleAspectLock;
            _tagHandlers[ImageSizeTagName] = HandleImageSize;
            _tagHandlers[ImageWidthTagName] = HandleImageWidth;
            _tagHandlers[ImageHeightTagName] = HandleImageHeight;

            d_initialised = true;
        }

        private void HandleColour(RenderedString rs, string value)
        {
            d_colours.SetColours(PropertyHelper.FromString<Colour>(value));
        }

        private void HandleFont(RenderedString rs, string value)
        {
            d_fontName = value;
        }

        private void HandleImage(RenderedString rs, string value)
        {
            var ric = new RenderedStringImageComponent(PropertyHelper.FromString<Image>(value));
            ric.SetPadding(d_padding);
            ric.SetColours(d_colours);
            ric.SetVerticalFormatting(d_vertAlignment);
            ric.SetSize(d_imageSize);
            ric.SetAspectLock(d_aspectLock);
            rs.AppendComponent(ric);
        }

        private void HandleWindow(RenderedString rs, string value)
        {
            var rwc=new RenderedStringWidgetComponent(value);
            rwc.SetPadding(d_padding);
            rwc.SetVerticalFormatting(d_vertAlignment);
            rwc.SetAspectLock(d_aspectLock);
            rs.AppendComponent(rwc);
        }

        private void HandleVertAlignment(RenderedString rs, string value)
        {
            if (value == TopAlignedValueName)
                d_vertAlignment = VerticalFormatting.TopAligned;
            else if (value == BottomAlignedValueName)
                d_vertAlignment = VerticalFormatting.BottomAligned;
            else if (value == CentreAlignedValueName)
                d_vertAlignment = VerticalFormatting.CentreAligned;
            else if (value == StretchAlignedValueName)
                d_vertAlignment = VerticalFormatting.Stretched;
            else
                System.GetSingleton().Logger.LogEvent("BasicRenderedStringParser::handleVertAlignment: unknown " +
                                                      "vertical alignment '" + value + "'.  Ignoring!");
        }

        private void HandlePadding(RenderedString rs, string value)
        {
            d_padding = PropertyHelper.FromString<Rectf>(value);
        }

        private void HandleTopPadding(RenderedString rs, string value)
        {
            d_padding.d_min.Y = Single.Parse(value);
        }

        private void HandleBottomPadding(RenderedString rs, string value)
        {
            d_padding.d_max.Y = Single.Parse(value);
        }

        private void HandleLeftPadding(RenderedString rs, string value)
        {
            d_padding.d_min.X = Single.Parse(value);
        }

        private void HandleRightPadding(RenderedString rs, string value)
        {
            d_padding.d_max.X = Single.Parse(value);
        }

        private void HandleAspectLock(RenderedString rs, string value)
        {
            d_aspectLock = Boolean.Parse(value);
        }

        private void HandleImageSize(RenderedString rs, string value)
        {
            d_imageSize = PropertyHelper.FromString<Sizef>(value);
        }

        private void HandleImageWidth(RenderedString rs, string value)
        {
            d_imageSize.Width = Single.Parse(value);
        }

        private void HandleImageHeight(RenderedString rs, string value)
        {
            d_imageSize.Height = Single.Parse(value);
        }

        /// <summary>
        /// Internal helper to parse part of a string, using backslash as an escape char 
        /// </summary>
        /// <param name="string"></param>
        /// <param name="pos"></param>
        /// <param name="end"></param>
        /// <param name="delim"></param>
        /// <param name="out"></param>
        /// <returns></returns>
        private static bool ParseSection(string @string, ref int pos, int end, char delim, StringBuilder @out)
        {
            const char escape='\\';
            @out.Clear();
            
            int startIter = pos;

            for ( ; pos != end; ++pos)
            {
                if (@string[pos] == delim)
                {
                    @out.Append(@string.Substring(startIter, pos-startIter));
                    pos++;
                    return true;
                }

                //if (@string[pos] == escape)
                //{
                //    @out.Append(@string.Substring(startIter, pos - startIter));
                //    pos++;

                //    if (pos == end)
                //        return false;

                //    startIter = pos;
                //}
            }

            @out.Append(@string.Substring(startIter, pos - startIter));
            return false;
        }

        #region Fields

        //! initial font name
        private string d_initialFontName;

        //! initial colours
        private ColourRect d_initialColours;
        
        //! active padding values.
        private Rectf d_padding;
        
        //! active colour values.
        private ColourRect d_colours;
        
        //! active font.
        private string d_fontName;
        
        //! active vertical alignment
        private VerticalFormatting d_vertAlignment;
        
        //! active image size
        private Sizef d_imageSize;
        
        //! active 'aspect lock' state
        private bool d_aspectLock;

        /// <summary>
        /// true if handlers have been registered
        /// </summary>
        private bool d_initialised;

        private readonly Dictionary<string, Action<RenderedString, String>> _tagHandlers =
            new Dictionary<string, Action<RenderedString, string>>();

        #endregion
    }
}