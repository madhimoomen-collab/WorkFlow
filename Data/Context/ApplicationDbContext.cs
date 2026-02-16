using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<WorkFlowDefinition> WorkFlowDefinitions { get; set; } = null!;
        public DbSet<Node> Nodes { get; set; } = null!;
        public DbSet<Edge> Edges { get; set; } = null!;
        public DbSet<WorkFlowInstance> WorkFlowInstances { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints here if needed
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Node>()
                .HasOne(n => n.WorkFlowDefinition)
                .WithMany(w => w.Nodes)
                .HasForeignKey(n => n.WorkFlowDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkFlowInstance>()
                .HasOne(wi => wi.WorkFlowDefinition)
                .WithMany(w => w.Instances)
                .HasForeignKey(wi => wi.WorkFlowDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkFlowInstance>()
                .HasOne(wi => wi.CurrentNode)
                .WithMany()
                .HasForeignKey(wi => wi.NodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}