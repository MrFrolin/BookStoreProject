using System;
using System.Collections.Generic;

namespace DataAccess.Entities;

public partial class Genre
{
    public int Id { get; set; }

    public string? Genre1 { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
