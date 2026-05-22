using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Data;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;

namespace TrainTicketApi.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Customer>> GetAllAsync() =>
        _context.Customers.AsNoTracking().OrderBy(x => x.CustomerId).ToListAsync();

    public Task<Customer?> GetByIdAsync(int customerId) =>
        _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);

    public Task<bool> ExistsDuplicateAsync(string phoneNumber, string email, string identityNumber, int? excludeCustomerId = null) =>
        _context.Customers.AnyAsync(x =>
            (x.PhoneNumber == phoneNumber || x.Email == email || x.IdentityNumber == identityNumber) &&
            (!excludeCustomerId.HasValue || x.CustomerId != excludeCustomerId.Value));

    public async Task<Customer> CreateAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return customer;
    }

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        var existing = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customer.CustomerId);
        if (existing is null)
        {
            return null;
        }

        existing.FullName = customer.FullName;
        existing.PhoneNumber = customer.PhoneNumber;
        existing.Email = customer.Email;
        existing.IdentityNumber = customer.IdentityNumber;
        existing.Address = customer.Address;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int customerId)
    {
        var entity = await _context.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId);
        if (entity is null)
        {
            return false;
        }

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
