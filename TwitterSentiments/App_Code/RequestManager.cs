using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;

namespace TwitterSentiments.Controllers
{
    internal class RequestManager
    {
        // API Key
        private string url = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        private string key = "1e3e99ee674b4a0c930aaca327eea231";
        private string contentType = "application/json";

        // To be wrapped around input text
        private string jsonPrefix = "{ \"documents\": [{\"language\": \"en\", \"id\": \"input\", \"text\": \"";
        private string jsonSuffix = "\"}] }";

        public RequestManager() { }

        public string MakeRequest(string text)
        {
            // Create a new client and give it the appropriate headers
            var c = new WebClient();
            c.Headers["Ocp-Apim-Subscription-Key"] = key;
            c.Headers["Content-Type"] = contentType;

            // Strip quotation marks from input text to maintain valid JSON format.
            text = text.Replace("\"", "");

            // Request only accepts ASCII.
            text = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(text));

            // Should consider changing this to be expandable.
            string jsonFormattedBody = jsonPrefix + text + jsonSuffix;

            // Uncomment to debug.
            // Debug.WriteLine(text);
            // char[] test = text.ToCharArray();

            try
            {
                return c.UploadString(url, jsonFormattedBody);
            }
            catch (WebException e)
            {
                // Bad request or API quota reached
                Debug.WriteLine(e.Status);
                Debug.WriteLine(e.Message);

                return null;
            }
        }
    }
}