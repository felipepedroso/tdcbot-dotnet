using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace TdcBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    await HandleMessageAsync(activity);
                    break;
                default:
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private async Task HandleMessageAsync(Activity activity)
        {
            await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
        }
    }
}