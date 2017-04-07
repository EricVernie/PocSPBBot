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
using Microsoft.Bot.Connector;
using PocSPBBot.Common;
using PocSPBBot.Model;
using PocSPBBot.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Internals.Fibers;
using static Microsoft.Bot.Builder.Dialogs.PromptDialog;
using System.Threading;
using Microsoft.Bot.Builder.FormFlow;

namespace PocSPBBot.Dialogs
{
    
    public class DialogBase
    {

        
        static ResumeAfter<object> _resume;
        static PromptOptions<IssueDataModel> _options;
        public static  void DisplayContactYourAdvisor(IDialogContext context, ResumeAfter<object> resume)
        {

            context.PostAsync(Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_NO_INCREASE_CALL_CONS);

        }
        public static void PromptHowMuchMoney(IDialogContext context, ResumeAfter<object> resume)
        {
            context.Call(new WithdrawalDomesticDialog(), resume);
        }
        public static void PromptPaymentMode(IDialogContext context, ResumeAfter<object> resume)
        {

            List<IssueDataModel> listIssues = new List<IssueDataModel>
                {
                    new IssueDataModel { Issue = ISSUE.CARD, Title =  Resources.BOT_PROMPT_ISSUE_CARD },
                    new IssueDataModel { Issue = ISSUE.TRANSFER, Title =  Resources.BOT_PROMPT_ISSUE_TRANSFER},
                    new IssueDataModel { Issue = ISSUE.CHEQUE, Title =  Resources.BOT_PROMPT_ISSUE_CHEQUE }
                };


            DialogBase.PromptSimpleDialogWithConfirmation(context,
                                          Resources.BOT_PROMPT_ISSUE,
                                          Resources.BOT_PROMPT_TRY_AGAIN_CHOOSE_ISSUE,
                                          Resources.BOT_PROMPT_TOO_MANY_TRY,
                                          listIssues,
                                          resume);

        }
        public static void PromptWithdrawalOrPayment(IDialogContext context, ResumeAfter<object> resume)
        {
            List<IssueDataModel> listOperations = new List<IssueDataModel>
                {
                    new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL, Title =  Resources.BOT_PROMPT_CARD_ISSUE_WITHDRAWAL },
                    new IssueDataModel { Issue = ISSUE.CARD_PAYMENT, Title =  Resources.BOT_PROMPT_CARD_ISSUE_PAYMENT},
                };

