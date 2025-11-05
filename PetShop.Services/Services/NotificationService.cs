using FirebaseAdmin.Messaging;

namespace PetShop.Services.Services
{
    public class NotificationService
    {
        public async Task SendMessageAsync(
            string fcmToken,
            string title,
            string body,
            int receiverId)
        {
            if (string.IsNullOrEmpty(fcmToken))
                return;

            var message = new Message
            {
                Token = fcmToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Data = new Dictionary<string, string>
                {
                    { "receiverId", receiverId.ToString() }
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                Console.WriteLine($"✅ Notification sent successfully: {response}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to send notification: {ex.Message}");
            }
        }
    }
}
