namespace PAYUNiSDK;

public class PAYUNiClient(IOptions<PAYUNiSettings> options)
{
    private readonly PAYUNiSettings clientSettings = options.Value;
    /// <summary>
    /// PAYUNi API網址
    /// </summary>
    private string ApiUrl => $"https://{(clientSettings.Environment == EnviromentType.SandBox ? "sandbox-" : "")}api.payuni.com.tw/api";

    public PAYUNiClient(PAYUNiSettings settings) : this(Options.Create(settings))
    {
    }
    /// <summary>
    /// 呼叫各類api
    /// </summary>
    /// <param name="encryptInfo"></param>
    /// <param name="tradeType"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public string UniversalTrade(EncryptInfoRequestModel encryptInfo, PAYUNiTradeType tradeType, string version = "1.0")
    {
        var parameter = new ParameterModel
        {
            Version = version
        };
        CheckRequiredParams(encryptInfo);
        CheckRequiredParamsByTradeType(tradeType, encryptInfo);
        if (tradeType == PAYUNiTradeType.Linepay) parameter.Version = "1.1";
        var apiUrl = Path.Combine(ApiUrl, GetApiPathByTradeType(tradeType));
        MapQueryParams(encryptInfo, parameter);

        if (tradeType == PAYUNiTradeType.Upp)
        {
            return BuildUppPage(apiUrl, parameter);
        }
        else
        {
            string CurlResult = ApiCall(parameter).Result;
            return JsonConvert.SerializeObject(ResultProcess(CurlResult));
        }
    }

    /// <summary>
    /// 處理api回傳的結果
    /// </summary>
    /// <param name="CurlResult"></param>
    /// <returns></returns>
    public ResultModel ResultProcess(string CurlResult)
    {
        var resultParam = JsonConvert.DeserializeObject<ParameterModel>(CurlResult);
        return ResultProcess(resultParam);
    }

    public ResultModel ResultProcess(ParameterModel resultParams)
    {
        ResultModel result = new()
        {
            Success = false
        };
        try
        {
            if (string.IsNullOrWhiteSpace(resultParams.EncryptInfo))
                throw resultParams.Status switch
                {
                    "API00003" => new PAYUNiException(ErrorCodes.MissingApiVersion),
                    _ => new PAYUNiException(ErrorCodes.MissingEncryptInfo),
                };
            if (string.IsNullOrWhiteSpace(resultParams.HashInfo))
                throw new PAYUNiException(ErrorCodes.MissingHashInfo);
            string chkHash = EncryptHelper.Hash(resultParams.EncryptInfo);
            if (chkHash != resultParams.HashInfo)
                throw new PAYUNiException(ErrorCodes.HashMismatch);
            result.Message = string.Empty;
            var decryptStr = EncryptHelper.Decrypt(resultParams.EncryptInfo, clientSettings.MerchantKey, clientSettings.MerchantIV);
            result.EncryptInfo = decryptStr.ConvertTo<EncryptInfoResponseModel>();
            result.Success = true;
            return result;
        }
        catch(Exception ex)
        {
            result.Message = $"處理結果失敗({ex.Message})";
            return result;
        }
    }

    /// <summary>
    /// 前景呼叫
    /// </summary>
    /// <returns></returns>
    private static string BuildUppPage(string url, ParameterModel Parameter)
    {
        string htmlprint = string.Empty;
        htmlprint += "<html><body onload='document.getElementById(\"upp\").submit();'>";
        htmlprint += "<form action='" + url + "' method='post' id='upp'>";
        htmlprint += "<input name='MerID' type='hidden' value='" + Parameter.MerID + "' />";
        htmlprint += "<input name='Version' type='hidden' value='" + Parameter.Version + "' />";
        htmlprint += "<input name='EncryptInfo' type='hidden' value='" + Parameter.EncryptInfo + "' />";
        htmlprint += "<input name='HashInfo' type='hidden' value='" + Parameter.HashInfo + "' />";
        htmlprint += "<input name='IsPlatForm' type='hidden' value='" + Parameter.IsPlatForm + "' />";
        htmlprint += "</form></body></html>";
        return htmlprint;
    }

