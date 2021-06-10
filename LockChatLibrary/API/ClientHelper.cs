using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LockChatLibrary.API
{
    internal static class ClientHelper
    {
        internal static async Task<TResult> PostAsync<TResult, TFirst>(ApiConfig config, string requestUrl, TFirst data)
        {
            using (var client = ClientHelper.GetClient(new Uri(config.BaseUrl), config.Token))
            {
                var dataJson = JsonConvert.SerializeObject(data);
                var content = new StringContent(dataJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(requestUrl, content);

                if (EnsureSuccessStatusCode(response) == false)
                {
                    throw new HttpRequestException(await GetErrorMessage(response));
                }

                return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
            }
        }
        internal static TResult Post<TResult, TFirst>(ApiConfig config, string requestUrl, TFirst data)
        {
            using (var client = ClientHelper.GetClient(new Uri(config.BaseUrl), config.Token))
            {
                var dataJson = JsonConvert.SerializeObject(data);
                var content = new StringContent(dataJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(requestUrl, content).Result;

                if (EnsureSuccessStatusCode(response) == false)
                {
                    throw new HttpRequestException(GetErrorMessage(response).Result);
                }

                return JsonConvert.DeserializeObject<TResult>(response.Content.ReadAsStringAsync().Result);
            }
        }

        internal static TResult Get<TResult, TFirst>(ApiConfig config, string requestUrl)
        {
            using (var client = ClientHelper.GetClient(new Uri(config.BaseUrl), config.Token))
            {

                HttpResponseMessage response = client.GetAsync(requestUrl).Result;

                if (EnsureSuccessStatusCode(response) == false)
                {
                    throw new HttpRequestException(GetErrorMessage(response).Result);
                }

                return JsonConvert.DeserializeObject<TResult>(response.Content.ReadAsStringAsync().Result);
            }
        }

        internal static HttpClient GetClient(Uri uri, string token)
        {
            var authValue = new AuthenticationHeaderValue("Bearer", token);

            var client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue },
                BaseAddress = uri,
            };

            return client;
        }

        private static bool EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static async Task<string> GetErrorMessage(HttpResponseMessage response)
        {
            string errorMsg = await response.Content.ReadAsStringAsync();
            JObject json = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(errorMsg);
            return json["message"].ToString();
        }
    }
}
