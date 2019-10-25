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

namespace SharpCEGui.Base
{
    /// <summary>
    /// Defines an 'animation instance' class
    /// 
    /// Animation classes hold definition of the animation. Whilst this class holds
    /// data needed to use the animation definition - target PropertySet, event
    /// receiver, animation position, ...
    /// 
    /// You have to define animation first and then instantiate it via
    /// AnimationManager::instantiateAnimation
    /// </summary>
    /// <seealso cref="Animation"/>
    public class AnimationInstance
    {
        // these are fired on event receiver, not this animation instance!
        
        /// <summary>
        /// fired when animation instance starts
        /// </summary>
        public event EventHandler<EventArgs> AnimationStarted;
        
        /// <summary>
        /// fired when animation instance stops
        /// </summary>
        public event EventHandler<EventArgs> AnimationStopped;
    
        /// <summary>
        /// fired when animation instance pauses
        /// </summary>
        public event EventHandler<EventArgs> AnimationPaused;

        /// <summary>
        /// fired when animation instance unpauses
        /// </summary>
        public event EventHandler<EventArgs> AnimationUnpaused;

        /// <summary>
        /// fired when animation instance ends
        /// </summary>
        public event EventHandler<EventArgs> AnimationEnded;

        /// <summary>
        /// fired when animation instance loops
        /// </summary>
        public event EventHandler<EventArgs> AnimationLooped;

        /// <summary>
        /// internal constructor, please use AnimationManager::instantiateAnimation
        /// </summary>
        /// <param name="definition"></param>
        internal AnimationInstance(Animation definition)
        {
            _definition = definition;

            _target = null;
            _eventReceiver = null;
            _eventSender = null;

            _position = 0f;
            _speed = 1f;
            _bounceBackwards = false;
            _running = false;
            _skipNextStep = false;

            // default behaviour is to never skip
            _maxStepDeltaSkip = -1.0f;
            
            // default behaviour is to never clamp
            _maxStepDeltaClamp = -1.0f;
            _autoSteppingEnabled = true;
        }

        /** internal destructor, please use
         * AnimationManager::destroyAnimationInstance
         */
        // TODO: ~AnimationInstance()
        //{
        //    if (d_eventSender)
        //    {
        //        d_definition->autoUnsubscribe(this);
        //    }
        //}

        /// <summary>
        /// Retrieves the animation definition that is used in this instance
        /// </summary>
        /// <returns></returns>
        public Animation GetDefinition()
        {
            return _definition;
        }

        /// <summary>
        /// Sets the target property set - this class will get it's properties
        /// affected by the Affectors!
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(PropertySet target)
        {
            _target = target;

            PurgeSavedPropertyValues();

            if (_definition.GetAutoStart() && !IsRunning())
            {
                Start();
            }
        }

        /// <summary>
        /// Retrieves the target property set
        /// </summary>
        /// <returns></returns>
        public PropertySet GetTarget()
        {
            return _target;
        }

        /// <summary>
        /// Sets event receiver - this class will receive events when something
        /// happens to the playback of this animation - it starts, stops, pauses,
        /// unpauses, ends and loops
        /// </summary>
        /// <param name="receiver"></param>
        public void SetEventReceiver(object /*EventSet*/ receiver)
        {
            _eventReceiver = receiver;
        }

        /// <summary>
        /// Retrieves the event receiver
        /// </summary>
        /// <returns></returns>
        public object /*EventSet*/ GetEventReceiver()
        {
            return _eventReceiver;
        }

        /// <summary>
        /// Sets event sender - this class will send events and can affect this
        /// animation instance if there are any auto subscriptions defined in the
        /// animation definition
        /// </summary>
        /// <param name="sender"></param>
        public void SetEventSender(object /*EventSet*/ sender)
        {
            if (_eventSender!=null)
            {
                _definition.AutoUnsubscribe(this);
            }

            _eventSender = sender;

            if (_eventSender!=null)
            {
                _definition.AutoSubscribe(this);
            }
        }

        /// <summary>
        /// Retrieves the event sender
        /// </summary>
        /// <returns></returns>
        public object /*EventSet*/ GetEventSender()
        {
            return _eventSender;
        }

        /// <summary>
        /// Helper method, sets given window as target property set, event receiver and event set
        /// </summary>
        /// <param name="target"></param>
        public void SetTargetWindow(Window target)
        {
            SetTarget(target);
            SetEventReceiver(target);
            SetEventSender(target);
        }

