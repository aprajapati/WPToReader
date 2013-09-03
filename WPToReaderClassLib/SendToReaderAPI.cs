using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace WPToReaderClassLib
{
    public class SendToReaderAPI
    {
        static string strCheckLogin = "https://sendtoreader.com/api/checklogin/";
        static string strSend = "https://sendtoreader.com/api/send/";

        // Async post Message. 
        // TODO: Localize this messages for Global audience. 
        public static async Task<String> postMessage(string url, string taskName)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage resp = await client.GetAsync(url);

            string message = "Operation successful!";
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                Dictionary<int, String> map = new Dictionary<int, String>();
                map[401] = "Exceeded the rate limit. Please wait 10 seconds and try again.";
                map[403] = "Invalid username or password.";
                map[404] = "Not Found";
                map[405] = "Error: Sendtoreader is not able to process this URL.";
                map[406] = "Error: Specified URL is not corrrect.";
                map[424] = "Internal Sendtoreader error. Please contact support for investigation.";
                map[427] = "Sendtoreader can’t send document to the user. Maybe wrong or no kindle email in this user’s settings.";
                map[500] = "The service encountered an error. Please try again later.";

                if (map.ContainsKey((int)resp.StatusCode))
                    message = map[(int)resp.StatusCode];
                else
                    message = "ConnectionError";
            }

            return message;
        }

        public static async Task<String> checkCreds(string userName, string password)
        {
            string url = strCheckLogin;
            url += "?username=";
            url += userName;
            url += "&password=";
            url += password;

            String message = await postMessage(url, "Settings");

            return message;
        }

        public static async Task<String> sendDoc(string userName, string password, string surl, string author, string title, string text)
        {
            string url = strSend;
            url += "?username=";
            url += userName;
            url += "&password=";
            url += password;
            url += "&author=";
            url += author;
            url += "&title=";
            url += title;
            if (surl != "")
            {
                url += "&url=";
                url += surl;
            }
            else
            {
                url += "&text=";
                url += text;
            }


#if DEBUG_AGENT
            String message = "Not Found:" + url;
#else

            String message = await postMessage(url, "Send Document");
            if (message == "Not Found")
            {
                // Append url to status message
                message += ":url=" + url;
                // Chcek if background task is enabled.
                //if (backgroundTaskEnabled)
                //{
                //    // Add task to pending list
                //    message = "Added to pending list due to lack of connectivity";
                //}
                //else
                //{
                //    message = "Couldn't connect to sendtoreader.com and background task is disabled for this applicaiton so please try again later";
                //}
            }
#endif
            return message;
        }


    }
}
