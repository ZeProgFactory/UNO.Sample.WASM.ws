using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json; // Install-Package System.Text.Json 

namespace ZPF
{
   public static class wsHelper
   {
      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      private static HttpClient _httpClient = null;

      public static bool Init()
      {
         if (_httpClient != null) return true;

#if __WASM__
            var innerHandler = new Uno.UI.Wasm.WasmHttpHandler();
#else
         var innerHandler = new HttpClientHandler();
#endif

         _httpClient = new HttpClient(innerHandler);

         return _httpClient != null;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static string LastError { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static async Task<string> wGet(Uri uri, string basicAuth = "")
      {
         Init();

         LastError = "";
         string data = "";

         try
         {
#if __WASM__
#else
            if (!string.IsNullOrEmpty(basicAuth))
            {
               byte[] byteArray = Encoding.ASCII.GetBytes(basicAuth);
               _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            };
#endif

            data = await _httpClient.GetStringAsync(uri);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);

            data = null;
         };

         return data;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static async Task<bool> wGetDownload(Uri uri, string FilePath)
      {
         Init();

         LastError = "";

         try
         {
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            byte[] byteArray = await _httpClient.GetByteArrayAsync(uri);

            // ZPF.XF.Basics.Current.FileIO.WriteStream(new MemoryStream(byteArray), FilePath);
            throw new NotImplementedException();

            return true;
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);
         };

         return false;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static async Task<string> wPost(Uri uri, object oData)
      {
         LastError = "";

         try
         {
            var json = JsonSerializer.Serialize(oData);
            return await wPost(uri, json);
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            return null;
         };
      }

      public static async Task<string> wPost(Uri uri, string json, string basicAuth = "")
      {
         LastError = "";
         string data = "";

         try
         {

#if __WASM__
#else
            if (!string.IsNullOrEmpty(basicAuth))
            {
               byte[] byteArray = Encoding.ASCII.GetBytes(basicAuth);
               _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            };
#endif
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _httpClient.BaseAddress = uri;

            var x = new StringContent(json, Encoding.Unicode);
            x.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync("", x);

            response.EnsureSuccessStatusCode();

            data = response.StatusCode.ToString();
         }
         catch (Exception ex)
         {
            LastError = ex.Message;
            Debug.WriteLine(ex.Message);

            data = null;
         };

         return data;
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}

