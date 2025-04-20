using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Ecommerce.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net.WebSockets;
using System.Text.RegularExpressions;

namespace Ecommerce.API.Repositories
{
    public class ProductRepository: IProductRepository
    {
        private readonly EcommerceDbContext _context;
        public ProductRepository(EcommerceDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync(string? include = null)
        {
            var query = _context.Products
                            .Include(p => p.Category)
                            .Include(p => p.Manufacturer)
                            .Include(p => p.Discount)
                            .Include(p => p.Images)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(include))
            {
                foreach(var prop in include.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop.Trim());
                }
            }
           
            return await query.ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(Guid id, string? include = null)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(include))
            {
                foreach (var prop in include.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop.Trim());
                }
            }
            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            //await _context.Entry(product)
            //    .Reference(p => p.Category).LoadAsync();
            //await _context.Entry(product)
            //    .Reference(p => p.Manufacturer).LoadAsync();
            //await _context.Entry(product)
            //    .Reference(p => p.Discount).LoadAsync();
            return product;
        }
        public async Task<Product?> UpdateAsync(Guid id,Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return null;
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Product?> GetBySlugAsync(string slug)
            => await _context.Products.FirstOrDefaultAsync(p => p.Slug == slug);

    }
}
