
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
using PocSPBBot.Model;
using PocSPBBot.Common;
using PocSPBBot.Resource;

namespace PocSPBBot.Dialogs
{
    [Serializable]
    public class WithdrawalDomesticOtherATMDialog : IDialog
    {
        IssueDataModel _issue;
        public WithdrawalDomesticOtherATMDialog(IssueDataModel issue)
        {
            _issue = issue;
        }
        public async Task StartAsync(IDialogContext context)
        {
            this.PromptIssues(context);

        }

        private void PromptIssues(IDialogContext context)
        {
            List<IssueDataModel> listOperations = new List<IssueDataModel>
                    {
                        new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_CONTACT_CONS, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_OTHER_ATM_CONTACT_CONS},
                        new IssueDataModel { Issue = ISSUE.CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_ORDER_NEW_CARD, Title =  Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_OTHER_ATM_ORDER_NEW_CARD },
                    };


            DialogBase.PromptSimpleDialogWithConfirmation(context,
                                          Resources.BOT_PROMPT_ISSUE_WITHDRAWAL_OTHER_ATM_CARD_KO,
                                          Resources.BOT_PROMPT_TRY_AGAIN_CHOOSE_ISSUE,
                                          Resources.BOT_PROMPT_TOO_MANY_TRY,
                                          listOperations,
                                          this.ResumeAfterOperationConfirmed);

        }
        private async Task ResumeAfterOperationConfirmed(IDialogContext context, IAwaitable<object> result)
        {

            var message = await result;
            context.Done(message);
        }



    }
}