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
using System.Linq;

namespace SharpCEGui.Base
{
    // TODO: moveKeyFrame, this will be vital for any animation editing tools
    /// <summary>
    /// Defines an 'affector' class.
    /// 
    /// Affector is part of Animation definition. It is set to affect
    /// one Property using one Interpolator.
    /// </summary>
    public class Affector
    {
        /// <summary>
        /// enumerates the possible methods of application
        /// </summary>
        public enum ApplicationMethod
        {
            /// <summary>
            /// applies values as absolutes
            /// </summary>
            Absolute,

            /// <summary>
            /// saves a base value after the animation is started and applies
            ///  relatively to that
            /// </summary>
            Relative,

            /// <summary>
            /// saves a base value after the animation is started and applies
            /// by multiplying this base value with key frame floats
            /// </summary>
            RelativeMultiply
        };

        /// <summary>
        /// internal constructor, please construct Affectors via
        /// Animation.CreateAffector only
        /// </summary>
        /// <param name="parent"></param>
        internal Affector(Animation parent)
        {
            _parent = parent;
            _applicationMethod=ApplicationMethod.Absolute;
            _targetProperty = "";
            _interpolator = null;
        }

        // TODO: destructor, this destroys all key frames defined inside this affector
        //~Affector()
        //{
        //    while (d_keyFrames.size() > 0)
        //    {
        //        destroyKeyFrame(getKeyFrameAtIdx(0));
        //    }
        //}

        /// <summary>
        /// Retrieves the parent animation of this keyframe
        /// </summary>
        /// <returns></returns>
        public Animation GetParent()
        {
    	    return _parent;
        }

        /// <summary>
        /// Retrieves index with which this affector is retrievable in parent Animation
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// The index is only valid as long as the list of affectors is unchanged in animation!
        /// </remarks>
        public int GetIdxInParent()
        {
            var parent = GetParent();
            global::System.Diagnostics.Debug.Assert(GetParent() != null, "No parent, no index in parent!");

            var i = 0;
            while (i < parent.GetNumAffectors())
            {
                if (parent.GetAffectorAtIdx(i) == this)
                {
                    return i;
                }

                ++i;
            }

            throw new UnknownObjectException("Affector wasn't found in parent, therefore its index is unknown!");
        }

        /// <summary>
        /// Sets the application method
        /// </summary>
        /// <param name="method">
        /// Values can be applied in 2 ways - as absolute values or relative to base
        /// value that is retrieved and saved after animation is started
        /// </param>
        public void SetApplicationMethod(ApplicationMethod method)
        {
            _applicationMethod = method;
        }

        /// <summary>
        /// Retrieves current application method
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Affector.SetApplicationMethod"/>
        public ApplicationMethod GetApplicationMethod()
        {
            return _applicationMethod;
        }

        /// <summary>
        /// Sets the property that will be affected
        /// </summary>
        /// <param name="target"></param>
        public void SetTargetProperty(string target)
        {
            _targetProperty = target;
        }

        /// <summary>
        /// Gets the property that will be affected
        /// </summary>
        /// <returns></returns>
        public string GetTargetProperty()
        {
            return _targetProperty;
        }

        /// <summary>
        /// Sets interpolator of this Affector
        /// <para>
        /// Interpolator has to be set for the Affector to work!
        /// </para>
        /// </summary>
        /// <param name="interpolator"></param>
        public void SetInterpolator(Interpolator interpolator)
        {
            _interpolator = interpolator;
        }

        /// <summary>
        /// Sets interpolator of this Affector
        /// <para>
        /// Interpolator has to be set for the Affector to work!
        /// </para>
        /// </summary>
        /// <param name="name"></param>
        public void SetInterpolator(string name)
        {
            _interpolator = AnimationManager.GetSingleton().GetInterpolator(name);
        }

        /// <summary>
        /// Retrieves currently used interpolator of this Affector
        /// </summary>
        /// <returns></returns>
        public Interpolator GetInterpolator()
        {
            return _interpolator;
        }

        /// <summary>
        /// Creates a KeyFrame at given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public KeyFrame CreateKeyFrame(float position)
        {
            if (_keyFrames.ContainsKey(position))
            {
                throw new InvalidRequestException(
                    "Unable to create KeyFrame at given position, there already is a KeyFrame on that position.");
            }

            var ret = new KeyFrame(this, position);
            _keyFrames.Add(position, ret);

            return ret;
        }

