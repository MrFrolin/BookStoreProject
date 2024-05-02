using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class VSalesPerStore
{
    public string? StoreName { get; set; }

    public decimal? SalesToday { get; set; }

    public decimal? SalesThisMonth { get; set; }

    public decimal? SalesThisYear { get; set; }
}
