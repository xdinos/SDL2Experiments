using System;

namespace SharpCEGui.Base
{
    /// <summary>
    /// Class that holds information about a property and it's required initial value.
    /// </summary>
    public class PropertyInitialiser
    {
        public PropertyInitialiser()
        {
            // Empty
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="property">
        /// String holding the name of the property targetted by this PropertyInitialiser.
        /// </param>
        /// <param name="value">
        /// String holding the value to be set by this PropertyInitialiser.
        /// </param>
        public PropertyInitialiser(string property, string value)
        {
            d_propertyName = property;
            d_propertyValue = value;
        }

        /// <summary>
        /// Apply this property initialiser to the specified target CEGUI::PropertySet object.
        /// </summary>
        /// <param name="target">
        /// CEGUI::PropertySet object to be initialised by this PropertyInitialiser.
        /// </param>
        public void Apply(PropertySet target)
        {
            try
            {
                target.SetProperty(d_propertyName, d_propertyValue);
            }
            catch (UnknownObjectException)
            {
                // allow 'missing' properties
            }
        }

        /*!
        \brief
            Sets the name of the property targetted by this PropertyInitialiser.

        \param name
            String object holding the name of the target property.

        \return
            Nothing.
        */

        public void SetTargetPropertyName(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the name of the property targetted by this PropertyInitialiser.
        /// </summary>
        /// <returns>
        /// String object holding the name of the target property.
        /// </returns>
        public string GetTargetPropertyName()
        {
            return d_propertyName;
        }

        /*!
        \brief
            Sets the value string to be set on the property targetted by this PropertyInitialiser.

        \return
            String object holding the value string.

        \return
            Nothing.
        */

        public void SetInitialiserValue(string value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the value string to be set on the property targetted by this PropertyInitialiser.
        /// </summary>
        /// <returns>
        /// String object holding the value string.
        /// </returns>
        public string GetInitialiserValue()
        {
            return d_propertyValue;
        }

        /*!
        \brief
            Writes an xml representation of this PropertyInitialiser to \a out_stream.

        \param xml_stream
            Stream where xml data should be output.


        \return
            Nothing.
        */

        public void WriteXMLToStream(XMLSerializer xml_stream)
        {
            throw new NotImplementedException();
        }

        private string d_propertyName; //!< Name of a property to be set.
        private string d_propertyValue; //!< Value string to be set on the property.
    }
}