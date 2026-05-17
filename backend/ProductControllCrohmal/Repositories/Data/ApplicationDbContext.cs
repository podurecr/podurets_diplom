using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Batch> Batches => Set<Batch>();
        public DbSet<QualityParameter> QualityParameters => Set<QualityParameter>();
        public DbSet<ProductQualitySpecification> ProductQualitySpecifications => Set<ProductQualitySpecification>();
        public DbSet<AnalysisResult> AnalysisResults => Set<AnalysisResult>();
        public DbSet<QualityAssessment> QualityAssessments => Set<QualityAssessment>();
        public DbSet<QualityCertificate> QualityCertificates => Set<QualityCertificate>();
        public DbSet<ShipmentDecision> ShipmentDecisions => Set<ShipmentDecision>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.FullName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Login)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.HasIndex(x => x.Login)
                    .IsUnique();

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.HasOne(x => x.Role)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Code)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(x => x.Name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.Unit)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(x => x.Code)
                    .IsUnique();
            });

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.BatchNumber)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Quantity)
                    .HasPrecision(18, 3);

                entity.Property(x => x.Unit)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.HasIndex(x => x.BatchNumber)
                    .IsUnique();

                entity.HasOne(x => x.Product)
                    .WithMany(x => x.Batches)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany(x => x.CreatedBatches)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QualityParameter>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Unit)
                    .HasMaxLength(30)
                    .IsRequired();
            });

            modelBuilder.Entity<ProductQualitySpecification>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.MinValue)
                    .HasPrecision(18, 3);

                entity.Property(x => x.MaxValue)
                    .HasPrecision(18, 3);

                entity.Property(x => x.TextNorm)
                    .HasMaxLength(300);

                entity.HasOne(x => x.Product)
                    .WithMany(x => x.QualitySpecifications)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.QualityParameter)
                    .WithMany(x => x.ProductQualitySpecifications)
                    .HasForeignKey(x => x.QualityParameterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AnalysisResult>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.NumericValue)
                    .HasPrecision(18, 3);

                entity.Property(x => x.TextValue)
                    .HasMaxLength(300);

                entity.Property(x => x.Comment)
                    .HasMaxLength(500);

                entity.HasOne(x => x.Batch)
                    .WithMany(x => x.AnalysisResults)
                    .HasForeignKey(x => x.BatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.QualityParameter)
                    .WithMany(x => x.AnalysisResults)
                    .HasForeignKey(x => x.QualityParameterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.EnteredByUser)
                    .WithMany(x => x.AnalysisResults)
                    .HasForeignKey(x => x.EnteredByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QualityAssessment>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Conclusion)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.HasIndex(x => x.BatchId)
                    .IsUnique();

                entity.HasOne(x => x.Batch)
                    .WithOne(x => x.QualityAssessment)
                    .HasForeignKey<QualityAssessment>(x => x.BatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.AssessedByUser)
                    .WithMany(x => x.QualityAssessments)
                    .HasForeignKey(x => x.AssessedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<QualityCertificate>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.CertificateNumber)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Conclusion)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(x => x.PdfPath)
                    .HasMaxLength(500);

                entity.HasIndex(x => x.CertificateNumber)
                    .IsUnique();

                entity.HasIndex(x => x.BatchId)
                    .IsUnique();

                entity.HasOne(x => x.Batch)
                    .WithOne(x => x.QualityCertificate)
                    .HasForeignKey<QualityCertificate>(x => x.BatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany(x => x.QualityCertificates)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ShipmentDecision>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.DecisionText)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.HasIndex(x => x.BatchId)
                    .IsUnique();

                entity.HasOne(x => x.Batch)
                    .WithOne(x => x.ShipmentDecision)
                    .HasForeignKey<ShipmentDecision>(x => x.BatchId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.CreatedByUser)
                    .WithMany(x => x.ShipmentDecisions)
                    .HasForeignKey(x => x.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}