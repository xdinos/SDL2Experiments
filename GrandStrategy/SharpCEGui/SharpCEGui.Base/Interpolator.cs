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
    /// Defines a 'interpolator' class.
    /// 
    /// Interpolator allows you to interpolate between 2 properties.
    /// You can jut pass them as strings and Interpolator does everything for you.
    /// 
    /// If you want to define your own interpolator, inherit this class and add it
    /// to AnimationManager via AnimationManager::addInterpolator to make it
    /// available for animations.
    /// </summary>
    /// <seealso cref="AnimationManager"/>
    public interface Interpolator
    {
        //! destructor
        // TODO: virtual ~Interpolator() {};

        /// <summary>
        /// returns type string of this interpolator
        /// </summary>
        /// <returns></returns>
        string GetInterpolatorType();

        // interpolate methods aren't const, because the interpolator could provide
        // some sort of caching mechanism if converting properties is too expensive
        // that it is worth it

        /// <summary>
        /// this is used when Affector is set to apply values in absolute mode
        /// (application method == AM_Absolute)
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        string InterpolateAbsolute(string value1, string value2, float position);

        /// <summary>
        /// this is used when Affector is set to apply values in relative mode
        /// (application method == AM_Relative)
        /// </summary>
        /// <param name="base"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        string InterpolateRelative(string @base, string value1, string value2, float position);

        /// <summary>
        /// this is used when Affector is set to apply values in relative multiply
        /// mode (application method == AM_RelativeMultiply)
        /// </summary>
        /// <param name="base"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        string InterpolateRelativeMultiply(string @base, string value1, string value2, float position);
    }

    public abstract class TplInterpolatorBase : Interpolator
    {
        protected TplInterpolatorBase(string type)
        {
            _type = type;
        }
    
        //! destructor
        // TODO: virtual ~TplInterpolatorBase() {}
    
        //! \copydoc Interpolator::getType
        public virtual string GetInterpolatorType()
        {
            return _type;
        }

        public abstract string InterpolateAbsolute(string value1, string value2, float position);

        public abstract string InterpolateRelative(string @base, string value1, string value2, float position);

        public abstract string InterpolateRelativeMultiply(string @base, string value1, string value2, float position);

        private readonly string _type;
    }

    /// <summary>
    /// Generic linear interpolator class
    /// </summary>
    /// <typeparam name="T">
    /// This class works on a simple formula: result = val1 * (1.0f - position) + val2 * (position);
    /// You can only use it on types that have operator*(float) and operator+(T) overloaded!
    /// </typeparam>
    public class TplLinearInterpolator<T> : TplInterpolatorBase
    {
        private Func<T, T, T> _add; 
        private Func<T, float, T> _mul;
 
        public TplLinearInterpolator(string type, Func<T, T, T> add, Func<T, float, T> mul) :
            base(type)
        {
            _add = add;
            _mul = mul;
        }
    
        //! destructor
        // TODO: virtual ~TplLinearInterpolator() {}
    
        //! \copydoc Interpolator::interpolateAbsolute
        public override string InterpolateAbsolute(string value1, string value2, float position)
        {
            var val1 = PropertyHelper.FromString<T>(value1);
            var val2 = PropertyHelper.FromString<T>(value2);

            var result = _add(_mul(val1, (1.0f - position)),_mul(val2,position));
            //var result = (val1 * (1.0f - position) + val2 * (position));

            return PropertyHelper.ToString<T>(result);
        }
    
        //! \copydoc Interpolator::interpolateRelative
        public override string InterpolateRelative(string @base, string value1, string value2, float position)
        {
            var bas = PropertyHelper.FromString<T>(@base);
            var val1 = PropertyHelper.FromString<T>(value1);
            var val2 = PropertyHelper.FromString<T>(value2);

            var result = _add(bas, _add(_mul(val1, (1.0f - position)), _mul(val2, position)));
            //T result = (bas + (val1 * (1.0f - position) + val2 * (position)));

            return PropertyHelper.ToString<T>(result);
        }
    
        //! \copydoc Interpolator::interpolateRelativeMultiply
        public override string InterpolateRelativeMultiply(string @base, string value1, string value2, float position)
        {
            var bas = PropertyHelper.FromString<T>(@base);
            var val1 = PropertyHelper.FromString<float>(value1);
            var val2 = PropertyHelper.FromString<float>(value2);

            var mul = val1 * (1.0f - position) + val2 * (position);

            T result = _mul(bas, mul);

            return PropertyHelper.ToString<T>(result);
        }
    }

    /// <summary>
    /// Generic discrete interpolator class
    /// 
    /// This class returns the value the position is closest to.
    /// You can only use it on any types (they must have a PropertyHelper of course).
    /// No requirements on operators 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TplDiscreteInterpolator<T> : TplInterpolatorBase
    {
        public TplDiscreteInterpolator(string type):
            base(type)
        {}
    
        //! destructor
        // TODO: virtual ~TplDiscreteInterpolator() {}
    
        //! \copydoc Interpolator::interpolateAbsolute
        public override string InterpolateAbsolute(string value1, string value2, float position)
        {
            var val1 = PropertyHelper.FromString<T>(value1);
            var val2 = PropertyHelper.FromString<T>(value2);

            T result = position < 0.5 ? val1 : val2;

            return PropertyHelper.ToString<T>(result);
        }
    
        //! \copydoc Interpolator::interpolateRelative
        public override string InterpolateRelative(string @base, string value1, string value2, float position)
        {
            var val1 = PropertyHelper.FromString<T>(value1);
            var val2 = PropertyHelper.FromString<T>(value2);
        
            T result = position < 0.5 ? val1 : val2;
        
            return PropertyHelper.ToString<T>(result);
        }
    
        //! \copydoc Interpolator::interpolateRelativeMultiply
        public override string InterpolateRelativeMultiply(string @base, string value1, string value2, float position)
        {
            var bas = PropertyHelper.FromString<T>(@base);

            // there is nothing we can do, we have no idea what operators T has overloaded
            return PropertyHelper.ToString<T>(bas);
        }
    }

    /// <summary>
    /// Generic discrete relative interpolator class
    /// 
    /// This class returns the value the position is closest to. It is different to discrete
    /// interpolator in interpolateRelative. It adds the resulting value to the base value.
    /// 
    /// You can use this on types that have operator+(T) overloaded
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TplDiscreteRelativeInterpolator<T> : TplDiscreteInterpolator<T>
    {
        private Func<T, T, T> _add;
 
        public TplDiscreteRelativeInterpolator(string type, Func<T, T, T> add) :
            base(type)
        {
            _add = add;
        }
    
        //! destructor
        // TODO: virtual ~TplDiscreteRelativeInterpolator() {}
    
        //! \copydoc Interpolator::interpolateRelative
        public override string InterpolateRelative(string @base, string value1, string value2, float position)
        {
            var bas = PropertyHelper.FromString<T>(@base);
            var val1 = PropertyHelper.FromString<T>(value1);
            var val2 = PropertyHelper.FromString<T>(value2);

            T result = _add(bas, (position < 0.5 ? val1 : val2));
        
            return PropertyHelper.ToString<T>(result);
        }
    };
}