        /// <summary>
        /// Creates a KeyFrame at given position
        /// <para>
        /// This is a helper method, you can set all these values after you create
        /// the KeyFrame
        /// </para>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="value"></param>
        /// <param name="progression"></param>
        /// <param name="sourceProperty"></param>
        /// <returns></returns>
        public KeyFrame CreateKeyFrame(float position, string value,
                                       KeyFrame.Progression progression = KeyFrame.Progression.Linear,
                                       string sourceProperty = "")
        {
            var ret = CreateKeyFrame(position);
            ret.SetValue(value);
            ret.SetProgression(progression);
            ret.SetSourceProperty(sourceProperty);

            return ret;
        }

        /// <summary>
        /// Destroys given keyframe
        /// </summary>
        /// <param name="keyframe"></param>
        public void DestroyKeyFrame(KeyFrame keyframe)
        {
            if (!_keyFrames.ContainsKey(keyframe.GetPosition()))
                throw new InvalidRequestException("Unable to destroy given KeyFrame! No such KeyFrame was found.");
            
            _keyFrames.Remove(keyframe.GetPosition());
            //TODO: CEGUI_DELETE_AO keyframe;
        }

        /// <summary>
        /// Retrieves a KeyFrame at given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public KeyFrame GetKeyFrameAtPosition(float position)
        {
            if (!_keyFrames.ContainsKey(position))
                throw new InvalidRequestException("Can't find a KeyFrame with given position.");

            return _keyFrames[position];
        }

        /// <summary>
        /// Checks whether there is a key frame at given position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool HasKeyFrameAtPosition(float position)
        {
            return _keyFrames.ContainsKey(position);
        }

        /// <summary>
        /// Retrieves a KeyFrame with given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public KeyFrame GetKeyFrameAtIdx(int index)
        {
            if (index >= _keyFrames.Count)
            {
                throw new InvalidRequestException("Out of bounds!");
            }

            return _keyFrames.Skip(index).First().Value;
        }

        /// <summary>
        /// Returns number of key frames defined in this affector
        /// </summary>
        /// <returns></returns>
        public int GetNumKeyFrames()
        {
            return _keyFrames.Count;
        }

        /// <summary>
        /// Moves given key frame to given new position
        /// </summary>
        /// <param name="keyframe"></param>
        /// <param name="newPosition"></param>
        public void MoveKeyFrameToPosition(KeyFrame keyframe, float newPosition)
        {
            if (keyframe.GetPosition() == newPosition)
		        return;

            if (!_keyFrames.ContainsKey(newPosition))
            {
                throw new InvalidRequestException("There is already a key frame at position: " +
                                                  PropertyHelper.ToString(newPosition) + ".");
            }

            foreach (var it in _keyFrames)
            {
                if (it.Value == keyframe)
                {
                    _keyFrames.Remove(it.Key);
                    _keyFrames.Add(newPosition, keyframe);

                    keyframe.NotifyPositionChanged(newPosition);
                    return;
                }
            }

            throw new UnknownObjectException("passed key frame wasn't found within this affector");
        }

        /// <summary>
        /// Moves key frame at given old position to given new position
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="newPosition"></param>
        public void MoveKeyFrameToPosition(float oldPosition, float newPosition)
        {
            var kf = GetKeyFrameAtPosition(oldPosition);

            MoveKeyFrameToPosition(kf, newPosition);
        }

        /// <summary>
        /// Internal method, causes all properties that are used by this affector
        /// and it's keyframes to be saved
        /// </summary>
        /// <param name="instance">
        /// So their values are still known after they've been affected.
        /// </param>
        public void SavePropertyValues(AnimationInstance instance)
        {
            switch (_applicationMethod)
            {
                case ApplicationMethod.Relative:
                case ApplicationMethod.RelativeMultiply:
                    instance.SavePropertyValue(_targetProperty);
                    break;
            }

            // now let all keyframes save their desired property values too
            foreach (var it in _keyFrames)
                it.Value.SavePropertyValue(instance);
        }

