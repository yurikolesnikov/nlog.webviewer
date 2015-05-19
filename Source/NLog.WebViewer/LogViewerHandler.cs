using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Web;

namespace NLog.WebViewer
{
    public class LogViewerHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var response = context.Response;
            var request = context.Request;

            if (request.Url.AbsoluteUri.EndsWith("logs.js"))
            {
                response.ContentType = "text/javascript";
                response.Write(GetResource("NLog.WebViewer.scripts.logs.js"));
                return;
            }

            if (request.Url.AbsoluteUri.EndsWith("main.css"))
            {
                response.ContentType = "text/css";
                response.Write(GetResource("NLog.WebViewer.content.main.css"));
                return;
            }

            LogModel model;

            DateTime date;
            var dateString = request.Form["date"] ?? request.QueryString["date"];

            if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                date = DateTime.Today;
            }

            if (request.HttpMethod == "POST")
            {
                int lastId;
                int.TryParse(request.Form["lastId"], out lastId);

                model = CreateLogModel(context, date, null, lastId);

                response.Write(JsonSerialize(model));
                
                return;
            }

            model = CreateLogModel(context, date, null);
            var html = GetResource("NLog.WebViewer.Logs.html");

            html = html.Replace("{toolbar}", model.GetHtmlToolbar(request));
            html = html.Replace("{table-header}", model.GetHtmlTableHeader());
            html = html.Replace("<script></script>", string.Format("<script>NL.lastId = {0};</script>", model.LastId));
            html = html.Replace("{table-rows}", model.Html);

            response.Write(html);
        }

        public LogModel CreateLogModel(HttpContext context, DateTime? date, string level, int minId = 0)
        {
            date = date ?? DateTime.Today;

            string logsPath = Config.Instance.Path.StartsWith("~") ? context.Server.MapPath(Config.Instance.Path) : Config.Instance.Path;

            return new LogModel(logsPath, date.Value, minId);
        }

        /// <summary>
        /// Serialize object to string in JSON format.
        /// </summary>
        private static string JsonSerialize<T>(T obj)
        {
            var stream = new MemoryStream();
            ////var settings = new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ssZ") };
            var serizlizer = new DataContractJsonSerializer(typeof(T));
            serizlizer.WriteObject(stream, obj);
            stream.Position = 0;
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Loads text resource from application resources.
        /// </summary>
        private static string GetResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            var result = string.Empty;

            if (stream != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}