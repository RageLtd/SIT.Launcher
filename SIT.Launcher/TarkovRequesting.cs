using ComponentAce.Compression.Libs.zlib;
using Octokit.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Launcher
{
    public class TarkovRequesting
    {
        public string Session;
        public string RemoteEndPoint;
        public bool isUnity;

        private static HttpClient httpClient;

        public TarkovRequesting(string session, string remoteEndPoint, bool isUnity = true)
        {
            Session = session;
            RemoteEndPoint = remoteEndPoint;
            httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Cookie", $"PHPSESSID={Session}");
            httpClient.DefaultRequestHeaders.Add("SessionId", Session);
            httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "deflate");
            httpClient.Timeout = new TimeSpan(0, 0, 1);
        }
        /// <summary>
        /// Send request to the server and get Stream of data back
        /// </summary>
        /// <param name="url">String url endpoint example: /start</param>
        /// <param name="method">POST or GET</param>
        /// <param name="data">string json data</param>
        /// <param name="compress">Should use compression gzip?</param>
        /// <returns>Stream or null</returns>
        private Stream Send(string url, string method = "GET", string data = null, bool compress = true)
        {
            // disable SSL encryption
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var fullUri = url;
            if (!Uri.IsWellFormedUriString(fullUri, UriKind.Absolute))
                fullUri = RemoteEndPoint + fullUri;

            if (!fullUri.StartsWith("https://") && !fullUri.StartsWith("http://"))
                fullUri = fullUri.Insert(0, "https://");

            var request = new HttpRequestMessage(new HttpMethod(method), fullUri);

            if (!string.IsNullOrEmpty(Session))
            {
                request.Headers.Add("Cookie", $"PHPSESSID={Session}");
                request.Headers.Add("SessionId", Session);
            }

            request.Headers.Add("Accept-Encoding", "deflate");

            if (method != "GET" && !string.IsNullOrEmpty(data))
            {
                var bytes = compress ? SimpleZlib.CompressToBytes(data, zlibConst.Z_BEST_SPEED) : Encoding.UTF8.GetBytes(data);

                var stream = new StreamContent(new MemoryStream(bytes));
                request.Content = stream;
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content.Headers.ContentLength = bytes.Length;

                if (compress)
                {
                    request.Content.Headers.Add("content-encoding", "deflate");
                }
            }

            // Send request
            try
            {
                var response = httpClient.SendAsync(request).Result;
                return response.Content.ReadAsStreamAsync().Result;
            }
            catch (Exception)
            {
                return SendHttp(url, method, data, compress);
            }
        }


        private Stream SendHttp(string url, string method = "GET", string data = null, bool compress = true)
        {
            // disable SSL encryption
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var fullUri = url;
            if (!Uri.IsWellFormedUriString(fullUri, UriKind.Absolute))
                fullUri = RemoteEndPoint + fullUri;

            if (!fullUri.StartsWith("http://"))
                fullUri = fullUri.Insert(0, "http://");

            var request = new HttpRequestMessage(new HttpMethod(method), fullUri);

            if (!string.IsNullOrEmpty(Session))
            {
                request.Headers.Add("Cookie", $"PHPSESSID={Session}");
                request.Headers.Add("SessionId", Session);
            }

            request.Headers.Add("Accept-Encoding", "deflate");

            if (method != "GET" && !string.IsNullOrEmpty(data))
            {
                var bytes = compress ? SimpleZlib.CompressToBytes(data, zlibConst.Z_BEST_SPEED) : Encoding.UTF8.GetBytes(data);

                var stream = new StreamContent(new MemoryStream(bytes));
                request.Content = stream;
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                request.Content.Headers.ContentLength = bytes.Length;

                if (compress)
                {
                    request.Content.Headers.Add("content-encoding", "deflate");
                }
            }

            // Send Request
            var response = httpClient.SendAsync(request).Result;
            return response.Content.ReadAsStreamAsync().Result;

        }

        public void PutJson(string url, string data, bool compress = true)
        {
            using Stream stream = Send(url, "PUT", data, compress);
        }

        public string GetJson(string url, bool compress = true)
        {
            using Stream stream = Send(url, "GET", null, compress);
            using MemoryStream ms = new();
            if (stream == null)
                return "";
            stream.CopyTo(ms);
            //return Encoding.UTF8.GetString(DecompressFile(ms.ToArray()));
            return SimpleZlib.Decompress(ms.ToArray(), null);
        }

        public string PostJson(string url, string data, bool compress = true)
        {
            using Stream stream = Send(url, "POST", data, compress);
            using MemoryStream ms = new();
            if (stream == null)
                return "";
            stream.CopyTo(ms);
            //return Encoding.UTF8.GetString(DecompressFile(ms.ToArray()));
            return SimpleZlib.Decompress(ms.ToArray(), null);
        }


    }
}
