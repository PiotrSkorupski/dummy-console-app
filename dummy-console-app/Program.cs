using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace dummy_console_app
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to send request.");
            Console.ReadKey();

            string URL_ADDRESS = "https://dummy-soap-service.azurewebsites.net/DummySoapService.svc";
            HttpWebRequest request = WebRequest.Create(new Uri(URL_ADDRESS)) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "text/xml";
            request.Headers.Add("SOAPAction", "http://tempuri.org/IDummySoapService/GetDataUsingDataContract");

            StringBuilder data = new StringBuilder();
            CompositeType dummyRequest = new CompositeType();
            dummyRequest.BoolValue = true;
            dummyRequest.StringValue = "This is testing";

            // Insert code to set properties and fields of the object.  
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CompositeType));
            // To write to a file, create a StreamWriter object.  

            //Serialize object to XmlDocument
            XmlDocument xmlPayload = new XmlDocument();
            XPathNavigator navigator = xmlPayload.CreateNavigator();

            using (var writer = navigator.AppendChild())
            {
                var serializer = new XmlSerializer(typeof(CompositeType));
                serializer.Serialize(writer, dummyRequest);
            }


            
            

            XmlDocument xmlResult = new XmlDocument();
            string result = "";

            string strPayload = AppendEnvelope(xmlPayload.OuterXml);

            using (Stream stream = request.GetRequestStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(strPayload);
                writer.Close();
            }

            Console.WriteLine("Sending request.");
            Console.WriteLine(strPayload);

            data.Append(strPayload);
            byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());
            request.ContentLength = byteData.Length;

            try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        result = reader.ReadToEnd();
                        reader.Close();
                    }
                    Console.WriteLine("Receiveing result.");
                    xmlResult.LoadXml(result);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error!");
                    Console.WriteLine(e.Message);
                }

            Console.WriteLine("Result");
            Console.WriteLine(result);
        }

        //private static string AppendEnvelope(string data)
        //{
        //    string head = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""  xmlns:tem=""http://tempuri.org/""><soapenv:Header/><soapenv:Body><tem:GetDataUsingDataContract>
        //    ";
        //    string end = @"</tem:GetDataUsingDataContract></soapenv:Body></soapenv:Envelope>";
        //    return head + data + end;
        //}
        private static string AppendEnvelope(string data)
        {
            string head = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""><soapenv:Header/><soapenv:Body><GetDataUsingDataContract xmlns=""http://tempuri.org/"">
            ";
            string end = @"</GetDataUsingDataContract></soapenv:Body></soapenv:Envelope>";
            return head + data + end;
        }
    }

    [XmlRoot (ElementName = "composite", Namespace = "http://tempuri.org/")]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [XmlElement(Namespace = "http://schemas.datacontract.org/2004/07/dummy_soap_service")]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [XmlElement(Namespace = "http://schemas.datacontract.org/2004/07/dummy_soap_service")]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
