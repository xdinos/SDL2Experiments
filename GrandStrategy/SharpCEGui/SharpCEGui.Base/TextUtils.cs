namespace SharpCEGui.Base
{
    public static class TextUtils
    {
        /// <summary>
        /// The default set of whitespace
        /// </summary>
        public const string DefaultWhitespace = " \n\t\r";

        /// <summary>
        /// default set of alphanumerical characters.
        /// </summary>
        public const string DefaultAlphanumerical = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        /// <summary>
        /// The default set of word-wrap delimiters.
        /// </summary>
        public const string DefaultWrapDelimiters = " \n\t\r";

        public static int GetWordStartIdx(string str, int idx)
        {
            var temp = str.CEGuiSubstring(0, idx);

            TrimTrailingChars(ref temp, DefaultWhitespace);

            if (temp.Length <= 1)
            {
                return 0;
            }

            // identify the type of character at 'pos'
            if (-1 != DefaultAlphanumerical.IndexOf(temp[temp.Length - 1]))
            {
                idx = temp.IndexNotOf(DefaultAlphanumerical);
            }
            // since whitespace was stripped, character must be a symbol
            else
            {
                idx = temp.IndexNotOf(DefaultAlphanumerical + DefaultWhitespace);
            }

            // make sure we do not go past end of string (+1)
            if (idx == -1)
            {
                return 0;
            }
            else
            {
                return idx + 1;
            }
        }

        public static int GetNextWordStartIdx(string str, int idx)
        {
            var str_len = str.Length;

            // do some checks for simple cases
            if ((idx >= str_len) || (str_len == 0))
            {
                return str_len;
            }

            // is character at 'idx' alphanumeric
            if (-1 != DefaultAlphanumerical.IndexOf(str[idx]))
            {
                // find position of next character that is not alphanumeric
                idx = str.IndexNotOf(DefaultAlphanumerical, idx);
            }
                // is character also not whitespace (therefore a symbol)
            else if (-1 == DefaultWhitespace.IndexOf(str[idx]))
            {
                // find index of next character that is either alphanumeric or whitespace
                idx = str.IndexOf(DefaultAlphanumerical + DefaultWhitespace, idx);
            }

            // check result at this stage.
            if (-1 == idx)
            {
                idx = str_len;
            }
            else
            {
                // if character at 'idx' is whitespace
                if (-1 != DefaultWhitespace.IndexOf(str[idx]))
                {
                    // find next character that is not whitespace
                    idx = str.IndexNotOf(DefaultWhitespace, idx);
                }

                if (-1 == idx)
                {
                    idx = str_len;
                }
            }

            return idx;
        }

        public static void TrimTrailingChars(ref string str, string chars)
        {
            str = str.TrimEnd(chars.ToCharArray());
            //String::size_type idx = str.find_last_not_of(chars);

            //if (idx != String::npos)
            //{
            //    str.resize(idx + 1);
            //}
            //else
            //{
            //    str.erase();
            //}
        }

    }
}