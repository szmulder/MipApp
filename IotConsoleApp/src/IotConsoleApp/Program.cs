using System;

using Amazon;
using Amazon.IoT;
using Amazon.Runtime;

namespace IotConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var awsCredentials = new BasicAWSCredentials("AKIAJ3ZJZ5X3NE3SX5TQ", "3UqgCkLw82AIjwk2brX0Mv6epVIu0LEyfKtwBSVM");
            var client = new AmazonIoTClient(awsCredentials, RegionEndpoint.APSoutheast2);

            var things = client.ListThingsAsync().Result;

            foreach (var thing in things.Things)
            {
                Console.WriteLine(thing.ThingName);
            }

            Console.WriteLine("Complete");
            Console.ReadLine();
        }
    }
}
