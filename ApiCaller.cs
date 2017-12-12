using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WeddingInfo
{
    public class WebRequest
    {
        public static async Task GetAddressDataAsync(string address, Action<Dictionary<string, object>> Callback)
        {
            using (var Client = new HttpClient()) // Creats a temporary HttpClient connection as variable Client
            {
                try
                {
                    Client.BaseAddress = new Uri($"https://maps.googleapis.com/maps/api/geocode/json?address={Encoding.UTF8.GetBytes(address)}&key=AIzaSyChLxYqe8DHBWFgfouhjBNnb00_2zGOFCs");
                    HttpResponseMessage Response = await Client.GetAsync(""); // Makes the API Call
                    Response.EnsureSuccessStatusCode(); // if the API call is unsuccesfull, throw an error (ensures if return is succesfull, and throws the error instead of crashing)
                    string StringResponse = await Response.Content.ReadAsStringAsync(); // if successful, returns the API response as a serialized string

                    // convert the string response into JSON/Dictionary format 
                    Dictionary<string, object> JsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(StringResponse);
                    Callback(JsonResponse);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }
    }
}