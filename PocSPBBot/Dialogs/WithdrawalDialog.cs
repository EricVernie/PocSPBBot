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
using PocSPBBot.Common;
using PocSPBBot.Model;

namespace PocSPBBot.Dialogs
{
    [Serializable]
    public class WithdrawalDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            DialogBase.PromptDomesticOrAbroad(context, ResumeAfterOperationConfirmed);

        }        
        private async Task ResumeAfterOperationConfirmed(IDialogContext context, IAwaitable<object> result)
        {
          
            var message = await result;
            context.Done(message);
        }

    }
}