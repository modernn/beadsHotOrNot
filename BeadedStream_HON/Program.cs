using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Xml;

namespace BeadedStream_HON
{
    class Program
    {
        private static bool debug = false;

        static void Main(string[] args)
        {
            SensorSorter sensorSorter = new SensorSorter();
            sensorSorter.Initialize(); // Get starting state

            bool done = false;

            while (!done)
            {
                // Wait 1 second
                System.Threading.Thread.Sleep(1000);
                Console.SetCursorPosition(0, 0);
                sensorSorter.UpdateSensorData();
                sensorSorter.CheckForNextSensor();

                done = sensorSorter.IsDone();

                OwdDS18B20 first = sensorSorter.GetSensorByHighestTemp();

                // Exit loop on key press
                if (Console.KeyAvailable)
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;
            }

            // Save, store, print, or burn EEPROM from list

        }
    }
}
