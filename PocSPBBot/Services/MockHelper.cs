using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PocSPBBot.Services
{
    public class MockHelper
    {

        public static bool IsValidCardNumber(string cardNumber)
        {
            if (cardNumber.Equals("0000")  ||
                cardNumber.Equals("0001")  ||
                cardNumber.Equals("0002")  ||
                cardNumber.Equals("00022") ||
                cardNumber.Equals("0003"))
                
            {
                return true;
            }
            return false ;
        }
        public static bool IsCardWorking(string cardNumber)
        {
            if (cardNumber.Equals("0000"))
            {
                return false;
            }

            return true;
        }

        public static bool IsCardNoOverDraft(string cardNumber)
        {
            if (cardNumber.Equals("0001"))
            {
                return false;
            }

            return true;
        }

        public static bool IsWithdrawalCustomerMaxReach(string cardNumber, int amount)
        {

            if (cardNumber.Equals("0002"))
            {
                if (amount > 100)
                {
                    return true;
                }                
            }
            return false;
        }

        public static bool IsWithdrawalMaxThreshold(string cardNumber, int newThreshold)
        {
            if (cardNumber.Equals("0002"))
            {
                if (newThreshold > 150)
                {
                    return true;
                }
                
            }           
           return false;           
        }

        public static bool IsValidCity(string cardNumber, string city)
        {
            if (cardNumber.Equals("0002") &&
                city.ToLower().Equals("paris"))
            {
                return true;
            }
            return false;
        }
        public static bool IsValidBirthDay(string cardNumber, DateTime birthDate)
        {
            if (cardNumber.Equals("0002") &&
                birthDate.Equals(new DateTime(1964,12,16)))
            {
                return true;
            }
            return false;
        }
        public static bool IsValidPhoneNumber(string cardNumber, string phoneNumber)
        {
            // TODO : Regex
            if (cardNumber.Equals("0002") &&
                phoneNumber.ToLower().Equals("0664404479"))
            {
                return true;
            }
            return false;
        }

    }
}