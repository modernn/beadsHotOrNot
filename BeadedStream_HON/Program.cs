using Newtonsoft.Json;
using RestSharp;
using System;
using System.Xml;

namespace BeadedStream_HON
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


            var client = new RestClient("http://169.254.1.1/details.xml");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Host", "169.254.1.1");
            request.AddHeader("Postman-Token", "bdd4e9d9-381e-4e37-ad3a-694bd4965381,64e4c9db-f3f9-4c87-bba9-944fd4c5ac07");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.17.1");
            request.AddHeader("Content-Type", "application/json");
            IRestResponse response = client.Execute(request);

            Console.WriteLine(response.Content);

            // Convert XML to JSON
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response.Content);

            string json = JsonConvert.SerializeXmlNode(doc);
            Console.WriteLine(json);

            // correct JSON to remove unneeded XML and dashes
            // We are working with single quotes to avoid escaping
            json = singleToDboule(json, "'?xml':{'@version':'1.0','@encoding':'UTF-8'},", "");
            json = singleToDboule(json, "'Devices-Detail-Response'", "'DevicesDetailResponse'");
            json = singleToDboule(json, "'@xmlns':'http://beadedstream.com/schema/netgate','@xmlns:xsi':'http://www.w3.org/2001/XMLSchema-instance',", "");
            json = singleToDboule(json, ":{'@Units':", ":{'Units':");
            json = singleToDboule(json, ",'#text':", ",'text':");
            json = singleToDboule(json, "{'@Description':", "{'Description':");
            //json = singleToDboule(json, "", "");

            Console.WriteLine("");
            Console.WriteLine(json);

            // Parse JSON for elements
            //var jsonObject = JsonConvert.SerializeXmlNode(doc);
            RootObject account = JsonConvert.DeserializeObject<RootObject>(json);

            Console.WriteLine("Hold");
            // Get the temperature

        }

        private static string singleToDboule(string originalString, string badXMLSingle, string replaceXMLSingle)
        {
            // Replace single quote with double quote
            string badXMLDouble = badXMLSingle.Replace("'", "\"");
            string replaceXMLDouble = replaceXMLSingle.Replace("'", "\""); 

            return originalString.Replace(badXMLDouble, replaceXMLDouble);
        }
    }


}
