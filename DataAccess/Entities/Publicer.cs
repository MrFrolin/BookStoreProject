using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Publicer
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
