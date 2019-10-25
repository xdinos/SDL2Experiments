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
    /// Defines a 'key frame' class.
    /// 
    /// Key frames are defined inside Affectors. The values they hold are used
    /// when animation is precisely at the key frame's position. If it's between
    /// two key frames, the value is interpolated.
    /// </summary>
    /// <seealso cref="Affector"/>
    public class KeyFrame
    {
        /// <summary>
        /// TOWARS this key frames, this means that progression method of the first
        /// key frame won't be used for anything!
        /// </summary>
        public enum Progression
        {
            /// <summary>
            /// linear progression
            /// </summary>
            Linear,
         
            /// <summary>
            /// progress is accelerated, starts slow and speeds up
            /// </summary>
            QuadraticAccelerating,
         
            //! 
            /// <summary>
            /// progress is decelerated, starts fast and slows down
            /// </summary>
            QuadraticDecelerating,

            /// <summary>
            /// left neighbour's value is picked if interpolation position is lower
            /// than 1.0, right is only picked when interpolation position is exactly 1.0
            /// </summary>
            Discrete
        };

        /// <summary>
        /// internal constructor, please use Affector::createKeyFrame
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        internal KeyFrame(Affector parent, float position)
        {
            d_parent = parent;
            d_position = position;
            d_progression = Progression.Linear;
        }

        //! internal destructor, please use Affector::destroyKeyFrame
        // TODO: ~KeyFrame(void);

        /*!
        \brief
            Retrieves parent Affector of this Key Frame
        */
        public Affector GetParent()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Retrieves index with which this keyframe is retrievable in parent Affector

        \note
            The index is only valid as long as the list of affectors is unchanged in animation!
        */
        public int GetIdxInParent()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Moves this keyframe to a new given position
        */
        public void MoveToPosition(float newPosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves position of this key frame in the animation (in seconds)
        /// </summary>
        /// <returns></returns>
        public float GetPosition()
        {
            return d_position;
        }

        /*!
        \brief
            Sets the value of this key frame

        \par
            This is only used if source property is empty!

        \see
            KeyFrame::setSourceProperty
        */
        public void SetValue(string value)
        {
            d_value = value;
        }

        /*!
        \brief
            Retrieves value of this key frame
        */
        public string GetValue()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Sets the source property of this key frame

        \par
            Key frame can get it's value from 2 places, it's either stored inside
            it (setValue, getValue methods) or it's linked to a property
            (setSourcePropery, getSourceProperty).

            The decision about what value is used is simple, if there is a source
            property (sourceProperty is not empty, it's used)
        */
        public void SetSourceProperty(string sourceProperty)
        {
            d_sourceProperty = sourceProperty;
        }

        /*!
        \brief
            Gets the source property of this key frame
        */
        public string GetSourceProperty()
        {
            throw new NotImplementedException();
        }

        /*!
        \brief
            Retrieves value of this for use when animating

        \par
            This is an internal method! Only use if you know what you're doing!

        \par
            This returns the base property value if source property is set on this
            keyframe, it works the same as getValue() if source property is empty
        */

        public string GetValueForAnimation(AnimationInstance instance)
        {
            if (!String.IsNullOrEmpty(d_sourceProperty))
            {
                return instance.GetSavedPropertyValue(d_sourceProperty);
            }

            return d_value;
        }

        /*!
        \brief
            Sets the progression method of this key frame

        \par
            This controls how the animation will progress TOWARDS this key frame,
            whether it will be a linear motion, accelerated, decelerated, etc...

            That means that the progression of the first key frame is never used!

            Please see KeyFrame::Progression
        */
        public void SetProgression(Progression p)
        {
            d_progression = p;
        }

        /*!
        \brief
            Retrieves progression method of this key frame
        */
        public Progression GetProgression()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Internal method, alters interpolation position based on progression method. 
        /// Don't use unless you know what you're doing!
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public float AlterInterpolationPosition(float position)
        {
            switch (d_progression)
            {
                case Progression.Linear:
                    return position;

                case Progression.QuadraticAccelerating:
                    return position * position;

                case Progression.QuadraticDecelerating:
                    return (float)Math.Sqrt(position);

                case Progression.Discrete:
                    return position < 1.0f ? 0.0f : 1.0f;
            }

            // todo: more progression methods?
            global::System.Diagnostics.Debug.Assert(false);

            return position;
        }

        /// <summary>
        /// Internal method, if this keyframe is using source property, this
        /// saves it's value to given instance before it's affected
        /// </summary>
        /// <param name="instance"></param>
        public void SavePropertyValue(AnimationInstance instance)
        {
            if (!String.IsNullOrEmpty(d_sourceProperty))
                instance.SavePropertyValue(d_sourceProperty);
        }

        /*!
        \brief
            internal method, notifies this keyframe that it has been moved

        \par
            DO NOT CALL DIRECTLY, should only be used by Affector class

        \see
            KeyFrame::moveToPosition
        */
        internal void NotifyPositionChanged(float newPosition)
        {
            throw new NotImplementedException();
        }

        /*!
	    \brief
		    Writes an xml representation of this KeyFrame to \a out_stream.

	    \param xml_stream
		    Stream where xml data should be output.
	    */
	    public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        //! parent affector
        private Affector d_parent;
        //! position of this key frame in the animation's timeline (in seconds)
        private float d_position;

        //! value of this key frame - key value
        private string d_value;
        //! source property
        private string d_sourceProperty;
        //! progression method used towards this key frame
        private Progression d_progression;
    }
}