using System;
using System.Xml;

namespace SharpCEGui.Base
{
    public class DefaultXmlParser : XMLParser
    {
        public override void ParseXml(XmlHandler handler, RawDataContainer source, string schemaName, bool allowXmlValidation = true)
        {
            var settings = new XmlReaderSettings {IgnoreWhitespace = true};
            if (!String.IsNullOrEmpty(schemaName))
            {
                using (var schemaFile = new RawDataContainer())
                {
                    System.GetSingleton().GetResourceProvider().LoadRawDataContainer(schemaName, schemaFile, "schemas");
                    settings.Schemas.Add(null, new XmlTextReader(schemaFile.Stream()));
                }
            }

            using (var reader = XmlReader.Create(source.Stream(), settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            OnStartElement(reader, handler);
                            reader.MoveToElement();
                            if (reader.IsEmptyElement)
                                OnEndElement(reader, handler);
                            break;
                        case XmlNodeType.EndElement:
                            OnEndElement(reader, handler);
                            break;
                        case XmlNodeType.Text:
                        case XmlNodeType.CDATA:
                            OnText(reader, handler);
                            break;
                    }
                }
            }
        }

        protected override bool InitialiseImpl()
        {
            return true;
        }

        protected override void CleanupImpl()
        {
            
        }

        private void OnStartElement(XmlReader reader, XmlHandler handler)
        {
            var element = reader.Name;
            var attributes = new XMLAttributes();
            while (reader.MoveToNextAttribute())
            {
                attributes.Add(reader.Name, reader.Value);
            }
            handler.ElementStart(element, attributes);
        }

        private static void OnEndElement(XmlReader reader, XmlHandler handler)
        {
            handler.ElementEnd(reader.Name);
        }

        private static void OnText(XmlReader reader, XmlHandler handler)
        {
            handler.Text(reader.Value);
        }
    }
}