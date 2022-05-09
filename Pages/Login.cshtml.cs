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
using Microsoft.AspNetCore.Http;

namespace MedicalWebApp.Pages
{
    public class LoginModel : PageModel
    {
        public const string SessionKeyName = "_UserName";
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

        public async Task<IActionResult> OnPostAsync(string Name, string Password)
        {
            var Url = "https://azmedicalsystem.azurewebsites.net/api/user/"+ Name +"/"+ Password;

            dynamic content = new { Name = Name};
            
            CancellationToken cancellationToken;


            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, Url))
            using (var httpContent = CreateHttpContent(content))
            {
                request.Content = httpContent;
                using (var response = await client
                   .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                   .ConfigureAwait(false))
                {

                    string resualtList = await (response.Content.ReadAsStringAsync());
                    
                    //List<UserTable> lst = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<UserTable>>(resualtList));

                    List<UserTable>  Ulst = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<UserTable>>(resualtList));

                    //HttpContext.Session.SetString("username", "abc");
                    
                    //ViewData["MedicineTable"] = lst;

                    //return Page();

                    if (Ulst != null)
                    {
                        //HttpContext.Session("CSharp")="Value";                        
                        HttpContext.Session.SetString(SessionKeyName, Ulst[0].Name.ToString()); 
                        TempData["Username"] = Ulst[0].Name.ToString();
                        //Session["username"]=Ulst[0].Name.ToString();   
                        ViewData["Username"] = Ulst[0].Name.ToString();                     
                        return RedirectToPage("AddMedicine",TempData["Username"]);
                    }
                    else{
                        TempData["Username"]="Guest";
                        return RedirectToPage(TempData["Username"]);
                        //return RedirectToPage("AddMedicine");
                    }

                    
                }
            }

        }
  
    }      
    
}
