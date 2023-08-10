using System;
using System.Collections.Generic;

namespace LinePayAPI.Models;

public partial class TShoppingCar
{
    public int Fid { get; set; }

    public string FUserId { get; set; } = null!;

    public string FPid { get; set; } = null!;

    public string FName { get; set; } = null!;

    public int FPrice { get; set; }

    public int FQty { get; set; }

    public string FIsApproved { get; set; } = null!;
}
