using DTOs.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng
        public DbSet<User> Users { get; set; }
        public DbSet<PaymentInfo> PaymentInfos { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<LabelClass> LabelClasses { get; set; }
        public DbSet<DataItem> DataItems { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Annotation> Annotations { get; set; }
        public DbSet<ReviewLog> ReviewLogs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<UserProjectStat> UserProjectStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình Quan hệ User (Tránh lỗi Multiple Cascade Paths) ---

            // 1. User - Project (Manager)
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Manager)
                .WithMany(u => u.ManagedProjects)
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict); // Xóa User không được xóa Project

            // 2. User - Assignment (Annotator)
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Annotator)
                .WithMany(u => u.Assignments)
                .HasForeignKey(a => a.AnnotatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. User - ReviewLog (Reviewer)
            modelBuilder.Entity<ReviewLog>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.ReviewsGiven)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. User - Invoice
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Annotator)
                .WithMany(u => u.Invoices)
                .HasForeignKey(i => i.AnnotatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. User - Stats
            modelBuilder.Entity<UserProjectStat>()
                .HasOne(s => s.User)
                .WithMany(u => u.ProjectStats)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- Các cấu hình khác ---

            // PaymentInfo: 1-1 với User
            modelBuilder.Entity<User>()
                .HasOne(u => u.PaymentInfo)
                .WithOne(p => p.User)
                .HasForeignKey<PaymentInfo>(p => p.UserId);
            modelBuilder.Entity<Annotation>()
                 .HasOne(a => a.LabelClass)
                 .WithMany()  
                 .HasForeignKey(a => a.LabelClassId)
                 .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invoice>()
                .Property(i => i.UnitPriceSnapshot)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);
        }
    }
}