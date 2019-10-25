namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CEGuiSubstring(this string text, int startIndex, int length)
        {
            return text.Substring(startIndex, length == -1 ? text.Length : length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delimiters"></param>
        /// <returns></returns>
        public static int IndexNotOf(this string text, string delimiters)
        {
            return IndexNotOf(text, delimiters, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delimiters"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int IndexNotOf(this string text, string delimiters, int startIndex)
        {
            var index = startIndex;

            while (index < text.Length)
            {
                if (delimiters.IndexOf(text[index]) == -1)
                {
                    return index;
                }

                index++;
            }

            return -1;
        }
    }
}