using System;
using Akka.Actor;

namespace UpForGrabber.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var system = ActorSystem.Create("upforgrabber");
        }
    }
}
