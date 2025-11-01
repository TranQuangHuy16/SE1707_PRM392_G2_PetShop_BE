namespace PetShop.Services.DTOs.Responses
{
    public class ZaloPayCallbackDataParsed
    {
        public string app_trans_id { get; set; } = string.Empty;
        public string embed_data { get; set; } = string.Empty;
        public int amount { get; set; }
    }
}
