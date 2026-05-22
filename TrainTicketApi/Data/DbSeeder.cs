using TrainTicketApi.Models;

namespace TrainTicketApi.Data;

public static class DbSeeder
{
    public static void SeedData(AppDbContext context)
    {
        if (!context.Routes.Any())
        {
            var routes = new List<TrainRoute>
            {
                new TrainRoute { DepartureStation = "Ha Noi", ArrivalStation = "Hai Phong", DistanceKm = 102m, Status = "Active" },
                new TrainRoute { DepartureStation = "Ha Noi", ArrivalStation = "Da Nang", DistanceKm = 764m, Status = "Active" },
                new TrainRoute { DepartureStation = "Ha Noi", ArrivalStation = "Lao Cai", DistanceKm = 296m, Status = "Active" },
                new TrainRoute { DepartureStation = "Da Nang", ArrivalStation = "Sai Gon", DistanceKm = 935m, Status = "Active" },
                new TrainRoute { DepartureStation = "Ha Noi", ArrivalStation = "Sai Gon", DistanceKm = 1726m, Status = "Active" }
            };
            context.Routes.AddRange(routes);
            context.SaveChanges();
        }

        if (!context.TrainTrips.Any())
        {
            var routeHN_HP = context.Routes.FirstOrDefault(r => r.DepartureStation == "Ha Noi" && r.ArrivalStation == "Hai Phong")?.RouteId ?? 1;
            var routeHN_DN = context.Routes.FirstOrDefault(r => r.DepartureStation == "Ha Noi" && r.ArrivalStation == "Da Nang")?.RouteId ?? 2;
            var routeHN_LC = context.Routes.FirstOrDefault(r => r.DepartureStation == "Ha Noi" && r.ArrivalStation == "Lao Cai")?.RouteId ?? 3;
            
            var today = DateTime.Today;

            var trips = new List<TrainTrip>
            {
                new TrainTrip { TrainCode = "HP1", RouteId = routeHN_HP, DepartureTime = today.AddDays(1).AddHours(6), ArrivalTime = today.AddDays(1).AddHours(8).AddMinutes(30), TotalSeats = 120, AvailableSeats = 120, BaseTicketPrice = 120000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "HP2", RouteId = routeHN_HP, DepartureTime = today.AddDays(1).AddHours(15), ArrivalTime = today.AddDays(1).AddHours(17).AddMinutes(30), TotalSeats = 120, AvailableSeats = 120, BaseTicketPrice = 120000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "SE1", RouteId = routeHN_DN, DepartureTime = today.AddDays(2).AddHours(19), ArrivalTime = today.AddDays(3).AddHours(11).AddMinutes(15), TotalSeats = 300, AvailableSeats = 300, BaseTicketPrice = 850000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "SE3", RouteId = routeHN_DN, DepartureTime = today.AddDays(3).AddHours(19), ArrivalTime = today.AddDays(4).AddHours(11).AddMinutes(15), TotalSeats = 300, AvailableSeats = 300, BaseTicketPrice = 850000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "SP1", RouteId = routeHN_LC, DepartureTime = today.AddDays(1).AddHours(21).AddMinutes(30), ArrivalTime = today.AddDays(2).AddHours(5).AddMinutes(30), TotalSeats = 250, AvailableSeats = 250, BaseTicketPrice = 350000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "SP3", RouteId = routeHN_LC, DepartureTime = today.AddDays(2).AddHours(22), ArrivalTime = today.AddDays(3).AddHours(6), TotalSeats = 250, AvailableSeats = 250, BaseTicketPrice = 350000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "HP3", RouteId = routeHN_HP, DepartureTime = today.AddDays(2).AddHours(6), ArrivalTime = today.AddDays(2).AddHours(8).AddMinutes(30), TotalSeats = 120, AvailableSeats = 120, BaseTicketPrice = 120000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "HP4", RouteId = routeHN_HP, DepartureTime = today.AddDays(2).AddHours(15), ArrivalTime = today.AddDays(2).AddHours(17).AddMinutes(30), TotalSeats = 120, AvailableSeats = 120, BaseTicketPrice = 120000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "SE5", RouteId = routeHN_DN, DepartureTime = today.AddDays(4).AddHours(8).AddMinutes(50), ArrivalTime = today.AddDays(5).AddHours(1), TotalSeats = 300, AvailableSeats = 300, BaseTicketPrice = 800000m, Status = "Scheduled" },
                new TrainTrip { TrainCode = "SP5", RouteId = routeHN_LC, DepartureTime = today.AddDays(3).AddHours(21).AddMinutes(30), ArrivalTime = today.AddDays(4).AddHours(5).AddMinutes(30), TotalSeats = 250, AvailableSeats = 250, BaseTicketPrice = 350000m, Status = "Scheduled" }
            };
            context.TrainTrips.AddRange(trips);
            context.SaveChanges();
        }

        if (!context.Customers.Any())
        {
            var customers = new List<Customer>();
            for (int i = 1; i <= 10; i++)
            {
                customers.Add(new Customer
                {
                    FullName = $"Khach Hang {i}",
                    PhoneNumber = $"090123456{i - 1}",
                    Email = $"khachhang{i}@example.com",
                    IdentityNumber = $"00109000000{i - 1}",
                    Address = "Ha Noi"
                });
            }
            context.Customers.AddRange(customers);
            context.SaveChanges();
        }

        if (!context.Tickets.Any())
        {
            var trips = context.TrainTrips.ToList();
            var customers = context.Customers.ToList();

            if (trips.Count > 0 && customers.Count > 0)
            {
                var tickets = new List<Ticket>();
                var rand = new Random(123); // fixed seed for reproducibility
                int ticketCounter = 1;

                foreach (var trip in trips.Take(5))
                {
                    for (int i = 1; i <= 4; i++) // 4 tickets per trip = 20 tickets
                    {
                        var cust = customers[rand.Next(customers.Count)];
                        var seatStr = i.ToString("D2"); // "01", "02", "03", "04"
                        
                        tickets.Add(new Ticket
                        {
                            TicketCode = $"TCK-{trip.TrainCode}-{seatStr}",
                            TrainTripId = trip.TrainTripId,
                            CustomerId = cust.CustomerId,
                            SeatNumber = seatStr,
                            Price = trip.BaseTicketPrice,
                            BookingDate = DateTime.Now.AddDays(-1),
                            PaymentStatus = "Paid",
                            TicketStatus = "Booked",
                            PaymentMethod = "BankTransfer",
                            PaidAt = DateTime.Now.AddDays(-1).AddHours(1),
                            CreatedBy = "Seeder"
                        });
                        trip.AvailableSeats--;
                    }
                }
                context.Tickets.AddRange(tickets);
                context.SaveChanges();
            }
        }
    }
}
