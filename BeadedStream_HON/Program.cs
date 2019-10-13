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
            string techName;
            Console.Write("Enter your name: ");
            techName = Console.ReadLine();

            SensorSorter sensorSorter = new SensorSorter();
            sensorSorter.Initialize(techName); // Get starting state

            bool done = false;

            while (!done)
            {
                // Wait 1 second
                System.Threading.Thread.Sleep(1000);

                // Reset console print position
                Console.Clear();
                Console.SetCursorPosition(0, 0);

                // Get the latest information from the website for the sensors
                sensorSorter.UpdateSensorData();

                // Print values
                sensorSorter.PrintAllSensors();

                // See if any sensor is being intentionally heated
                sensorSorter.CheckForNextSensor();

                // Check if all sensors have been found and ordered
                done = sensorSorter.IsDone();

                // Exit loop on key press
                if (Console.KeyAvailable)
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;
            }

            // Save, store, print, or burn EEPROM from list
            Console.Beep();
            Console.Beep();
            Console.Beep();
            sensorSorter.PrintSensors(sensorSorter.orderedSensorList, "Final: ");

            string report = sensorSorter.GenerateReportOutput();
            Console.Write(report);

            System.IO.File.WriteAllText(@".\report.txt", report);
        }
    }
}
