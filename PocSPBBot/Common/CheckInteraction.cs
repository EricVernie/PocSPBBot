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

using EV.Cognitives.Services.TextAnalytics;
using Microsoft.Bot.Builder.Luis.Models;
using PocSPBBot.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PocSPBBot.Common
{
    public class CheckStatus
    {
        public static async Task GetIntentsAsync(UserDataModel userState)
        {

        }
        public static async Task GetSentimentAsync(UserDataModel userState)
        {
                     
            DetectedLanguageDataModel sentimentDetected = await TextEngine.Instance.DetectSentimentAsync(userState.Sentences, userState.Language);
            // 0 < 0,40 : Angry+
            // 0,40> Angry- <0,53 
            // 0,53 > Neutral < 0,65
            // 0,65 > Happy
            if (sentimentDetected.documents.Count == 0)
            {
                // OK I assume neutral sentiment
                userState.Sentiment = 0.53;
            }
            else
            {
                //Get the last sentiment
                int index = sentimentDetected.documents.Count - 1;
                Document doc = sentimentDetected.documents[index];
                userState.Sentiment = doc.score;
            }
            Log.Write(sentimentDetected);
        }

       public static async Task<UserDataModel> GetLanguageAsync(string text)
        {

            string currentLanguage = null;
            List<string> texts = new List<string>();
            texts.Add(text);
            ResultAnalyzeDataModel languageDetected = await TextEngine.Instance.DetectLanguageAsync(texts, 1);            
            // First check if no error
            if (languageDetected.errors.Count > 0)
            {
                // Log the message
                Log.Write(languageDetected.errors[0].message);
            }
            if (languageDetected.documents.Count > 0)
            {
                // Select with a language with a score equal 1
                var query = from language in languageDetected.documents[0].detectedLanguages where language.score > 0.9 select language.iso6391Name;
                currentLanguage = query.FirstOrDefault();
            }
            return new UserDataModel { Language = currentLanguage != null ? currentLanguage : "fr" , Sentences = texts };
        }

        public static  bool IsIssueTypeWithdrawal(LuisResult result)
        {

            var query = from entity in result.Entities where entity.Type.Equals("IssueType::Withdrawal") select entity;
            var list = query.FirstOrDefault();

            return list != null ? true : false;
        }
      
        public static bool IsBlockedCard(LuisResult result)
        {

            var query = from entity in result.Entities where entity.Type.Equals("IssueCard::BlockCard") select entity;
            var list = query.FirstOrDefault();

            return list != null ? true : false;
        }
        public static bool IsPayModeCard(LuisResult result)
        {

            var query = from entity in result.Entities where entity.Type.Equals("PayMode::Card") select entity;
            var list = query.FirstOrDefault();

            return list != null ? true : false;
        }
    }
}