using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using PayuniSDK.Models;
using PayuniSDK.Enums;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Engines;

namespace PayuniSDK.APIs;

public class PayuniAPI
{
    /// <summary>
    /// 加密資訊
    /// </summary>
    public EncryptInfoModel EncryptInfo;
    /// <summary>
    /// 傳入傳出參數
    /// </summary>
    public ParameterModel Parameter;
    /// <summary>
    /// 回傳結果
    /// </summary>
    public ResultModel Result;
    /// <summary>
    /// 加密字串
    /// </summary>
    public static string Plain { get; set; }
    /// <summary>
    /// 介接的 merKey
    /// </summary>
    public static string MerKey { get; set; }
    /// <summary>
    /// 介接的 merIV
    /// </summary>
    public static string MerIV { get; set; }
    /// <summary>
    /// api網址
    /// </summary>
    public static string ApiUrl { get; set; }
    /// <summary>
    /// 網域
    /// </summary>
    public static string Prefix { get; set; }
    /// <summary>
    /// 型態
    /// </summary>
    public static EnviromentType Type { get; set; }


    public PayuniAPI(string key, string iv, EnviromentType type = EnviromentType.Production)
    {
        EncryptInfo = new EncryptInfoModel();
        Parameter = new ParameterModel();
        Result = new ResultModel();
        MerKey = key;
        MerIV = iv;
        Type = type;
        ApiUrl = "api.payuni.com.tw/api/";
        Prefix = "https://";
        if (Type == EnviromentType.SandBox)
        {
            Prefix += "sandbox-";
        }
        ApiUrl = Prefix + ApiUrl;
    }

