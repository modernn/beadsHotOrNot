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
        RootObject currentState;
        public List<OwdDS18B20> orderedSensorList { get; set; } 

        private string techName;
        private int totalDevices;
        private int totalSensors;
        private bool hasEEPROM;

        private DateTime completed;

        enum States { 
            WaitingForWire, 
            WaitingForDevices,
            WaitingForInitialHealth, 
            ReadyForLearning, 
            LearningInProgress, 
            WaitingForCompleteHealth, 
            GeneratingReport, 
            Done };

        States state;

        public void Initialize()
        {
            Initialize("");
        }

        public void Initialize(string techName)
        {
            this.techName = techName;
            orderedSensorList = new List<OwdDS18B20>();
            state = new States();
            state = States.WaitingForWire;
            UpdateSensorData();
        }

        private void GatherWireInformation(RootObject wireData)
        {
            int.TryParse(wireData.DevicesDetailResponse.DevicesConnected, out this.totalDevices);
            this.totalSensors = wireData.DevicesDetailResponse.owd_DS18B20.Count;
            this.hasEEPROM = (wireData.DevicesDetailResponse.owd_DS18B20 != null);
        }

        public void CheckForNextSensor()
        {
            // If we are not ready, don't check
            if (!IsReady())
                return;

            // If we haven't received a new state after the initial startState, don't check
            if (currentState == null)
                return;

            // Remove any found sensors from Current State
            int items = currentState.DevicesDetailResponse.owd_DS18B20.Count;
            items--; // make the for loop easier
            for (int i = items; i >= 0; i--)
            {
                OwdDS18B20 sensor = currentState.DevicesDetailResponse.owd_DS18B20[i];
                foreach (OwdDS18B20 os in orderedSensorList)
                    if (sensor.SensorID.Equals(os.SensorID))
                        currentState.DevicesDetailResponse.owd_DS18B20.RemoveAt(i);
            }

            // Add the Sensors to a dictionary with the ID as the key
            Dictionary<string, OwdDS18B20> dictStartingState = new Dictionary<string, OwdDS18B20>();
            Dictionary<string, OwdDS18B20> dictCurrentState = new Dictionary<string, OwdDS18B20>();

            foreach (OwdDS18B20 sensor in startState.DevicesDetailResponse.owd_DS18B20)
                dictStartingState.Add(sensor.SensorID, sensor);

            foreach (OwdDS18B20 sensor in currentState.DevicesDetailResponse.owd_DS18B20)
                dictCurrentState.Add(sensor.SensorID, sensor);

            // Check if any sensor is above threshold (2degrees c) compared to the starting state
            // Loop over the CURRENT state, because we've removed some of the sensors that 
            // we've already found from the list above
            float currentTemp = 0.0f;
            float startTemp = 0.0f;
            float threshold = 2.0f;
            foreach (KeyValuePair<string, OwdDS18B20> sensor in dictCurrentState)
            {
                float.TryParse(dictStartingState[sensor.Key].TemperatureCalibrated, out startTemp);
                float.TryParse(sensor.Value.TemperatureCalibrated, out currentTemp);

                // If it is above the threshold, add it to the "Hot List"
                if (currentTemp > startTemp + threshold)
                {
                    Console.Beep();
                    orderedSensorList.Add(sensor.Value);
                    break; // Once we find the one that is above start temp by threshold, save it
                }
            }
        }

        // If the "Hot List" has the same number of elements as the StartState.DevicesDetailResponse.DevicesConnected
        // then return true
        public bool IsDone()
        {
            // Can't be done if we didn't start
            if (startState == null)
                return false;

            Console.WriteLine("State: " + this.state.ToString());
            Console.WriteLine("Found: " + orderedSensorList.Count);

            int devicesConnected = startState.DevicesDetailResponse.owd_DS18B20.Count;
            if (devicesConnected > 0 && orderedSensorList.Count >= devicesConnected)
            {
                completed = DateTime.Now;
                return true;
            }

            return false;
        }

        public bool IsReady()
        {
            if (startState == null)
            {
                state = States.WaitingForWire;
                return false;
            }

            if (startState.DevicesDetailResponse.owd_DS18B20.Count == 0)
            {
                state = States.WaitingForDevices;
                return false;
            }

            int health = 0;

            // Check if any of the sensors are not ready (health = 1-7)
            // Meaning, it got at least 1 successful response back when checking the sensor
            // 7 = got 7 successful sensors readings back
            bool isReady = true;
            foreach (OwdDS18B20 sensor in startState.DevicesDetailResponse.owd_DS18B20)
            {
                int.TryParse(sensor.Health, out health);
                if (health < 1)
                {
                    isReady = false;
                    break;
                }
            }

            // If we are not ready at this point, 
            // it means not all sensors are reporting back at least one time
            if (!isReady)
                state = States.WaitingForInitialHealth;

            return isReady;
        }

        public OwdDS18B20 GetSensorByHighestTemp(RootObject wireData = null)
        {
            if (wireData == null)
                wireData = currentState;

            if (wireData == null)
                wireData = startState;

            // Get the temperature
            PrintSensors(wireData, "o: ");

            wireData = SortSensorsByTemp(wireData, "desc");

            // Find the ID for the highest temperature
            Comparison<OwdDS18B20> comparison = (x, y) => x.TemperatureCalibrated.CompareTo(y.TemperatureCalibrated);
            wireData.DevicesDetailResponse.owd_DS18B20.Sort(comparison);
            wireData.DevicesDetailResponse.owd_DS18B20.Reverse();

            var first = wireData.DevicesDetailResponse.owd_DS18B20[0];

            // Print the temperatures and ID's
            PrintSensors(wireData, "S: ");

            Console.WriteLine("F: " + first.SensorID + ": " + first.TemperatureCalibrated);

            return (first);
        }

        public void PrintAllSensors()
        {
            PrintSensors(startState, "Original: ");
            PrintSensors(currentState, "Current:  ");
        }

        public void PrintSensors(RootObject wireData, string prefixString)
        {
            if (wireData != null)
                PrintSensors(wireData.DevicesDetailResponse.owd_DS18B20, prefixString);
        }

        public void PrintSensors(List<OwdDS18B20> sensors, string prefixString)
        {
            foreach (OwdDS18B20 sensor in sensors)
            {
                Console.WriteLine(prefixString + sensor.SensorID + ": " + sensor.Health + ": " + sensor.TemperatureCalibrated);
            }
            Console.WriteLine("");
        }

        // Call this as often as you want to update the sensor data
        // The data will store in currentState
        // The first time it will instead store in startState
        // If the health of the sensors are not all 7, continue to wait
        // This will be used to determine each sensors individual temp increase beyond a threshold
        public void UpdateSensorData()
        {
            RootObject wireData = GetSensorData();
            if (startState == null || !IsReady())
            {
                startState = wireData;
            }
            else
            {
                currentState = wireData;

                if (currentState != null)
                {
                    int connected = 0;
                    int.TryParse(currentState.DevicesDetailResponse.DevicesConnected, out connected);
                    if (connected == 0)
                    {
                        state = States.WaitingForWire;
                        currentState = null;
                        startState =  null;
                        return;
                    }
                }

                // As long as we are not already learning, update the state to ReadyForLearning
                if (state < States.ReadyForLearning)
                {
                    state = States.ReadyForLearning;
                    GatherWireInformation(startState);
                }

                // If we've identifed at least one, we are in progress of learning
                if (this.orderedSensorList.Count > 0)
                    state = States.LearningInProgress;
            }
        }

        // This is used to get sensor data if you don't care about storing that data
        // This is also used by UpdateSensorData to store the data in this instance.
        public static RootObject GetSensorData()
        {

            // Download the details.xml file
            IRestResponse response = GetXMLFile("http://169.254.1.1/details.xml");

            // Save the XML file
            System.IO.File.WriteAllText(@".\details.xml", response.Content);

            // Convert XML to JSON
            string json = XMLtoJSON(response.Content);

            // Correct JSON to remove unneeded XML, dashes, @ and # symbols
            // We are working with single quotes to avoid escaping each double quote
            json = SingleToDouble(json, "'?xml':{'@version':'1.0','@encoding':'UTF-8'},", "");
            json = SingleToDouble(json, "'Devices-Detail-Response'", "'DevicesDetailResponse'");
            json = SingleToDouble(json, "'@xmlns':'http://beadedstream.com/schema/netgate','@xmlns:xsi':'http://www.w3.org/2001/XMLSchema-instance',", "");
            json = SingleToDouble(json, ":{'@Units':", ":{'Units':");
            json = SingleToDouble(json, ",'#text':", ",'text':");
            json = SingleToDouble(json, "{'@Description':", "{'Description':");

            // Parse JSON for elements
            //var jsonObject = JsonConvert.SerializeXmlNode(doc);
            RootObject wireData = JsonConvert.DeserializeObject<RootObject>(json);

            return wireData;
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

            // Save the JSON file
            System.IO.File.WriteAllText(@".\details.json", json);

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

        private static string SingleToDouble(string originalString, string badXMLSingle, string replaceXMLSingle)
        {
            // Replace single quote with double quote
            string badXMLDouble = badXMLSingle.Replace("'", "\"");
            string replaceXMLDouble = replaceXMLSingle.Replace("'", "\"");

            return originalString.Replace(badXMLDouble, replaceXMLDouble);
        }

        public string GenerateReportOutput()
        {
            string pattern = 
@"Techs Name                {0}
Total Sensors:              {1}
Total good sensors:         {2}
Total bad sensors:          {3}
Last successful sensor #    {4}
Time completed:             {5}

Sensors:";

            string result = string.Format(pattern, 
                this.techName, 
                this.totalSensors, 
                this.orderedSensorList.Count,
                this.totalSensors - this.orderedSensorList.Count,
                this.orderedSensorList.Count,
                this.completed.ToString());

            foreach(OwdDS18B20 sensor in this.orderedSensorList)
            {
                result = result + System.Environment.NewLine;
                result = result + sensor.SensorID;
            }

            return result;
        }
    }
}