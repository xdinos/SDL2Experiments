using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpCEGui.Base.Svg
{
    public class SvgDataManager
    {
        #region Implementation of Singleton

        private static readonly Lazy<SvgDataManager> Instance = new Lazy<SvgDataManager>(() => new SvgDataManager());

        public static SvgDataManager GetSingleton()
        {
            return Instance.Value;
        }

        #endregion

        private SvgDataManager()
        {
        }

        // TODO: ~SVGDataManager(){ destroyAll(); }

        /// <summary>
        /// Creates an instance of SVGData class registered using the name \a name.
        /// </summary>
        /// <param name="name">
        /// String object describing the name that the newly created instance will
        /// be created with.  This name must be unique within the system. 
        /// </param>
        /// <returns></returns>
        /// <exception cref="AlreadyExistsException">
        /// thrown if an Image instance named \a name already exists.
        /// </exception>
        public SvgData Create(string name)
        {
            if (_svgDataMap.ContainsKey(name))
                throw new AlreadyExistsException("An SVGData object named " + name + " already exists.");

            var svg_data = new SvgData(name);
            _svgDataMap[name] = svg_data;

            LogSvgDataCreation(svg_data);

            return svg_data;
        }

        /*!
    \brief
        Creates an instance of SVGData class registered using the name \a name.

    \param name
        String object describing the name that the newly created instance will
        be created with.  This name must be unique within the system. 

    \param filename
        String object that specifies the path and filename of the image file to
        use when creating the texture.

    \param resourceGroup
        String object that specifies the resource group identifier to be passed
        to the resource provider when loading the texture file \a filename.

    \exception UnknownObjectException
        thrown if no Image subclass has been registered using identifier \a type.

    \exception AlreadyExistsException
        thrown if an Image instance named \a name already exists.
    */

        public SvgData Create(string name, string filename, string resourceGroup)
        {
            if (_svgDataMap.ContainsKey(name))
                throw new AlreadyExistsException("An SVGData object named " + name + " already exists.");

            var svg_data = new SvgData(name, filename, resourceGroup);
            _svgDataMap[name] = svg_data;

            LogSvgDataCreation(svg_data);

            return svg_data;
        }

        /*!
    \brief
        Destroys the SVGData object.

    \param svgData
        The object to destroy
    */

        public void Destroy(SvgData svgData)
        {
            Destroy(svgData.getName());
        }

        /// <summary>
        /// Destroys the SVGData object registered using the name \a name.
        /// </summary>
        /// <param name="name">
        /// The String holding the name of the SVGData object to be destroyed.
        /// </param>
        public void Destroy(string name)
        {
            if (_svgDataMap.ContainsKey(name))
            {
                var item = _svgDataMap[name];
                System.GetSingleton().Logger.LogEvent("[SVGDataManager] Deleted SVGData object: " + item.getName());

                _svgDataMap.Remove(name);
            }
        }

        /// <summary>
        /// Destroys all of the SVGData objects created with this manager.
        /// </summary>
        public void DestroyAll()
        {
            while (_svgDataMap.Count != 0)
                Destroy(_svgDataMap.First().Key);
        }

        /*!
    \brief
        Returns a SVGData object that was previously created by calling a
        create function of this class.

    \param name
        String holding the name of the SVGData object to be returned.

    \exceptions
        - UnknownObjectException - thrown if no SVGData object named \a name
          exists within the system.
    */

        public SvgData getSVGData(string name)
        {
            if (!_svgDataMap.ContainsKey(name))
                throw new UnknownObjectException("No SVGData named '" + name + "' is available.");

            return _svgDataMap[name];
        }


        /*!
    \brief
        Return whether an SVGData object with the given name exists.

    \param name
        String holding the name of the SVGData object to be checked.
    */

        public bool IsSVGDataDefined(string name)
        {
            return _svgDataMap.ContainsKey(name);
        }

        /// <summary>
        /// Logs the creation of the SVGData object.
        /// </summary>
        /// <param name="svgData">
        /// The SVGData object that was created.
        /// </param>
        private void LogSvgDataCreation(SvgData svgData)
        {
            System.GetSingleton().Logger
                  .LogEvent("[SVGDataManager] Created SVGData object: '" + svgData.getName() + "' " +
                            "(" + svgData.GetHashCode().ToString("X8") + ")");
        }

        /// <summary>
        /// container holding the SVGData objects.
        /// </summary>
        private readonly Dictionary<string, SvgData> _svgDataMap = new Dictionary<string, SvgData>();
    }
}