using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Store.Web.App;

namespace Store.DTO.EF
{
    public class StoreDbContext : IdentityDbContext<User, UserRole, Guid>
    {
        public DbSet<BookDto> Books { get; set; }
        public DbSet<OrderDto> Orders { get; set; }
        public DbSet<OrderItemDto> OrderItems { get; set; }

        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            BuildBooks(modelBuilder);
            BuildOrderItems(modelBuilder);
            BuildOrders(modelBuilder);
        }
        private static void BuildBooks(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookDto>(action =>
            {
                action.Property(dto => dto.Isbn)
                      .HasMaxLength(17)
                      .IsRequired();

                action.Property(dto => dto.Title)
                      .IsRequired();

                action.Property(dto => dto.Price)
                      .HasColumnType("money");

                action.HasData
                (
                    new BookDto
                    {
                        Id = 1,
                        Isbn = "ISBN 11111-12341",
                        Title = "Clr via c#",
                        Author = "Джефри Рихтер",
                        Description = "description",
                        Price = 10m
                    },
                    new BookDto
                    {
                        Id = 2,
                        Isbn = "ISBN 11111-12342",
                        Title = "C# 4.0",
                        Author = "Герберт Шилдт",
                        Description = "description",
                        Price = 8m
                    },
                    new BookDto
                    {
                        Id = 3,
                        Isbn = "ISBN 11111-12343",
                        Title = "Clear Code",
                        Author = "Роберт Мартин",
                        Description = "description",
                        Price = 12m
                    }
                );
            });

        }
        private static void BuildOrderItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItemDto>(action =>
            {
                action.Property(dto => dto.Price)
                      .HasColumnType("money");

                action.HasOne(dto => dto.Order)
                      .WithMany(dto => dto.Items)
                      .IsRequired();
            });
        }
        private static void BuildOrders(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDto>(action =>
            {
                action.Property(dto => dto.CellPhone)
                      .HasMaxLength(17);
                action.Property(dto => dto.DeliveryPrice)
                      .HasColumnType("money");

                action.Property(dto => dto.DeliveryParameters)
                      .HasConversion(
                        value => JsonConvert.SerializeObject(value),
                        value => JsonConvert.DeserializeObject<Dictionary<string, string>>(value));

                action.Property(dto => dto.PaymentParameters)
                      .HasConversion(
                        value => JsonConvert.SerializeObject(value),
                        value => JsonConvert.DeserializeObject<Dictionary<string, string>>(value));
            });
        }

    }
}