using System;
using System.Collections.Generic;

namespace LinePayAPI.Models;

public partial class TMember
{
    public int FId { get; set; }

    public string FUserId { get; set; } = null!;

    public string FPwd { get; set; } = null!;

    public string FName { get; set; } = null!;

    public string FEmail { get; set; } = null!;

    public DateTime? FBirthday { get; set; }

    public string FClass { get; set; } = null!;

    public string? VerificationCode { get; set; }

    public bool IsVerified { get; set; }
}
