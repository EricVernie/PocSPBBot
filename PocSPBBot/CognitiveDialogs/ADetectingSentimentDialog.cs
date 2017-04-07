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

using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using EV.Cognitives.Services.TextAnalytics;
using PocSPBBot.Common;
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using System.Threading;
using PocSPBBot.Resource;
using PocSPBBot.Model;

namespace PocSPBBot.CognitiveDialogs
{
    [Serializable]
    public class ADetectingSentimentDialog : IDialog
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }
        public ADetectingSentimentDialog()
        {
         
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            var text = message.Text;
            // First detect the language for this conversation
            

            var userState = await CheckStatus.GetLanguageAsync(text);
            
            Helper.PushCultureForCurrentConversation(userState.Language);

            // Detect sentiment if any
            await CheckStatus.GetSentimentAsync(userState);

            

            // Save the user's state
            context.SaveUserState(userState);

            await context.PostAsync(Resources.Z_COG_BOT_WELCOME);
            
            

            await context.PostAsync(Resources.Z_COG_BOT_HOW_HELP_YOU);
            context.Wait(MessageReceivedAsync);
        }

    
        

    }
}