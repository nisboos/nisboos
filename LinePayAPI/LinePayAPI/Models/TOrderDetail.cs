using System;
using System.Collections.Generic;

namespace LinePayAPI.Models;

public partial class TOrderDetail
{
    public int FId { get; set; }

    public string? FOrderGuid { get; set; }

    public string? FUserId { get; set; }

    public string? FPid { get; set; }

    public string? FName { get; set; }

    public int? FPrice { get; set; }

    public int? FQty { get; set; }

    public string? FIsApproved { get; set; }
}
