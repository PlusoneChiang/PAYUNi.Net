using payuniSDK;
using System;
using static payuniSDK.PayuniAPI;

namespace example.examples.cardit_bind
{
    public class CardBind
    {        
        /// <summary>
        /// api宣告
        /// </summary>
        public PayuniAPI payuniapi;
        /// <summary>
        /// 加密資訊宣告
        /// </summary>
        public EncryptInfoModel info;
        public CardBind(string request) {
             string key = "12345678901234567890123456789012";
             string iv = "1234567890123456";
             payuniapi = new PayuniAPI(key, iv);
        }
        /// <summary>
        /// trade bind query sample code
        /// </summary>
        public void tradeBindQuery()
        {
            info = new EncryptInfoModel();
            info.MerID = "abc";
            info.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string result = payuniapi.UniversalTrade(info, TradeType.CreditBindQuery);
        }
        /// <summary>
        /// treade bind cancel sample code
        /// </summary>
        public void tradeBindCancel()
        {
            info = new EncryptInfoModel();
            info.MerID = "abc";
            info.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            info.UseTokenType = "1";
            info.BindVal = "1";
            string result = payuniapi.UniversalTrade(info, TradeType.CreditBindCancel);
        }
    }

    
}
