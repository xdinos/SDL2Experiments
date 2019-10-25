using System;
using System.Text.RegularExpressions;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Interface for Regex matching support classes
    /// </summary>
    public abstract class RegexMatcher
    {
        /// <summary>
        /// Enumeration of possible states when cosidering a regex match.
        /// </summary>
        public enum MatchState
        {
            /// <summary>
            /// String matches the regular expression completely.
            /// </summary>
            MS_VALID,

            /// <summary>
            /// String does not match the regular expression at all.
            /// </summary>
            MS_INVALID,

            /// <summary>
            /// String partially matches. Changes to the string could result in
            /// either an MS_VALID or MS_INVALID MatchState.
            /// </summary>
            MS_PARTIAL
        }

        //! Destructor.
        //TODO: virtual ~RegexMatcher() {}

        /// <summary>
        /// Set the regex string that will be matched against.
        /// </summary>
        /// <param name="regex"></param>
        public abstract void SetRegexString(string regex);

        /// <summary>
        /// Return reference to current regex string set.
        /// </summary>
        /// <returns></returns>
        public abstract string GetRegexString();

        /// <summary>
        /// Return the MatchState result for the given String.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public abstract MatchState GetMatchStateOfString(string str);
    }


    /// <summary>
    /// WindowEventArgs based class that is used for notifications regarding
    /// RegexMatcher::MatchState changes for some component.
    /// </summary>
    public class RegexMatchStateEventArgs : WindowEventArgs
    {
        public RegexMatchStateEventArgs(Window wnd, RegexMatcher.MatchState state) :
            base(wnd)
        {
            matchState = state;
        }

        public RegexMatcher.MatchState matchState;
    }

    public class DefaultRegexMatcher : RegexMatcher
    {
        public override void SetRegexString(string regex)
        {
            _string = String.Empty;
            _regex = new Regex("^"+regex+"$", RegexOptions.Compiled);
            _string = regex;
        }

        public override string GetRegexString()
        {
            return _string;
        }

        public override MatchState GetMatchStateOfString(string str)
        {
            // if the regex is not valid, then an exception is thrown
            if (_regex == null)
                throw new InvalidRequestException("Attempt to use invalid RegEx '" + _string + "'.");

            var state = MatchState.MS_PARTIAL;

            try
            {
                return _regex.IsMatch(str) ? MatchState.MS_VALID : MatchState.MS_INVALID;
            }
            catch
            {
                state = MatchState.MS_INVALID;
            }

            return state;
        }

        private string _string;
        private Regex _regex;
    }
}