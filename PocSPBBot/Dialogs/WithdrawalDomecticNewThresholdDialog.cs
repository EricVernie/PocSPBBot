
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
using Microsoft.Bot.Builder.FormFlow;
using PocSPBBot.Common;
using PocSPBBot.Resource;
using PocSPBBot.Services;
using System;
using System.Threading.Tasks;

namespace PocSPBBot.Dialogs
{
    [Serializable]
    public class WithdrawalDomecticNewThresholdDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var checkWithdrawalFormDialog = FormDialog.FromForm(BuildCheckWithdrawalForm, FormOptions.PromptInStart);
            context.Call(checkWithdrawalFormDialog, ResumeAfterBuildCheckWithdrawalFormDialog);
        }

        private async Task ResumeAfterBuildCheckWithdrawalFormDialog(IDialogContext context,
            IAwaitable<WithdrawalDomecticQuery> result)
        {
            var withdrawal = await result;
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
            if (MockHelper.IsWithdrawalMaxThreshold(cardNumber, withdrawal.NewThreshold))
            {
                // Retry 
                await context.PostAsync(Resources.USER_DLG_THRESHOLD_TO_HIGH);
                this.StartAsync(context);
                return;
            }

            context.Call(new UserAuthenticationDialog(Resources.USER_DLG_NEW_THRESHOLD_ACCEPTED), ResumeAfterAuthenticationAsync);

        }

        private async Task ResumeAfterAuthenticationAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
            context.Done(message);
        }

        private IForm<WithdrawalDomecticQuery> BuildCheckWithdrawalForm()
        {
            OnCompletionAsyncDelegate<WithdrawalDomecticQuery> processCheckingWithdrawalNewThreshold = async (context, state) =>
            {
                await context.PostAsync(Resources.USER_DLG_CHECKING_WITHDRAWAL);
            };

            return new FormBuilder<WithdrawalDomecticQuery>()
                .Field(nameof(WithdrawalDomecticQuery.NewThreshold))
                .OnCompletion(processCheckingWithdrawalNewThreshold)
                .Build();
        }
    }
}