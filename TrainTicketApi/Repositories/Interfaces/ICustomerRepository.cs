using TrainTicketApi.Models;

namespace TrainTicketApi.Repositories.Interfaces;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int customerId);
    Task<bool> ExistsDuplicateAsync(string phoneNumber, string email, string identityNumber, int? excludeCustomerId = null);
    Task<Customer> CreateAsync(Customer customer);
    Task<Customer?> UpdateAsync(Customer customer);
    Task<bool> DeleteAsync(int customerId);
}