    /// <summary>
    /// 呼叫各類api
    /// </summary>
    /// <param name="EnInfo"></param>
    /// <param name="tradeType"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public string UniversalTrade(EncryptInfoModel EnInfo, TradeType tradeType, string version = "1.0")
    {
        EncryptInfo = EnInfo;
        Parameter.Version = version;
        Result = CheckParams();
        if (Result.Success)
        {
            try
            {
                switch (tradeType)
                {
                    case TradeType.Upp:// 交易建立 整合式支付頁
                    case TradeType.Atm:// 交易建立 虛擬帳號幕後
                    case TradeType.Cvs:// 交易建立 超商代碼幕後
                    case TradeType.Linepay:// 交易建立 Line Pay 幕後
                    case TradeType.AfteeDirect://交易建立 AFTEE 幕後
                        if (tradeType == TradeType.Linepay)
                        {
                            Parameter.Version = "1.1";
                        }
                        if (string.IsNullOrEmpty(EncryptInfo.MerTradeNo))
                        {
                            Result.Message = "商店訂單編號為必填(MerTradeNo is not setting)";
                        }
                        if (string.IsNullOrEmpty(EncryptInfo.TradeAmt))
                        {
                            Result.Message = "訂單金額為必填(TradeAmt is not setting)";
                        }
                        break;
                    case TradeType.Credit:// 交易建立 信用卡幕後
                        if (string.IsNullOrEmpty(EncryptInfo.MerTradeNo))
                        {
                            Result.Message = "商店訂單編號為必填(MerTradeNo is not setting)";
                        }
                        if (string.IsNullOrEmpty(EncryptInfo.TradeAmt))
                        {
                            Result.Message = "訂單金額為必填(TradeAmt is not setting)";
                        }
                        if (EncryptInfo.CreditHash == null) {
                            if (string.IsNullOrEmpty(EncryptInfo.CardNo))
                            {
                                Result.Message = "信用卡卡號為必填(CardNo is not setting)";
                            }
                            if (string.IsNullOrEmpty(EncryptInfo.CardExpired))
                            {
                                Result.Message = "信用卡到期年月為必填(CardExpired is not setting)";
                            }
                            if (string.IsNullOrEmpty(EncryptInfo.CardCVC))
                            {
                                Result.Message = "信用卡安全碼為必填(CardCVC is not setting)";
                            }
                        }                        
                        break;
                    case TradeType.TradeClose:// 交易請退款
                        if (string.IsNullOrEmpty(EncryptInfo.TradeNo))
                        {
                            Result.Message = "uni序號為必填(TradeNo is not setting)";
                        }
                        if (string.IsNullOrEmpty(EncryptInfo.CloseType))
                        {
                            Result.Message = "關帳類型為必填(CloseType is not setting)";
                        }
                        break;
                    case TradeType.TradeCancel:// 交易取消授權
                    case TradeType.TradeConfirmAftee:// 後支付確認(AFTEE)
                        if (string.IsNullOrEmpty(EncryptInfo.TradeNo))
                        {
                            Result.Message = "uni序號為必填(TradeNo is not setting)";
                        }
                        break;
                    case TradeType.CancelCvs:// 交易取消超商代碼(CVS)
                        if (string.IsNullOrEmpty(EncryptInfo.PayNo))
                        {
                            Result.Message = "超商代碼為必填(PayNo is not setting)";
                        }
                        break;
                    case TradeType.CreditBindCancel:// 信用卡token取消(約定/記憶卡號)
                        if (string.IsNullOrEmpty(EncryptInfo.UseTokenType))
                        {
                            Result.Message = "信用卡Token類型為必填(UseTokenType is not setting)";
                        }
                        if (string.IsNullOrEmpty(EncryptInfo.BindVal))
                        {
                            Result.Message = "綁定回傳值 /信用卡Token(BindVal is not setting)";
                        }
                        break;
                    case TradeType.TradeRefundIcash:// 愛金卡退款(ICASH)
                    case TradeType.TradeRefundAftee:// 後支付退款(AFTEE)
                    case TradeType.TradeRefundLinepay:// LINE Pay退款(LINE)
                        if (string.IsNullOrEmpty(EncryptInfo.TradeNo))
                        {
                            Result.Message = "uni序號為必填(TradeNo is not setting)";
                        }
                        if (string.IsNullOrEmpty(EncryptInfo.TradeAmt))
                        {
                            Result.Message = "訂單金額為必填(TradeAmt is not setting)";
                        }
                        break;
                    case TradeType.TradeQuery:// 交易查詢
                    case TradeType.CreditBindQuery:// 信用卡token查詢(約定)
                        break;
                    default:
                        Result.Message = "未提供該參數類型(Unknown params)";
                        break;
                }

                if (string.IsNullOrEmpty(Result.Message))
                {
                    SetParams(Contrast(tradeType));

                    if (tradeType == TradeType.Upp)
                    {
                        return HtmlApi();
                    }
                    else
                    {
                        string CurlResult = CurlApi().Result;
                        return JsonSerializer.Serialize(ResultProcess(CurlResult));
                    }
                }
                else {
                    Result.Success = false;
                    return JsonSerializer.Serialize(Result);
                }
               
            }
            catch (Exception e)
            {
                Result.Success = false;
                Result.Message = e.Message;
                return JsonSerializer.Serialize(Result);
            }
        }
        else
        {
            return JsonSerializer.Serialize(Result);
        }
    }

