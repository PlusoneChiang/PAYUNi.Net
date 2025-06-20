namespace PAYUNiSDK.Models;

public class PAYUNiSettings
{
    public string MerchantId { get; set; } = string.Empty;
    public string MerchantKey { get; set; } = string.Empty;
    public string MerchantIV { get; set; } = string.Empty;
    public bool IsPlatForm { get; set; } = false;
    public EnviromentType Environment { get; set; } = EnviromentType.SandBox;
}
