using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Entities;

public partial class Book
{
    public string Isbn13 { get; set; } = null!;

    public string? Title { get; set; }

    public string? Language { get; set; }

    public decimal? Price { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public int? GenreId { get; set; }

    public int? PublicerId { get; set; }

    public virtual Genre? Genre { get; set; } = new();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Publicer? Publicer { get; set; } = new();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}
