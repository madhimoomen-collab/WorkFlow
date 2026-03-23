using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Domain.Interface;
using Domain.Models;
using Data.Context;
using System.Linq.Expressions;

namespace Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ── Base query: ALWAYS excludes soft-deleted records ──────────────────
        private IQueryable<T> ActiveQuery => _dbSet.Where(e => !e.IsDeleted);

        // ── GetByIdAsync ──────────────────────────────────────────────────────
        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await ActiveQuery.FirstOrDefaultAsync(e => e.Id == id);
        }

        // ── GetAllAsync ───────────────────────────────────────────────────────
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await ActiveQuery.ToListAsync();
        }

        // ── FindAsync (simple predicate) ──────────────────────────────────────
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await ActiveQuery.Where(predicate).ToListAsync();
        }

        // ── FindAsync (predicate + eager-load includes array) ─────────────────
        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = ActiveQuery;
            foreach (var include in includes)
                query = query.Include(include);
            return await query.Where(predicate).ToListAsync();
        }

        // ── FindAsync (full overload — used by GetListGenericHandler) ─────────
        // This is the one called by all controllers via MediatR GetListGenericQuery.
        // Previously started from raw _dbSet, so deleted records leaked through.
        public async Task<IEnumerable<T>> FindAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = ActiveQuery;   // ← was: _dbSet (the bug)

            if (includes != null)
                query = includes(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        // ── GetAsync (used by GetGenericHandler) ──────────────────────────────
        public async Task<T?> GetAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object?>>? includes = null)
        {
            IQueryable<T> query = ActiveQuery;   // ← was: _dbSet (the bug)
            if (includes != null)
                query = includes(query);
            return await query.FirstOrDefaultAsync(predicate);
        }

        // ── AddAsync ──────────────────────────────────────────────────────────
        public async Task<T> AddAsync(T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.IsDeleted = false;
            await _dbSet.AddAsync(entity);
            return entity;
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────
        public async Task<T> UpdateAsync(T entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        // ── DeleteAsync — soft-delete + cascade ───────────────────────────────
        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            if (entity == null) return false;

            var now = DateTime.UtcNow;
            entity.IsDeleted = true;
            entity.UpdatedAt = now;
            _dbSet.Update(entity);

            // WorkFlowDefinition → cascade to Nodes → cascade to their Edges
            if (entity is WorkFlowDefinition wf)
            {
                var nodes = await _context.Set<Node>()
                    .Where(n => n.WorkFlowDefinitionId == wf.Id && !n.IsDeleted)
                    .ToListAsync();

                foreach (var node in nodes)
                {
                    node.IsDeleted = true;
                    node.UpdatedAt = now;
                    await SoftDeleteEdgesForNodeAsync(node.Id, now);
                }
            }

            // Node → cascade to all connected Edges (source or target)
            if (entity is Node node2)
            {
                await SoftDeleteEdgesForNodeAsync(node2.Id, now);
            }

            return true;
        }

        // ── SaveChangesAsync ──────────────────────────────────────────────────
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // ── Private helper ────────────────────────────────────────────────────
        private async Task SoftDeleteEdgesForNodeAsync(Guid nodeId, DateTime now)
        {
            var edges = await _context.Set<Edge>()
                .Where(e => (e.NodeId == nodeId || e.TargetId == nodeId) && !e.IsDeleted)
                .ToListAsync();

            foreach (var edge in edges)
            {
                edge.IsDeleted = true;
                edge.UpdatedAt = now;
            }
        }
    }
}