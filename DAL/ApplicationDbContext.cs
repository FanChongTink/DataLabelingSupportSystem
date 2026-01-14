using Microsoft.EntityFrameworkCore;
using DTOs.Entities;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<LabelClass> LabelClasses { get; set; }
        public DbSet<DataItem> DataItems { get; set; }
        public DbSet<Annotation> Annotations { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- CẤU HÌNH ĐỂ TRÁNH LỖI MULTIPLE CASCADE PATHS ---

            // 1. Ngắt cascade delete giữa LabelClass và Annotation (Sửa lỗi bạn đang gặp)
            modelBuilder.Entity<Annotation>()
                .HasOne(a => a.LabelClass)
                .WithMany()
                .HasForeignKey(a => a.LabelClassId)
                .OnDelete(DeleteBehavior.Restrict); // Hoặc DeleteBehavior.NoAction

            // 2. Giữ nguyên các cấu hình Restrict User trước đó để tránh vòng lặp với bảng User
            modelBuilder.Entity<DataItem>()
                .HasOne(d => d.Annotator)
                .WithMany(u => u.AssignedDataItems)
                .HasForeignKey(d => d.AnnotatorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DataItem>()
                .HasOne(d => d.Reviewer)
                .WithMany(u => u.ReviewedDataItems)
                .HasForeignKey(d => d.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Manager)
                .WithMany(u => u.ManagedProjects)
                .HasForeignKey(p => p.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}