        /// <summary>
        /// Sets playback position. Has to be higher or equal to 0.0 and lower or
        /// equal to Animation definition's duration.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(float position)
        {
            if (position < 0.0 || position > _definition.GetDuration())
            {
                throw new InvalidRequestException(
                    "Unable to set position of this animation instance " +
                    "because given position isn't in interval " +
                    "[0.0, duration of animation].");
            }

            _position = position;
        }

        /// <summary>
        /// Retrieves current playback position
        /// </summary>
        /// <returns></returns>
        public float GetPosition()
        {
            return _position;
        }

        /// <summary>
        /// Sets playback speed - you can speed up / slow down individual instances
        /// of the same animation. 1.0 means normal playback.
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(float speed)
        {
            // first sort out the adventurous users
            if (speed < 0.0f)
                throw new InvalidRequestException("You can't set playback speed to a value that's lower than 0.0");

            if (speed == 0.0f)
                throw new InvalidRequestException(
                    "AnimationInstance::setSpeed: You can't set playback speed to zero, please use AnimationInstance::pause instead");

            _speed = speed;
        }

        /// <summary>
        /// Retrieves current playback speed
        /// </summary>
        /// <returns></returns>
        public float GetSpeed()
        {
            return _speed;
        }

        /// <summary>
        /// Controls whether the next time step is skipped
        /// </summary>
        /// <param name="skip"></param>
        public void SetSkipNextStep(bool skip)
        {
            _skipNextStep = skip;
        }

        /// <summary>
        /// Returns true if the next step is *going* to be skipped
        /// <para>
        /// If it was skipped already, this returns false as step resets
        /// it to false after it skips one step.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public bool GetSkipNextStep()
        {
            return _skipNextStep;
        }

        /// <summary>
        /// Sets the max delta before step skipping occurs
        /// </summary>
        /// <param name="maxDelta">
        /// maxDelta delta in seconds, if this value is reached, the step is skipped
        /// (use -1.0f if you never want to skip - this is the default)
        /// </param>
        /// <remarks>
        /// If you want to ensure your animation is not skipped entirely after layouts
        /// are loaded or other time consuming operations are done, use this method.
        /// <para>
        /// For example setMaxStepDeltaSkip(1.0f / 25.0f) ensures that if FPS drops
        /// below 25, the animation just stops progressing and waits till FPS raises.
        /// </para>
        /// </remarks>
        public void SetMaxStepDeltaSkip(float maxDelta)
        {
            _maxStepDeltaSkip = maxDelta;
        }

        /// <summary>
        /// Gets the max delta before step skipping occurs
        /// </summary>
        /// <returns></returns>
        public float GetMaxStepDeltaSkip()
        {
            return _maxStepDeltaSkip;
        }

        /// <summary>
        /// Sets the max delta before step clamping occurs
        /// </summary>
        /// <param name="maxDelta">
        /// maxDelta delta in seconds, if this value is reached, the step is clamped.
        /// (use -1.0f if you never want to clamp - this is the default)
        /// </param>
        /// <remarks>
        /// If you want to ensure the animation steps at most 1.0 / 60.0 seconds at a time
        /// you should call setMaxStepDeltaClamp(1.0f / 60.0f). This essentially slows
        /// the animation down in case the FPS drops below 60.
        /// </remarks>
        public void SetMaxStepDeltaClamp(float maxDelta)
        {
            _maxStepDeltaClamp = maxDelta;
        }

        /// <summary>
        /// Gets the max delta before step clamping occurs
        /// </summary>
        /// <returns></returns>
        public float GetMaxStepDeltaClamp()
        {
            return _maxStepDeltaClamp;
        }

        /// <summary>
        /// Starts this animation instance - sets position to 0.0 and unpauses
        /// </summary>
        /// <param name="skipNextStep">
        /// skipNextStep if true the next injected time pulse is skipped
        /// <para>
        /// This also causes base values to be purged!
        /// </para>
        /// </param>
        public void Start(bool skipNextStep = true)
        {
            SetPosition(0.0f);
            _skipNextStep = skipNextStep;

            if (_definition!=null && _definition.GetDuration() > 0)
            {
                _running = true;
                OnAnimationStarted();
            }
            else
            {
                System.GetSingleton().Logger
                      .LogEvent(
                          "AnimationInstance::start - Starting an animation instance with no animation definition or 0 duration has no effect!",
                          LoggingLevel.Warnings);
                OnAnimationStarted();
                OnAnimationEnded();
            }
        }

