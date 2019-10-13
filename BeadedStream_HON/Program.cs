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
            ArrayList orderedList = new ArrayList();

            do
            {
                while (!Console.KeyAvailable)
                {
                    Console.SetCursorPosition(0, 0);
                    OwdDS18B20 first = SensorSorter.gatherSensors();
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

        }
    }
}
