using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpcUa.Client.Host
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            using (var host = new OpcUaHost())
            {
                host.Start();

                Console.WriteLine("OPC UA Host running.");
                Console.WriteLine("Press Ctrl+C to exit.");

                var quit = new ManualResetEvent(false);

                Console.CancelKeyPress += (s, e) =>
                {
                    e.Cancel = true;
                    quit.Set();
                };

                quit.WaitOne();
            }
        }
    }
}
