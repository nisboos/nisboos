namespace LinePayAPI.Models.Parameters
{
    public class PaymentRequestParameter
    {
        public string OrderId { get; set; }

        public List<PaymentProduct> Products { get; set; }
    }

    public class PaymentProduct
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
