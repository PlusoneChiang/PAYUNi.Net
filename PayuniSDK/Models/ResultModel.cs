namespace PayuniSDK.Models
{
    public class ResultModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public EncryptInfoModel EncryptInfo { get; set; } = new();
    }
}
