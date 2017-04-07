// ******************************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
//
// ******************************************************************

using EV.Cognitives.Services.TextAnalytics;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using PocSPBBot.CognitiveDialogs;
using PocSPBBot.Common;
using PocSPBBot.Resource;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PocSPBBot.Controllers
{
    public class CognitiveController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                
               // await Conversation.SendAsync(activity, () => new ADetectingSentimentDialog());

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                if (activity.Text.Equals("reset"))
                {

                    await connector.Conversations.SendToConversationAsync(activity, activity.Conversation.Id);
                }


                // TODO : Test the starting language for the current conversation
                var userState = await CheckStatus.GetLanguageAsync(activity.Text);
                
                if (userState.Language.Equals("fr"))
                {
                    Helper.PushCultureForCurrentConversation(userState.Language);
                    
                    await Conversation.SendAsync(activity, () => new RootLuisDialogFr());
                }
                else if(userState.Language.Equals("en"))
                {
                    Helper.PushCultureForCurrentConversation("en");
                    Activity reply = activity.CreateReply(Resources.USER_DLG_NOT_IMPLEMENTED);                    
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else //other language
                {
                    Helper.PushCultureForCurrentConversation("en");
                    Activity reply = activity.CreateReply(Resources.Z_COG_BOT_LANGUAGE_NOT_SUPPORTED);
                    
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                

            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
