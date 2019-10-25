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
    /// <summary>
    /// Defines an 'animation' class
    /// 
    /// This is definition of Animation. Can be reused multiple times via
    /// AnimationInstance class. You can't step this class directly, you have to
    /// instantiate it via AnimationManager::instantiateAnimation.
    /// 
    /// AnimationInstance provides means for stepping the animation and applying
    /// it to PropertySets.
    /// 
    /// \par
    /// Animation itself doesn't contain key frames. It is composed of Affector(s).
    /// Each Affector affects one Property. So one Animation can affect multiple
    /// properties.
    /// </summary>
    /// <seealso cref="AnimationInstance"/>
    /// <seealso cref="Affector"/>
    public class Animation
    {
        /// <summary>
        /// enumerates possible replay modes
        /// </summary>
        public enum ReplayMode
        {
            /// <summary>
            /// plays the animation just once, then stops
            /// </summary>
            Once,

            /// <summary>
            /// loops the animation infinitely
            /// </summary>
            Loop,

            /// <summary>
            /// infinitely plays the animation forward, when it reaches the end, it
            /// plays it backwards, etc...
            /// </summary>
            Bounce
        }

        /// <summary>
        /// internal constructor, please only construct animations via
        /// AnimationManager::createAnimation method
        /// </summary>
        /// <param name="name"></param>
        internal Animation(string name)
        {
            _name = name;
            _replayMode = ReplayMode.Loop; // the right default value that confirms things are working
            _duration = 0.0f;
            _autoStart = false;
        }

        //! destructor, this destroys all affectors defined inside this animation
        // TODO: ~Animation(void);

        /// <summary>
        /// Retrieves name of this Animation definition
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return _name;
        }

        /// <summary>
        /// Sets the replay mode of this animation
        /// </summary>
        /// <param name="mode"></param>
        public void SetReplayMode(ReplayMode mode)
        {
            _replayMode = mode;
        }

        /// <summary>
        /// Retrieves the replay mode of this animation
        /// </summary>
        /// <returns></returns>
        public ReplayMode GetReplayMode()
        {
            return _replayMode;
        }

        /// <summary>
        /// Sets the duration of this animation
        /// </summary>
        /// <param name="duration"></param>
        public void SetDuration(float duration)
        {
            _duration = duration;
            // todo: iterate through existing key frames if any and if we
            //       find a keyframe that is now outside of the [0, duration] interval,
            //       rant about it in the log
        }

        /// <summary>
        /// Retrieves the duration of this animation
        /// </summary>
        /// <returns></returns>
        public float GetDuration()
        {
            return _duration;
        }

        /// <summary>
        /// Sets whether this animation auto starts or not
        /// <para>
        /// Auto start means that the animation instances of this definition call
        /// Start on themselves once their target is set.
        /// </para>
        /// </summary>
        /// <param name="autoStart"></param>
        public void SetAutoStart(bool autoStart)
        {
            _autoStart = autoStart;
        }

        /// <summary>
        /// Retrieves auto start.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Animation.SetAutoStart"/>
        public bool GetAutoStart()
        {
            return _autoStart;
        }

        /// <summary>
        /// Creates a new Affector
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Affector"/>
        public Affector CreateAffector()
        {
            // no checking needed!

            var ret = new Affector(this);
            _affectors.Add(ret);

            return ret;
        }

        /// <summary>
        /// Creates a new Affector
        /// <para>This is just a helper, finger saving method.</para>
        /// </summary>
        /// <param name="targetProperty"></param>
        /// <param name="interpolator"></param>
        /// <returns></returns>
        public Affector CreateAffector(string targetProperty, string interpolator)
        {
            var ret = CreateAffector();
            ret.SetTargetProperty(targetProperty);
            ret.SetInterpolator(interpolator);

            return ret;
        }

        /// <summary>
        /// Destroys given Affector
        /// </summary>
        /// <param name="affector"></param>
        public void DestroyAffector(Affector affector)
        {
            if (!_affectors.Contains(affector))
                throw new InvalidRequestException("Given affector not found!");
            
            _affectors.Remove(affector);
            // TODO: CEGUI_DELETE_AO affector;
        }

        /// <summary>
        /// Retrieves the Affector at given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Affector GetAffectorAtIdx(int index)
        {
            if (index >= _affectors.Count)
                throw new InvalidRequestException("Out of bounds.");

            //AffectorList::const_iterator it = d_affectors.begin();
            //std::advance(it, index);

            //return *it;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves number of Affectors defined in this Animation
        /// </summary>
        /// <returns></returns>
        public int GetNumAffectors()
        {
            return _affectors.Count;
        }

        /// <summary>
        /// This defined a new auto subscription.
        /// </summary>
        /// <param name="eventName">
        /// eventName the name of the event we want to subscribe to, CEGUI::Window::EventClicked for example
        /// </param>
        /// <param name="action">
        /// action is the action that will be invoked on the animation instance if this event is fired
        /// </param>
        /// <remarks>
        /// Auto Subscription does subscribe to event sender (usually target window)
        /// of Animation Instance when the event source is set.
        /// 
        /// Usable action strings:
        /// - Start
        /// - Stop
        /// - Pause
        /// - Unpause
        /// - TogglePause
        /// 
        /// eventName is the name of the event we want to subscribe to
        /// </remarks>
        public void DefineAutoSubscription(string eventName, string action)
        {
            if (_autoSubscriptions.ContainsKey(eventName))
            {
                var actions = _autoSubscriptions[eventName];
                if (actions.Any(x => x == action))
                {
                    throw new InvalidOperationException("Unable to define given Auto Subscription - exactly the same auto subscription is already there!");
                }

                actions.Add(action);
            }
            else
            {
                _autoSubscriptions.Add(eventName, new List<string>(new[] {action}));
            }
        }

        /// <summary>
        /// This undefines previously defined auto subscription.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="action"></param>
        /// <seealso cref="DefineAutoSubscription"/>
        public void UndefineAutoSubscription(string eventName,string action)
        {
            throw new NotImplementedException();
            //SubscriptionMap::iterator it = d_autoSubscriptions.find(eventName);

            //while (it != d_autoSubscriptions.end() && it->first == eventName)
            //{
            //    if (it->second == action)
            //    {
            //        d_autoSubscriptions.erase(it);
            //        return;
            //    }

            //    ++it;
            //}

            //CEGUI_THROW(InvalidRequestException(
            //    "Unable to undefine given Auto Subscription - not found!"));

        }

        /// <summary>
        /// This undefines all previously defined auto subscriptions.
        /// </summary>
        /// <seealso cref="DefineAutoSubscription"/>
        public void UndefineAllAutoSubscriptions()
        {
            _autoSubscriptions.Clear();
        }

        /// <summary>
        /// Subscribes all auto subscriptions with information from given animation instance
        /// <para>
        /// This is internal method! Only use if you know what you're doing!
        /// </para>
        /// </summary>
        /// <param name="instance"></param>
        public void AutoSubscribe(AnimationInstance instance)
        {
            var eventSender = instance.GetEventSender();

            if (eventSender == null)
                return;
            foreach (var it in _autoSubscriptions.SelectMany(x=> x.Value.Select(z=>new Tuple<string,string>(x.Key,z))))
            {
                var e = it.Item1;
                var a = it.Item2;

                BoundSlot connection;

                if (a == "Start")
                {
                    connection = ((IEventSet) eventSender).SubscribeEvent(e, instance.HandleStart);
                    //        connection = eventSender->subscribeEvent(e,
                    //                     CEGUI::Event::Subscriber(&AnimationInstance::handleStart, instance));
                }
            //    else if (a == "Stop")
            //    {
            //        connection = eventSender->subscribeEvent(e,
            //                     CEGUI::Event::Subscriber(&AnimationInstance::handleStop, instance));
            //    }
            //    else if (a == "Pause")
            //    {
            //        connection = eventSender->subscribeEvent(e,
            //                     CEGUI::Event::Subscriber(&AnimationInstance::handlePause, instance));
            //    }
            //    else if (a == "Unpause")
            //    {
            //        connection = eventSender->subscribeEvent(e,
            //                     CEGUI::Event::Subscriber(&AnimationInstance::handleUnpause, instance));
            //    }
            //    else if (a == "TogglePause")
            //    {
            //        connection = eventSender->subscribeEvent(e,
            //                     CEGUI::Event::Subscriber(&AnimationInstance::handleTogglePause, instance));
            //    }
            //    else
            //    {
            //        CEGUI_THROW(InvalidRequestException(
            //                        "Unable to auto subscribe! "
            //                        "'" + a + "' is not a valid action."));
            //    }

                //instance.AddAutoConnection(connection);
            }
        }

        /// <summary>
        /// Unsubscribes all auto subscriptions with information from given animation instance
        /// <para>
        /// This is internal method! Only use if you know what you're doing!
        /// </para>
        /// </summary>
        /// <param name="instance"></param>
        public void AutoUnsubscribe(AnimationInstance instance)
        {
            // just a delegate to make things clean
            instance.UnsubscribeAutoConnections();
        }

        /// <summary>
        /// Internal method, causes all properties that are used by this animation
        /// and it's affectors to be saved
        /// </summary>
        /// <param name="instance">
        /// So their values are still known after they've been affected.
        /// </param>
        public void SavePropertyValues(AnimationInstance instance)
        {
            foreach (var it in _affectors)
                it.SavePropertyValues(instance);
        }

        /// <summary>
        /// Applies this Animation definition using information from given AnimationInstance
        /// <para>This is internal method, only use if you know what you're doing!</para>
        /// </summary>
        /// <param name="instance"></param>
        public void Apply(AnimationInstance instance)
        {
            foreach (var it in _affectors)
                it.Apply(instance);
        }

        /// <summary>
        /// Writes an xml representation of this Animation definition to \a out_stream.
        /// </summary>
        /// <param name="xml_stream">
        /// Stream where xml data should be output.
        /// </param>
        /// <param name="name_override">
        /// If given, this value overrides the name attribute written to the stream.
        /// This is useful when writing out looknfeels
        /// </param>
        public void WriteXmlToStream(XMLSerializer xml_stream, string name_override = "")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// name of this animation
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// currently used replay mode
        /// </summary>
        private ReplayMode _replayMode;

        /// <summary>
        /// duration of animation (in seconds)
        /// </summary>
        private float _duration;
        
        /// <summary>
        /// if true, instantiations of this animation call start on themselves when
        /// their target is set
        /// </summary>
        private bool _autoStart;

        /// <summary>
        /// list of affectors defined in this animation
        /// </summary>
        private readonly List<Affector> _affectors = new List<Affector>();

        //typedef std::multimap<String, String, std::less<String>
        //    CEGUI_MAP_ALLOC(String, String)> SubscriptionMap;
        ///** holds pairs of 2 strings, the left string is the Event that we will
        // * subscribe to, the right string is the action that will be invoked to the
        // * instance if the event is fired on target window
        // */
        //SubscriptionMap d_autoSubscriptions;
        private readonly Dictionary<string, List<string>> _autoSubscriptions = new Dictionary<string, List<string>>();
    }
}