using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using RailsClient.ExtensionMethods;

namespace RailsClient
{
    public class RESTfulResource : RESTfulResourceBase
    {
        public int id;

        public void save()
        {
            if (this.id == 0)
            {
                this.create();
            }
            else
            {
                this.update();
            }
        }

        private string ResourcePath()
        {
            return baseurl + this.GetType().Name.ToString().ToLower().Pluralize() + "/" + this.id.ToString() + ".xml";
        }

        private string CollectionResourcePath()
        {
            return baseurl + this.GetType().Name.ToString().ToLower().Pluralize() + ".xml";
        }

        private void create()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ToXml());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(CollectionResourcePath());
            request.ContentType = "text/xml";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.Created)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                // response.GetResponseHeader("location); contains the URL of the newly created resource
            }
            else // Unprocessible Entity (422)
            {
                // display all the errors
            }
        }

        private void update()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ToXml());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ResourcePath());
            request.ContentType = "text/xml";
            request.ContentLength = bytes.Length;
            request.Method = "PUT";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // Resource was updated successfully
            }
            else // Unprocessible Entity (422)
            {
                // display all the errors
            }
        }

        public void delete()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ResourcePath());
            request.Method = "DELETE";
            Stream requestStream = request.GetRequestStream();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
            }
        }

        private string ToXml()
        {
            Type type = this.GetType();
            string xmlroot = type.Name.ToString().ToLower();

            MemoryStream stream = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings() { Encoding = new UTF8Encoding(true), OmitXmlDeclaration = false, Indent = true };
            XmlWriter writer = XmlWriter.Create(stream, settings);
            XmlSerializer serializer = new XmlSerializer(type, new XmlRootAttribute(xmlroot));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            serializer.Serialize(writer, this, namespaces);
            stream.Seek(0, 0);
            string xml = new StreamReader(stream).ReadToEnd();
            return xml;
        }
    }
}