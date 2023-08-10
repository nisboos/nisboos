using System;
using System.Collections.Generic;

namespace LinePayAPI.Models;

public partial class TProduct
{
    public int FId { get; set; }

    public string? FPid { get; set; }

    public string? FName { get; set; }

    public int? FPrice { get; set; }

    public string? FImg { get; set; }

    public string? FDescription { get; set; }
}
