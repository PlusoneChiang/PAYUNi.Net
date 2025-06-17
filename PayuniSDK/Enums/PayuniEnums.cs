namespace PayuniSDK.Enums;

public enum EnviromentType
    {
        /// <summary>
        /// 連線測試區
        /// </summary>
        SandBox,
        /// <summary>
        /// 連線正式區
        /// </summary>
        Production
    }

    public enum TradeType
    {
        /// <summary>
        /// 交易建立 整合式
        /// </summary>
        Upp,
        /// <summary>
        /// 交易建立 虛擬帳號幕後
        /// </summary>
        Atm,
        /// <summary>
        /// 交易建立 超商代碼幕後
        /// </summary>
        Cvs,
        /// <summary>
        /// 交易建立 Line Pay 幕後
        /// </summary>
        Linepay,
        /// <summary>
        /// 交易建立 Aftee 幕後
        /// </summary>
        AfteeDirect,
        /// <summary>
        /// 交易建立 信用卡幕後
        /// </summary>
        Credit,
        /// <summary>
        /// 交易請退款
        /// </summary>
        TradeClose,
        /// <summary>
        /// 交易取消授權
        /// </summary>
        TradeCancel,
        /// <summary>
        /// 後支付確認(Aftee)
        /// </summary>
        TradeConfirmAftee,
        /// <summary>
        /// 交易取消超商代碼(Cvs)
        /// </summary>
        CancelCvs,
        /// <summary>
        /// 信用卡Token取消(約定/記憶卡號)
        /// </summary>
        CreditBindCancel,
        /// <summary>
        /// 愛金卡退款(Icash)
        /// </summary>
        TradeRefundIcash,
        /// <summary>
        /// 後支付退款(Aftee)
        /// </summary>
        TradeRefundAftee,
        /// <summary>
        /// Line Pay退款(Line)
        /// </summary>
        TradeRefundLinepay,
        /// <summary>
        /// 交易查詢
        /// </summary>
        TradeQuery,
        /// <summary>
        /// 信用卡Token查詢(約定)
        /// </summary>
        CreditBindQuery,
    }