            DialogBase.PromptSimpleDialogWithConfirmation(context,
                                 Resources.BOT_PROMPT_CARD_ISSUE,
                                 Resources.BOT_PROMPT_TRY_AGAIN_CHOOSE_ISSUE,
                                 Resources.BOT_PROMPT_TOO_MANY_TRY,
                                 listOperations,
                                 resume);

        }
        public static void PromptDomesticOrAbroad(IDialogContext context, ResumeAfter<object> resume)
        {
            List<IssueDataModel> listOperations = new List<IssueDataModel>
                    {
                        new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_DOMESTIC, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_DOMESTIC},
                        new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_ABROAD, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_ABROAD },
                    };


            DialogBase.PromptSimpleDialogWithConfirmation(context,
                                          Resources.BOT_PROMPT_ISSUE_WITHDRAWAL,
                                          Resources.BOT_PROMPT_TRY_AGAIN_CHOOSE_ISSUE,
                                          Resources.BOT_PROMPT_TOO_MANY_TRY,
                                          listOperations,
                                          resume);

        }

        public static void PromptSimpleDialogWithConfirmation(IDialogContext context, 
                                             string prompt,
                                             string retry,
                                             string tooMany,
                                             IEnumerable<IssueDataModel> options,
                                             ResumeAfter<object> resume)
        {
            _resume = resume;
            _options=new PromptOptions<IssueDataModel>(prompt, 
                                                       retry,
                                                       tooMany, 
                                                       options: options.ToList(), 
                                                       attempts: Constant.MaxRetry, 
                                                       promptStyler: new PromptStyler(PromptStyle.Auto));
            
            PromptSimpleDialog(context);
        }
        private static void PromptSimpleDialog(IDialogContext context)
        {
            var child = new PromptChoice<IssueDataModel>(_options);
            context.Call<IssueDataModel>(child, OnOperationSelected);
        }
        public static PromptChoice<IssueDataModel> PromptWithdrawalOrPayment2(IDialogContext context, ResumeAfter<object> resume)
        {
            List<IssueDataModel> listOperations = new List<IssueDataModel>
                {
                    new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL, Title =  Resources.BOT_PROMPT_CARD_ISSUE_WITHDRAWAL },
                    new IssueDataModel { Issue = ISSUE.CARD_PAYMENT, Title =  Resources.BOT_PROMPT_CARD_ISSUE_PAYMENT},
                };

            return DialogBase.PromptSimpleDialogWithConfirmation2(context,
                                 Resources.BOT_PROMPT_CARD_ISSUE,
                                 Resources.BOT_PROMPT_TRY_AGAIN_CHOOSE_ISSUE,
                                 Resources.BOT_PROMPT_TOO_MANY_TRY,
                                 listOperations,
                                 resume);

        }

        public static PromptChoice<IssueDataModel> PromptSimpleDialogWithConfirmation2(IDialogContext context,
                                            string prompt,
                                            string retry,
                                            string tooMany,
                                            IEnumerable<IssueDataModel> options,
                                            ResumeAfter<object> resume)
        {
            _resume = resume;
            var promptOptions = new PromptOptions<IssueDataModel>(prompt,
                                                       retry,
                                                       tooMany,
                                                       options: options.ToList(),
                                                       attempts: Constant.MaxRetry,
                                                       promptStyler: new PromptStyler(PromptStyle.Auto));

            return PromptSimpleDialog2(context, promptOptions);
        }
        private static PromptChoice<IssueDataModel> PromptSimpleDialog2(IDialogContext context, PromptOptions<IssueDataModel> options)
        {
            var child = new PromptChoice<IssueDataModel>(_options);
            //context.Call<IssueDataModel>(child, OnOperationSelected);
            return child;
        }

        static IssueDataModel _issueSelected = null;
        static public async Task OnOperationSelected(IDialogContext context, IAwaitable<IssueDataModel> result)
        {
            try
            {
               

                _issueSelected = await result;
                var child = new PromptConfirm(Resources.BOT_PROMPT_CONFIRM_CHOICE,
                                              Resources.BOT_PROMPT_CONFIRM_WRONG_CHOICE,
                                              Constant.MaxRetry, PromptStyle.Auto);
                context.Call(child, OnOperationConfirmed);
                
                
                
            }
            catch (TooManyAttemptsException)
            {
                context.Done(Resources.USER_DLG_BACK_TO_START);
            }
        }
       
        public static async Task ChooseDialogAsync(IDialogContext context, ISSUE issueSelected, ResumeAfter<object> resume)
        {
            switch (issueSelected)
            {
                case ISSUE.CARD:                                       
                    context.UserData.SetValue(ContextUserConstants.IssueType, ISSUE.CARD);
                    CheckCardDialog dlg = new CheckCardDialog();
                    context.Call(dlg, resume);                    
                    break;
                case ISSUE.TRANSFER:
                case ISSUE.CHEQUE:
                    context.UserData.SetValue(ContextUserConstants.IssueType, _issueSelected.Issue);
                    await context.PostAsync(Resources.USER_DLG_NOT_IMPLEMENTED);
                    context.Done("");
                    break;
                case ISSUE.CARD_WITHDRAWAL:
                    context.UserData.SetValue(ContextUserConstants.ProblemType, ISSUE.CARD_WITHDRAWAL);
                    context.Call(new WithdrawalDialog(), resume);
                    break;
                case ISSUE.CARD_PAYMENT:
                    context.Done(Resources.USER_DLG_NOT_IMPLEMENTED);
                    context.UserData.SetValue(ContextUserConstants.ProblemType, ISSUE.CARD_PAYMENT);
                    break;
                case ISSUE.CARD_WITHDRAWAL_DOMESTIC:
                    context.UserData.SetValue(ContextUserConstants.DomesticOrAbroad, ISSUE.CARD_WITHDRAWAL_DOMESTIC);
                    context.Call(new WithdrawalDomesticDialog(), resume);
                    break;
                case ISSUE.CARD_WITHDRAWAL_ABROAD:
                    context.Done("ABROAD PAS IMPLEMENTEE"); // Resources.USER_DLG_NOT_IMPLEMENTED);
                    context.UserData.SetValue(ContextUserConstants.DomesticOrAbroad, ISSUE.CARD_WITHDRAWAL_ABROAD);
                    break;
                case ISSUE.CARD_WITHDRAWAL_DOMESTIC_NO_INCREASE_THRESHOLD:
                    context.Done(Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_NO_THRESHOLD);
                    break;
                case ISSUE.CARD_WITHDRAWAL_DOMESTIC_YES_INCREASE_THRESHOLD:
                    context.Call(new WithdrawalDomecticNewThresholdDialog(), resume);
                    break;
                case ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_NO:
                case ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_YES:
                    context.Call(new WithdrawalDomesticOtherATMDialog(_issueSelected), _resume);
                    break;
                case ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_CONTACT_CONS:
                    context.Done(Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_OTHER_ATM_CALLING_CONS);
                    break;
                case ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_ORDER_NEW_CARD:
                    context.Call(new UserAuthenticationDialog(Resources.USER_DLG_NEW_CARD_ORDERED), resume);
                    break;
            }
        }
        private static async Task OnOperationConfirmed(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                if (await result == true)
                {
                    await ChooseDialogAsync(context, _issueSelected.Issue, _resume);                    
                }
                else
                {                    
                    PromptSimpleDialog(context);
                }
            }
            catch (TooManyAttemptsException)
            {
                //context.Done(Resources.BOT_PROMPT_TOO_MANY_TRY);
                await context.PostAsync(Resources.USER_DLG_BACK_TO_START);
                //context.Done("retry");
            }
            catch(Exception ex)
            {
                var a = ex.Message;
            }
        }
      
    }
}