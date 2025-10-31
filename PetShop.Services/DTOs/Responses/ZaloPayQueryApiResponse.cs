namespace PetShop.Services.DTOs.Responses
{
    public class ZaloPayQueryApiResponse
    {
        public int return_code { get; set; }
        public string? return_message { get; set; }
        
        public object? sub_return_code { get; set; }
        public string? sub_return_message { get; set; }
        
        public object? zp_trans_id { get; set; }
        
        public long amount { get; set; }
        public long discount_amount { get; set; }
        
        public long server_time { get; set; }
        public string? discountAmount { get; set; }
    }
}
