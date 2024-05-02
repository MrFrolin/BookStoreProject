using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Stock
{
    public int StoreId { get; set; }

    public string Isbnid { get; set; } = null!;

    public int? StockBalance { get; set; }

    public virtual Book Isbn { get; set; } = null!;

    public virtual Store Store { get; set; } = null!;
}
