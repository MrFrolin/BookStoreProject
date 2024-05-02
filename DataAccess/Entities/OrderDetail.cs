using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class OrderDetail
{
    public int OrderId { get; set; }

    public string BookIsbn { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Book BookIsbnNavigation { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
