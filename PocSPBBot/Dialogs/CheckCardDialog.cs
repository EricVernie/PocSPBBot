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
using PocSPBBot.Resource;
using Microsoft.Bot.Builder.FormFlow;
using PocSPBBot.Services;
using PocSPBBot.Model;
using PocSPBBot.Common;

namespace PocSPBBot.Dialogs
{
    [Serializable]
    public class CheckCardDialog : IDialog<object>

    {
        Action<IDialogContext, ResumeAfter<object>> _action = null;
        public void MakeCheckCardFormDialog(Action<IDialogContext, ResumeAfter<object>> action)
        {
            _action = action;
        }
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Resources.DBG_CARD_RESOLVER);
            var checkCardFormDialog = FormDialog.FromForm(BuildCheckCardNumberForm, FormOptions.PromptInStart);
            context.Call(checkCardFormDialog, ResumeAfterBuildCheckCardNumberFormDialog);
        }
        private IForm<CardQuery> BuildCheckCardNumberForm()
        {
            OnCompletionAsyncDelegate<CardQuery> processCheckingCard = async (context, state) =>
            {
                await context.PostAsync(Resources.USER_DLG_CHECKING_CARD_NUMBER);
            };

            return new FormBuilder<CardQuery>()
                .Field(nameof(CardQuery.CardNumber))                                
                .OnCompletion(processCheckingCard)
                .Build();
        }
        private async Task ResumeAfterBuildCheckCardNumberFormDialog(IDialogContext context, IAwaitable<CardQuery> result)
        {
            var card = await result;
            // For simulation purpose
            ////await Task.Delay(1000);
            if (!MockHelper.IsValidCardNumber(card.CardNumber))
            {
                context.Done(Resources.USER_DLG_CHECKING_CARD_KO);
                return;
            }

            if (!MockHelper.IsCardWorking(card.CardNumber))
            {
                context.Done(Resources.USER_DLG_CHECKING_CARD_BLOCKED);
                return;
            }

            if (!MockHelper.IsCardNoOverDraft(card.CardNumber))
            {
                context.Done(Resources.USER_DLG_CHECKING_CARD_OVERDRAFT);
                return;
            }
            // Keep the card number for the current conversation            
            context.UserData.SetValue(ContextUserConstants.CardNumber, card.CardNumber);
            if (_action == null)
            {
                DialogBase.PromptWithdrawalOrPayment(context, ResumeAfterOperationConfirmed);
            }
            else
            {
                _action(context, ResumeAfterOperationConfirmed);
            }
            
            
        }
     
        private async Task ResumeAfterOperationConfirmed(IDialogContext context, IAwaitable<object> result)
        {
                       
            var message = await result;
            context.Done(message);
        }

    }    
}