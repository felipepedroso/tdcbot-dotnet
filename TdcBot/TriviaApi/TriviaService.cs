using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace TriviaApi
{
    public class TriviaService
    {
        public static async Task<Trivia> GetTrivia()
        {
            string url = "https://opentdb.com/api.php?amount=1&type=multiple";

            string triviaJsonRaw;

            using (WebClient webClient = new WebClient())
            {
                triviaJsonRaw = await webClient.DownloadStringTaskAsync(url).ConfigureAwait(false);
            }

            var serializer = new JavaScriptSerializer();

            Results results = serializer.Deserialize<Results>(triviaJsonRaw);

            return results.results[0];
        }
    }
}