﻿using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllAsync();
        Task<OrderDto?> GetByIdAsync(Guid id);
        Task<OrderDto> CreateAsync(OrderDto dto);
        Task<OrderDto> UpdateAsync(OrderDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<OrderDto?> UpdateStatusAsync(Guid id, string status);
        Task<IEnumerable<OrderDto>> GetAllByUserId(Guid id);
        Task<OrderDto> GetOrderByOrderCode(string orderCode);
    }
}
