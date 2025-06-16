using Microsoft.EntityFrameworkCore;
using Project.Models;
using Project.Models.Clients;

namespace Project.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<IndividualClient> IndividualClients { get; set; } = null!;
    public DbSet<CompanyClient> CompanyClients { get; set; } = null!;
    public DbSet<SoftwareSystem> SoftwareSystems { get; set; } = null!;
    public DbSet<SoftwareVersion> Versions { get; set; } = null!;
    public DbSet<Discount> Discounts { get; set; } = null!;
    public DbSet<Contract> Contracts { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;
    
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Login)
            .IsUnique();
        
        modelBuilder.Entity<Client>()
            .HasDiscriminator<string>("ClientType")
            .HasValue<IndividualClient>("Individual")
            .HasValue<CompanyClient>("Company");
        
        modelBuilder.Entity<IndividualClient>(entity =>
        {
            entity.HasIndex(e => e.Pesel).IsUnique();
            entity.Property(e => e.Pesel).IsFixedLength();
        });

        modelBuilder.Entity<CompanyClient>(entity =>
        {
            entity.HasIndex(e => e.KrsNumber).IsUnique();
            entity.Property(e => e.KrsNumber).IsFixedLength();
        });

        modelBuilder.Entity<Client>(entity => { entity.HasIndex(e => e.Email).IsUnique(); });
        
        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Version)
            .WithMany(v => v.Contracts)
            .HasForeignKey(c => c.SoftwareVersionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        
        modelBuilder.Entity<Contract>()
            .Property(c => c.Status)
            .HasConversion<string>();
        modelBuilder.Entity<Employee>()
            .Property(e => e.Role)
            .HasConversion<string>();


        modelBuilder.Entity<IndividualClient>().HasData(
                new IndividualClient
                {
                    Id = 1,
                    FirstName = "Jan",
                    LastName = "Kowalski",
                    Pesel = "85010112345",
                    Address = "ul. Marszałkowska 1, 00-001 Warszawa",
                    Email = "jan.kowalski@email.com",
                    PhoneNumber = "+48123456789",
                    IsDeleted = false
                },
                new IndividualClient
                {
                    Id = 2,
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Pesel = "90020287654",
                    Address = "ul. Nowy Świat 15, 00-001 Warszawa",
                    Email = "anna.nowak@email.com",
                    PhoneNumber = "+48987654321",
                    IsDeleted = false
                },
                new IndividualClient
                {
                    Id = 3,
                    FirstName = "Piotr",
                    LastName = "Wiśniewski",
                    Pesel = "75121565432",
                    Address = "ul. Krakowskie Przedmieście 3, 31-013 Kraków",
                    Email = "piotr.wisniewski@email.com",
                    PhoneNumber = "+48555123456",
                    IsDeleted = false
                },
                new IndividualClient
                {
                    Id = 4,
                    FirstName = "Maria",
                    LastName = "Wójcik",
                    Pesel = "88030445678",
                    Address = "ul. Piotrkowska 100, 90-001 Łódź",
                    Email = "maria.wojcik@email.com",
                    PhoneNumber = "+48666789012",
                    IsDeleted = false
                },
                new IndividualClient
                {
                    Id = 5,
                    FirstName = "Tomasz",
                    LastName = "Kaczmarek",
                    Pesel = "92111098765",
                    Address = "ul. Świdnicka 5, 50-001 Wrocław",
                    Email = "tomasz.kaczmarek@email.com",
                    PhoneNumber = "+48777234567",
                    IsDeleted = false
                }
            );

        modelBuilder.Entity<CompanyClient>().HasData(
                new CompanyClient
                {
                    Id = 6,
                    CompanyName = "TechSoft Sp. z o.o.",
                    KrsNumber = "0000123456",
                    Address = "al. Jerozolimskie 123, 02-001 Warszawa",
                    Email = "kontakt@techsoft.pl",
                    PhoneNumber = "+48222345678"
                },
                new CompanyClient
                {
                    Id = 7,
                    CompanyName = "Global Solutions S.A.",
                    KrsNumber = "0000234567",
                    Address = "ul. Floriańska 12, 31-019 Kraków",
                    Email = "biuro@globalsolutions.pl",
                    PhoneNumber = "+48123456789"
                },
                new CompanyClient
                {
                    Id = 8,
                    CompanyName = "Innowacja Polska Sp. z o.o.",
                    KrsNumber = "0000345678",
                    Address = "ul. Narutowicza 88, 80-001 Gdańsk",
                    Email = "info@innowacja.pl",
                    PhoneNumber = "+48581234567"
                },
                new CompanyClient
                {
                    Id = 9,
                    CompanyName = "Digital Partners S.A.",
                    KrsNumber = "0000456789",
                    Address = "ul. Andersa 7, 61-001 Poznań",
                    Email = "contact@digitalpartners.pl",
                    PhoneNumber = "+48616789012"
                },
                new CompanyClient
                {
                    Id = 10,
                    CompanyName = "Future Tech Sp. z o.o.",
                    KrsNumber = "0000567890",
                    Address = "ul. Piłsudskiego 25, 90-001 Łódź",
                    Email = "hello@futuretech.pl",
                    PhoneNumber = "+48423456789"
                }
            );
            modelBuilder.Entity<SoftwareSystem>().HasData(
                new SoftwareSystem
                {
                    Id = 1,
                    Name = "AccountingPro",
                    Description = "Comprehensive accounting software for small to medium businesses",
                    Category = "Finance",
                    SubscriptionCost = 29.99m,
                    UpfrontCost = 299.99m,
                },
                new SoftwareSystem
                {
                    Id = 2,
                    Name = "EduManager",
                    Description = "Complete student management system for educational institutions",
                    Category = "Education",
                    SubscriptionCost = 49.99m,
                    UpfrontCost = null
                },
                new SoftwareSystem
                {
                    Id = 3,
                    Name = "InvoiceExpress",
                    Description = "Simple invoicing solution for freelancers and consultants",
                    Category = "Finance",
                    SubscriptionCost = 15.99m,
                    UpfrontCost = 159.99m
                },
                new SoftwareSystem
                {
                    Id = 4,
                    Name = "LearningHub",
                    Description = "E-learning platform with course management and analytics",
                    Category = "Education",
                    SubscriptionCost = null,
                    UpfrontCost = 1999.99m
                },
                new SoftwareSystem
                {
                    Id = 5,
                    Name = "InventoryMaster",
                    Description = "Advanced inventory management system for retail businesses",
                    Category = "Retail",
                    SubscriptionCost = 39.99m,
                    UpfrontCost = null
                }
            );
            modelBuilder.Entity<SoftwareVersion>().HasData(
                new SoftwareVersion { Id = 1, Name = "1.0.0", SoftwareSystemId = 1 },
                new SoftwareVersion { Id = 2, Name = "2.0.0", SoftwareSystemId = 1 },
                new SoftwareVersion { Id = 3, Name = "3.0.0", SoftwareSystemId = 1 , IsCurrent = true},
                
                new SoftwareVersion { Id = 4, Name = "1.0.0", SoftwareSystemId = 2 },
                new SoftwareVersion { Id = 5, Name = "1.5.0", SoftwareSystemId = 2 },
                new SoftwareVersion { Id = 6, Name = "2.0.0", SoftwareSystemId = 2 , IsCurrent = true},
                
                new SoftwareVersion { Id = 7, Name = "1.0.0", SoftwareSystemId = 3 },
                new SoftwareVersion { Id = 8, Name = "1.1.0", SoftwareSystemId = 3 },
                new SoftwareVersion { Id = 9, Name = "2.0.0", SoftwareSystemId = 3 , IsCurrent = true},
                
                new SoftwareVersion { Id = 10, Name = "1.0.0", SoftwareSystemId = 4 },
                new SoftwareVersion { Id = 11, Name = "2.0.0", SoftwareSystemId = 4 },
                new SoftwareVersion { Id = 12, Name = "2.1.0", SoftwareSystemId = 4 , IsCurrent = true},
                
                new SoftwareVersion { Id = 13, Name = "1.0.0", SoftwareSystemId = 5 },
                new SoftwareVersion { Id = 14, Name = "1.2.0", SoftwareSystemId = 5 },
                new SoftwareVersion { Id = 15, Name = "2.0.0", SoftwareSystemId = 5 , IsCurrent = true}
            );
            
        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Id = 1,
                Name = "Black Friday Discount",
                DiscountType = DiscountType.SubscriptionCost,
                Percentage = 10,
                StartDate = new DateTime(DateTime.Now.Year, 06, 13),
                EndDate = new DateTime(DateTime.Now.Year, 07, 30),
            },
            new Discount
            {
                Id = 2,
                Name = "New Year Discount",
                DiscountType = DiscountType.UpfrontCost,
                Percentage = 15,
                StartDate = new DateTime(DateTime.Now.Year, 06, 13),
                EndDate = new DateTime(DateTime.Now.Year, 07, 30),
            },
            new Discount
            {
                Id = 3,
                Name = "Black Friday Discount",
                DiscountType = DiscountType.UpfrontCost,
                Percentage = 25,
                StartDate = new DateTime(DateTime.Now.Year, 04, 13),
                EndDate = new DateTime(DateTime.Now.Year, 05, 30),
            }
        );
    }
}