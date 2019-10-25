using System;
using System.Collections.Generic;
using NBidi;

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class NBidiVisualMapping : BidiVisualMapping
    {
        public override BidiCharType GetBidiCharType(char charToCheck)
        {
            switch (NBidi.UnicodeCharacterDataResolver.GetBidiCharacterType(charToCheck))
            {
                case BidiCharacterType.RLE:
                    return BidiCharType.BCT_RIGHT_TO_LEFT;
                case BidiCharacterType.LRE:
                    return BidiCharType.BCT_LEFT_TO_RIGHT;
                default:
                    return BidiCharType.BCT_NEUTRAL;
            }
        }

        public override bool ReorderFromLogicalToVisual(string logical, out string visual, List<int> l2v, List<int> v2l)
        {
            try
            {
                var indexes = new int[] {};
                var lengths = new int[] {};
                visual = !String.IsNullOrEmpty(logical)
                                 ? NBidi.NBidi.LogicalToVisual(logical, out indexes, out lengths)
                                 : String.Empty;

                v2l.Clear();
                v2l.AddRange(indexes);
                l2v.Clear();
                l2v.AddRange(lengths);
                return true;
            }
            catch
            {
                visual = String.Empty;
                return false;
            }
        }
    }
}