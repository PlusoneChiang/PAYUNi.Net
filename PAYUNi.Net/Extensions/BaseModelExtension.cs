namespace PAYUNiSDK.Extensions;

internal static class BaseModelExtension
{
    /// <summary>
    /// 轉換QueryString
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToQueryString<T>(this T model) where T : SDKBaseModel, new()
    {
        var properties = model.GetType().GetProperties()
            .Where(p => p.GetValue(model, null) != null)
            .Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(model, null).ToString())}");
        string queryString = string.Join("&", properties);
        return queryString;
    }

    /// <summary>
    /// 轉換QueryString為物件
    /// </summary>
    public static T ConvertTo<T>(this string queryString) where T : SDKBaseModel, new()
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
}
