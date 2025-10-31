namespace PetShop.Services.Interfaces
{
    public interface IZaloPayService
    {
        Task<ZaloPayCreateResponse> CreatePaymentAsync(int orderId, int userId);
        Task<ZaloPayCallbackResponse> ProcessCallbackAsync(ZaloPayCallbackData callbackData);
        Task<ZaloPayQueryResponse> QueryOrderStatusAsync(int orderId);
    }

    public class ZaloPayCallbackData
    {
        public string Data { get; set; } = string.Empty;
        public string Mac { get; set; } = string.Empty;
    }

    public class ZaloPayCreateResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = string.Empty;
        public string OrderUrl { get; set; } = string.Empty;
        public string ZpTransToken { get; set; } = string.Empty;
    }

    public class ZaloPayCallbackResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = string.Empty;
    }

    public class ZaloPayQueryResponse
    {
        public int ReturnCode { get; set; }
        public string ReturnMessage { get; set; } = string.Empty;
        public bool IsProcessing { get; set; }
        public int Amount { get; set; }
        public string? ZpTransId { get; set; }
    }
}
