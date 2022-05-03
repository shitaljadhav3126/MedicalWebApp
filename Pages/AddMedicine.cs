using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MedicalWebApp.Pages
{
    public class AddMedicine : PageModel
    {
        private AppSettings AppSettings { get; set; }

        public AddMedicine(IOptions<AppSettings> settings)
        {
            AppSettings = settings.Value;
        }

        //private readonly ILogger<IndexModel> _logger;

        //public IndexModel(ILogger<IndexModel> logger)
        //{
        //    _logger = logger;
        //}

        public void OnGet()
        {

        }

        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            //if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }

        public async Task<IActionResult> OnPostAsync(string Name, string Count, string Location, string StoreName, string ProviderType, string BatchID)
        {
            var Url = "https://azmedicalsystem.azurewebsites.net/api/medicine?";

            dynamic content = new { Name = Name, Location = Location, StoreName= StoreName, Count= Count, ProviderType= ProviderType, BatchID= BatchID };
            

            CancellationToken cancellationToken;


            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, Url))
            using (var httpContent = CreateHttpContent(content))
            {
                request.Content = httpContent;

                


                //using (var response = await client
                //    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                //    .ConfigureAwait(false))
                using (var response = await client
                   .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                   .ConfigureAwait(false))
                {

                    string resualtList = await (response.Content.ReadAsStringAsync());
                    //string responseBody = await response.Content.ReadAsStringAsync();
                    //List<MedicineTable> lst = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<MedicineTable>>(resualtList));
                    //.ReadAsStringAsync();
                    //ReadAsAsync<List<MedicineTable>>();

                    List<MedicineTable> emp = new List<MedicineTable>
                    {
                        new MedicineTable{},
                        new MedicineTable{ }
                    };


                    ViewData["MedicineTable"] = lst;

                    return Page();
                }
            }

        }
    }
}