    /// <summary>
    /// 處理api回傳的結果
    /// </summary>
    /// <param name="CurlResult"></param>
    /// <returns></returns>
    public ResultModel ResultProcess(string CurlResult)
    {
        var resultParam = JsonSerializer.Deserialize<ParameterModel>(CurlResult);
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
            if (!string.IsNullOrEmpty(resultParams.EncryptInfo))
            {
                if (!string.IsNullOrEmpty(resultParams.HashInfo))
                {
                    string chkHash = Hash(resultParams.EncryptInfo);
                    if (chkHash != resultParams.HashInfo)
                    {
                        result.Message = "Hash值比對失敗(Hash mismatch)";
                        return result;
                    }
                    result.Message = string.Empty;
                    result.EncryptInfo = ConvertTo<EncryptInfoModel>(Decrypt(resultParams.EncryptInfo));
                    result.Success = true;
                }
                else
                {
                    result.Message = "缺少Hash資訊(missing HashInfo)";
                }
            }
            else
            {
                result.Message = "缺少加密字串(missing EncryptInfo)";
                switch (resultParams.Status)
                {
                    case "API00003":
                        result.Message = "無API版本號";
                        break;
                }
            }
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
    private string HtmlApi()
    {
        string htmlprint = string.Empty;
        htmlprint += "<html><body onload='document.getElementById(\"upp\").submit();'>";
        htmlprint += "<form action='" + ApiUrl + "' method='post' id='upp'>";
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
    private async Task<string> CurlApi()
    {
        string parame = GetQueryString(Parameter);
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
    private static string Contrast(TradeType tradeType)
    {
        return tradeType switch
        {
            TradeType.Upp => "upp",
            TradeType.Atm => "atm",
            TradeType.Cvs => "cvs",
            TradeType.Linepay => "linepay",
            TradeType.AfteeDirect => "aftee_direct",
            TradeType.Credit => "credit",
            TradeType.CancelCvs => "cancel_cvs",
            TradeType.TradeQuery => "trade/query",
            TradeType.TradeClose => "trade/close",
            TradeType.TradeCancel => "trade/cancel",
            TradeType.CreditBindQuery => "credit_bind/query",
            TradeType.CreditBindCancel => "credit_bind/cancel",
            TradeType.TradeRefundIcash => "trade/common/refund/icash",
            TradeType.TradeRefundAftee => "trade/common/refund/aftee",
            TradeType.TradeConfirmAftee => "trade/common/confirm/aftee",
            TradeType.TradeRefundLinepay => "trade/common/refund/linepay",
            _ => throw new ArgumentOutOfRangeException(nameof(tradeType), tradeType, null)
        };
    }
    /// <summary>
    /// 設定要curl的參數
    /// </summary>
    /// <param name="type"></param>
    private void SetParams(string type = "")
    {
        string isPlatForm = string.Empty;
        if (!string.IsNullOrEmpty(EncryptInfo.IsPlatForm)) {
            isPlatForm = EncryptInfo.IsPlatForm;
            EncryptInfo.IsPlatForm = string.Empty;
        }
        Plain = GetQueryString(EncryptInfo);
        Parameter.MerID = EncryptInfo.MerID;
        Parameter.EncryptInfo = Encrypt();
        Parameter.HashInfo = Hash(Parameter.EncryptInfo);
        Parameter.IsPlatForm = isPlatForm;

        ApiUrl += type;
    }
    /// <summary>
    /// 檢查必填參數是否存在
    /// </summary>
    /// <returns></returns>
    private ResultModel CheckParams()
    {
        Result = new ResultModel
        {
            Success = false
        };
        if (string.IsNullOrEmpty(MerKey))
        {
            Result.Message = "key is not setting";
            return Result;
        }

        if (string.IsNullOrEmpty(MerIV))
        {
            Result.Message = "iv is not setting";
            return Result;
        }
        try
        {
            if (string.IsNullOrEmpty(EncryptInfo.MerID))
            {
                Result.Message = "商店代號為必填(MerID is not setting)";
                return Result;
            }
            if (string.IsNullOrEmpty(EncryptInfo.Timestamp.ToString()))
            {
                Result.Message = "時間戳記為必填(Timestamp is not setting)";
                return Result;
            }
            Result.Success = true;
            return Result;
        }
        catch (Exception e)
        {
            Result.Message = e.Message;
            return Result; ;
        }
    }

    /// <summary>
    /// 轉換QueryString
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private static string GetQueryString(object obj)
    {
        var properties = obj.GetType().GetProperties()
            .Where(p => p.GetValue(obj, null) != null)
            .Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(obj, null).ToString())}");
        string queryString = string.Join("&", [.. properties]);
        return queryString;
    }

    /// <summary>
    /// 轉換QueryString為物件
    /// </summary>
    private static T ConvertTo<T>(string queryString) where T : new()
    {
        var obj = new T();
        var properties = typeof(T).GetProperties();
        var pairs = queryString.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=');
            if (keyValue.Length == 2)
            {
                var property = properties.FirstOrDefault(p => p.Name.Equals(keyValue[0], StringComparison.OrdinalIgnoreCase));
                if (property != null && property.CanWrite)
                {
                    property.SetValue(obj, HttpUtility.UrlDecode(keyValue[1]));
                }
            }
        }
        return obj;
    }


