using System;
using System.Collections.Generic;

namespace LinePayAPI.Models;

public partial class Tmyf
{
    public int FId { get; set; }

    public string CId { get; set; } = null!;

    public string CImg { get; set; } = null!;

    public string CName { get; set; } = null!;

    public string CGender { get; set; } = null!;

    public string CStar { get; set; } = null!;

    public string CInterest { get; set; } = null!;

    public string CItem { get; set; } = null!;

    public string CYears { get; set; } = null!;

    public string? CUserId { get; set; }
}
