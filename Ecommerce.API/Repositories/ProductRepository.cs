﻿using Asp.Versioning;
using AutoMapper.QueryableExtensions;
using Ecommerce.Infrastructure.Entity;
using Ecommerce.Infrastructure.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Globalization;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Ecommerce.API.Repositories
{
    public class ProductRepository : IProductRepository
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
                            .Include(p => p.StoreInventories)
                                .ThenInclude(si => si.StoreLocation)
                            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(include))
            {
                foreach (var prop in include.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(prop.Trim());
                }
            }

            return await query.ToListAsync();
        }
        public async Task<Product?> GetByIdAsync(Guid id, string? include = null)
        {
            var query = _context.Products
                 .Include(p => p.Category)
                            .Include(p => p.Manufacturer)
                            .Include(p => p.Discount)
                            .Include(p => p.Images)
                            .Include(p => p.StoreInventories)
                                .ThenInclude(si => si.StoreLocation)
                            .AsQueryable();
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
        public async Task<Product?> UpdateAsync(Guid id, Product product)
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
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Manufacturer)
                .Include(p => p.Discount)
                .Include(p => p.Images)
                .Include(p => p.StoreInventories)
                      .ThenInclude(si => si.StoreLocation)
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }


        public async Task<PaginationResponse<Product>> GetAllProductsPaginatedAsync(
                    ProductQueryParameters parameters,
                    CancellationToken cancellationToken = default)
        {
            var query = _context.Products
                         .Where(p => p.IsActive == true &&
                                     p.StoreInventories.Sum(si => si.Quantity) > 0)
                         .Include(p => p.Category)
                         .Include(p => p.Manufacturer)
                         .Include(p => p.Discount)
                         .Include(p => p.Images)
                         .Include(p => p.StoreInventories)
                             .ThenInclude(si => si.StoreLocation)
                         .AsQueryable();


            // Filter
            if (!string.IsNullOrEmpty(parameters.SearchTerm))
                query = query.Where(p => p.Name.Contains(parameters.SearchTerm));

            if (parameters.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == parameters.CategoryId);

            if (parameters.MinPrice.HasValue)
                query = query.Where(p => p.Price >= parameters.MinPrice);

            if (parameters.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= parameters.MaxPrice);

            if (parameters.IsActive.HasValue)
                query = query.Where(p => p.IsActive == parameters.IsActive);

            // Sort
            if (!string.IsNullOrEmpty(parameters.SortBy))
            {
                var propertyInfo = typeof(Product).GetProperty(parameters.SortBy);
                if (propertyInfo != null)
                {
                    if (parameters.SortOrder?.ToLower() == "desc")
                    {
                        query = query.OrderByDescending(p => EF.Property<object>(p, parameters.SortBy));
                    }
                    else
                    {
                        query = query.OrderBy(p => EF.Property<object>(p, parameters.SortBy));
                    }
                }
            }

            // Include custom relationships (if needed)
            if (!string.IsNullOrEmpty(parameters.Include))
            {
                foreach (var prop in parameters.Include.Split(','))
                {
                    query = query.Include(prop.Trim());
                }
            }

            // Pagination
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((parameters.PageIndex - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResponse<Product>(
                parameters.PageIndex,
                parameters.PageSize,
                totalCount,
                items
            );
        }
        public async Task<List<Product>> GetMostClickedProductsAsync(int topN, string? include = null)
        {
            var query = _context.Products
                .Where(p => p.StoreInventories.Any(si => si.Quantity > 0) && p.IsActive == true) 
                .Join(_context.ProductClickTrackings,
                    product => product.Id,
                    click => click.ProductId,
                    (product, click) => new
                    {
                        Product = product,
                        ClickCount = click.ClickCount
                    })
                .OrderByDescending(x => x.ClickCount) 
                .Take(topN)
                .Select(x => x.Product) 
                .Include(x => x.Images) 
                .Include(x => x.StoreInventories) 
                    .ThenInclude(x => x.StoreLocation)
                .AsQueryable();

            return await query.ToListAsync();
        }

        public async Task IncrementClickCountAsync(Guid productId)
        {
            try
            {
                var clickTracking = await _context.ProductClickTrackings
                    .FirstOrDefaultAsync(ct => ct.ProductId == productId);

                if (clickTracking == null)
                {
                    clickTracking = new ProductClickTracking
                    {
                        ProductId = productId,
                        ClickCount = 1
                    };
                    _context.ProductClickTrackings.Add(clickTracking);
                }
                else
                {
                    clickTracking.ClickCount++;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await operation();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

    }
}

