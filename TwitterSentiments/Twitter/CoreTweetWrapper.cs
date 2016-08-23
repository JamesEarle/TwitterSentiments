using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CoreTweet;
using CoreTweet.Core;

namespace TwitterSentiments.App_Start
{
    public class CoreTweetWrapper
    {
        // API information
        private string consumerKey = "KKkdkUnUX2HikXO0NbxwNK4A1";
        private string accessToken = "2789039840-7EWx1MKjSBNCvUPTxsb2hlTYwm6ZqpLcOYsU1IF";

        // Connection tokens object
        public Tokens tokens; 

        public CoreTweetWrapper()
        {
            // Use OAuth to open an authenticated session and make requests
            tokens = Tokens.Create(consumerKey, "8GSJ0HNkPaLJ89jqwYMFgRj015gdSBhscQ46xY6grs8FD9PQXm", accessToken, "WrcRG29RNb4U0bvCpW85E2L0jlmRSKWyIbjzPldxfMHEC", screenName: "JamesMSP");
        }

        // Return the text of the most recent status, given the user name
        public string GetUserMostRecentStatus(string ScreenName)
        {
            return tokens.Users.Show(new { screen_name = ScreenName }).Status.Text;
        }
        
        // Return the n most recent statuses, provided the user name and number of statuses wanted
        public ListedResponse<Status> GetUserTimeline(string ScreenName, int Count)
        {
             return tokens.Users.IncludedTokens.Statuses.UserTimeline(new { screen_name = ScreenName, count = Count });
        }
    }
}