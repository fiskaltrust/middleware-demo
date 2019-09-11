using CommandLine;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace csConsoleApplicationSCU
{
    public class ScuOptions
    {
        [Option('u', "fiskaltrust-service-url", Required = true, HelpText = "Url for fiskaltrust signing service.", Default = "https://signing-sandbox.fiskaltrust.at/primesignhsm/")]
        public string ServiceUrl { get; set; }
        [Option('c', "cashboxid", Required = true, HelpText = "API Cashbox Id for the accessing the configuration. (GUID formatted)")]
        public Guid CashboxId { get; set; }
        [Option('a', "accesstoken", Required = true, HelpText = "API Accesstoken for the used cashbox.")]
        public string AccessToken { get; set; }

        public const int MaxParamValueLength = 8 * 1024;

        public static ScuOptions GetScuOptionsFromCommandLine(string[] args)
        {

            var option = new ScuOptions();
            Parser.Default.ParseArguments<ScuOptions>(args)
              .WithParsed(opts => { option = opts; })
              .WithNotParsed((errs) =>
              {
                  option = GetScuOptionsFromReadLineLoop();
              });
            return option;
        }

        private static string ReadLine()
        {
            Stream inputStream = Console.OpenStandardInput(MaxParamValueLength);
            byte[] bytes = new byte[MaxParamValueLength];
            int outputLength = inputStream.Read(bytes, 0, MaxParamValueLength);
            char[] chars = Encoding.UTF8.GetChars(bytes, 0, outputLength);
            var result = new string(chars);
            if (result.EndsWith(Environment.NewLine))
            {
                result = result.Substring(0, result.Length - Environment.NewLine.Length);
            }
            return result;
        }

        public static ScuOptions GetScuOptionsFromReadLineLoop()
        {
            var option = new ScuOptions();
            var properties = typeof(ScuOptions).GetProperties();

            foreach (var property in properties)
            {
                OptionAttribute attr = property.GetCustomAttributes(typeof(OptionAttribute), true).FirstOrDefault() as OptionAttribute;
                if (attr != null)
                {
                    string value = "";
                    while (value == string.Empty)
                    {
                        if (attr.Default != null)
                            Console.Write($"{attr.LongName} ({attr.Default}):");
                        else
                            Console.Write($"{attr.LongName}:");

                        value = ReadLine();
                        if (string.IsNullOrWhiteSpace(value) && attr.Default != null)
                        {
                            Console.Error.WriteLine($"No value given for {attr.LongName}. Using Default Value: {attr.Default}");
                            value = attr.Default.ToString();
                        }

                        if (string.IsNullOrEmpty(value))
                        {
                            Console.Error.WriteLine($"Error. Please provide a value for {attr.LongName}.");
                        }
                    }

                    property.SetValue(option, TypeDescriptor.GetConverter(property.PropertyType).ConvertFromInvariantString(value));
                }
            }
            return option;
        }
    }
}
