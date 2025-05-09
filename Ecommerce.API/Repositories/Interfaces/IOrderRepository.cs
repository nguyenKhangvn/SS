﻿using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> CreateAsync(Order order);
        Task<Order> UpdateAsync(Order order);
        Task<bool> DeleteAsync(Guid id);
        Task<Order?> UpdateStatusAsync(Guid id, string Status);
        Task<IEnumerable<Order>> GetAllByUserId(Guid id);
        Task<Order> GetOrderByOrderCode(string orderCode);
    }
}
