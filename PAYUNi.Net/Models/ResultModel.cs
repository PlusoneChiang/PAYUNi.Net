namespace PAYUNiSDK.Models;

public class ResultModel : SDKBaseModel
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public EncryptInfoModel EncryptInfo { get; set; } = new();
}
