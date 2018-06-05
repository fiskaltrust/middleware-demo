using CommandLine;
using System;
using System.ComponentModel;
using System.Linq;

namespace csConsoleApplicationSCU
{
    public class ScuOptions
    {
        [Option('u', "serviceurl", Required = true, HelpText = "Service url for fiskaltrust signing.", Default = "https://signing-sandbox.fiskaltrust.at/primesignhsm/")]
        public string ServiceUrl { get; set; }
        [Option('c', "cashboxid", Required = true, HelpText = "Id for the used cashbox. (This id needs to be a guid)")]
        public Guid CashboxId { get; set; }
        [Option('a', "accesstoken", Required = true, HelpText = "Accesstoken for the used cashbox.")]
        public string AccessToken { get; set; }

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

                        value = Console.ReadLine();
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
