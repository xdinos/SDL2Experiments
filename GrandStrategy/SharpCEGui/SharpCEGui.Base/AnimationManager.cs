using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCEGui.Base.Collections;

namespace SharpCEGui.Base
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AnimationManager : IDisposable
    {
        #region Implementation of Singleton
        private static readonly Lazy<AnimationManager> Instance = new Lazy<AnimationManager>(()=>new AnimationManager());
        public static AnimationManager GetSingleton()
        {
            return Instance.Value;
        }
        #endregion

        /// <summary>
        /// Name of the schema used for loading animation xml files.
        /// </summary>
        public const string XMLSchemaName = "Animation.xsd";

        /// <summary>
        /// Constructs a new AnimationManager object.
        /// 
        /// NB: Client code should not create AnimationManager objects - they are of
        /// limited use to you!  The intended pattern of access is to get a pointer
        /// to the GUI system's AnimationManager via the System object, and use
        /// that.
        /// </summary>
        private AnimationManager()
        {
            System.GetSingleton().Logger
                .LogEvent(String.Format("CEGUI::AnimationManager singleton created (0x{0:X8})", GetHashCode()));

            // create and add basic interpolators shipped with CEGUI
            AddBasicInterpolator(new TplDiscreteRelativeInterpolator<string>("String", (v1, v2) => v1 + v2));
            AddBasicInterpolator(new TplLinearInterpolator<float>("float", (v1, v2) => v1 + v2, (v, f) => v*f));
            AddBasicInterpolator(new TplLinearInterpolator<int>("int", (v1, v2) => v1 + v2, (v, f) => (int)(v*f)));
            AddBasicInterpolator(new TplLinearInterpolator<uint>("uint", (v1, v2) => v1 + v2, (v, f) => (uint)(v*f)));
            AddBasicInterpolator(new TplDiscreteInterpolator<bool>("bool"));
            AddBasicInterpolator(new TplLinearInterpolator<Sizef >("Sizef", (v1, v2) => v1 + v2, (v, f) => v*f));
            AddBasicInterpolator(new TplLinearInterpolator<Lunatics.Mathematics.Vector2>("Vector2", (v1, v2) => v1 + v2, (v, f) => v * f));
            AddBasicInterpolator(new TplLinearInterpolator<Lunatics.Mathematics.Vector3>("Vector3", (v1, v2) => v1 + v2, (v, f) => v * f));
            AddBasicInterpolator(new QuaternionSlerpInterpolator());
            AddBasicInterpolator(new TplLinearInterpolator<Rectf >("Rectf", (v1, v2) => v1 + v2, (v, f) => v*f));
            // TODO: AddBasicInterpolator(new TplLinearInterpolator<Colour>("Colour", (v1, v2) => v1 + v2, (v, f) => v*f)));
            AddBasicInterpolator(new TplLinearInterpolator<ColourRect>("ColourRect", (v1, v2) => v1 + v2, (v, f) => v*f));
            AddBasicInterpolator(new TplLinearInterpolator<UDim>("UDim", (v1, v2) => v1 + v2, (v, f) => v*f));
            AddBasicInterpolator(new TplLinearInterpolator<UVector2>("UVector2", (v1, v2) => v1 + v2, (v, f) => v*f));
            AddBasicInterpolator(new TplLinearInterpolator<URect>("URect", (v1, v2) => v1 + v2, (v, f) => v*f));
            AddBasicInterpolator(new TplLinearInterpolator<UBox>("UBox", (v1, v2) => v1 + v2, (v, f) => v*f));
            AddBasicInterpolator(new TplLinearInterpolator<USize>("USize", (v1, v2) => v1 + v2, (v, f) => v*f));
        }

        private void AddBasicInterpolator(Interpolator inteporlator)
        {
            AddInterpolator(inteporlator);
            d_basicInterpolators.Add(inteporlator);
        }


        /*!
        \brief
            Destructor for AnimationManager objects

            This will properly destroy all remaining AnimationInstance and Animation
            objects.
        */
        //TODO: ~AnimationManager(void);

        public void Dispose()
        {
            // by destroying all animations their instances also get deleted
            DestroyAllAnimations();

            // and lastly, we remove all interpolators, but we don't delete them!
            // it is the creator's responsibility to delete them
            d_interpolators.Clear();

            // TODO: we only destroy inbuilt interpolators
            //foreach (var it in d_basicInterpolators)
            //    CEGUI_DELETE_AO *it;

            d_basicInterpolators.Clear();

            System.GetSingleton().Logger
                .LogEvent("CEGUI::AnimationManager singleton destroyed " + GetHashCode().ToString("X8"));
        }

        /// <summary>
        /// Adds interpolator to be available for Affectors
        /// <para>
        /// CEGUI ships with several basic interpolators that are always available,
        /// float, bool, colour, UDim, UVector2, ... but you can add your own
        /// custom interpolator if needed! just note that AnimationManager only
        /// deletes inbuilt interpolators. It will remove your interpolator if you
        /// don't do it yourself, but you definitely have to delete it yourself!
        /// </para>
        /// </summary>
        /// <param name="interpolator"></param>
        public void AddInterpolator(Interpolator interpolator)
        {
            if (d_interpolators.ContainsKey(interpolator.GetInterpolatorType()))
            {
                throw new AlreadyExistsException("Interpolator of type '"
                    + interpolator.GetInterpolatorType() + "' already exists.");
            }

            d_interpolators.Add(interpolator.GetInterpolatorType(), interpolator);
        }

        /// <summary>
        /// Removes interpolator
        /// </summary>
        /// <param name="interpolator"></param>
        public void RemoveInterpolator(Interpolator interpolator)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves interpolator by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Interpolator GetInterpolator(string type)
        {
            Interpolator i;
            if (!d_interpolators.TryGetValue(type, out i))
                throw new UnknownObjectException("Interpolator of type '" + type + "' not found.");

            return i;
        }

        /// <summary>
        /// Creates a new Animation definition
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <seealso cref="Animation"/>
        public Animation CreateAnimation(string name = "")
        {
            if (IsAnimationPresent(name))
            {
                throw new UnknownObjectException("Animation with name '" + name + "' already exists.");
            }

            var finalName = String.IsNullOrEmpty(name) ? GenerateUniqueAnimationName() : name;

            var ret = new Animation(finalName);
            d_animations.Add(finalName, ret);

            return ret;
        }

        /// <summary>
        /// Destroys given animation definition
        /// </summary>
        /// <param name="animation"></param>
        public void DestroyAnimation(Animation animation)
        {
            DestroyAnimation(animation.GetName());
        }

        /// <summary>
        /// Destroys given animation definition by name
        /// </summary>
        /// <param name="name"></param>
        public void DestroyAnimation(string name)
        {
            throw new NotImplementedException();
        }
    
        /// <summary>
        /// Destroys all animations in existence!
        /// </summary>
        public void DestroyAllAnimations()
        {
            // we have to destroy all instances to avoid dangling pointers
            // destroying all instances now is also faster than doing that for each
            // animation that is being destroyed
            DestroyAllAnimationInstances();
    
            // TODO: ...
            //for (AnimationMap::const_iterator it = d_animations.begin();
            //     it != d_animations.end(); ++it)
            //{
            //    CEGUI_DELETE_AO it->second;
            //}

            d_animations.Clear();
        }

        /// <summary>
        /// Retrieves animation by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Animation GetAnimation(string name)
        {
            if (d_animations.ContainsKey(name))
                return d_animations[name];

            throw new UnknownObjectException("Animation with name '" + name + "' not found.");
        }

        /// <summary>
        /// Examines the list of Animations to see if one exists with the given name
        /// </summary>
        /// <param name="name">
        /// String holding the name of the Animation to look for.
        /// </param>
        /// <returns>
        /// true if an Animation was found with a name matching \a name.
        /// false if no matching Animation was found.
        /// </returns>
        public bool IsAnimationPresent(string name)
        {
            return d_animations.ContainsKey(name);
        }

        /// <summary>
        /// Retrieves animation by index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Animation GetAnimationAtIdx(int index)
        {
            if (index >= d_animations.Count)
                throw new InvalidRequestException("Out of bounds.");

            return d_animations.Values.Skip(index).First();
        }

        /// <summary>
        /// Retrieves number of defined animations
        /// </summary>
        /// <returns></returns>
        public int GetNumAnimations()
        {
            return d_animations.Count;
        }

        /// <summary>
        /// Instantiates given animation
        /// </summary>
        /// <param name="animation"></param>
        /// <returns></returns>
        /// <seealso cref="AnimationInstance"/>
        public AnimationInstance InstantiateAnimation(Animation animation)
        {
            if (animation == null)
                throw new InvalidRequestException(
                    "I refuse to instantiate NULL animation, please provide a valid pointer.");

            var ret = new AnimationInstance(animation);
            d_animationInstances.Add(animation, ret);

            return ret;
        }

        /// <summary>
        /// Instantiates given animation by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <seealso cref="AnimationInstance"/>
        public AnimationInstance InstantiateAnimation(string name)
        {
            return InstantiateAnimation(GetAnimation(name));
        }

        /// <summary>
        /// Destroys given animation instance
        /// </summary>
        /// <param name="instance"></param>
        public void DestroyAnimationInstance(AnimationInstance instance)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Destroys all instances of given animation
        /// </summary>
        /// <param name="animation"></param>
        public void DestroyAllInstancesOfAnimation(Animation animation)
        {
            //AnimationInstanceMap::iterator it = d_animationInstances.find(animation);

            //// the first instance of given animation is now it->second (if there is any)
            //while (it != d_animationInstances.end() && it->first == animation)
            //{
            //    AnimationInstanceMap::iterator toErase = it;
            //    ++it;

            //    CEGUI_DELETE_AO toErase->second;
            //    d_animationInstances.erase(toErase);
            //}

            if (d_animationInstances.ContainsKey(animation))
            {
                var instances = d_animationInstances[animation];
                // TODO: call dispose ??? 
                instances.Clear();
            }
        }
    
        /// <summary>
        /// Destroys all instances of all animations
        /// </summary>
        public void DestroyAllAnimationInstances()
        {
            // TODO: ...
            //for (AnimationInstanceMap::const_iterator it = d_animationInstances.begin(); 
            //     it != d_animationInstances.end(); ++it)
            //{
            //    CEGUI_DELETE_AO it->second;
            //}

            d_animationInstances.Clear();
        }

        /// <summary>
        /// Retrieves animation instance at given index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AnimationInstance GetAnimationInstanceAtIdx(int index)
        {
            if (index >= d_animationInstances.Count)
                throw new InvalidRequestException("Out of bounds.");

            return d_animationInstances.SelectMany(x => x.Value).Skip(index).First();
        }

        /// <summary>
        /// Retrieves number of animation instances, number of times any animation was instantiated.
        /// </summary>
        /// <returns></returns>
        public int GetNumAnimationInstances()
        {
            return d_animationInstances.Count;
        }

        /// <summary>
        /// Internal method, gets called by CEGUI::System automatically.
        /// 
        /// Only use if you know what you're doing!
        /// <para>
        /// Steps animation instances with auto stepping enabled forward
        /// by given delta.</para>
        /// </summary>
        /// <param name="delta"></param>
        public void AutoStepInstances(float delta)
        {
            foreach (var it in d_animationInstances.Values.SelectMany(x=>x))
            {
                if (it.IsAutoSteppingEnabled())
                    it.Step(delta);
            }
        }

        /// <summary>
        /// Parses an XML file containing animation specifications to create
        /// and initialise Animation objects.
        /// </summary>
        /// <param name="filename">
        /// String object holding the filename of the XML file to be processed.
        /// </param>
        /// <param name="resourceGroup">
        /// Resource group identifier to be passed to the resource provider when
        /// loading the XML file.
        /// </param>
        public void LoadAnimationsFromXml(string filename, string resourceGroup = "")
        {
            if (String.IsNullOrEmpty(filename))
                throw new InvalidRequestException("filename supplied for file loading must be valid.");

            var handler=new AnimationXmlHandler();

            // do parse (which uses handler to create actual data)
            try
            {
                System.GetSingleton().GetXMLParser()
                      .ParseXmlFile(handler, filename, XMLSchemaName,
                                    String.IsNullOrEmpty(resourceGroup)
                                        ? s_defaultResourceGroup
                                        : resourceGroup);
            }
            catch
            {
                System.GetSingleton().Logger
                      .LogEvent(
                          "AnimationManager::loadAnimationsFromXML: loading of animations from file '" + filename +
                          "' has failed.",
                          LoggingLevel.Errors);

                throw;
            }
        }

        /// <summary>
        /// Parses XML source containing animation specifications to create and initialise Animation objects. 
        /// </summary>
        /// <param name="source">
        /// String object holding the XML source to be processed.
        /// </param>
        public void LoadAnimationsFromString(string source)
        {
            // do parse (which uses handler to create actual data)
            try
            {
                System.GetSingleton()
                      .GetXMLParser()
                      .ParseXmlString(new AnimationXmlHandler(), source, XMLSchemaName);
            }
            catch
            {
                System.GetSingleton()
                      .Logger
                      .LogEvent("AnimationManager.LoadAnimationsFromString - loading of animations from string failed.",
                                LoggingLevel.Errors);
                throw;
            }
        }

        /// <summary>
        /// Writes given animation definition to the given OutStream.
        /// </summary>
        /// <param name="animation">
        /// Animation definition to write
        /// </param>
        /// <param name="outStream">
        /// OutStream (std::ostream based) object where data is to be sent.
        /// </param>
        public void WriteAnimationDefinitionToStream(Animation animation, StreamWriter outStream)
        {
            var xml = new XMLSerializer(outStream);
            animation.WriteXmlToStream(xml);
        }

        /// <summary>
        /// Writes given animation definition and returns the result as String
        /// </summary>
        /// <param name="animation">
        /// Animation definition to write
        /// </param>
        /// <returns>
        /// String containing the resulting XML
        /// </returns>
        /// <remarks>
        /// This is a convenience function and isn't designed to be fast at all! Use the other alternatives
        /// if you want performance.
        /// </remarks>
        public String GetAnimationDefinitionAsString(Animation animation)
        {
            throw new NotImplementedException();
            //std::ostringstream str;
            //WriteAnimationDefinitionToStream(animation, str);

            //return String(reinterpret_cast<const encoded_char*>(str.str().c_str()));
        }

        /// <summary>
        /// Sets the default resource group to be used when loading animation xml data
        /// </summary>
        /// <param name="resourceGroup">
        /// String describing the default resource group identifier to be used.
        /// </param>
        public static void SetDefaultResourceGroup(string resourceGroup)
        {
            s_defaultResourceGroup = resourceGroup;
        }

        /// <summary>
        /// Returns the default resource group currently set for loading animation xml data.
        /// </summary>
        /// <returns>
        /// String describing the default resource group identifier that will be
        /// used when loading Animation xml data.
        /// </returns>
        public static string GetDefaultResourceGroup()
        {
            return s_defaultResourceGroup;
        }

        private string GenerateUniqueAnimationName()
        {
            var ret = GeneratedAnimationNameBase + PropertyHelper.ToString(d_uid_counter);

            // update counter for next time
            var oldUid = d_uid_counter;
            ++d_uid_counter;

            // log if we ever wrap-around (which should be pretty unlikely)
            if (d_uid_counter < oldUid)
                System.GetSingleton()
                      .Logger.LogEvent(
                          "UID counter for generated Animation names has wrapped around - the fun shall now commence!");

            return ret;
        }

        #region Fields
        
        /// <summary>
        /// Counter used to generate unique animation names.
        /// </summary>
        private ulong d_uid_counter;

        /// <summary>
        /// stores available interpolators
        /// </summary>
        private readonly Dictionary<string, Interpolator> d_interpolators = new Dictionary<string, Interpolator>();

        /// <summary>
        /// stores interpolators that are inbuilt in CEGUI
        /// </summary>
        private readonly List<Interpolator> d_basicInterpolators = new List<Interpolator>();

        /// <summary>
        /// all defined animations
        /// </summary>
        private readonly Dictionary<string, Animation> d_animations = new Dictionary<string, Animation>();

        /// <summary>
        /// all instances of animations
        /// </summary>
        private readonly MultiValueDictionary<Animation, AnimationInstance> d_animationInstances = new MultiValueDictionary<Animation, AnimationInstance>();

        /// <summary>
        /// Default resource group used when loading animation xml files.
        /// </summary>
        private static String s_defaultResourceGroup;

        /// <summary>
        /// Base name to use for generated window names.
        /// </summary>
        private const string GeneratedAnimationNameBase = "__ceanim_uid_";
        

        #endregion
    }
}