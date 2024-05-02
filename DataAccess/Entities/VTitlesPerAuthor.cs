using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class VTitlesPerAuthor
{
    public string FullName { get; set; } = null!;

    public string Age { get; set; } = null!;

    public int? NumberOfTitles { get; set; }

    public string StockValue { get; set; } = null!;
}
