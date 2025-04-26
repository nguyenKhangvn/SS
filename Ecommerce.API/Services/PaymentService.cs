using Ecommerce.Infrastructure.Models.Dtos;

namespace Ecommerce.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PaymentDto>> GetAllAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentDto>>(payments);
        }

        public async Task<PaymentDto?> GetByIdAsync(Guid id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto?> GetByOrderIdAsync(Guid orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            return payment == null ? null : _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> CreateAsync(PaymentDto dto)
        {
            var payment = _mapper.Map<Payment>(dto);
            var created = await _paymentRepository.CreateAsync(payment);
            return _mapper.Map<PaymentDto>(created);
        }

        public async Task<PaymentDto?> UpdateAsync(PaymentDto dto)
        {
            var existing = await _paymentRepository.GetByIdAsync(dto.Id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _paymentRepository.UpdateAsync(existing);
            return _mapper.Map<PaymentDto>(updated);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _paymentRepository.DeleteAsync(id);
        }
    }

}
