using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public int? SellingStore { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Store? SellingStoreNavigation { get; set; }
}
