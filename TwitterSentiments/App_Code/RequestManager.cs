using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using CoreTweet;
using System.Text;
using System;

namespace TwitterSentiments.App_Start
{
    public class RequestManager
    {
        // API properties
        private string url = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";
        private string key = "1e3e99ee674b4a0c930aaca327eea231";
        private string contentType = "application/json";

        public RequestManager() { }

        public string MakeRequest(List<Status> tweets)
        {
            // Create a new client and add the necessary headers
            var client = new WebClient();
            client.Headers["Ocp-Apim-Subscription-Key"] = key;
            client.Headers["Content-Type"] = contentType;

            // JsonManager is responsible for Json formatting the request body.
            JsonManager json = new JsonManager(tweets);
            var jsonFormattedBody = json.FormatDocument();

            try
            {
                // Make request, return response.
                return client.UploadString(url, jsonFormattedBody);
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

    // Scope issues are created when this class is put in /App_Code, even when auto-generating 
    // using Visual Studio intellisense, a possible bug?
    internal class JsonManager
    {
        public string documents { get; set; } = "";

        // Default document structure
        private string rootBeginning = "{ \"documents\": [";
        private string rootEnding = "] }";

        // Individual document structure
        private string jsonPrefix = "{\"language\": \"en\", \"id\": \"";
        private string jsonMid = "\", \"text\": \"";
        private string jsonSuffix = "\"}";

        private List<Status> tweets;

        public JsonManager(List<Status> tweets)
        {
            this.tweets = tweets;
        }

        public string FormatDocument()
        {
            // Add each status as a Json document 
            for (int i = 0; i < tweets.Count; i++)
            {
                documents += AddDocument(tweets[i].Text, i);

                // Append a comma to each document if it is not the last.
                documents += i < tweets.Count ? ", " : String.Empty;
            }

            // Return documents wrapped in root Json structure
            return rootBeginning + documents + rootEnding;
        }

        public string AddDocument(string text, int id)
        {
            // Strip quotation marks
            text = text.Replace("\"", "");

            // Convert to ASCII for API request
            text = Encoding.ASCII.GetString(Encoding.UTF8.GetBytes(text));

            // Insert the correct document ID and text
            return jsonPrefix + id.ToString() + jsonMid + text + jsonSuffix;
        }
    }
}