using System;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Special interpolator class for Quaternion
    /// 
    /// Quaternions can't be interpolated as floats and/or vectors, we have to use
    /// "Spherical linear interpolator" instead.
    /// </summary>
    public class QuaternionSlerpInterpolator : Interpolator
    {
        //! \copydoc Interpolator::getType
        public string GetInterpolatorType()
        {
            return "QuaternionSlerp";
        }

        //! \copydoc Interpolator::interpolateAbsolute
        public string InterpolateAbsolute(string value1, string value2, float position)
        {
            var val1 = MathematicsEx.QuaternionParse(value1);
            var val2 = MathematicsEx.QuaternionParse(value2);

            return PropertyHelper.ToString(Lunatics.Mathematics.Quaternion.Slerp(val1, val2, position));
        }

        //! \copydoc Interpolator::interpolateRelative
        public string InterpolateRelative(string @base, string value1, string value2, float position)
        {
            throw new NotImplementedException();
        }

        //! \copydoc Interpolator::interpolateRelativeMultiply
        public string InterpolateRelativeMultiply(string @base, string value1, string value2, float position)
        {
            throw new NotImplementedException();
        }
    }
}