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
using PocSPBBot.Resource;
using Microsoft.Bot.Builder.FormFlow;
using PocSPBBot.Common;
using PocSPBBot.Services;

namespace PocSPBBot.Dialogs
{
    [Serializable]
    public class UserAuthenticationDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync(Resources.USER_DLG_AUTHENTICATION);
            var checkUserAuthenticationDialog = FormDialog.FromForm(BuildCheckAuthenticationForm, FormOptions.PromptInStart);
            context.Call(checkUserAuthenticationDialog, ResumeAfterBuildAuthenticationFormDialog);
        }
        string _endMessage = null;
        public UserAuthenticationDialog(string endMessage)
        {
            _endMessage = endMessage;
        }
        private IForm<UserAuthenticationQuery> BuildCheckAuthenticationForm()
        {
            OnCompletionAsyncDelegate<UserAuthenticationQuery> processCheckingWithdrawal = async (context, state) =>
            {
                await context.PostAsync(Resources.USER_DLG_CHECKING_WITHDRAWAL);
            };

            return new FormBuilder<UserAuthenticationQuery>()
                .Field(nameof(UserAuthenticationQuery.City))
                .Message("...")
                .AddRemainingFields()
                .OnCompletion(processCheckingWithdrawal)
                .Build();
        }
        private async Task ResumeAfterBuildAuthenticationFormDialog(IDialogContext context, IAwaitable<UserAuthenticationQuery> result)
        {
            var claims = await result;
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

            if (!MockHelper.IsValidCity(cardNumber, claims.City) ||
                !MockHelper.IsValidPhoneNumber(cardNumber, claims.PhoneNumber) ||
                !MockHelper.IsValidBirthDay(cardNumber, claims.BirthDate))
            {
                //Retry
                this.StartAsync(context);                

            }
            else
            {
                context.Done(_endMessage);
            }

            
        }
    }
}