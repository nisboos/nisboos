namespace LinePayAPI.Dtos.Request
{
    public class PaymentRequestDto
    {
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string OrderId { get; set; }
        public List<PackageDto> Packages { get; set; }
        public RedirectUrlsDto RedirectUrls { get; set; }
    }
    public class PackageDto
    {
        public string Id { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public List<LinePayProductDto> Products { get; set; }
    }
    public class LinePayProductDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
    }



    public class RedirectUrlsDto
    {
        public string ConfirmUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
