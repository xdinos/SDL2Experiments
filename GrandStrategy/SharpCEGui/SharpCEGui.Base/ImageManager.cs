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
using System.Globalization;
using System.Linq;
using SharpCEGui.Base.Svg;

namespace SharpCEGui.Base
{
    using ImageMap = Dictionary<string, Tuple<Image, ImageFactory>>;
    using ImageFactoryRegistry = Dictionary<string, ImageFactory>;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ImageManager : ChainedXmlHandler, IDisposable
    {
        #region Implementation of Singleton
        private static readonly Lazy<ImageManager> Instance = new Lazy<ImageManager>(()=>new ImageManager());
        public static ImageManager GetSingleton()
        {
            return Instance.Value;
        }
        #endregion

        private ImageManager()
        {
            System.GetSingleton().Logger
                  .LogEvent(String.Format("[SharpCEGui.Base.ImageManager] Singleton created (0x{0:X8})", GetHashCode()));

            // self-register the built in 'BitmapImage' type.
            AddImageType(BitmapImage.TypeName, new TplImageFactory<BitmapImage>(x => new BitmapImage(x), x => new BitmapImage(x)));
            // self-register the built in 'SVGImage' type.
            AddImageType(SvgImage.TypeName, new TplImageFactory<SvgImage>(x => new SvgImage(x), x => new SvgImage(x)));

        }
        
        #region Implementation of IDisposable

        public void Dispose()
        {
            DestroyAll();

            while (d_factories.Count!=0)
                RemoveImageType(d_factories.First().Key);

            System.GetSingleton().Logger
                .LogEvent("[CEGUI::ImageManager] Singleton destroyed " + GetHashCode().ToString("X8"));
        }

        #endregion

        /*!
        \brief
            Register an Image subclass with the system and associate it with the
            identifier \a name.

            This registers a subclass of the Image class, such that instances of
            that subclass can subsequently be created by using the identifier
            \a name.

        \tparam T
            The Image subclass to be instantiated when an Image is created using the
            type identifier \a name.

        \param name
            String object describing the identifier that the Image subclass will be
            registered under.

        \exception AlreadyExistsException
            thrown if an Image subclass is already registered using \a name.
        */

        //public void AddImageType<T>(string name)
        //{
        //    if (IsImageTypeAvailable(name))
        //        throw new AlreadyExistsException(String.Format("Image type already exists: {0}", name));

        //    d_factories[name] = new TplImageFactory<T>();

        //    System.GetSingleton().Logger
        //        .LogEvent(String.Format("[SharpCEGui.Base.ImageManager] Registered Image type: {0}", name));
        //}

        public void AddImageType(string name, ImageFactory factory)
        {
            if (IsImageTypeAvailable(name))
                throw new AlreadyExistsException(String.Format("Image type already exists: {0}", name));

            d_factories[name] = factory;

            System.GetSingleton().Logger
                .LogEvent(String.Format("[SharpCEGui.Base.ImageManager] Registered Image type: {0}", name));
        }

        /// <summary>
        /// Unregister the Image subclass that was registered under the identifier
        /// \a name.
        /// </summary>
        /// <param name="name">
        /// String object describing the identifier of the Image subclass that is to
        /// be unregistered.  If no such identifier is known within the system, no
        /// action is taken.
        /// </param>
        /// <remarks>
        /// You should avoid removing Image subclass types that are still in use.
        /// Internally a factory system is employed for the creation and deletion
        /// of Image based objects; if an Image subclass - and therefore it's
        /// factory - is removed while instances of that class are still active, it
        /// will not be possible to safely delete those instances.
        /// </remarks>
        public void RemoveImageType(string name)
        {
            if (!d_factories.ContainsKey(name))
                return;
            
            System.GetSingleton().Logger
                .LogEvent("[SharpCEGui.Base.ImageManager] Unregistered Image type: " + name);

            //CEGUI_DELETE_AO i->second;
            d_factories.Remove(name);
        }

        /// <summary>
        /// Return whether an Image subclass has been registered using the
        /// identifier \a name.
        /// </summary>
        /// <param name="name">
        /// - true if an Image subclass is registered using the identifier \a name.
        /// - false if no Image subclass is registered using the identifier \a name.
        /// </param>
        /// <returns></returns>
        public bool IsImageTypeAvailable(string name)
        {
            return d_factories.ContainsKey(name);
        }

        /*!
        \brief
            Create an instance of Image subclass registered for identifier \a type
            using the name \a name.

        \param type
            String object describing the identifier of the Image subclass that is to
            be created.

        \param name
            String object describing the name that the newly created instance will
            be created with.  This name must be unique within the system. 

        \exception UnknownObjectException
            thrown if no Image subclass has been registered using identifier \a type.

        \exception AlreadyExistsException
            thrown if an Image instance named \a name already exists.
        */

        public Image Create(string type, string name)
        {
            if (d_images.ContainsKey(name))
                throw new AlreadyExistsException("Image already exists: " + name);

            if (!d_factories.ContainsKey(type)) 
                throw new UnknownObjectException("Unknown Image type: " + type);

            var factory = d_factories[type];
            var image = factory.Create(name);
            d_images.Add(name, new Tuple<Image, ImageFactory>(image, factory));
            
            System.GetSingleton().Logger
                  .LogEvent("[ImageManager] Created image: '" + name + "' (" + image.GetHashCode().ToString("X8") +
                            ") of type: " + type);

            return image;
        }

        public Image Create(XMLAttributes attributes)
        {
            var name = attributes.GetValueAsString(ImageNameAttribute);

            if (String.IsNullOrEmpty(name))
                throw new InvalidRequestException("Invalid (empty) image name passed to create.");

            if (d_images.ContainsKey(name))
                throw new AlreadyExistsException("Image already exists: " + name);

            if (!d_factories.ContainsKey(s_imagesetType))
                throw new UnknownObjectException("Unknown Image type: " + s_imagesetType);

            var factory = d_factories[s_imagesetType];
            var image = factory.Create(attributes);

            // sanity check that the created image uses the name specified in the
            // attributes block
            if (image.GetName() != name)
            {
                var message = "Factory for type: " + s_imagesetType + " created Image named: " + image.GetName() +
                              ".  Was expecting name: " + name;

                factory.Destroy(image);

                throw new InvalidRequestException(message);
            }

            d_images[name] = new Tuple<Image, ImageFactory>(image, factory);

            System.GetSingleton().Logger
                .LogEvent(String.Format("[ImageManager] Created image: '{0}' (0x{1:X8}) of type: {2}", name, image.GetHashCode(), s_imagesetType));

            return image;
        }

        public void Destroy(Image image)
        {
            Destroy(image.GetName());
        }

        public void Destroy(string name)
        {
            var i = d_images.SingleOrDefault(x => x.Key == name);
            if (!i.Equals(default(KeyValuePair<string, Tuple<Image, ImageFactory>>)))
                Destroy(ref i);
        }

        public void DestroyAll()
        {
            while (d_images.Count!=0)
                Destroy(d_images.First().Key);
        }

        public Image Get(string name)
        {
            Tuple<Image, ImageFactory> i;
            if (d_images.TryGetValue(name, out i))
                return i.Item1;

            throw new UnknownObjectException("Image not defined: " + name);
        }

        public bool IsDefined(string name)
        {
            return d_images.ContainsKey(name);
        }

        public int GetImageCount()
        {
            return d_images.Count;
        }

        public void LoadImageset(string filename, string resourceGroup = "")
        {
            System.GetSingleton().GetXMLParser()
                  .ParseXmlFile(this, filename, ImagesetSchemaName, String.IsNullOrEmpty(resourceGroup)
                                                                        ? d_imagesetDefaultResourceGroup
                                                                        : resourceGroup);
        }

        public void LoadImagesetFromString(string source)
        {
            // System::getSingleton().getXMLParser()->parseXMLString(
            //*this, source, ImagesetSchemaName);
            throw new NotImplementedException();
        }

        public void DestroyImageCollection(string prefix, bool deleteTexture = true)
        {
            // Logger::getSingleton().logEvent(
            //    "[ImageManager] Destroying image collection with prefix: " + prefix);

            //ImagePrefixMatchPred p(prefix);

            //ImageMap::iterator i;
            //while ((i = std::find_if(d_images.begin(), d_images.end(), p)) != d_images.end())
            //    destroy(i);

            //if (delete_texture)
            //    System::getSingleton().getRenderer()->destroyTexture(prefix);
            throw new NotImplementedException();
        }

        public void AddBitmapImageFromFile(string name, string filename, string resourceGroup = "")
        {
            // create texture from image
            var tex = System.GetSingleton().GetRenderer()
                            .CreateTexture(name, filename,
                                           String.IsNullOrEmpty(resourceGroup)
                                               ? d_imagesetDefaultResourceGroup
                                               : resourceGroup);

            var image = (BitmapImage)Create(BitmapImage.TypeName, name);
            image.SetTexture(tex);
            var rect = new Rectf(Lunatics.Mathematics.Vector2.Zero, tex.GetOriginalDataSize());
            image.SetImageArea(rect);
        }

        /// <summary>
        /// Notify the ImageManager that the display size may have changed.
        /// </summary>
        /// <param name="size">
        /// Size object describing the display resolution
        /// </param>
        public void NotifyDisplaySizeChanged(Sizef size)
        {
            foreach (var i in d_images)
                i.Value.Item1.NotifyDisplaySizeChanged(size);
        }

        /// <summary>
        /// Sets the default resource group to be used when loading imageset data
        /// </summary>
        /// <param name="resourceGroup">
        /// String describing the default resource group identifier to be used.
        /// </param>
        public static void SetImagesetDefaultResourceGroup(string resourceGroup)
        {
            d_imagesetDefaultResourceGroup = resourceGroup;
        }

        /// <summary>
        /// Returns the default resource group currently set for Imagesets.
        /// </summary>
        /// <returns>
        /// String describing the default resource group identifier that will be
        /// used when loading Imageset data.
        /// </returns>
        public static string GetImagesetDefaultResourceGroup()
        {
            return d_imagesetDefaultResourceGroup;
        }

        #region Overrides of ChainedXMLHandler

        // XMLHandler overrides
        public override string GetSchemaName()
        {
            return ImagesetSchemaName;
        }

        public override string GetDefaultResourceGroup()
        {
            return d_imagesetDefaultResourceGroup;
        }

        // TODO ...
        ////! container type used to hold the images.
        //typedef std::map<String, std::pair<Image*, ImageFactory*>,
        //                 StringFastLessCompare
        //                 CEGUI_MAP_ALLOC(String, Image*)> ImageMap;

        ////! ConstBaseIterator type definition.
        //typedef ConstMapIterator<ImageMap> ImageIterator;

        ///*!
        //\brief
        //    Return a ImageManager::ImageIterator object to iterate over the available
        //    Image objects.
        //*/
        //ImageIterator getIterator() const {
        // return ImageIterator(d_images.begin(), d_images.end());
        //}

        // implement chained xml handler abstract interface
        protected override void ElementStartLocal(string element, XMLAttributes attributes)
        {
            if (element == ImageElement)
                ElementImageStart(attributes);
            else if (element == ImagesetElement)
                ElementImagesetStart(attributes);
            else
                System.GetSingleton().Logger.LogEvent(
                    "[ImageManager] Unknown XML element encountered: <" + element + ">", LoggingLevel.Errors);
        }

        protected override void ElementEndLocal(string element)
        {
        }

        #endregion

        // TODO: ...
        //! container type used to hold the registered Image types.
        //typedef std::map<String, ImageFactory*, StringFastLessCompare
        //    CEGUI_MAP_ALLOC(String, ImageFactory*)> ImageFactoryRegistry;

        /// <summary>
        /// helper to delete an image given an map iterator.
        /// </summary>
        /// <param name="iter"></param>
        private void Destroy(ref KeyValuePair<string, Tuple<Image, ImageFactory>> iter)
        {
            System.GetSingleton().Logger.LogEvent("[ImageManager] Deleted image: " + iter.Key);

            // use the stored factory to destroy the image it created.
            iter.Value.Item2.Destroy(iter.Value.Item1);

            d_images.Remove(iter.Key);
        }

        // XML parsing helper functions.
        private void ElementImagesetStart(XMLAttributes attributes)
        {
            // get name of the imageset.
            string name = attributes.GetValueAsString(ImagesetNameAttribute);
            // get name of the imageset.
            s_imagesetType = attributes.GetValueAsString(ImagesetTypeAttribute, ImageTypeAttributeDefault);
            // get texture image filename
            string filename= attributes.GetValueAsString(ImagesetImageFileAttribute);
            // get resource group to use for image file.
            string resource_group=attributes.GetValueAsString(ImagesetResourceGroupAttribute);

            var logger = System.GetSingleton().Logger;
            logger.LogEvent("[ImageManager] Started creation of Imageset from XML specification:");
            logger.LogEvent("[ImageManager] ---- CEGUI Imageset name: " + name);
            logger.LogEvent("[ImageManager] ---- Source texture file: " + filename);
            logger.LogEvent("[ImageManager] ---- Source texture resource group: " +
                            (String.IsNullOrEmpty(resource_group) ? "(Default)" : resource_group));

            ValidateImagesetFileVersion(attributes);

            var renderer = System.GetSingleton().GetRenderer();

            // if the texture already exists, 
            if (renderer.IsTextureDefined(name))
            {
                System.GetSingleton().Logger
                    .LogEvent("[ImageManager] WARNING: Using existing texture: " + name);
                s_texture = renderer.GetTexture(name);
            }
            else
            {
                // create texture from image
                s_texture = renderer.CreateTexture(name, filename,
                                                   String.IsNullOrEmpty(resource_group)
                                                       ? d_imagesetDefaultResourceGroup
                                                       : resource_group);
            }

            // set native resolution for imageset
            s_nativeResolution = new Sizef(attributes.GetValueAsFloat(ImagesetNativeHorzResAttribute, 640),
                                           attributes.GetValueAsFloat(ImagesetNativeVertResAttribute, 480));

            // set auto-scaling as needed
            s_autoScaled =
                PropertyHelper.FromString<AutoScaledMode>(attributes.GetValueAsString(ImagesetAutoScaledAttribute,
                                                                                      "false"));
        }

        private void ElementImageStart(XMLAttributes attributes)
        {
            var image_name = s_texture.GetName() + '/' + attributes.GetValueAsString(ImageNameAttribute);

            if (IsDefined(image_name))
            {
                System.GetSingleton().Logger.LogEvent("[ImageManager] WARNING: Using existing image :" + image_name);
                return;
            }

            XMLAttributes rwAttrs = attributes;

            // rewrite the name attribute to include the texture name
            rwAttrs.Add(ImageNameAttribute, image_name);

            if (!rwAttrs.Exists(ImageTextureAttribute))
                rwAttrs.Add(ImageTextureAttribute, s_texture.GetName());

            if (!rwAttrs.Exists(ImagesetAutoScaledAttribute))
                rwAttrs.Add(ImagesetAutoScaledAttribute, s_autoScaled.ToString());

            if (!rwAttrs.Exists(ImagesetNativeHorzResAttribute))
                rwAttrs.Add(ImagesetNativeHorzResAttribute,
                             s_nativeResolution.Width.ToString(CultureInfo.InvariantCulture));

            if (!rwAttrs.Exists(ImagesetNativeVertResAttribute))
                rwAttrs.Add(ImagesetNativeVertResAttribute,
                             s_nativeResolution.Height.ToString(CultureInfo.InvariantCulture));

            DeleteChaniedHandler = false;
            ChainedHandler = Create(rwAttrs);
        }

        //! throw exception if file version is not supported.
        private void ValidateImagesetFileVersion(XMLAttributes attrs)
        {
            var version = attrs.GetValueAsString(ImagesetVersionAttribute, "unknown");
            
            if (version == NativeVersion)
                return;

            throw new InvalidRequestException(
                "You are attempting to load an imageset of version '" + version +
                "' but this CEGUI version is only meant to load imagesets of version '" +
                NativeVersion + "'. Consider using the migrate.py script bundled with " +
                "CEGUI Unified Editor to migrate your data.");
        }

        //! Default resource group specifically for Imagesets.
        private static string d_imagesetDefaultResourceGroup;

        //! container holding the factories.
        //private /*ImageFactoryRegistry*/ Dictionary<string, ImageFactory> d_factories;
        private ImageFactoryRegistry d_factories = new ImageFactoryRegistry();

        //! container holding the images.
        //private /*ImageMap*/ Dictionary<string, Tuple<Image, ImageFactory>> d_images;

        private ImageMap d_images = new ImageMap();

        #region

        // Internal Strings holding XML element and attribute names
        private const string ImagesetSchemaName = "Imageset.xsd";
        private const string ImagesetElement = "Imageset";
        private const string ImageElement = "Image";
        private const string ImagesetImageFileAttribute = "imagefile";
        private const string ImagesetResourceGroupAttribute = "resourceGroup";
        private const string ImagesetNameAttribute = "name";
        private const string ImagesetTypeAttribute = "type";
        private const string ImagesetNativeHorzResAttribute = "nativeHorzRes";
        private const string ImagesetNativeVertResAttribute = "nativeVertRes";
        private const string ImagesetAutoScaledAttribute = "autoScaled";
        private const string ImageTextureAttribute = "texture";
        private const string ImageSvgDataAttribute = "SVGData";
        private const string ImageNameAttribute = "name";
        private const string ImagesetVersionAttribute = "version";
        // Internal Strings holding XML element and attribute defaults
        private const string ImageTypeAttributeDefault = BitmapImage.TypeName;

        //----------------------------------------------------------------------------//
        // note: The assets' versions aren't usually the same as CEGUI version, they
        // are versioned from version 1 onwards!
        //
        // previous versions (though not specified in files until 2)
        // 1 - CEGUI up to and including 0.7.x
        // 2 - CEGUI version 1.x.x (Custom Image support,
        //                          changed case of attr names, added version support)
        private const string NativeVersion = "2";

        //----------------------------------------------------------------------------//
        // Internal variables used when parsing XML
        private static Texture s_texture = null;
        // TODO: private static SVGData s_SVGData = 0;
        private static string s_imagesetType = "";
        private static AutoScaledMode s_autoScaled = AutoScaledMode.Disabled;
        private static Sizef s_nativeResolution = new Sizef(640.0f, 480.0f);

        #endregion
    }
}