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


namespace PocSPBBot.Common
{
    public class Constant
    {
        public const int MaxRetry = 3;
    }
    public class ContextUserConstants
    {
        public const string IssueType = "IssueType";
        public const string ProblemType = "ProblemType";
        public const string DomesticOrAbroad = "DomesticOrAbroad";
        public const string WithdrawalThresholdExceeded = "ThresholdExceeded";
        public const string CardNumber = "CardNumber";
        public const string IsWelcome = "IsWelcome";
        
    }

    public class TextAnalysisConstants
    {
        public const string UserSentimentStateKey  = "UserSentimentStateKey";
        
    }
    public enum ISSUE
    {
        CARD = 1,
        TRANSFER = 2,
        CHEQUE = 3,
        CARD_WITHDRAWAL = 4, 
        CARD_PAYMENT = 5,
        CARD_WITHDRAWAL_ABROAD = 6,
        CARD_WITHDRAWAL_DOMESTIC = 7,
        CARD_WITHDRAWAL_DOMESTIC_NO_INCREASE_THRESHOLD = 8,
        CARD_WITHDRAWAL_DOMESTIC_YES_INCREASE_THRESHOLD = 9,
        CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_NO = 10,
        CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_YES = 11,
        CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_ORDER_NEW_CARD=12,
        CARD_WITHDRAWAL_DOMESTIC_OTHER_ATM_CONTACT_CONS = 13
    }

}