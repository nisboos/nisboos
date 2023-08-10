using System;
using System.Collections.Generic;

namespace LinePayAPI.Models;

public partial class BookCoach
{
    public int FId { get; set; }

    public string FUserId { get; set; } = null!;

    public string CId { get; set; } = null!;

    public string CName { get; set; } = null!;

    public DateTime? CDate { get; set; }

    public TimeSpan? CTime { get; set; }
}
