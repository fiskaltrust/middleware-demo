using CommandLine;
using System;
using System.ComponentModel;
using System.Linq;

namespace ExecuteConfigurationTemplate
{
    public class ExecuteConfigurationTemplateOptions
    {
        [Option('u', "helipadurl", Required = true, HelpText = "Service url for fiskaltrust helipad.", Default = "https://helipad-sandbox.fiskaltrust.at/")]
        public string HelipadUrl { get; set; }
        [Option('i', "accountid", Required = true, HelpText = "Id for the used accountid. (This id needs to be a guid)")]
        public Guid AccountId { get; set; }
        [Option('a', "accesstoken", Required = true, HelpText = "Accesstoken for the used cashbox.")]
        public string AccessToken { get; set; }
        [Option('t', "template", Required = true, HelpText = "Template for the cashbox. JSON")]
        public string Template { get; set; }


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
