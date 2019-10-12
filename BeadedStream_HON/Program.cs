using RestSharp;
using System;

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
        }
    }
}
