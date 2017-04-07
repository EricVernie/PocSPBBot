using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using PocSPBBot.Common;
using PocSPBBot.Dialogs;
using PocSPBBot.Model;
using PocSPBBot.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PocSPBBot.CognitiveDialogs
{
    [Serializable]
    [LuisModel("42cc7c41-1b4a-4eaf-952d-a9875518d77d", "3cea665036ba4a5a92f0026694e70c7c")]
    public class RootLuisDialogFr : LuisDialog<object>
    {
       
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            //userState.Sentences.Add(result.Query);

            //await CheckInteraction.GetSentimentAsync(userState);
            //if (userState.Sentiment >= 0 && userState.Sentiment < 0.40)
            //{

            //}
            //else
            //{
            //    await context.PostAsync(Resources.COG_BOT_LUIS_NOT_UNDERSTAND_LEVEL_1);
            //}
            await context.PostAsync(Resources.Z_COG_BOT_LUIS_NOT_UNDERSTAND_LEVEL_2);
            //context.SaveUserState(userState);
            context.Wait(this.MessageReceived);
        }
        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {

            await context.PostAsync(Resources.USER_DLG_NOT_IMPLEMENTED);
            context.Wait(this.MessageReceived);
        }
        
        [LuisIntent("Issue")]
        public async Task Issue(IDialogContext context, LuisResult result)
        {
            if (result.Entities.Count == 0)
            {
                DialogBase.PromptPaymentMode (context,this.ResumeAfterCardIssue);
            }
            else
            {

                CheckCardDialog checkDialog = new CheckCardDialog();
                if (result.IsAbroad())
                {
                    await context.PostAsync("Pas encore implementée");
                }
                else if (result.IsIssueDomesticWithDrawal()) //j'ai un problème de retrait domestique
                {

                    checkDialog.MakeCheckCardFormDialog(DialogBase.PromptHowMuchMoney);
                    
                }                
                else if (CheckStatus.IsIssueTypeWithdrawal(result)) //J'ai un problème de retrait
                {
                    checkDialog.MakeCheckCardFormDialog(DialogBase.PromptDomesticOrAbroad);
                                        
                }
                else if               (CheckStatus.IsPayModeCard(result)) //J'ai un problème
                {
                    
                        checkDialog.MakeCheckCardFormDialog(DialogBase.PromptWithdrawalOrPayment);                                                                                                    
                }
                context.Call(checkDialog, ResumeAfterCardIssue);                
            }
            
        }
        [LuisIntent("Sentiment")]
        public async Task Sentiment(IDialogContext context, LuisResult result)
        {
            //Recupère le ton du client
            await context.PostAsync(Resources.Z_COG_BOT_LUIS_NOT_UNDERSTAND_LEVEL_2);
            //context.SaveUserState(userState);
            context.Wait(this.MessageReceived);
        }
        [LuisIntent("Salutation")]
        public async Task Salutation(IDialogContext context, LuisResult result)
        {
            var userState = new UserDataModel();
            
            await context.PostAsync(Resources.Z_COG_BOT_HOW_HELP_YOU);
            context.Wait(this.MessageReceived);
        }

      
        private async Task ResumeAfterCardIssue(IDialogContext context, IAwaitable<object> result)
        {

            var message = await result;
            await context.PostAsync(message.ToString());

            this.StartAsync(context);
        }

        private async Task ResumeAfterDomestic(IDialogContext context, IAwaitable<object> result)
        {

            var message = await result;
         
        }

    }
}