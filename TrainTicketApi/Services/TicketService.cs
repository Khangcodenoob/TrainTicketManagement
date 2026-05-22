using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TrainTicketApi.Data;
using TrainTicketApi.DTOs.Tickets;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ITrainTripRepository _trainTripRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TicketService(
        ITicketRepository ticketRepository,
        ITrainTripRepository trainTripRepository,
        ICustomerRepository customerRepository,
        AppDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        _ticketRepository = ticketRepository;
        _trainTripRepository = trainTripRepository;
        _customerRepository = customerRepository;
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<TicketResponseDto>> GetAllAsync() =>
        (await _ticketRepository.GetAllAsync()).Select(MapResponse).ToList();

    public async Task<List<TicketResponseDto>> SearchAsync(string? keyword, string? ticketStatus, string? paymentStatus)
    {
        var query = _context.Tickets.Include(t => t.Customer).Include(t => t.TrainTrip).AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLower();
            query = query.Where(t => t.TicketCode.ToLower().Contains(kw) || 
                                     (t.Customer != null && t.Customer.FullName.ToLower().Contains(kw)) ||
                                     (t.Customer != null && t.Customer.PhoneNumber.ToLower().Contains(kw)) ||
                                     (t.TrainTrip != null && t.TrainTrip.TrainCode.ToLower().Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(ticketStatus))
            query = query.Where(t => t.TicketStatus.Equals(ticketStatus));

        if (!string.IsNullOrWhiteSpace(paymentStatus))
            query = query.Where(t => t.PaymentStatus.Equals(paymentStatus));

        var tickets = await query.OrderByDescending(t => t.BookingDate).ToListAsync();
        return tickets.Select(MapResponse).ToList();
    }


    public async Task<ServiceResult<TicketResponseDto>> GetByIdAsync(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        return ticket is null
            ? ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y vÃ© tÃ u.")
            : ServiceResult<TicketResponseDto>.Ok(MapResponse(ticket), "Láº¥y thÃ´ng tin vÃ© tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<TicketResponseDto>> CreateAsync(CreateTicketRequestDto request)
    {
        var trainTrip = await _trainTripRepository.GetByIdAsync(request.TrainTripId);
        if (trainTrip is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status400BadRequest, "TrainTripId khÃ´ng tá»“n táº¡i.");
        }

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
        if (customer is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status400BadRequest, "CustomerId khÃ´ng tá»“n táº¡i.");
        }

        if (trainTrip.DepartureTime <= DateTime.UtcNow)
        {
            return ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status400BadRequest, "KhÃ´ng thá»ƒ Ä‘áº·t vÃ© cho chuyáº¿n tÃ u Ä‘Ã£ khá»Ÿi hÃ nh.");
        }

        if (trainTrip.AvailableSeats <= 0)
        {
            return ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status400BadRequest, "Chuyáº¿n tÃ u Ä‘Ã£ háº¿t gháº¿.");
        }

        var seatExists = await _ticketRepository.ExistsSeatInTripAsync(request.TrainTripId, request.SeatNumber.Trim());
        if (seatExists)
        {
            return ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status400BadRequest, "Sá»‘ gháº¿ Ä‘Ã£ Ä‘Æ°á»£c Ä‘áº·t trong chuyáº¿n tÃ u nÃ y.");
        }

        var bookingDate = DateTime.UtcNow;
        var code = await GenerateTicketCodeAsync(bookingDate);
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

        var created = await _ticketRepository.CreateAsync(new Ticket
        {
            TicketCode = code,
            TrainTripId = request.TrainTripId,
            CustomerId = request.CustomerId,
            SeatNumber = request.SeatNumber.Trim().ToUpperInvariant(),
            Price = request.Price ?? trainTrip.BaseTicketPrice,
            BookingDate = bookingDate,
            PaymentStatus = string.IsNullOrWhiteSpace(request.PaymentMethod) ? "Unpaid" : "Paid",
            PaymentMethod = request.PaymentMethod,
            PaidAt = string.IsNullOrWhiteSpace(request.PaymentMethod) ? null : bookingDate,
            TicketStatus = "Pending",
            CreatedBy = userName
        });

        trainTrip.AvailableSeats -= 1;
        await _trainTripRepository.UpdateAsync(trainTrip);
        await WriteAuditLogAsync("Create", "Ticket", created.TicketId.ToString(), $"TicketCode={created.TicketCode}");

        return ServiceResult<TicketResponseDto>.Created(MapResponse(created), "Äáº·t vÃ© tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<TicketResponseDto>> UpdateAsync(int ticketId, UpdateTicketRequestDto request)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            return ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y vÃ© tÃ u.");
        }

        ticket.PaymentStatus = request.PaymentStatus.Trim();
        ticket.TicketStatus = request.TicketStatus.Trim();
        var updated = await _ticketRepository.UpdateAsync(ticket);

        return updated is null
            ? ServiceResult<TicketResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y vÃ© tÃ u.")
            : ServiceResult<TicketResponseDto>.Ok(MapResponse(updated), "Cáº­p nháº­t vÃ© tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<object>> CancelAsync(int ticketId, CancelTicketRequestDto request)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Không tìm thấy vé tàu.");
        }

        if (ticket.TicketStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Vé tàu đã được hủy trước đó.");
        }

        var trainTrip = await _trainTripRepository.GetByIdAsync(ticket.TrainTripId);
        if (trainTrip is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Không tìm thấy chuyến tàu liên kết.");
        }

        ticket.TicketStatus = "Cancelled";
        ticket.CancelledAt = DateTime.UtcNow;
        ticket.CancelReason = request.CancelReason;

        if (ticket.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
        {
            ticket.PaymentStatus = "Refunded";
        }

        await _ticketRepository.UpdateAsync(ticket);

        trainTrip.AvailableSeats += 1;
        await _trainTripRepository.UpdateAsync(trainTrip);
        await WriteAuditLogAsync("Cancel", "Ticket", ticket.TicketId.ToString(), $"TicketCode={ticket.TicketCode}, Reason={request.CancelReason}");

        return ServiceResult<object>.Ok(null, "Hủy vé thành công.");
    }

    public async Task<ServiceResult<object>> PayAsync(int ticketId, PayTicketRequestDto request)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "Không tìm thấy vé tàu.");
        }

        if (ticket.TicketStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Không thể thanh toán vé đã hủy.");
        }

        if (ticket.PaymentStatus.Equals("Paid", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<object>.Fail(StatusCodes.Status400BadRequest, "Vé tàu đã được thanh toán.");
        }

        ticket.PaymentStatus = "Paid";
        ticket.TicketStatus = "Paid";
        ticket.PaymentMethod = request.PaymentMethod;
        ticket.PaidAt = DateTime.UtcNow;

        await _ticketRepository.UpdateAsync(ticket);
        await WriteAuditLogAsync("Pay", "Ticket", ticket.TicketId.ToString(), $"TicketCode={ticket.TicketCode}, Method={request.PaymentMethod}");

        return ServiceResult<object>.Ok(null, "Thanh toán vé thành công.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y vÃ© tÃ u.");
        }

        var deleted = await _ticketRepository.DeleteAsync(ticketId);
        if (!deleted)
        {
            return ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y vÃ© tÃ u.");
        }
        await WriteAuditLogAsync("Delete", "Ticket", ticketId.ToString(), $"TicketCode={ticket.TicketCode}");

        return ServiceResult<object>.Ok(null, "XÃ³a vÃ© thÃ nh cÃ´ng.");
    }

    private async Task<string> GenerateTicketCodeAsync(DateTime bookingDateUtc)
    {
        var datePart = bookingDateUtc.ToString("yyyyMMdd");
        var prefix = $"TICKET-{datePart}-";
        var todayCount = await _context.Tickets.CountAsync(x => x.TicketCode.StartsWith(prefix));
        return $"{prefix}{(todayCount + 1):D4}";
    }

    private static TicketResponseDto MapResponse(Ticket ticket) =>
        new()
        {
            TicketId = ticket.TicketId,
            TicketCode = ticket.TicketCode,
            TrainTripId = ticket.TrainTripId,
            CustomerId = ticket.CustomerId,
            SeatNumber = ticket.SeatNumber,
            Price = ticket.Price,
            BookingDate = ticket.BookingDate,
            PaymentStatus = ticket.PaymentStatus,
            TicketStatus = ticket.TicketStatus,
            PaymentMethod = ticket.PaymentMethod,
            PaidAt = ticket.PaidAt,
            CancelledAt = ticket.CancelledAt,
            CancelReason = ticket.CancelReason,
            CreatedBy = ticket.CreatedBy
        };

    private async Task WriteAuditLogAsync(string action, string entityName, string entityId, string details)
    {
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";
        _context.AuditLogs.Add(new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Details = details,
            PerformedBy = userName,
            PerformedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }
}
