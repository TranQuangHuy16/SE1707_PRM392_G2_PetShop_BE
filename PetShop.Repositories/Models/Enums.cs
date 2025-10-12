namespace PetShop.Repositories.Models.Enums
{
    public enum UserRoleEnum
    {
        Customer = 0,
        Admin = 1
    }

    public enum OrderStatusEnum
    {
        Pending = 0,
        Paid = 1,
        Cancelled = 2
    }

    public enum PaymentMethodEnum
    {
        Cash = 0,
        CreditCard = 1,
        Momo = 2,
        ZaloPay = 3
    }

    public enum PaymentStatusEnum
    {
        Success = 0,
        Failed = 1,
        Pending = 2
    }
}
