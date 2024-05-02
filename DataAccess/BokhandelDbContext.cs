using System;
using System.Collections.Generic;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public partial class BokhandelDbContext : DbContext
{
    public BokhandelDbContext()
    {
    }

    public BokhandelDbContext(DbContextOptions<BokhandelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Publicer> Publicers { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    public virtual DbSet<VSalesPerStore> VSalesPerStores { get; set; }

    public virtual DbSet<VTitlesPerAuthor> VTitlesPerAuthors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=FROLIN;Initial Catalog=BokhandelDb;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authors__3214EC0781C0ADE7");

            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn13).HasName("PK__Books__3BF79E0300280B8A");

            entity.Property(e => e.Isbn13)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBN13");
            entity.Property(e => e.Language).HasMaxLength(60);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(120);

            entity.HasOne(d => d.Genre).WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_Id_Genre");

            entity.HasOne(d => d.Publicer).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublicerId)
                .HasConstraintName("FK_Id_Publicers");

            entity.HasMany(d => d.Authors).WithMany(p => p.BookIsbns)
                .UsingEntity<Dictionary<string, object>>(
                    "AuthorBook",
                    r => r.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AuthorId_Author"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookIsbn")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_BookId_Books"),
                    j =>
                    {
                        j.HasKey("BookIsbn", "AuthorId");
                        j.ToTable("AuthorBooks");
                        j.IndexerProperty<string>("BookIsbn")
                            .HasMaxLength(13)
                            .IsUnicode(false)
                            .IsFixedLength()
                            .HasColumnName("BookISBN");
                    });
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC0784105038");

            entity.Property(e => e.Email).HasMaxLength(120);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(10);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genres__3214EC073F24DEF0");

            entity.Property(e => e.Genre1)
                .HasMaxLength(120)
                .HasColumnName("Genre");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_CustomerId_Customers");

            entity.HasOne(d => d.SellingStoreNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.SellingStore)
                .HasConstraintName("FK_SellingStore_Stores");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.BookIsbn });

            entity.ToTable("Order_detail");

            entity.Property(e => e.BookIsbn)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("BookISBN");

            entity.HasOne(d => d.BookIsbnNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.BookIsbn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BookISBN_Books");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderId_Orders");
        });

        modelBuilder.Entity<Publicer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Publicer__3214EC07EC619E05");

            entity.Property(e => e.Name).HasMaxLength(120);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.Isbnid });

            entity.ToTable("Stock");

            entity.Property(e => e.Isbnid)
                .HasMaxLength(13)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ISBNID");

            entity.HasOne(d => d.Isbn).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.Isbnid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ISBNID_Books");

            entity.HasOne(d => d.Store).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StoreId_Stores");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stores__3214EC0789661777");

            entity.Property(e => e.Adress).HasMaxLength(120);
            entity.Property(e => e.City).HasMaxLength(60);
            entity.Property(e => e.Country).HasMaxLength(120);
            entity.Property(e => e.StoreName).HasMaxLength(120);
        });

        modelBuilder.Entity<VSalesPerStore>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vSalesPerStore");

            entity.Property(e => e.SalesThisMonth).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.SalesThisYear).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.SalesToday).HasColumnType("decimal(38, 2)");
            entity.Property(e => e.StoreName).HasMaxLength(120);
        });

        modelBuilder.Entity<VTitlesPerAuthor>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vTitlesPerAuthor");

            entity.Property(e => e.Age)
                .HasMaxLength(18)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(101);
            entity.Property(e => e.StockValue)
                .HasMaxLength(44)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
