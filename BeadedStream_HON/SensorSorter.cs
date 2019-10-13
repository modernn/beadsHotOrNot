using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BeadedStream_HON
{
    class SensorSorter
    {
        private static bool debug = false;
        RootObject startState;

        public void Initialize()
        {

        }


        public void getHighestTemp()
        {

        }

        public static OwdDS18B20 gatherSensors()
        {

            // Download the details.xml file
            IRestResponse response = GetXMLFile("http://169.254.1.1/details.xml");

            // Convert XML to JSON
            string json = XMLtoJSON(response.Content);

            // Correct JSON to remove unneeded XML, dashes, @ and # symbols
            // We are working with single quotes to avoid escaping each double quote
            json = SingleToDboule(json, "'?xml':{'@version':'1.0','@encoding':'UTF-8'},", "");
            json = SingleToDboule(json, "'Devices-Detail-Response'", "'DevicesDetailResponse'");
            json = SingleToDboule(json, "'@xmlns':'http://beadedstream.com/schema/netgate','@xmlns:xsi':'http://www.w3.org/2001/XMLSchema-instance',", "");
            json = SingleToDboule(json, ":{'@Units':", ":{'Units':");
            json = SingleToDboule(json, ",'#text':", ",'text':");
            json = SingleToDboule(json, "{'@Description':", "{'Description':");

            // Parse JSON for elements
            //var jsonObject = JsonConvert.SerializeXmlNode(doc);
            RootObject wireData = JsonConvert.DeserializeObject<RootObject>(json);

            // Get the temperature
            foreach (OwdDS18B20 sensor in wireData.DevicesDetailResponse.owd_DS18B20)
            {
                Console.WriteLine("O: " + sensor.SensorID + ": " + sensor.TemperatureCalibrated);
            }

            wireData = SortSensorsByTemp(wireData, "desc");

            // Find the ID for the highest temperature
            Comparison<OwdDS18B20> comparison = (x, y) => x.TemperatureCalibrated.CompareTo(y.TemperatureCalibrated);
            wireData.DevicesDetailResponse.owd_DS18B20.Sort(comparison);
            wireData.DevicesDetailResponse.owd_DS18B20.Reverse();

            var first = wireData.DevicesDetailResponse.owd_DS18B20[0];

            // Print the temperatures and ID's
            foreach (OwdDS18B20 sensor in wireData.DevicesDetailResponse.owd_DS18B20)
            {
                Console.WriteLine("S: " + sensor.SensorID + ": " + sensor.TemperatureCalibrated);
            }

            Console.WriteLine("F: " + first.SensorID + ": " + first.TemperatureCalibrated);

            return(first);
        }


        private static string getHighestTemp(Array sensors)
        {


            return "";
        }

        // Sort the sensors in descending order, unless passing in something else
        private static RootObject SortSensorsByTemp(RootObject wireData, string order = "desc")
        {
            Comparison<OwdDS18B20> comparison = (x, y) => x.TemperatureCalibrated.CompareTo(y.TemperatureCalibrated);
            wireData.DevicesDetailResponse.owd_DS18B20.Sort(comparison);

            if (order.Equals("desc"))
                wireData.DevicesDetailResponse.owd_DS18B20.Reverse();

            return wireData;
        }

        private static string XMLtoJSON(string xmlData)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);
            string json = JsonConvert.SerializeXmlNode(doc);

            if (debug)
                Console.WriteLine(json);

            return json;
        }

        private static IRestResponse GetXMLFile(string xmlURL = "http://169.254.1.1/details.xml")
        {
            var client = new RestClient(xmlURL);
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

            if (debug)
                Console.WriteLine(response.Content);

            return response;
        }

        private static string SingleToDboule(string originalString, string badXMLSingle, string replaceXMLSingle)
        {
            // Replace single quote with double quote
            string badXMLDouble = badXMLSingle.Replace("'", "\"");
            string replaceXMLDouble = replaceXMLSingle.Replace("'", "\"");

            return originalString.Replace(badXMLDouble, replaceXMLDouble);
        }
    }
}