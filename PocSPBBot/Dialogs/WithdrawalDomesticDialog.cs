
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
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using PocSPBBot.Resource;
using PocSPBBot.Services;
using PocSPBBot.Common;
using PocSPBBot.Model;

namespace PocSPBBot.Dialogs
{
    [Serializable]
    public class WithdrawalDomesticDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var checkWithdrawalFormDialog = FormDialog.FromForm(BuildCheckWithdrawalForm, FormOptions.PromptInStart);
            context.Call(checkWithdrawalFormDialog, ResumeAfterBuildCheckWithdrawalFormDialog);
        }
        private IForm<WithdrawalDomecticQuery> BuildCheckWithdrawalForm()
        {
            OnCompletionAsyncDelegate<WithdrawalDomecticQuery> processCheckingWithdrawal = async (context, state) =>
            {
                await context.PostAsync(Resources.USER_DLG_CHECKING_WITHDRAWAL);
            };

            return new FormBuilder<WithdrawalDomecticQuery>()
                .Field(nameof(WithdrawalDomecticQuery.Amount))
                .OnCompletion(processCheckingWithdrawal)
                .Build();
        }
        private async Task ResumeAfterBuildCheckWithdrawalFormDialog(IDialogContext context, IAwaitable<WithdrawalDomecticQuery> result)
        {
            var withdrawal = await result;
            // Get the current card Number
            string cardNumber = String.Empty;
            if (!context.UserData.TryGetValue(ContextUserConstants.CardNumber, out cardNumber))
            {
                context.Done(Resources.ERROR_DEBUG_SOMETHING_WRONG_WITH_CARD);
                return;
            }
            if (cardNumber == null)
            {
                context.Done(Resources.ERROR_DEBUG_SOMETHING_WRONG_WITH_CARD);
            }

            if (MockHelper.IsWithdrawalCustomerMaxReach(cardNumber, withdrawal.Amount))
            {
                context.UserData.SetValue(ContextUserConstants.WithdrawalThresholdExceeded, true);
                this.PromptIssuesWithdrawalExceededThreshold(context);
            }    
            else
            {
                context.UserData.SetValue(ContextUserConstants.WithdrawalThresholdExceeded, false);
                this.PromptIssuesWithdrawalNotExceededThreshold(context);
            }        
       }

        private void PromptIssuesWithdrawalNotExceededThreshold(IDialogContext context)
        {
            List<IssueDataModel> listOperations = new List<IssueDataModel>
                {
                    new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_YES, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_OTHER_ATM_YES },
                    new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_NO, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_OTHER_ATM_NO},
                };


            DialogBase.PromptSimpleDialogWithConfirmation(context,
                                         Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_OTHER_ATM,
                                         Resources.BOT_PROMPT_TRY_AGAIN_CHOOSE_ISSUE,
                                         Resources.BOT_PROMPT_TOO_MANY_TRY,
                                         listOperations,
                                         this.ResumeAfterOperationConfirmed);


        }

        private void PromptIssuesWithdrawalExceededThreshold(IDialogContext context)
        {
            List<IssueDataModel> listOperations = new List<IssueDataModel>
                {
                    new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_DOMESTIC_YES_INCREASE_THRESHOLD, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_YES_INCREASE },
                    new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_DOMESTIC_NO_INCREASE_THRESHOLD, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_NO_INCREASE_CALL_CONS},
                };
        

            DialogBase.PromptSimpleDialogWithConfirmation(context,
                                         Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_YESNO,
                                         Resources.BOT_PROMPT_TRY_AGAIN_CHOOSE_ISSUE,
                                         Resources.BOT_PROMPT_TOO_MANY_TRY,
                                         listOperations,
                                         this.ResumeAfterOperationConfirmed);

        }
        private async Task ResumeAfterOperationConfirmed(IDialogContext context, IAwaitable<object> result)
        {
            if (result == null ) //retry
            {
                bool thresholdExceeded;
                if (context.UserData.TryGetValue(ContextUserConstants.WithdrawalThresholdExceeded,out thresholdExceeded))
                {
                    if (thresholdExceeded )
                    {
                        this.PromptIssuesWithdrawalExceededThreshold(context);
                    }
                    else
                    {
                        this.PromptIssuesWithdrawalNotExceededThreshold(context);
                    }
                }
                else
                {
                    context.Done(Resources.ERROR_DEBUG_SOMETHING_WRONG_WITH_CARD);
                }
                return;
            }
            
            var message = await result;
            context.Done(message);
        }
    }
}