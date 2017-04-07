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

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace PocSPBBot
{
    internal class LocalizationResourceHelper
    {
        static ResourceManager _resourcesManager;
        static LocalizationResourceHelper()
        {
            var assembly = typeof(LocalizationResourceHelper).Assembly;
             _resourcesManager = new ResourceManager("PocSPBBot.Resource", assembly);

        }
        
        private static string GetString([CallerMemberName] String resourceName = null)
        {
            string message=string.Empty;
            try
            {
                message= _resourcesManager.GetString(resourceName);
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }
                

            return message;
        }
        public static String BOT_WELCOME
        {
            get { return GetString(); }
        }
    }
   
}