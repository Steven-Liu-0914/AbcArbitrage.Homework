using System;
using AbcArbitrage.Homework.Routing;
using BenchmarkDotNet.Running;

namespace AbcArbitrage.Homework
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MessageRouterBenchmarks>();
            //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }
    }
}
