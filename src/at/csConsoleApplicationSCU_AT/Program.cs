using CommandLine;
using fiskaltrust.ifPOS.v0;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace csConsoleApplicationSCU
{
    class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                ServicePointManager.DefaultConnectionLimit = 65535;
                var options = ScuOptions.GetScuOptionsFromCommandLine(args);
                var proxy = ProxyFactory.CreateProxy(options);

                CheckSCU(proxy);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();
        }

        static void CheckSCU(IATSSCD proxy)
        {
            try
            {
                var echoResult = proxy.BeginEcho("Hello World", null, null);
                Console.WriteLine(proxy.EndEcho(echoResult));

                proxy.BeginZDA((result) =>
                {
                    var p = (IATSSCD)result.AsyncState;
                    var z = p.EndZDA(result);
                    Console.WriteLine(z);
                }, proxy);

                var zda = proxy.ZDA();
                if (string.IsNullOrWhiteSpace(zda))
                {
                    throw new Exception("reading ZDA failed!");
                }
                var certificate = proxy.Certificate();
                if (certificate == null)
                {
                    throw new Exception("reading certificate failed!");
                }
                var x509cert = new X509Certificate2(certificate);
                Console.WriteLine($"0x{x509cert.GetSerialNumberString()}");
                var CertBase64 = Convert.ToBase64String(x509cert.GetRawCertData());

                var startMoment = DateTime.UtcNow;
                int n = 20;
                for (int i = 0; i < n; i++)
                {
                    byte[] data = null;
                    ExecuteWithStopWatch(() =>
                    {
                        data = proxy.Sign(Guid.NewGuid().ToByteArray());
                    });

                    if (data == null)
                    {
                        throw new Exception("signing failed!");
                    }
                    Console.WriteLine(BitConverter.ToString(data));
                }
                Console.WriteLine($"avg: {DateTime.UtcNow.Subtract(startMoment).TotalMilliseconds / n}ms");

            }
            catch (Exception x)
            {
                Console.Error.WriteLine(x);
            }
        }

        public static void ExecuteWithStopWatch(Action action)
        {
            var sw = new Stopwatch();
            sw.Restart();
            action();
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}