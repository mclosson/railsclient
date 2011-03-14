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
    }
}
