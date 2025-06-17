using PayuniSDK.Models;
using PayuniSDK.APIs;
using PayuniSDK.Enums;
using System;

namespace example.examples.trade
{
    public class Trade
    {
        /// <summary>
        /// api宣告
        /// </summary>
        public PayuniAPI payuniapi;
        /// <summary>
        /// 加密資訊宣告
        /// </summary>
        public EncryptInfoModel info;

        public Trade(string request)
        {
            string key = "12345678901234567890123456789012";
            string iv = "1234567890123456";
            payuniapi = new PayuniAPI(key, iv);
        }

        /// <summary>
        /// trade query sample code
        /// </summary>
        public void tradeQuery()
        {
            info = new EncryptInfoModel();
            info.MerID = "abc";
            info.MerTradeNo = "test20220829111528";
            info.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            string result = payuniapi.UniversalTrade(info, TradeType.TradeQuery);
        }

        /// <summary>
        /// trade close sample code
        /// </summary>
        public void tradeClose()
        {
            info = new EncryptInfoModel();
            info.MerID = "abc";
            info.MerTradeNo = "16614190477810373246";
            info.CloseType = "1";
            info.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            string result = payuniapi.UniversalTrade(info, TradeType.TradeClose);
        }

        /// <summary>
        /// trade cancel sample code
        /// </summary>
        public void tradeCancel()
        {
            info = new EncryptInfoModel();
            info.MerID = "abc";
            info.MerTradeNo = "16614190477810373246";
            info.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            string result = payuniapi.UniversalTrade(info, TradeType.TradeCancel);
        }

        /// <summary>
        /// trade refund icash sample code
        /// </summary>
        public void tradeRefundIcash()
        {
            info = new EncryptInfoModel();
            info.MerID = "abc";
            info.MerTradeNo = "1665472985627866043";
            info.TradeAmt = "100";
            info.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            string result = payuniapi.UniversalTrade(info, TradeType.TradeRefundIcash);
        }

        /// <summary>
        /// trade refund aftee sample code
        /// </summary>
        public void tradeRefundAftee()
        {
            info = new EncryptInfoModel();
            info.MerID = "abc";
            info.TradeNo = "1665472985627866043";
            info.TradeAmt = "100";
            info.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            string result = payuniapi.UniversalTrade(info, TradeType.TradeRefundAftee);
        }
    }
}
