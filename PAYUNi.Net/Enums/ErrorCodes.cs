namespace PAYUNiSDK.Enums;

public enum ErrorCodes
{
    [Description("商店訂單編號為必填(MerTradeNo is not setting)")]
    MerTradeNoRequired,

    [Description("訂單金額為必填(TradeAmt is not setting)")]
    TradeAmtRequired,

    [Description("信用卡卡號為必填(CardNo is not setting)")]
    CardNoRequired,

    [Description("信用卡到期年月為必填(CardExpired is not setting)")]
    CardExpiredRequired,

    [Description("信用卡安全碼為必填(CardCVC is not setting)")]
    CardCVCRequired,

    [Description("uni序號為必填(TradeNo is not setting)")]
    TradeNoRequired,

    [Description("關帳類型為必填(CloseType is not setting)")]
    CloseTypeRequired,

    [Description("超商代碼為必填(PayNo is not setting)")]
    PayNoRequired,

    [Description("信用卡Token類型為必填(UseTokenType is not setting)")]
    UseTokenTypeRequired,

    [Description("綁定回傳值 /信用卡Token(BindVal is not setting)")]
    BindValRequired,

    [Description("未提供該參數類型(Unknown params)")]
    UnknownParams,

    [Description("Hash值比對失敗(Hash mismatch)")]
    HashMismatch,
    [Description("缺少Hash資訊(missing HashInfo)")]
    MissingHashInfo,
    [Description("缺少加密字串(missing EncryptInfo)")]
    MissingEncryptInfo,
    [Description("無API版本號")]
    MissingApiVersion,
    [Description("MerKey未設定(MerKey is not setting)")]
    MissingMerKey,
    [Description("MerIV未設定(MerIV is not setting)")]
    MissingMerIV,
    [Description("商店代號為必填(MerID is not setting)")]
    MissingMerID,
    [Description("時間戳記為必填(Timestamp is not setting)")]
    MissingTimestamp
}