        /// <summary>
        /// Stops this animation instance - sets position to 0.0 and pauses
        /// </summary>
        public void Stop()
        {
            SetPosition(0.0f);
            _running = false;
            OnAnimationStopped();
        }

        /// <summary>
        /// Pauses this animation instance - stops it from stepping forward
        /// </summary>
        public void Pause()
        {
            _running = false;
            OnAnimationPaused();
        }

        /// <summary>
        /// Unpauses this animation instance - allows it to step forward again
        /// </summary>
        /// <param name="skipNextStep">
        /// skipNextStep if true the next injected time pulse is skipped
        /// </param>
        public void Unpause(bool skipNextStep = true)
        {
            _skipNextStep = skipNextStep;

            if (_definition!=null && _definition.GetDuration() > 0)
            {
                _running = true;
                OnAnimationUnpaused();
            }
            else
            {
                System.GetSingleton().Logger
                      .LogEvent(
                          "AnimationInstance::unpause - Unpausing an animation instance with no animation definition or 0 duration has no effect!",
                          LoggingLevel.Warnings);
                OnAnimationUnpaused();
                OnAnimationEnded();
            }
        }

        /// <summary>
        /// Pauses the animation if it's running and unpauses it if it isn't 
        /// </summary>
        /// <param name="skipNextStep">
        /// if true the next injected time pulse is skipped (only applies when unpausing!)
        /// </param>
        public void TogglePause(bool skipNextStep = true)
        {
            if (IsRunning())
            {
                Pause();
            }
            else
            {
                Unpause(skipNextStep);
            }
        }

        /// <summary>
        /// Returns true if this animation instance is currently unpaused,
        /// if it is stepping forward.
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return _running;
        }

        /// <summary>
        /// Controls whether auto stepping is enabled
        /// <para>
        /// If auto stepping is enabled, CEGUI will step this animation instance forward
        /// whenever CEGUI::System::injectTimePulse is called
        /// </para>
        /// </summary>
        /// <param name="enabled"></param>
        public void SetAutoSteppingEnabled(bool enabled)
        {
            _autoSteppingEnabled = enabled;
        }

        /// <summary>
        /// Checks whether auto stepping is enabled
        /// </summary>
        /// <returns></returns>
        public bool IsAutoSteppingEnabled()
        {
            return _autoSteppingEnabled;
        }

        /// <summary>
        /// Steps the animation forward by the given delta
        /// <para>
        /// You don't need to call this unless AutoStepping is disabled (it is enabled by default)
        /// </para>
        /// </summary>
        /// <param name="delta"></param>
        public void Step(float delta)
        {
            if (!_running)
            {
                // nothing to do if this animation instance isn't running
                return;
            }

            if (delta < 0.0f)
            {
                throw new InvalidRequestException("You can't step the Animation Instance with negative " +
                                                  "delta! You can't reverse the flow of time, stop " +
                                                  "trying!");
            }

            // first we deal with delta size
            if (_maxStepDeltaSkip > 0.0f && delta > _maxStepDeltaSkip)
            {
                // skip the step entirely if delta gets over the threshold
                // note that default value is 0.0f which means this never gets triggered
                delta = 0.0f;
            }

            if (_maxStepDeltaClamp > 0.0f)
            {
                // clamp to threshold, note that default value is -1.0f which means
                // this line does nothing (delta should always be larger or equal than 0.0f
                delta = Math.Min(delta, _maxStepDeltaClamp);
            }

            // if asked to do so, we skip this step, but mark that the next one
            // shouldn't be skipped
            // NB: This gets rid of annoying animation skips when FPS gets too low
            //     after complex layout loading, etc...
            if (_skipNextStep)
            {
                _skipNextStep = false;
                // we skip the step by setting delta to 0, this doesn't step the time
                // but still sets the animation position accordingly
                delta = 0.0f;
            }

            var duration = _definition.GetDuration();

            // we modify the delta according to playback speed
            delta *= _speed;

            // the position could have gotten out of the desired range, we have to
            // alter it depending on replay method of our animation definition

            // first a simple clamp with RM_Once
            if (_definition.GetReplayMode() == Animation.ReplayMode.Once)
            {
                var newPosition = _position + delta;

                newPosition = Math.Max(0.0f, newPosition);

                if (newPosition >= duration)
                {
                    newPosition = duration;

                    Stop();
                    OnAnimationEnded();
                }

                SetPosition(newPosition);
            }
            // a both sided wrap with RM_Loop
            else if (_definition.GetReplayMode() == Animation.ReplayMode.Loop)
            {
                float newPosition = _position + delta;

                while (newPosition > duration)
                {
                    newPosition -= duration;
                    OnAnimationLooped();
                }

                SetPosition(newPosition);
            }
            // bounce back and forth with RM_Bounce
            else if (_definition.GetReplayMode() == Animation.ReplayMode.Bounce)
            {
                if (_bounceBackwards)
                {
                    delta = -delta;
                }

                float newPosition = _position + delta;

                while (newPosition < 0.0f || newPosition > duration)
                {
                    if (newPosition < 0.0f)
                    {
                        _bounceBackwards = false;

                        newPosition = -newPosition;
                        OnAnimationLooped();
                    }

                    if (newPosition > duration)
                    {
                        _bounceBackwards = true;

                        newPosition = 2.0f * duration - newPosition;
                        OnAnimationLooped();
                    }
                }

                SetPosition(newPosition);
            }

            Apply();
        }

