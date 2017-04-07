using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocSPBBot.Common
{
    public static class LuisResultExtensions
    {
        public static bool IsAbroad(this LuisResult result)
        {
            

            var query = from entity in result.Entities where entity.Type.Equals("WithdrawalLocalization::Abroad") select entity;
            var list = query.FirstOrDefault();

            return list != null ? true : false;
        
        }
        public static bool IsIssueDomesticWithDrawal(this LuisResult result)
        {


            var query = from entity in result.Entities where entity.Type.Equals("WithdrawalLocalization::Domestic") select entity;
            var list = query.FirstOrDefault();

            return list != null ? true : false;

        }
    }
}