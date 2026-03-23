using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<WorkFlowDefinition> WorkFlowDefinitions { get; set; } = null!;
        public DbSet<Node> Nodes { get; set; } = null!;
        public DbSet<Edge> Edges { get; set; } = null!;
        public DbSet<WorkFlowInstance> WorkFlowInstances { get; set; } = null!;
        public DbSet<WorkFlowInstanceHistory> WorkFlowInstanceHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // UserRole → User
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserRole → Role
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Node → WorkFlowDefinition
            modelBuilder.Entity<Node>()
                .HasOne(n => n.WorkFlowDefinition)
                .WithMany(w => w.Nodes)
                .HasForeignKey(n => n.WorkFlowDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Edge → SourceNode
            modelBuilder.Entity<Edge>()
                .HasOne(e => e.SourceNode)
                .WithMany(n => n.OutgoingEdges)
                .HasForeignKey(e => e.NodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Edge → TargetNode (no collection on Node side to avoid cycle)
            modelBuilder.Entity<Edge>()
                .HasOne(e => e.TargetNode)
                .WithMany()
                .HasForeignKey(e => e.TargetId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkFlowInstance → WorkFlowDefinition
            modelBuilder.Entity<WorkFlowInstance>()
                .HasOne(wi => wi.WorkFlowDefinition)
                .WithMany(w => w.Instances)
                .HasForeignKey(wi => wi.WorkFlowDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkFlowInstance → CurrentNode
            modelBuilder.Entity<WorkFlowInstance>()
                .HasOne(wi => wi.CurrentNode)
                .WithMany()
                .HasForeignKey(wi => wi.NodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkFlowInstanceHistory → WorkFlowInstance
            modelBuilder.Entity<WorkFlowInstanceHistory>()
                .HasOne(h => h.WorkFlowInstance)
                .WithMany()
                .HasForeignKey(h => h.WorkFlowInstanceId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkFlowInstanceHistory → FromNode
            modelBuilder.Entity<WorkFlowInstanceHistory>()
                .HasOne(h => h.FromNode)
                .WithMany()
                .HasForeignKey(h => h.FromNodeId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkFlowInstanceHistory → ToNode
            modelBuilder.Entity<WorkFlowInstanceHistory>()
                .HasOne(h => h.ToNode)
                .WithMany()
                .HasForeignKey(h => h.ToNodeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}