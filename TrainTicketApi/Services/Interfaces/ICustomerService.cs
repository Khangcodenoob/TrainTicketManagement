using TrainTicketApi.DTOs.Customers;

namespace TrainTicketApi.Services.Interfaces;

public interface ICustomerService
{
    Task<List<CustomerResponseDto>> GetAllAsync();
    Task<ServiceResult<CustomerResponseDto>> GetByIdAsync(int customerId);
    Task<ServiceResult<CustomerResponseDto>> CreateAsync(CustomerRequestDto request);
    Task<ServiceResult<CustomerResponseDto>> UpdateAsync(int customerId, CustomerRequestDto request);
    Task<ServiceResult<object>> DeleteAsync(int customerId);
}
