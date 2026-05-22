using TrainTicketApi.DTOs.Customers;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<List<CustomerResponseDto>> GetAllAsync() =>
        (await _customerRepository.GetAllAsync()).Select(MapResponse).ToList();

    public async Task<ServiceResult<CustomerResponseDto>> GetByIdAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        return customer is null
            ? ServiceResult<CustomerResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y khÃ¡ch hÃ ng.")
            : ServiceResult<CustomerResponseDto>.Ok(MapResponse(customer), "Láº¥y thÃ´ng tin khÃ¡ch hÃ ng thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<CustomerResponseDto>> CreateAsync(CustomerRequestDto request)
    {
        var duplicate = await _customerRepository.ExistsDuplicateAsync(request.PhoneNumber, request.Email, request.IdentityNumber);
        if (duplicate)
        {
            return ServiceResult<CustomerResponseDto>.Fail(StatusCodes.Status400BadRequest, "Sá»‘ Ä‘iá»‡n thoáº¡i, email hoáº·c CCCD Ä‘Ã£ tá»“n táº¡i.");
        }

        var created = await _customerRepository.CreateAsync(new Customer
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email.Trim(),
            IdentityNumber = request.IdentityNumber.Trim(),
            Address = request.Address?.Trim()
        });

        return ServiceResult<CustomerResponseDto>.Created(MapResponse(created), "Táº¡o khÃ¡ch hÃ ng thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<CustomerResponseDto>> UpdateAsync(int customerId, CustomerRequestDto request)
    {
        var duplicate = await _customerRepository.ExistsDuplicateAsync(request.PhoneNumber, request.Email, request.IdentityNumber, customerId);
        if (duplicate)
        {
            return ServiceResult<CustomerResponseDto>.Fail(StatusCodes.Status400BadRequest, "Sá»‘ Ä‘iá»‡n thoáº¡i, email hoáº·c CCCD Ä‘Ã£ tá»“n táº¡i.");
        }

        var updated = await _customerRepository.UpdateAsync(new Customer
        {
            CustomerId = customerId,
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email.Trim(),
            IdentityNumber = request.IdentityNumber.Trim(),
            Address = request.Address?.Trim()
        });

        return updated is null
            ? ServiceResult<CustomerResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y khÃ¡ch hÃ ng.")
            : ServiceResult<CustomerResponseDto>.Ok(MapResponse(updated), "Cáº­p nháº­t khÃ¡ch hÃ ng thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int customerId)
    {
        var deleted = await _customerRepository.DeleteAsync(customerId);
        return deleted
            ? ServiceResult<object>.Ok(null, "XÃ³a khÃ¡ch hÃ ng thÃ nh cÃ´ng.")
            : ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y khÃ¡ch hÃ ng.");
    }

    private static CustomerResponseDto MapResponse(Customer customer) =>
        new()
        {
            CustomerId = customer.CustomerId,
            FullName = customer.FullName,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            IdentityNumber = customer.IdentityNumber,
            Address = customer.Address
        };
}