    /// <summary>
    /// CURL
    /// </summary>
    /// <returns></returns>
    private async Task<string> ApiCall(ParameterModel parameter)
    {
        string parame = parameter.ToQueryString();
        var client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(1000)
        };
        client.DefaultRequestHeaders.Add("User-Agent", "PRESCOSDKAPI");
        var content = new StringContent(parame, Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await client.PostAsync(ApiUrl, content);
        response.EnsureSuccessStatusCode();
        string result = await response.Content.ReadAsStringAsync();
        return result;
    }

    /// <summary>
    /// 轉換為網址
    /// </summary>
    /// <param name="tradeType"></param>
    /// <returns></returns>
    private static string GetApiPathByTradeType(PAYUNiTradeType tradeType)
    {
        return tradeType switch
        {
            PAYUNiTradeType.Upp => "upp",
            PAYUNiTradeType.Atm => "atm",
            PAYUNiTradeType.Cvs => "cvs",
            PAYUNiTradeType.Linepay => "linepay",
            PAYUNiTradeType.AfteeDirect => "aftee_direct",
            PAYUNiTradeType.Credit => "credit",
            PAYUNiTradeType.CancelCvs => "cancel_cvs",
            PAYUNiTradeType.TradeQuery => "trade/query",
            PAYUNiTradeType.TradeClose => "trade/close",
            PAYUNiTradeType.TradeCancel => "trade/cancel",
            PAYUNiTradeType.CreditBindQuery => "credit_bind/query",
            PAYUNiTradeType.CreditBindCancel => "credit_bind/cancel",
            PAYUNiTradeType.TradeRefundIcash => "trade/common/refund/icash",
            PAYUNiTradeType.TradeRefundAftee => "trade/common/refund/aftee",
            PAYUNiTradeType.TradeConfirmAftee => "trade/common/confirm/aftee",
            PAYUNiTradeType.TradeRefundLinepay => "trade/common/refund/linepay",
            _ => throw new ArgumentOutOfRangeException(nameof(tradeType), tradeType, null)
        };
    }

    /// <summary>
    /// 設定要curl的參數
    /// </summary>
    /// <param name="type"></param>
    private void MapQueryParams(EncryptInfoRequestModel encryptInfo, ParameterModel parameter)
    {
        string isPlatForm = clientSettings.IsPlatForm ? "1" : string.Empty;
        if (!string.IsNullOrEmpty(encryptInfo.IsPlatForm))
        {
            isPlatForm = encryptInfo.IsPlatForm;
            encryptInfo.IsPlatForm = string.Empty;
        }
        var plain = encryptInfo.ToQueryString();
        parameter.MerID = encryptInfo.MerID;
        parameter.EncryptInfo = EncryptHelper.Encrypt(plain, clientSettings.MerchantKey, clientSettings.MerchantIV);
        parameter.HashInfo = EncryptHelper.Hash(string.Concat(clientSettings.MerchantKey, parameter.EncryptInfo, clientSettings.MerchantIV));
        parameter.IsPlatForm = isPlatForm;
    }

    /// <summary>
    /// 檢查必填參數是否存在
    /// </summary>
    /// <returns></returns>
    private void CheckRequiredParams(EncryptInfoRequestModel encryptInfo)
    {
        if (string.IsNullOrWhiteSpace(clientSettings.MerchantId))
            throw new PAYUNiException(ErrorCodes.MissingMerID);
        if (string.IsNullOrWhiteSpace(clientSettings.MerchantKey))
            throw new PAYUNiException(ErrorCodes.MissingMerKey);
        if (string.IsNullOrWhiteSpace(clientSettings.MerchantIV))
            throw new PAYUNiException(ErrorCodes.MissingMerIV);
        if (encryptInfo == null)
            throw new PAYUNiException(ErrorCodes.MissingEncryptInfo);
        if (string.IsNullOrWhiteSpace(encryptInfo.MerID))
            throw new PAYUNiException(ErrorCodes.MissingMerID);
        if (string.IsNullOrWhiteSpace(encryptInfo.Timestamp))
            throw new PAYUNiException(ErrorCodes.MissingTimestamp);
    }

