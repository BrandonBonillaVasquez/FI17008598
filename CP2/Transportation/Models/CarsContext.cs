using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Transportation.Models;

public partial class CarsContext : DbContext
{
    public CarsContext()
    {
    }

    public CarsContext(DbContextOptions<CarsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<CarOption> CarOptions { get; set; }

    public virtual DbSet<CarPart> CarParts { get; set; }

    public virtual DbSet<CarVin> CarVins { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerOwnership> CustomerOwnerships { get; set; }

    public virtual DbSet<Dealer> Dealers { get; set; }

    public virtual DbSet<ManufacturePlant> ManufacturePlants { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=Cars.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandName)
                .HasColumnType("VARCHAR(50)")
                .HasColumnName("brand_name");
        });

        modelBuilder.Entity<CarOption>(entity =>
        {
            entity.HasKey(e => e.OptionSetId);

            entity.ToTable("Car_Options");

            entity.Property(e => e.OptionSetId).HasColumnName("option_set_id");
            entity.Property(e => e.ChassisId).HasColumnName("chassis_id");
            entity.Property(e => e.Color)
                .HasColumnType("VARCHAR(30)")
                .HasColumnName("color");
            entity.Property(e => e.EngineId).HasColumnName("engine_id");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.OptionSetPrice).HasColumnName("option_set_price");
            entity.Property(e => e.PremiumSoundId).HasColumnName("premium_sound_id");
            entity.Property(e => e.TransmissionId).HasColumnName("transmission_id");

            entity.HasOne(d => d.Chassis).WithMany(p => p.CarOptionChasses)
                .HasForeignKey(d => d.ChassisId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Engine).WithMany(p => p.CarOptionEngines)
                .HasForeignKey(d => d.EngineId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Model).WithMany(p => p.CarOptions).HasForeignKey(d => d.ModelId);

            entity.HasOne(d => d.PremiumSound).WithMany(p => p.CarOptionPremiumSounds).HasForeignKey(d => d.PremiumSoundId);

            entity.HasOne(d => d.Transmission).WithMany(p => p.CarOptionTransmissions)
                .HasForeignKey(d => d.TransmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CarPart>(entity =>
        {
            entity.HasKey(e => e.PartId);

            entity.ToTable("Car_Parts");

            entity.Property(e => e.PartId).HasColumnName("part_id");
            entity.Property(e => e.ManufactureEndDate)
                .HasColumnType("DATE")
                .HasColumnName("manufacture_end_date");
            entity.Property(e => e.ManufacturePlantId).HasColumnName("manufacture_plant_id");
            entity.Property(e => e.ManufactureStartDate)
                .HasColumnType("DATE")
                .HasColumnName("manufacture_start_date");
            entity.Property(e => e.PartName)
                .HasColumnType("VARCHAR(100)")
                .HasColumnName("part_name");
            entity.Property(e => e.PartRecall)
                .HasDefaultValue(0)
                .HasColumnName("part_recall");

            entity.HasOne(d => d.ManufacturePlant).WithMany(p => p.CarParts)
                .HasForeignKey(d => d.ManufacturePlantId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CarVin>(entity =>
        {
            entity.HasKey(e => e.Vin);

            entity.ToTable("Car_Vins");

            entity.Property(e => e.Vin).HasColumnName("vin");
            entity.Property(e => e.ManufacturedDate)
                .HasColumnType("DATE")
                .HasColumnName("manufactured_date");
            entity.Property(e => e.ManufacturedPlantId).HasColumnName("manufactured_plant_id");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.OptionSetId).HasColumnName("option_set_id");

            entity.HasOne(d => d.ManufacturedPlant).WithMany(p => p.CarVins)
                .HasForeignKey(d => d.ManufacturedPlantId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Model).WithMany(p => p.CarVins)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OptionSet).WithMany(p => p.CarVins)
                .HasForeignKey(d => d.OptionSetId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Birthdate)
                .HasColumnType("DATE")
                .HasColumnName("birthdate");
            entity.Property(e => e.Email)
                .HasColumnType("VARCHAR(128)")
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasColumnType("VARCHAR(50)")
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasColumnType("STRING")
                .HasColumnName("gender");
            entity.Property(e => e.HouseholdIncome).HasColumnName("household_income");
            entity.Property(e => e.LastName)
                .HasColumnType("VARCHAR(50)")
                .HasColumnName("last_name");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
        });

        modelBuilder.Entity<CustomerOwnership>(entity =>
        {
            entity.HasKey(e => new { e.CustomerId, e.Vin });

            entity.ToTable("Customer_Ownership");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Vin).HasColumnName("vin");
            entity.Property(e => e.DealerId).HasColumnName("dealer_id");
            entity.Property(e => e.PurchaseDate)
                .HasColumnType("DATE")
                .HasColumnName("purchase_date");
            entity.Property(e => e.PurchasePrice).HasColumnName("purchase_price");
            entity.Property(e => e.WaranteeExpireDate)
                .HasColumnType("DATE")
                .HasColumnName("warantee_expire_date");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerOwnerships)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Dealer).WithMany(p => p.CustomerOwnerships)
                .HasForeignKey(d => d.DealerId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.VinNavigation).WithMany(p => p.CustomerOwnerships)
                .HasForeignKey(d => d.Vin)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Dealer>(entity =>
        {
            entity.Property(e => e.DealerId).HasColumnName("dealer_id");
            entity.Property(e => e.DealerAddress)
                .HasColumnType("VARCHAR(100)")
                .HasColumnName("dealer_address");
            entity.Property(e => e.DealerName)
                .HasColumnType("VARCHAR(50)")
                .HasColumnName("dealer_name");

            entity.HasMany(d => d.Brands).WithMany(p => p.Dealers)
                .UsingEntity<Dictionary<string, object>>(
                    "DealerBrand",
                    r => r.HasOne<Brand>().WithMany()
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Dealer>().WithMany()
                        .HasForeignKey("DealerId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j =>
                    {
                        j.HasKey("DealerId", "BrandId");
                        j.ToTable("Dealer_Brand");
                        j.IndexerProperty<int>("DealerId").HasColumnName("dealer_id");
                        j.IndexerProperty<int>("BrandId").HasColumnName("brand_id");
                    });
        });

        modelBuilder.Entity<ManufacturePlant>(entity =>
        {
            entity.ToTable("Manufacture_Plant");

            entity.Property(e => e.ManufacturePlantId).HasColumnName("manufacture_plant_id");
            entity.Property(e => e.CompanyOwned).HasColumnName("company_owned");
            entity.Property(e => e.PlantLocation)
                .HasColumnType("VARCHAR(100)")
                .HasColumnName("plant_location");
            entity.Property(e => e.PlantName)
                .HasColumnType("VARCHAR(50)")
                .HasColumnName("plant_name");
            entity.Property(e => e.PlantType)
                .HasColumnType("VARCHAR (7)")
                .HasColumnName("plant_type");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.ModelBasePrice).HasColumnName("model_base_price");
            entity.Property(e => e.ModelName)
                .HasColumnType("VARCHAR(50)")
                .HasColumnName("model_name");

            entity.HasOne(d => d.Brand).WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
