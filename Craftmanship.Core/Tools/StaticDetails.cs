namespace Craftmanship.Core.Utility
{
    public static class StaticDetails
    {
        public const string Role_User_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";

        public const string StatusPending = "Väntande";
        public const string StatusInprocess = "Pågående tillverkning";
        public const string StatusCompleted = "Klar för leverans";
        public const string StatusShipped = "Skickad";
        public const string StatusCancelled = "Makulerad";
        public const string StatusRefunded = "Återbetalas";

        public const string PaymentStatusPending = "Väntande betalning";
        public const string PaymentStatusApproved = "Godkänd betalning";
        public const string PaymentStatusDelayedPayment = "Sen betalning godkänd";
        public const string PaymentStatusRejected = "Avvisad betalning";
    }
}