    /// <summary>
    /// 依交易類型檢查必要參數
    /// </summary>
    /// <param name="tradeType"></param>
    /// <param name="encryptInfo"></param>
    private void CheckRequiredParamsByTradeType(PAYUNiTradeType tradeType, EncryptInfoRequestModel encryptInfo)
    {
        switch (tradeType)
        {
            case PAYUNiTradeType.Upp:
            case PAYUNiTradeType.Atm:
            case PAYUNiTradeType.Cvs:
            case PAYUNiTradeType.Linepay:
            case PAYUNiTradeType.AfteeDirect:
                if (string.IsNullOrEmpty(encryptInfo.MerTradeNo))
                    throw new PAYUNiException(ErrorCodes.MerTradeNoRequired);
                if (string.IsNullOrEmpty(encryptInfo.TradeAmt))
                    throw new PAYUNiException(ErrorCodes.TradeAmtRequired);
                break;
            case PAYUNiTradeType.Credit:
                if (string.IsNullOrEmpty(encryptInfo.MerTradeNo))
                    throw new PAYUNiException(ErrorCodes.MerTradeNoRequired);
                if (string.IsNullOrEmpty(encryptInfo.TradeAmt))
                    throw new PAYUNiException(ErrorCodes.TradeAmtRequired);
                if (encryptInfo.CreditHash == null)
                {
                    if (string.IsNullOrEmpty(encryptInfo.CardNo))
                        throw new PAYUNiException(ErrorCodes.CardNoRequired);
                    if (string.IsNullOrEmpty(encryptInfo.CardExpired))
                        throw new PAYUNiException(ErrorCodes.CardExpiredRequired);
                    if (string.IsNullOrEmpty(encryptInfo.CardCVC))
                        throw new PAYUNiException(ErrorCodes.CardCVCRequired);
                }
                break;
            case PAYUNiTradeType.TradeClose:
                if (string.IsNullOrEmpty(encryptInfo.TradeNo))
                    throw new PAYUNiException(ErrorCodes.TradeNoRequired);
                if (string.IsNullOrEmpty(encryptInfo.CloseType))
                    throw new PAYUNiException(ErrorCodes.CloseTypeRequired);
                break;
            case PAYUNiTradeType.TradeCancel:
            case PAYUNiTradeType.TradeConfirmAftee:
                if (string.IsNullOrEmpty(encryptInfo.TradeNo))
                    throw new PAYUNiException(ErrorCodes.TradeNoRequired);
                break;
            case PAYUNiTradeType.CancelCvs:
                if (string.IsNullOrEmpty(encryptInfo.PayNo))
                    throw new PAYUNiException(ErrorCodes.PayNoRequired);
                break;
            case PAYUNiTradeType.CreditBindCancel:
                if (string.IsNullOrEmpty(encryptInfo.UseTokenType))
                    throw new PAYUNiException(ErrorCodes.UseTokenTypeRequired);
                if (string.IsNullOrEmpty(encryptInfo.BindVal))
                    throw new PAYUNiException(ErrorCodes.BindValRequired);
                break;
            case PAYUNiTradeType.TradeRefundIcash:
            case PAYUNiTradeType.TradeRefundAftee:
            case PAYUNiTradeType.TradeRefundLinepay:
                if (string.IsNullOrEmpty(encryptInfo.TradeNo))
                    throw new PAYUNiException(ErrorCodes.TradeNoRequired);
                if (string.IsNullOrEmpty(encryptInfo.TradeAmt))
                    throw new PAYUNiException(ErrorCodes.TradeAmtRequired);
                break;
            case PAYUNiTradeType.TradeQuery:
            case PAYUNiTradeType.CreditBindQuery:
                break;
            default:
                throw new PAYUNiException(ErrorCodes.UnknownParams);
        }
    }
}