namespace PAYUNiSDK.Helpers;

internal static class EncryptHelper
{
    /// <summary>
    /// 加密
    /// </summary>
    /// <returns></returns>
    public static string Encrypt(string Plain, string MerKey, string MerIV)
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
    public static string Decrypt(string encryptStr, string MerKey, string MerIV)
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
    public static string Hash(string encryptStr)
    {
        var byteArray = SHA256.HashData(Encoding.UTF8.GetBytes(encryptStr));
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
            int hight = result[i] >> 4 & 0x0f;
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
        if (hexstring.Length % 2 != 0)
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
