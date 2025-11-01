using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetShop.Repositories.DBContext;
using PetShop.Repositories.Models;
using PetShop.Repositories.Models.Enums;
using PetShop.Services.DTOs.Responses;
using PetShop.Services.Interfaces;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PetShop.Services.Services
{
    public class ZaloPayService : IZaloPayService
    {
        private readonly PetShopDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private readonly string _appId;
        private readonly string _key1;
        private readonly string _key2;
        private readonly string _endpoint;

        public ZaloPayService(PetShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();

            _appId = _configuration["ZaloPay:AppId"] ?? "2553";
            _key1 = _configuration["ZaloPay:Key1"] ?? "PcY4iZIKFCIdgZvA6ueMcMHHUbRLYjPL";
            _key2 = _configuration["ZaloPay:Key2"] ?? "kLtgPl8HHhfvMuDHPwKfgfsY4Ydm9eIz";
            _endpoint = _configuration["ZaloPay:Endpoint"] ?? "https://sb-openapi.zalopay.vn/v2/create";
        }

        public async Task<ZaloPayCreateResponse> CreatePaymentAsync(int orderId, int userId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found or unauthorized");

            if (order.Status != OrderStatusEnum.Pending)
                throw new Exception("Order cannot be paid");

            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var appTransId = $"{DateTime.Now:yyMMdd}_{orderId}_{timestamp}";

            // Tạo embed_data và item
            var embedData = new
            {
                redirecturl = "petshop://payment/callback",
                orderId = orderId
            };
            var items = new[]
            {
                new
                {
                    itemid = $"order_{orderId}",
                    itemname = $"Order #{orderId}",
                    itemprice = (long)order.TotalAmount,
                    itemquantity = 1
                }
            };

            var appTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var amount = (long)order.TotalAmount;

            // Tạo MAC signature
            var data = $"{_appId}|{appTransId}|{order.User?.Username ?? "user"}|{amount}|{appTime}|{JsonSerializer.Serialize(embedData)}|{JsonSerializer.Serialize(items)}";
            var mac = ComputeHmacSha256(data, _key1);

            // Lấy callback URL từ configuration
            var callbackUrl = _configuration["ZaloPay:CallbackUrl"] ?? "http://localhost:5044/api/Payments/zalopay-callback";

            // Tạo request body - ZaloPay yêu cầu form-data, không phải JSON
            var formData = new Dictionary<string, string>
            {
                { "app_id", _appId },
                { "app_trans_id", appTransId },
                { "app_user", order.User?.Username ?? "user" },
                { "app_time", appTime.ToString() },
                { "amount", amount.ToString() },
                { "item", JsonSerializer.Serialize(items) },
                { "embed_data", JsonSerializer.Serialize(embedData) },
                { "description", $"Payment for Order #{orderId}" },
                { "callback_url", callbackUrl },
                { "bank_code", "" },
                { "mac", mac }
            };

            // Gọi ZaloPay API với form-urlencoded content
            var content = new FormUrlEncodedContent(formData);
            var response = await _httpClient.PostAsync(_endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            var zaloPayResponse = JsonSerializer.Deserialize<ZaloPayApiResponse>(responseString);

            if (zaloPayResponse?.return_code == 1)
            {
                // Lưu thông tin payment vào database
                var payment = new Payment
                {
                    Order = order,
                    Amount = order.TotalAmount,
                    PaymentMethod = PaymentMethodEnum.ZaloPay,
                    PaymentStatus = PaymentStatusEnum.Pending,
                    PaymentDate = DateTime.Now,
                    IsActive = true,
                    TransactionId = appTransId
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Lưu app_trans_id vào Order.Note hoặc tạo mapping riêng
                // Tạm thời dùng cách đơn giản: lưu vào dict trong memory hoặc cache
                // Để callback match được, ta sẽ dùng OrderId từ app_trans_id

                return new ZaloPayCreateResponse
                {
                    ReturnCode = zaloPayResponse.return_code,
                    ReturnMessage = zaloPayResponse.return_message ?? "Success",
                    OrderUrl = zaloPayResponse.order_url ?? "",
                    ZpTransToken = zaloPayResponse.zp_trans_token ?? ""
                };
            }

            throw new Exception($"ZaloPay error: {zaloPayResponse?.return_message ?? "Unknown error"}");
        }

        public async Task<ZaloPayCallbackResponse> ProcessCallbackAsync(ZaloPayCallbackData callbackData)
        {
            try
            {
                // Verify MAC
                var computedMac = ComputeHmacSha256(callbackData.Data, _key2);
                if (computedMac != callbackData.Mac)
                {
                    return new ZaloPayCallbackResponse
                    {
                        ReturnCode = -1,
                        ReturnMessage = "Invalid MAC"
                    };
                }

                // Parse callback data
                var data = JsonSerializer.Deserialize<ZaloPayCallbackDataParsed>(callbackData.Data);
                if (data == null)
                {
                    return new ZaloPayCallbackResponse
                    {
                        ReturnCode = -1,
                        ReturnMessage = "Invalid data format"
                    };
                }

                // Extract orderId from embed_data
                // app_trans_id format: yyMMdd_appid_timestamp
                // embed_data should contain orderId
                int orderId = 0;
                if (!string.IsNullOrEmpty(data.embed_data))
                {
                    try
                    {
                        var embedData = JsonSerializer.Deserialize<Dictionary<string, object>>(data.embed_data);
                        if (embedData != null && embedData.ContainsKey("orderId"))
                        {
                            orderId = int.Parse(embedData["orderId"].ToString() ?? "0");
                        }
                    }
                    catch { }
                }

                if (orderId == 0)
                {
                    return new ZaloPayCallbackResponse
                    {
                        ReturnCode = -1,
                        ReturnMessage = "Order ID not found in callback data"
                    };
                }

                // Tìm payment theo orderId
                var payment = await _context.Payments
                    .Include(p => p.Order)
                    .Where(p => p.OrderId == orderId)
                    .OrderByDescending(p => p.PaymentDate)
                    .FirstOrDefaultAsync();

                if (payment == null)
                {
                    return new ZaloPayCallbackResponse
                    {
                        ReturnCode = -1,
                        ReturnMessage = "Payment not found"
                    };
                }

                // Update payment status
                payment.PaymentStatus = PaymentStatusEnum.Success;

                // Update order status
                if (payment.Order != null)
                {
                    payment.Order.Status = OrderStatusEnum.Paid;
                }

                await _context.SaveChangesAsync();

                return new ZaloPayCallbackResponse
                {
                    ReturnCode = 1,
                    ReturnMessage = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ZaloPayCallbackResponse
                {
                    ReturnCode = 0,
                    ReturnMessage = $"Error: {ex.Message}"
                };
            }
        }

        public async Task<ZaloPayQueryResponse> QueryOrderStatusAsync(int orderId)
        {
            try
            {
                var payment = await _context.Payments
                    .Where(p => p.OrderId == orderId)
                    .OrderByDescending(p => p.PaymentDate)
                    .FirstOrDefaultAsync();

                if (payment == null)
                {
                    return new ZaloPayQueryResponse
                    {
                        ReturnCode = -1,
                        ReturnMessage = "Payment not found",
                        IsProcessing = false,
                        Amount = 0
                    };
                }

                var zaloPayStatus = await QueryZaloPayApiAsync(orderId);
                
                if (zaloPayStatus.return_code == 1 && payment.PaymentStatus == PaymentStatusEnum.Pending)
                {
                    payment.PaymentStatus = PaymentStatusEnum.Success;
                    payment.PaymentDate = DateTime.Now;
                    
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null && order.Status == OrderStatusEnum.Pending)
                    {
                        order.Status = OrderStatusEnum.Paid;
                    }
                    
                    await _context.SaveChangesAsync();
                }

                return new ZaloPayQueryResponse
                {
                    ReturnCode = zaloPayStatus.return_code,
                    ReturnMessage = zaloPayStatus.return_message ?? payment.PaymentStatus.ToString(),
                    IsProcessing = payment.PaymentStatus == PaymentStatusEnum.Pending,
                    Amount = (int)payment.Amount,
                    ZpTransId = zaloPayStatus.zp_trans_id?.ToString()
                };
            }
            catch (Exception ex)
            {
                return new ZaloPayQueryResponse
                {
                    ReturnCode = 0,
                    ReturnMessage = $"Error: {ex.Message}",
                    IsProcessing = false,
                    Amount = 0
                };
            }
        }

        private async Task<ZaloPayQueryApiResponse> QueryZaloPayApiAsync(int orderId)
        {
            try
            {
                var latestPayment = await _context.Payments
                    .Where(p => p.OrderId == orderId && p.TransactionId != null)
                    .OrderByDescending(p => p.PaymentDate)
                    .FirstOrDefaultAsync();

                if (latestPayment == null || string.IsNullOrEmpty(latestPayment.TransactionId))
                {
                    return new ZaloPayQueryApiResponse
                    {
                        return_code = -1,
                        return_message = "Payment not found or no transaction ID"
                    };
                }

                var appTransId = latestPayment.TransactionId;

                var appTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                var data = $"{_appId}|{appTransId}|{_key1}";
                var mac = ComputeHmacSha256(data, _key1);

                var queryEndpoint = "https://sb-openapi.zalopay.vn/v2/query";
                var formData = new Dictionary<string, string>
                {
                    { "app_id", _appId },
                    { "app_trans_id", appTransId },
                    { "mac", mac }
                };

                var content = new FormUrlEncodedContent(formData);
                var response = await _httpClient.PostAsync(queryEndpoint, content);
                var responseString = await response.Content.ReadAsStringAsync();
                var queryResponse = JsonSerializer.Deserialize<ZaloPayQueryApiResponse>(responseString, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
                
                return queryResponse ?? new ZaloPayQueryApiResponse
                {
                    return_code = -1,
                    return_message = "Invalid response"
                };
            }
            catch (Exception ex)
            {
                return new ZaloPayQueryApiResponse
                {
                    return_code = 0,
                    return_message = ex.Message
                };
            }
        }

        private string ComputeHmacSha256(string data, string key)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
