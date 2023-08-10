using System;
using System.Collections.Generic;

namespace LinePayAPI.Models;

public partial class TOrder
{
    public int FId { get; set; }

    public string? FOrderGuid { get; set; }

    public string? FUserId { get; set; }

    public string? FReceiver { get; set; }

    public string? FEmail { get; set; }

    public string? FAddress { get; set; }

    public DateTime? FDate { get; set; }

    public string? FTrancsationId { get; set; }

    public string? FPaid { get; set; }
}
