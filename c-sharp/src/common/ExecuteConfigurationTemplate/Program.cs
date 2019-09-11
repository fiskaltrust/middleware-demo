using ExecuteConfigurationTemplate.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExecuteConfigurationTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var options = ExecuteConfigurationTemplateOptions.GetOptionsFromCommandLine(args);
                var cashbox = ExecuteTemplate(options).Result;
                Console.WriteLine($"Successfully created cashbox with id {cashbox.CashBoxId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task<NewCashBoxModel> ExecuteTemplate(ExecuteConfigurationTemplateOptions options)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("accountid", options.AccountId.ToString());
                client.DefaultRequestHeaders.Add("accesstoken", options.AccessToken);
                client.DefaultRequestHeaders.Add("version", "v0");

                var content = new StringContent(JsonConvert.SerializeObject(options.Template), Encoding.UTF8, "application/json");
                var uriBuilder = new UriBuilder($"{options.HelipadUrl}api/configuration");
                var response = await client.PostAsync(uriBuilder.Uri, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<NewCashBoxModel>(responseContent);
                }
                else
                {
                    throw new Exception($"Request failed with Statuscode: {response.StatusCode}. {response.ReasonPhrase} - {responseContent}");
                }
            }
        }
    }
}