    /// <summary>
    /// 加密
    /// </summary>
    /// <returns></returns>
    private static string Encrypt()
    {
        if (string.IsNullOrEmpty(Plain))
        {
            return Plain;
        }
        //參數設定
        var tagLength = 16;
        var key = Encoding.UTF8.GetBytes(MerKey);
        var iv = Encoding.UTF8.GetBytes(MerIV);
        var plaintextData = Encoding.UTF8.GetBytes(Plain);
        var encryptedTagData = new byte[plaintextData.Length + tagLength];
        byte[] encrypted = new byte[plaintextData.Length];
        byte[] tag = new byte[tagLength];
        //加密設定
        var cipher = new GcmBlockCipher(new AesEngine());
        var keyParameters = new AeadParameters(new KeyParameter(key), tagLength * 8, iv);
        cipher.Init(true, keyParameters);
        var offset = cipher.ProcessBytes(plaintextData, 0, plaintextData.Length, encryptedTagData, 0);
        //加密:密文+tag
        cipher.DoFinal(encryptedTagData, offset);
        //分解密文和tag
        Array.Copy(encryptedTagData, encrypted, plaintextData.Length);
        Array.Copy(encryptedTagData, plaintextData.Length, tag, 0, tagLength);
        return Bin2hex(Encoding.UTF8.GetBytes(Convert.ToBase64String(encrypted) + ":::" + Convert.ToBase64String(tag))).Trim();
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="encryptStr"></param>
    /// <returns></returns>
    private static string Decrypt(string encryptStr)
    {
        if (string.IsNullOrEmpty(encryptStr))
        {
            return encryptStr;
        }
        //參數設定
        encryptStr = Encoding.UTF8.GetString(Hex2bin(encryptStr));
        var key = Encoding.UTF8.GetBytes(MerKey);
        var iv = Encoding.UTF8.GetBytes(MerIV);
        string[] spliter = [":::"];
        string[] data = encryptStr.Split(spliter, StringSplitOptions.RemoveEmptyEntries);
        byte[] encryptData = Convert.FromBase64String(data[0]);
        byte[] tagData = Convert.FromBase64String(data[1]);
        var encryptedTagData = new byte[encryptData.Length + tagData.Length];
        Array.Copy(encryptData, 0, encryptedTagData, 0, encryptData.Length);
        Array.Copy(tagData, 0, encryptedTagData, encryptData.Length, tagData.Length);
        var cipher = new GcmBlockCipher(new AesEngine());
        var keyParameters = new AeadParameters(new KeyParameter(key), tagData.Length * 8, iv);
        cipher.Init(false, keyParameters);
        var result = new byte[encryptData.Length];
        int len = cipher.ProcessBytes(encryptedTagData, 0, encryptedTagData.Length, result, 0);
        cipher.DoFinal(result, len);
        return Encoding.UTF8.GetString(result);
    }

    /// <summary>
    /// Hash
    /// </summary>
    /// <param name="encryptStr"></param>
    /// <returns></returns>
    private static string Hash(string encryptStr = "")
    {
        var byteArray = SHA256.HashData(Encoding.UTF8.GetBytes(MerKey + encryptStr + MerIV));
        return Bin2hex(byteArray).ToUpper();
    }

    /// <summary>
    /// 2進位轉16進位
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private static string Bin2hex(byte[] result)
    {
        StringBuilder sb = new(result.Length * 2);
        for (int i = 0; i < result.Length; i++)
        {
            int hight = (result[i] >> 4) & 0x0f;
            int low = result[i] & 0x0f;
            sb.Append(hight > 9 ? (char)(hight - 10 + 'a') : (char)(hight + '0'));
            sb.Append(low > 9 ? (char)(low - 10 + 'a') : (char)(low + '0'));
        }
        return sb.ToString();
    }

    /// <summary>
    /// 16進位轉2進位
    /// </summary>
    /// <param name="hexstring"></param>
    /// <returns></returns>
    private static byte[] Hex2bin(string hexstring)
    {
        hexstring = hexstring.Replace(" ", "");
        if ((hexstring.Length % 2) != 0)
        {
            hexstring += " ";
        }
        byte[] returnBytes = new byte[hexstring.Length / 2];
        for (int i = 0; i < returnBytes.Length; i++)
        {
            returnBytes[i] = Convert.ToByte(hexstring.Substring(i * 2, 2), 16);
        }
        return returnBytes;
    }
}
