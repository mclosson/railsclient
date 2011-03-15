using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using RailsClient.ExtensionMethods;

namespace RailsClient
{
    public class RESTfulFinder
    {
        public static T find<T>(string id)
        {
            Type type = typeof(T);
            string xmlroot = type.Name.ToString().ToLower();
            string ResourceURL = RESTfulResourceBase.baseurl + type.Name.ToString().ToLower().Pluralize() + "/" + id + ".xml";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ResourceURL);
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                XmlSerializer serializer = new XmlSerializer(type, new XmlRootAttribute(xmlroot));
                return (T)serializer.Deserialize(stream);
            }
            //else if (response.StatusCode == HttpStatusCode.NotFound)
            throw new System.Exception("404 Resource does not exist");
        }

        public static List<T> all<T>()
        {
            Type type = typeof(T);
            Type list = typeof(List<T>);

            Type[] extraTypes = new Type[1];
            extraTypes[0] = typeof(T);

            string defaultNamespace = "";
            string xmlroot = type.Name.ToString().Pluralize().ToLower();
            string ResourceCollectionURL = RESTfulResourceBase.baseurl + type.Name.ToString().ToLower().Pluralize() + ".xml";

            XmlAttributes xmlAttributes = new XmlAttributes();
            XmlTypeAttribute xmlTypeAttribute = new XmlTypeAttribute(type.ToString().ToLower());
            XmlAttributeOverrides xmlAttributeOverrides = new XmlAttributeOverrides();
            xmlAttributes.XmlType = xmlTypeAttribute;
            xmlAttributeOverrides.Add(type, xmlAttributes);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ResourceCollectionURL);
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                XmlSerializer serializer = new XmlSerializer(list, xmlAttributeOverrides, extraTypes, new XmlRootAttribute(xmlroot), defaultNamespace);
                return (List<T>)serializer.Deserialize(stream);
            }
            //else if (response.StatusCode == HttpStatusCode.NotFound)
            throw new System.Exception("404 Resource does not exist");
        }
    }
}
