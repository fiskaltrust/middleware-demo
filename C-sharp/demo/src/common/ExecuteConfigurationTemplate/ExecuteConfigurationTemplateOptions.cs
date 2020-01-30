using CommandLine;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace ExecuteConfigurationTemplate
{
    public class ExecuteConfigurationTemplateOptions
    {
        [Option('u', "helipadurl", Required = true, HelpText = "Service url for fiskaltrust helipad.", Default = "https://helipad-sandbox.fiskaltrust.at/")]
        public string HelipadUrl { get; set; }
        [Option('i', "accountid", Required = true, HelpText = "API Account Id for the accessing the configuration. (GUID formatted)")]
        public Guid AccountId { get; set; }
        [Option('a', "accesstoken", Required = true, HelpText = "API Accesstoken for the used account.")]
        public string AccessToken { get; set; }
        [Option('t', "template", Required = true, HelpText = "Template for the creation of the cashbox. (JSON serialization)")]
        public string Template { get; set; }

        public const int MaxParamValueLength = 8 * 1024;

        public static ExecuteConfigurationTemplateOptions GetOptionsFromCommandLine(string[] args)
        {

            var option = new ExecuteConfigurationTemplateOptions();
            Parser.Default.ParseArguments<ExecuteConfigurationTemplateOptions>(args)
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

        public static ExecuteConfigurationTemplateOptions GetScuOptionsFromReadLineLoop()
        {
            var option = new ExecuteConfigurationTemplateOptions();
            var properties = typeof(ExecuteConfigurationTemplateOptions).GetProperties();

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