        /// <summary>
        /// handler that starts the animation instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool HandleStart(EventArgs e)
        {
            Start();
            return true;
        }

        /// <summary>
        /// handler that stops the animation instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public void /*bool*/ HandleStop(object sender, EventArgs e)
        {
            Stop();
            // TODO: return true;
        }

        /// <summary>
        /// handler that pauses the animation instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public void /*bool*/ HandlePause(object sender, EventArgs e)
        {
            Pause();
            // TODO: return true;
        }

        /// <summary>
        /// handler that unpauses the animation instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void /*bool*/ HandleUnpause(object sender, EventArgs e)
        {
            Unpause();
            // TODO: return true;
        }

        /// <summary>
        /// handler that toggles pause on this animation instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public void /*bool*/ HandleTogglePause(object sender, EventArgs e)
        {
            TogglePause();
            // TODO: return true;
        }

        /// <summary>
        /// Internal method, saves given property (called before it's affected)
        /// </summary>
        /// <param name="propertyName"></param>
        public void SavePropertyValue(string propertyName)
        {
            global::System.Diagnostics.Debug.Assert(_target!=null);

            _savedPropertyValues[propertyName] = _target.GetProperty(propertyName);
        }

        /// <summary>
        /// this purges all saved values forcing this class to gather new ones fresh
        /// from the properties
        /// </summary>
        public void PurgeSavedPropertyValues()
        {
            _savedPropertyValues.Clear();
        }

        /// <summary>
        /// retrieves saved value, if it isn't cached already, it retrieves it fresh
        /// from the properties
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetSavedPropertyValue(string propertyName)
        {
            if (!_savedPropertyValues.ContainsKey(propertyName))
            {
                // even though we explicitly save all used property values when
                // starting the animation, this can happen when user changes
                // animation definition whilst the animation is running
                // (Yes, it's nasty, but people do nasty things)
                SavePropertyValue(propertyName);
                return GetSavedPropertyValue(propertyName);
            }

            return _savedPropertyValues[propertyName];
        }

        /*!
        \brief
            Internal method, adds reference to created auto connection

        \par
            DO NOT USE THIS DIRECTLY
        */
        // TODO: void addAutoConnection(Event::Connection conn);

        /// <summary>
        /// Internal method, unsubscribes auto connections
        /// <para>
        /// DO NOT USE THIS DIRECTLY
        /// </para>
        /// </summary>
        public void UnsubscribeAutoConnections()
        {
            //for (ConnectionTracker::iterator it = d_autoConnections.begin();
            //     it != d_autoConnections.end(); ++it)
            //{
            //    (*it)->disconnect();
            //}

            //d_autoConnections.clear();
        }

        /// <summary>
        /// Applies this animation instance
        /// <para>
        /// You should not need to use this directly unless your requirements are very special.
        /// CEGUI calls this automatically in most cases.
        /// </para>
        /// </summary>
        public void Apply()
        {
            if (_target != null)
                _definition.Apply(this);
        }

        /// <summary>
        /// this is called when animation starts
        /// </summary>
        private void OnAnimationStarted()
        {
            PurgeSavedPropertyValues();
            _definition.SavePropertyValues(this);

            if (_eventReceiver != null)
            {
                // TODO: d_eventReceiver->fireEvent(EventAnimationStarted, args, EventNamespace);
                var args = new AnimationEventArgs(this);
                var handler = AnimationStarted;
                if (handler != null)
                    handler(this, args);
            }
        }