        /// <summary>
        /// Applies this Affector's definition with parameters from given Animation Instance
        /// <para>This function is internal so unless you know what you're doing, don't touch!</para>
        /// </summary>
        /// <param name="instance"></param>
        /// <seealso cref="AnimationInstance"/>
        public void Apply(AnimationInstance instance)
        {
            var target = instance.GetTarget();
            var position = instance.GetPosition();

            // special case
            if (_keyFrames.Count==0)
                return;

            if (String.IsNullOrEmpty(_targetProperty))
            {
                System.GetSingleton().Logger.LogEvent("Affector can't be applied when target property is empty!", LoggingLevel.Warnings);
                return;
            }

            if (_interpolator==null)
            {
                System.GetSingleton().Logger.LogEvent("Affector can't be applied when no interpolator is set!", LoggingLevel.Warnings);
                return;
            }

            KeyFrame left = null;
            KeyFrame right = null;

            // find 2 neighbouring keyframes
            foreach (var it in _keyFrames)
            {
                var current = it.Value;
                if (current.GetPosition() <= position)
                {
                    left = current;
                }

                if (current.GetPosition() >= position && right==null)
                {
                    right = current;
                }
            }

            float leftDistance, rightDistance;

            if (left!=null)
            {
                leftDistance = position - left.GetPosition();
            }
            else
            {
                // if no keyframe is suitable for left neighbour, pick the first one
                left = _keyFrames.First().Value;
                leftDistance = 0;
            }

            if (right!=null)
            {
                rightDistance = right.GetPosition() - position;
            }
            else
            {
                // if no keyframe is suitable for the right neighbour, pick the last one
                right = _keyFrames.Last().Value;
                rightDistance = 0;
            }

            // if there is just one keyframe and we are right on it
            if (leftDistance + rightDistance == 0f)
            {
                leftDistance = rightDistance = 0.5f;
            }

            // alter interpolation position using the right neighbours progression method
            var interpolationPosition = right.AlterInterpolationPosition(leftDistance / (leftDistance + rightDistance));

            // absolute application method
            if (_applicationMethod == ApplicationMethod.Absolute)
            {
                var result = _interpolator.InterpolateAbsolute(left.GetValueForAnimation(instance),
                                                                right.GetValueForAnimation(instance),
                                                                interpolationPosition);

                target.SetProperty(_targetProperty, result);
            }
            // relative application method
            else if (_applicationMethod == ApplicationMethod.Relative)
            {
                var @base = instance.GetSavedPropertyValue(GetTargetProperty());

                var result = _interpolator.InterpolateRelative(@base,
                                                                left.GetValueForAnimation(instance),
                                                                right.GetValueForAnimation(instance),
                                                                interpolationPosition);

                target.SetProperty(_targetProperty, result);
            }
            // relative multiply application method
            else if (_applicationMethod == ApplicationMethod.RelativeMultiply)
            {
                var @base = instance.GetSavedPropertyValue(GetTargetProperty());

                var result = _interpolator.InterpolateRelativeMultiply(@base,
                                                                        left.GetValueForAnimation(instance),
                                                                        right.GetValueForAnimation(instance),
                                                                        interpolationPosition);

                target.SetProperty(_targetProperty, result);
            }
            else
            {
                // todo: more application methods?
                global::System.Diagnostics.Debug.Assert(false);
            }
        }

        /// <summary>
        /// Writes an xml representation of this Affector to \a out_stream.
        /// </summary>
        /// <param name="xmlStream">
        /// Stream where xml data should be output.
        /// </param>
        public void WriteXmlToStream(XMLSerializer xmlStream)
        {
            xmlStream.OpenTag(AnimationAffectorHandler.ElementName);

            var applicationMethod = String.Empty;
            switch(GetApplicationMethod())
            {
            case ApplicationMethod.Absolute:
                applicationMethod = AnimationAffectorHandler.ApplicationMethodAbsolute;
                break;
            case ApplicationMethod.Relative:
                applicationMethod = AnimationAffectorHandler.ApplicationMethodRelative;
                break;
            case ApplicationMethod.RelativeMultiply:
                applicationMethod = AnimationAffectorHandler.ApplicationMethodRelativeMultiply;
                break;

            default:
                global::System.Diagnostics.Debug.Assert(false, "How did we get here?");
                break;
            }

            xmlStream.Attribute(AnimationAffectorHandler.ApplicationMethodAttribute, applicationMethod);

            xmlStream.Attribute(AnimationAffectorHandler.TargetPropertyAttribute, GetTargetProperty());

            if (GetInterpolator()!=null)
            {
                xmlStream.Attribute(AnimationAffectorHandler.InterpolatorAttribute, GetInterpolator().GetInterpolatorType());
            }

            foreach (var it in _keyFrames)
            {
                it.Value.WriteXMLToStream(xmlStream);
            }

            xmlStream.CloseTag();
        }

        #region Fields

        /// <summary>
        /// application method
        /// </summary>
        private ApplicationMethod _applicationMethod;

        /// <summary>
        /// property that gets affected by this affector
        /// </summary>
        private String _targetProperty;

        /// <summary>
        /// curently used interpolator (has to be set for the Affector to work!)
        /// </summary>
        private Interpolator _interpolator;

        /// <summary>
        /// parent animation definition
        /// </summary>
        private readonly Animation _parent;
        
        /// <summary>
        /// keyframes of this affector (if there are no keyframes, this affector won't do anything!)
        /// </summary>
        private readonly Dictionary<float, KeyFrame> _keyFrames =new Dictionary<float, KeyFrame>();

        #endregion
    }
}