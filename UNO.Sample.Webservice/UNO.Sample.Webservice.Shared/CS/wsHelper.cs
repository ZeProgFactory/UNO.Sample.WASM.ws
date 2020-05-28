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

      public static string wsServer = "";
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


      private static Uri CalcURI(string function)
      {
         if (string.IsNullOrEmpty(wsServer))
         {
            return null;
         };

         if (string.IsNullOrEmpty(function))
         {
            return null;
         };

         if (!wsServer.EndsWith("/"))
         {
            wsServer = wsServer + '/';
         };

         if (function.StartsWith("/"))
         {
            function = function.Substring(1);
         };

         string URL = wsServer + function;

         return new Uri(URL);
      }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      public static string LastError { get; set; }

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="Function"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<string> wGet(string Function, string basicAuth = "")
      {
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            return null;
         };

         return await wGet(uri, basicAuth);
      }

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="Function"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<T> wGet<T>(string Function, string basicAuth = "")
      {
         Uri uri = CalcURI(Function);

         if (uri == null)
         {
            return default(T);
         };

         var data = await wGet(uri, basicAuth);
         return JsonSerializer.Deserialize<T>(data);
      }

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="Function"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
      public static async Task<T> wGet<T>(Uri uri, string basicAuth = "")
      {
         var data = await wGet(uri, basicAuth);
         return JsonSerializer.Deserialize<T>(data);
      }

      /// <summary>
      /// HTTP Get - Retrieve data - OK
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
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

      #region NotYetTested
      /// <summary>
      /// not tested
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="FilePath"></param>
      /// <returns></returns>
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

      /// <summary>
      /// not tested
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="oData"></param>
      /// <returns></returns>
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

      /// <summary>
      /// not tested
      /// </summary>
      /// <param name="uri"></param>
      /// <param name="json"></param>
      /// <param name="basicAuth"></param>
      /// <returns></returns>
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
      #endregion

      // - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  - -  -
   }
}

