using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;
using Improbable;

namespace Vehicles
{
    internal class Options
    {
        [Value(0, MetaName = "output", Default = "./snapshots/default.snapshot", HelpText = "Output file.")]
        public string OutputFile { get; set; }
    }

    public class Snapshots
    {
        public static int Main(string[] args)
        {
            var parser = new Parser();
            var result = parser.ParseArguments<Options>(args);

            return result.MapResult(opts => Run(opts), errs =>
            {
                Console.Error.Write(HelpText.AutoBuild(result));
                return 1;
            });
        }

        private static int Run(Options options)
        {
            Assembly.Load("GeneratedCode");

            Console.WriteLine("Writing to: {0}", options.OutputFile);
            var outputStream = new EntityOutputStream(options.OutputFile);

            outputStream.WriteEntity(new EntityId(1), EntityTemplates.CreatePlayerEntity());

            Console.WriteLine("Summary:");
            outputStream.PrintSummary();

            return 0;
        }
    }
}