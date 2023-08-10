using LinePayAPI.Dtos;
using LinePayAPI.Domain;
using Microsoft.AspNetCore.Mvc;
using LinePayAPI.Dtos.Confirm;
using LinePayAPI.Dtos.Request;
using LinePayAPI.Models;
using LinePayAPI.Models.Parameters;

namespace LinePayAPI.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class LinePayController : ControllerBase
    {
        
        private readonly LinePayService _linePayService;
        private readonly ProjectContext _dbContext;
        public LinePayController(ProjectContext context)
        {
            _dbContext = context;
            _linePayService = new LinePayService();
        }
        //https://localhost:8080/api/LinePay/Create
        //從資料庫撈資料傳給linepay api資料
        [HttpPost("Create")]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] PaymentRequestParameter orderRequest)
        {
            //先檢查顧客是否有帶入參數與產品id與數量
            if (orderRequest == null)
            {
                return BadRequest("沒有必要參數");
            }

            if (!orderRequest.Products.Any())
            {
                return BadRequest("無此產品");
            }

            var linePayProducts = new List<LinePayProductDto>();
            int totalAmount = 0;

            foreach (var product in orderRequest.Products)
            {
                //比對進來的ProductId與T.Product的FPid是否相同
                var productDetail = _dbContext.TProducts.Where(p => p.FPid == product.ProductId).FirstOrDefault();

                if (productDetail != null)
                {
                    //若裡面沒有值則將值從資料庫帶入欄位
                    linePayProducts.Add(new LinePayProductDto
                    {
                        Name = productDetail.FName,
                        Quantity = product.Quantity,
                        Price = productDetail.FPrice ?? 0
                    });
                    totalAmount += (productDetail.FPrice ?? 0) * product.Quantity;
                }
            }
            //帶入上方的totalAmount&linePayProducts或是其他預設輸出值
            PaymentRequestDto dto = new PaymentRequestDto
            {
                Amount = totalAmount,
                Currency = "TWD",
                OrderId = orderRequest.OrderId, //string的guid型態
                Packages = new List<PackageDto>
                {
                    new PackageDto
                    {
                        Id = DateTime.Now.ToString("yyyyMMddHHmm"),
                        Amount = totalAmount,
                        Products = linePayProducts
                    }
                },
                RedirectUrls = new RedirectUrlsDto
                {
                    ConfirmUrl = "https://localhost:44369/Payment/Confirm",
                    CancelUrl = "https://6ddcf789.ngrok.io/cancelUrl"
                }
            };
            //DSqlWeek15MdfContext db = new DSqlWeek15MdfContext();
            ////TODO: 紀錄 transaction id
            //var ResponseInfoDto = new ResponseInfoDto();
            //if (ResponseInfoDto != null)
            //{
            //    // 取得回傳的 transactionId
            //    string transactionId = ResponseInfoDto.TransactionId;

            //    // 將 transactionId 儲存到資料庫
            //    // 假設您的 tOrderDetail 資料表中有一個叫做 transationId 的欄位
            //    var order = db.TOrders.FirstOrDefault(o => o.FOrderGuid == orderRequest.OrderId);
            //    if (order != null)
            //    {
            //        order.TransationId = transactionId;
            //        _dbContext.SaveChanges();
            //    }
            //}
            //打入linepay api
            return await _linePayService.SendPaymentRequest(dto);
        }

        //https://localhost:8080/api/LinePay/Confirm?transactionId=2023071201851074310&orderId=order504ac11a-1888-4410-89b2-75382fef61b3
        [HttpPost("Confirm")]
        public async Task<PaymentConfirmResponseDto>ConfirmPayment([FromQuery] string transactionId, [FromQuery] string orderId, PaymentConfirmDto dto)
        {
            return await _linePayService.ConfirmPayment(transactionId, orderId, dto);
        }
        //https://localhost:8080/api/LinePay/Cancel?transactionId=2023071201851074310&orderId=order504ac11a-1888-4410-89b2-75382fef61b3
        [HttpGet("Cancel")]
        public void CancelTransaction([FromQuery] string transactionId)
        {
            _linePayService.TransactionCancel(transactionId);
        }
    }
}
