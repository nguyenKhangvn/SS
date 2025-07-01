using System;

namespace Ecommerce.API.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EcommerceDbContext _context;

        public PaymentRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Payment>> GetAllAsync(Guid userId)
        {
            return await _context.Payments
                .Include(o => o.Order)
                .Where(o => o.Order != null && o.Order.UserId == userId)
                .ToListAsync();
        }

        public async Task<Payment?> GetByOrderIdAsync(Guid id)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == id); // Use async method
            return payment;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null) return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Payment?> GetByIdAsync(Guid id)
        {
            return await _context.Payments.FindAsync(id);
        }
    }
}