        /// <summary>
        /// this is called when animation stops
        /// </summary>
        private void OnAnimationStopped()
        {
            if (_eventReceiver != null)
            {
                // TODO: d_eventReceiver->fireEvent(EventAnimationStopped, args, EventNamespace);
                ((IEventSet) _eventReceiver).FireEvent("AnimationStopped", new AnimationEventArgs(this));
                //var handler = AnimationStopped;
                //if (handler != null)
                //    handler(this, new AnimationEventArgs(this));
            }
        }

        //! this is called when animation pauses
        private void OnAnimationPaused()
        {
            if (_eventReceiver!=null)
            {
                // TODO: d_eventReceiver->fireEvent(EventAnimationPaused, args, EventNamespace);
                var handler = AnimationPaused;
                if (handler != null)
                    handler(this, new AnimationEventArgs(this));
            }
        }

        /// <summary>
        /// this is called when animation unpauses
        /// </summary>
        private void OnAnimationUnpaused()
        {
            if (_eventReceiver != null)
            {
                // TODO: d_eventReceiver->fireEvent(EventAnimationUnpaused, args, EventNamespace);
                var handler = AnimationUnpaused;
                if (handler != null)
                    handler(this, new AnimationEventArgs(this));
            }
        }

        /// <summary>
        /// this is called when animation ends
        /// </summary>
        private void OnAnimationEnded()
        {
            if (_eventReceiver!=null)
            {
                // TODO: d_eventReceiver->fireEvent(EventAnimationEnded, args, EventNamespace);
              //  ((IEventSet)_eventReceiver).FireEvent("AnimationEnded", new AnimationEventArgs(this));
                var handler = AnimationEnded;
                if (handler != null)
                    handler(this, new AnimationEventArgs(this));
            }
        }

        /// <summary>
        /// this is called when animation loops (in RM_Loop or RM_Bounce mode)
        /// </summary>
        private void OnAnimationLooped()
        {
            if (_eventReceiver != null)
            {
                // TODO: d_eventReceiver->fireEvent(EventAnimationLooped, args, EventNamespace);
                var handler = AnimationLooped;
                if (handler != null)
                    handler(this, new AnimationEventArgs(this));
            }
        }

        #region Fields

        /// <summary>
        /// target property set, properties of this are affected by Affectors
        /// </summary>
        private PropertySet _target;
        
        /// <summary>
        /// event receiver, receives events about this animation instance
        /// </summary>
        private object /*EventSet*/ _eventReceiver;

        /// <summary>
        /// event sender, sends events and can control this animation instance if
        ///  there are any auto subscriptions
        /// </summary>
        private object /*EventSet*/ _eventSender;

        /// <summary>
        /// position of this animation instance,
        /// should always be higher or equal to 0.0 and lower or equal to duration of
        /// animation definition
        /// </summary>
        private float _position;
        
        /// <summary>
        /// playback speed, 1.0 means normal playback
        /// </summary>
        private float _speed;

        /// <summary>
        /// needed for RM_Bounce mode, if true, we bounce backwards
        /// </summary>
        private bool _bounceBackwards;
        
        /// <summary>
        /// true if this animation is unpaused
        /// </summary>
        private bool _running;
        
        /// <summary>
        /// skip next update (true if the next update should be skipped entirely)
        /// </summary>
        private bool _skipNextStep;
        
        /// <summary>
        /// skip the update if the step is larger than this value
        /// </summary>
        private float _maxStepDeltaSkip;
        
        /// <summary>
        /// always clamp step delta to this value
        /// </summary>
        private float _maxStepDeltaClamp;
        
        /// <summary>
        /// true if auto stepping is enabled
        /// </summary>
        private bool _autoSteppingEnabled;

        /// <summary>
        /// parent Animation definition
        /// </summary>
        private readonly Animation _definition;
        
        /// <summary>
        /// cached saved values, used for relative application method
        /// and keyframe property source, see Affector and KeyFrame classes
        /// </summary>
        private readonly Dictionary<string, string> _savedPropertyValues = new Dictionary<string, string>();

        // TODO: ...
        //typedef std::vector<Event::Connection
        //    CEGUI_VECTOR_ALLOC(Event::Connection)> ConnectionTracker;
        ////! tracks auto event connections we make.
        //ConnectionTracker d_autoConnections;

        #endregion
    }
}