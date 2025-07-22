namespace PAYUNiSDK.Models;

public class EncryptInfoResponseModel : SDKBaseModel
{
    // 共通欄位
    public string Status { get; set; }
    public string Message { get; set; }
    public string MerID { get; set; }
    public string MerTradeNo { get; set; }
    public string TradeNo { get; set; }
    public string TradeAmt { get; set; }
    public string TradeStatus { get; set; }
    public string PaymentType { get; set; }
    public string Gateway { get; set; }
    public string BuyerHash { get; set; }

    // Credit card (paymentType = 1)
    public string Card6No { get; set; }
    public string Card4No { get; set; }
    public string CardInst { get; set; }
    public string FirstAmt { get; set; }
    public string EachAmt { get; set; }
    public string ResCode { get; set; }
    public string ResCodeMsg { get; set; }
    public string AuthCode { get; set; }
    public string AuthBank { get; set; }
    public string AuthBankName { get; set; }
    public string AuthType { get; set; }
    public string AuthDay { get; set; }
    public string AuthTime { get; set; }
    public string CreditHash { get; set; }
    public string CreditLife { get; set; }
    public string CardBank { get; set; }

    // ATM (paymentType = 2)
    public string BankType { get; set; }
    public string PayNo { get; set; }
    public string PaySet { get; set; }
    public string ExpireDate { get; set; }

    // CVS (paymentType = 3)
    public string Store { get; set; }
    // public string PayNo { get; set; }
    // public string ExpireDate { get; set; }

    // 貨到付款 (paymentType = 5)
    public string PartnerId { get; set; }
    public string ShipTradeNo { get; set; }
    public string GoodsType { get; set; }
    public string LgsType { get; set; }
    public string ShipType { get; set; }
    public string ServiceType { get; set; }
    public string ShipAmt { get; set; }
    public string StoreID { get; set; }
    public string StoreName { get; set; }
    public string StoreAddr { get; set; }
    public string Consignee { get; set; }
    public string ConsigneeMobile { get; set; }
    public string ConsigneeMail { get; set; }

    // ICash (paymentType = 6)
    // public string PayNo { get; set; }
    public string PayTime { get; set; }

    // Aftee (paymentType = 7)
    // public string PayNo { get; set; }
    public string CreateDT { get; set; }

    // Line Pay (paymentType = 9)
    // public string PayNo { get; set; }
    // public string PayTime { get; set; }

    // 宅配到付 (paymentType = 10)
    public string TradeType { get; set; }
    // public string ShipTradeNo { get; set; }
    // public string GoodsType { get; set; }
    // public string LgsType { get; set; }
    // public string ShipType { get; set; }
    // public string ServiceType { get; set; }
    // public string ShipAmt { get; set; }
    // public string Consignee { get; set; }
    // public string ConsigneeMobile { get; set; }
    public string ConsigneeTel { get; set; }
    public string ConsigneeAddress { get; set; }
    public string DeliveryTimeTag { get; set; }
    public string ProductTypeId { get; set; }
    public string ProdDesc { get; set; }

    // JKoPay (paymentType = 11)
    public string JKoTradeNo { get; set; }
    public string JKoStrCupAmt { get; set; }
    public string JKoChannel { get; set; }
    // public string PayTime { get; set; }
}