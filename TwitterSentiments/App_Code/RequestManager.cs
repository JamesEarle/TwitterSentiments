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

        public RequestManager() { }

/* 
{
    "documents": [{ "language": "en",
                    "id": "something",
                    "text": "here is some happy text!" }]
}
*/

        public string MakeRequest(string text)
        {
            var c = new WebClient();

            c.Headers["Ocp-Apim-Subscription-Key"] = key;
            c.Headers["Content-Type"] = contentType;

            // Strip quotation marks from input text to maintain valid JSON format.
            text = text.Replace("\"", "");
            text = text.Replace("\u00A0", " "); // "NO BREAK SPACE
            text = text.Replace("\u2013", "-"); // EN DASH

            // Request only accepts ASCII.
            text = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(text));

            // Should consider changing this to be expandable.
            string jsonFormattedBody = "{ \"documents\": [{\"language\": \"en\", \"id\": \"input\", \"text\": \"" + text + "\"}] }";

            Debug.WriteLine(text);

            char[] test = text.ToCharArray();

            try
            {
                return c.UploadString(url, jsonFormattedBody);
            }
            catch (WebException e)
            {
                // Some special characters cause the request to return 400

                Debug.WriteLine(e.Status);
                Debug.WriteLine(e.Message);

                return null;
            }
        }
    }
}