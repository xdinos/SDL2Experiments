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

using System.Text;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class used to parse stand alone Animation XML files.
    /// </summary>
    internal class AnimationDefinitionHandler : ChainedXmlHandler
    {
        /// <summary>
        /// String holding the element handled by this class.
        /// </summary>
        public const string ElementName = "AnimationDefinition";

        public const string NameAttribute = "name";
        public const string DurationAttribute = "duration";

        public const string ReplayModeAttribute = "replayMode";
        public const string ReplayModeOnce = "once";
        public const string ReplayModeLoop = "loop";
        public const string ReplayModeBounce = "bounce";

        public const string AutoStartAttribute = "autoStart";

        public AnimationDefinitionHandler(XMLAttributes attributes, string namePrefix)
        {
            _anim = null;

            var animName = namePrefix + attributes.GetValueAsString(NameAttribute);

            Logger.LogInsane(
                "Defining animation named: " +
                animName +
                "  Duration: " +
                attributes.GetValueAsString(DurationAttribute) +
                "  Replay mode: " +
                attributes.GetValueAsString(ReplayModeAttribute) +
                "  Auto start: " +
                attributes.GetValueAsString(AutoStartAttribute, "false"));

            _anim = AnimationManager.GetSingleton().CreateAnimation(animName);

            _anim.SetDuration(attributes.GetValueAsFloat(DurationAttribute));

            var replayMode = attributes.GetValueAsString(ReplayModeAttribute, ReplayModeLoop);
            if (replayMode == ReplayModeOnce)
                _anim.SetReplayMode(Animation.ReplayMode.Once);
            else if (replayMode == ReplayModeBounce)
                _anim.SetReplayMode(Animation.ReplayMode.Bounce);
            else
                _anim.SetReplayMode(Animation.ReplayMode.Loop);

            _anim.SetAutoStart(attributes.GetValueAsBool(AutoStartAttribute));
        }

        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            if (element == AnimationAffectorHandler.ElementName)
                ChainedHandler = new AnimationAffectorHandler(attributes, _anim);
            else if (element == AnimationSubscriptionHandler.ElementName)
                ChainedHandler = new AnimationSubscriptionHandler(attributes, _anim);
            else
                System.GetSingleton().Logger
                      .LogEvent(
                          "AnimationDefinitionHandler.ElementStart: <" + element + "> is invalid at this location.",
                          LoggingLevel.Errors);
        }

        protected override void ElementEndLocal(string element)
        {
            // set completed status when we encounter our own end element
            if (element == ElementName)
                d_completed = true;
        }

        /// <summary>
        /// Pointer to the Animation created by this handler.
        /// </summary>
        private readonly Animation _anim;
    }

    /// <summary>
    /// Chained sub-handler for Affector XML elements
    /// </summary>
    internal class AnimationAffectorHandler : ChainedXmlHandler
    {
        /// <summary>
        /// String holding the element handled by this class.
        /// </summary>
        public const string ElementName = "Affector";

        public const string TargetPropertyAttribute = "property";
        public const string InterpolatorAttribute = "interpolator";

        public const string ApplicationMethodAttribute = "applicationMethod";
        public const string ApplicationMethodAbsolute = "absolute";
        public const string ApplicationMethodRelative = "relative";
        public const string ApplicationMethodRelativeMultiply = "relative multiply";

        public AnimationAffectorHandler(XMLAttributes attributes, Animation anim)
        {
            _affector = null;

            Logger.LogInsane("\tAdding affector for property: " + attributes.GetValueAsString(TargetPropertyAttribute) +
                             "  Interpolator: " + attributes.GetValueAsString(InterpolatorAttribute) +
                             "  Application method: " +
                             attributes.GetValueAsString(ApplicationMethodAttribute, "absolute"));

            _affector = anim.CreateAffector(attributes.GetValueAsString(TargetPropertyAttribute),
                                             attributes.GetValueAsString(InterpolatorAttribute));

            if (attributes.GetValueAsString(ApplicationMethodAttribute) == ApplicationMethodRelative)
            {
                _affector.SetApplicationMethod(Affector.ApplicationMethod.Relative);
            }
            else if (attributes.GetValueAsString(ApplicationMethodAttribute) == ApplicationMethodRelativeMultiply)
            {
                _affector.SetApplicationMethod(Affector.ApplicationMethod.RelativeMultiply);
            }
            else
            {
                _affector.SetApplicationMethod(Affector.ApplicationMethod.Absolute);
            }
        }

        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            if (element == AnimationKeyFrameHandler.ElementName)
                ChainedHandler = new AnimationKeyFrameHandler(attributes, _affector);
            else
                System.GetSingleton().Logger
                      .LogEvent(
                          "AnimationAffectorHandler.ElementStartLocal: " + "<" + element +
                          "> is invalid at this location.", LoggingLevel.Errors);

        }

        protected override void ElementEndLocal(string element)
        {
            // set completed status when we encounter our own end element
            if (element == ElementName)
                d_completed = true;
        }

        /// <summary>
        /// Affector created by this handler.
        /// </summary>
        private readonly Affector _affector;
    }

    /// <summary>
    /// Chained sub-handler for KeyFrame XML elements.
    /// </summary>
    internal class AnimationKeyFrameHandler : ChainedXmlHandler
    {
        /// <summary>
        /// String holding the element handled by this class.
        /// </summary>
        public const string ElementName = "KeyFrame";

        public const string PositionAttribute = "position";
        public const string ValueAttribute = "value";
        public const string SourcePropertyAttribute = "sourceProperty";

        public const string ProgressionAttribute = "progression";
        public const string ProgressionLinear = "linear";
        public const string ProgressionDiscrete = "discrete";
        public const string ProgressionQuadraticAccelerating = "quadratic accelerating";
        public const string ProgressionQuadraticDecelerating = "quadratic decelerating";

        public AnimationKeyFrameHandler(XMLAttributes attributes, Affector affector)
        {
            //throw new NotImplementedException();
            var progressionStr = attributes.GetValueAsString(ProgressionAttribute);

            var logEvent = new StringBuilder("\t\tAdding KeyFrame at position: " +
                                             attributes.GetValueAsString(PositionAttribute) +
                                             "  Value: " +
                                             attributes.GetValueAsString(ValueAttribute));

            if (!string.IsNullOrEmpty(progressionStr))
            {
                logEvent.Append("  Progression: " + attributes.GetValueAsString(ProgressionAttribute, ProgressionLinear));
            }

            Logger.LogInsane(logEvent.ToString());

            KeyFrame.Progression progression;
            if (progressionStr == ProgressionDiscrete)
                progression = KeyFrame.Progression.Discrete;
            else if (progressionStr == ProgressionQuadraticAccelerating)
                progression = KeyFrame.Progression.QuadraticAccelerating;
            else if (progressionStr == ProgressionQuadraticDecelerating)
                progression = KeyFrame.Progression.QuadraticDecelerating;
            else
                progression = KeyFrame.Progression.Linear;

            affector.CreateKeyFrame(
                attributes.GetValueAsFloat(PositionAttribute),
                attributes.GetValueAsString(ValueAttribute),
                progression,
                attributes.GetValueAsString(SourcePropertyAttribute));

            if (affector.GetNumKeyFrames() == 1 && !string.IsNullOrEmpty(progressionStr))
            {
                System.GetSingleton().Logger
                      .LogEvent("WARNING: progression type specified for first keyframe in animation will be ignored.");
            }

            d_completed = true;
        }

        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            System.GetSingleton().Logger
                  .LogEvent(
                      "AnimationAffectorHandler::elementStart: " +
                      "<" + element + "> is invalid at this location.", LoggingLevel.Errors);
        }

        protected override void ElementEndLocal(string element)
        {
            // set completed status when we encounter our own end element
            if (element == ElementName)
                d_completed = true;
        }
    }

    /// <summary>
    /// Chained sub-handler for Subscription XML elements.
    /// </summary>
    internal class AnimationSubscriptionHandler : ChainedXmlHandler
    {
        #region Constants

        /// <summary>
        /// String holding the element handled by this class.
        /// </summary>
        public const string ElementName = "Subscription";

        public const string EventAttribute = "event";
        public const string ActionAttribute = "action";

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="anim"></param>
        public AnimationSubscriptionHandler(XMLAttributes attributes, Animation anim)
        {
            Logger.LogInsane("\tAdding subscription to event: " +
                             attributes.GetValueAsString(EventAttribute) +
                             "  Action: " +
                             attributes.GetValueAsString(ActionAttribute));
            
            anim.DefineAutoSubscription(attributes.GetValueAsString(EventAttribute),
                                        attributes.GetValueAsString(ActionAttribute));

            d_completed = true;
        }

        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            System.GetSingleton().Logger
                  .LogEvent(
                      "AnimationAffectorHandler::elementStart: " + "</" + element + "> is invalid at this location.",
                      LoggingLevel.Errors);
        }

        protected override void ElementEndLocal(string element)
        {
            // set completed status when we encounter our own end element
            if (element == ElementName)
                d_completed = true;
        }
    }
}