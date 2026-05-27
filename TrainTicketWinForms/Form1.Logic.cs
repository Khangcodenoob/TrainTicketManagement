using System.Text;
using Newtonsoft.Json;
using TrainTicketWinForms.Dialogs;
using TrainTicketWinForms.Models;
using TrainTicketWinForms.Services;

namespace TrainTicketWinForms;

public partial class Form1
{
    // ═══════════════════════════════════════════════════════════
    //  FORM EVENTS
    // ═══════════════════════════════════════════════════════════

    private async void Form1_Load(object? sender, EventArgs e)
    {
        AuthService.UnauthorizedReceived += HandleUnauthorized;

        await LoadDashboardAsync();
        await LoadRoutesAsync();
        await LoadTripsAsync();
        await LoadCustomersAsync();
        await LoadTicketsAsync();

        ActivateMenu(_btnDashboard, _dashboardView, "📊  Bảng điều khiển");
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        AuthService.UnauthorizedReceived -= HandleUnauthorized;
    }

    private void HandleUnauthorized()
    {
        if (_handlingUnauthorized)
        {
            return;
        }

        _handlingUnauthorized = true;
        BeginInvoke(() =>
        {
            ShowToast("Phien dang nhap da het han. Vui long dang nhap lai.", UiTheme.Warning);
            MessageBox.Show(
                "Phien dang nhap da het han hoac khong hop le. Vui long dang nhap lai.",
                "Unauthorized",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            ShouldReturnToLogin = true;
            Close();
        });
    }

    // ═══════════════════════════════════════════════════════════
    //  LOAD DATA
    // ═══════════════════════════════════════════════════════════

    private async Task LoadDashboardAsync()
    {
        await WrapAsync(async () =>
        {
            var json = await _httpClient.GetStringAsync("api/dashboard/stats");
            var data = JsonConvert.DeserializeObject<ApiResponse<DashboardStats>>(json, JsonSettings)?.Data;
            if (data is null) { ShowToast("Không thể tải dashboard.", UiTheme.Danger); return; }

            _lblTotalTickets.Text = data.TotalTickets.ToString("N0");
            _lblRevenue.Text      = $"{data.TotalRevenue:N0}";
            _lblTrips.Text        = data.TotalTrips.ToString("N0");
            _lblCustomers.Text    = data.TotalCustomers.ToString("N0");

            // Bar chart data
            _chartLabels = new[] { "Đã đặt", "Đã hủy", "Sắp chạy", "Tổng vé", "Tổng chuyến" };
            _chartValues = new double[]
            {
                data.BookedTickets,
                data.CancelledTickets,
                data.UpcomingTripsCount,
                data.TotalTickets,
                data.TotalTrips
            };
            _chartPanel?.Invalidate();
        });
    }

    private async Task LoadRoutesAsync()
    {
        await WrapAsync(async () =>
        {
            var json = await _httpClient.GetStringAsync("api/routes");
            _routeCache = JsonConvert.DeserializeObject<ApiResponse<List<RouteItem>>>(json, JsonSettings)?.Data
                          ?? new List<RouteItem>();
            _routesGrid.DataSource = _routeCache.ToList();
        });
    }

    private async Task LoadTripsAsync(bool useFilter = false)
    {
        await WrapAsync(async () =>
        {
            var endpoint = "api/train-trips/search";
            if (useFilter)
                endpoint += $"?departureStation={Uri.EscapeDataString(_txtTripFrom.Text.Trim())}" +
                            $"&arrivalStation={Uri.EscapeDataString(_txtTripTo.Text.Trim())}" +
                            $"&departureDate={_dtTripDate.Value:yyyy-MM-dd}";

            var json = await _httpClient.GetStringAsync(endpoint);
            _tripCache = JsonConvert.DeserializeObject<ApiResponse<List<TrainTripItem>>>(json, JsonSettings)?.Data
                         ?? new List<TrainTripItem>();
            _tripsGrid.DataSource = _tripCache.ToList();

            // Refresh trip combobox
            RefreshTripCombo();
        });
    }

    private async Task LoadCustomersAsync()
    {
        await WrapAsync(async () =>
        {
            var json = await _httpClient.GetStringAsync("api/customers");
            _customerCache = JsonConvert.DeserializeObject<ApiResponse<List<CustomerItem>>>(json, JsonSettings)?.Data
                             ?? new List<CustomerItem>();
            _customersGrid.DataSource = _customerCache.ToList();

            // Refresh customer combobox
            RefreshCustomerCombo();
        });
    }

    private async Task ShowRouteDialogAsync(RouteItem? route = null)
    {
        using var dialog = new RouteDialog(route);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        if (route is null)
            await CreateRouteAsync(dialog.Result!);
        else
            await UpdateRouteAsync(route.RouteId, dialog.Result!);
    }

    private async Task EditSelectedRouteAsync()
    {
        var selected = _routesGrid.CurrentRow?.DataBoundItem as RouteItem;
        if (selected is null) { ShowToast("⚠️  Vui lòng chọn tuyến tàu để sửa.", UiTheme.Warning); return; }
        await ShowRouteDialogAsync(selected);
    }

    private async Task DeleteSelectedRouteAsync()
    {
        var selected = _routesGrid.CurrentRow?.DataBoundItem as RouteItem;
        if (selected is null) { ShowToast("⚠️  Vui lòng chọn tuyến tàu để xóa.", UiTheme.Warning); return; }
        await DeleteRouteAsync(selected.RouteId);
    }

    private async Task ShowTrainTripDialogAsync(TrainTripItem? trip = null)
    {
        using var dialog = new TrainTripDialog(_routeCache, trip);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        if (trip is null)
            await CreateTrainTripAsync(dialog.Result!);
        else
            await UpdateTrainTripAsync(trip.TrainTripId, dialog.Result!);
    }

    private async Task EditSelectedTrainTripAsync()
    {
        var selected = _tripsGrid.CurrentRow?.DataBoundItem as TrainTripItem;
        if (selected is null) { ShowToast("⚠️  Vui lòng chọn chuyến tàu để sửa.", UiTheme.Warning); return; }
        await ShowTrainTripDialogAsync(selected);
    }

    private async Task DeleteSelectedTrainTripAsync()
    {
        var selected = _tripsGrid.CurrentRow?.DataBoundItem as TrainTripItem;
        if (selected is null) { ShowToast("⚠️  Vui lòng chọn chuyến tàu để xóa.", UiTheme.Warning); return; }
        await DeleteTrainTripAsync(selected.TrainTripId);
    }

    private async Task ShowCustomerDialogAsync(CustomerItem? customer = null)
    {
        using var dialog = new CustomerDialog(customer);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        if (customer is null)
            await CreateCustomerAsync(dialog.Result!);
        else
            await UpdateCustomerAsync(customer.CustomerId, dialog.Result!);
    }

    private async Task EditSelectedCustomerAsync()
    {
        var selected = _customersGrid.CurrentRow?.DataBoundItem as CustomerItem;
        if (selected is null) { ShowToast("⚠️  Vui lòng chọn khách hàng để sửa.", UiTheme.Warning); return; }
        await ShowCustomerDialogAsync(selected);
    }

    private async Task DeleteSelectedCustomerAsync()
    {
        var selected = _customersGrid.CurrentRow?.DataBoundItem as CustomerItem;
        if (selected is null) { ShowToast("⚠️  Vui lòng chọn khách hàng để xóa.", UiTheme.Warning); return; }
        await DeleteCustomerAsync(selected.CustomerId);
    }

    private async Task CreateRouteAsync(RouteItem route)
    {
        await WrapAsync(async () =>
        {
            var payload = new
            {
                DepartureStation = route.DepartureStation,
                ArrivalStation   = route.ArrivalStation,
                DistanceKm       = route.DistanceKm,
                Status           = route.Status
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/routes", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Thêm tuyến tàu thành công.", UiTheme.Success);
            await LoadRoutesAsync();
        });
    }

    private async Task UpdateRouteAsync(int routeId, RouteItem route)
    {
        await WrapAsync(async () =>
        {
            var payload = new
            {
                DepartureStation = route.DepartureStation,
                ArrivalStation   = route.ArrivalStation,
                DistanceKm       = route.DistanceKm,
                Status           = route.Status
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/routes/{routeId}", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Cập nhật tuyến tàu thành công.", UiTheme.Success);
            await LoadRoutesAsync();
        });
    }

    private async Task DeleteRouteAsync(int routeId)
    {
        if (!ConfirmDialog.Show(this, $"Bạn có chắc muốn xóa tuyến #{routeId}?", "Xóa tuyến", "🗑️"))
            return;

        await WrapAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync($"api/routes/{routeId}");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Xóa tuyến tàu thành công.", UiTheme.Success);
            await LoadRoutesAsync();
        });
    }

    private async Task CreateTrainTripAsync(TrainTripItem trip)
    {
        await WrapAsync(async () =>
        {
            var payload = new
            {
                TrainCode      = trip.TrainCode,
                RouteId        = trip.RouteId,
                DepartureTime  = trip.DepartureTime,
                ArrivalTime    = trip.ArrivalTime,
                TotalSeats     = trip.TotalSeats,
                AvailableSeats = trip.AvailableSeats,
                BaseTicketPrice= trip.BaseTicketPrice,
                Status         = trip.Status
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/train-trips", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Thêm chuyến tàu thành công.", UiTheme.Success);
            await LoadTripsAsync();
        });
    }

    private async Task UpdateTrainTripAsync(int tripId, TrainTripItem trip)
    {
        await WrapAsync(async () =>
        {
            var payload = new
            {
                TrainCode      = trip.TrainCode,
                RouteId        = trip.RouteId,
                DepartureTime  = trip.DepartureTime,
                ArrivalTime    = trip.ArrivalTime,
                TotalSeats     = trip.TotalSeats,
                AvailableSeats = trip.AvailableSeats,
                BaseTicketPrice= trip.BaseTicketPrice,
                Status         = trip.Status
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/train-trips/{tripId}", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Cập nhật chuyến tàu thành công.", UiTheme.Success);
            await LoadTripsAsync();
        });
    }

    private async Task DeleteTrainTripAsync(int tripId)
    {
        if (!ConfirmDialog.Show(this, $"Bạn có chắc muốn xóa chuyến tàu #{tripId}?", "Xóa chuyến tàu", "🗑️"))
            return;

        await WrapAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync($"api/train-trips/{tripId}");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Xóa chuyến tàu thành công.", UiTheme.Success);
            await LoadTripsAsync();
        });
    }

    private async Task CreateCustomerAsync(CustomerItem customer)
    {
        await WrapAsync(async () =>
        {
            var payload = new
            {
                FullName       = customer.FullName,
                PhoneNumber    = customer.PhoneNumber,
                Email          = customer.Email,
                IdentityNumber = customer.IdentityNumber,
                Address        = customer.Address
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/customers", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Thêm khách hàng thành công.", UiTheme.Success);
            await LoadCustomersAsync();
        });
    }

    private async Task UpdateCustomerAsync(int customerId, CustomerItem customer)
    {
        await WrapAsync(async () =>
        {
            var payload = new
            {
                FullName       = customer.FullName,
                PhoneNumber    = customer.PhoneNumber,
                Email          = customer.Email,
                IdentityNumber = customer.IdentityNumber,
                Address        = customer.Address
            };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/customers/{customerId}", content);
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Cập nhật khách hàng thành công.", UiTheme.Success);
            await LoadCustomersAsync();
        });
    }

    private async Task DeleteCustomerAsync(int customerId)
    {
        if (!ConfirmDialog.Show(this, $"Bạn có chắc muốn xóa khách hàng #{customerId}?", "Xóa khách hàng", "🗑️"))
            return;

        await WrapAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync($"api/customers/{customerId}");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                ShowToast($"❌  {body}", UiTheme.Danger);
                return;
            }
            ShowToast("✅  Xóa khách hàng thành công.", UiTheme.Success);
            await LoadCustomersAsync();
        });
    }

    private async Task LoadTicketsAsync()
    {
        await WrapAsync(async () =>
        {
            var json = await _httpClient.GetStringAsync("api/tickets");
            _ticketCache = JsonConvert.DeserializeObject<ApiResponse<List<TicketItem>>>(json, JsonSettings)?.Data
                           ?? new List<TicketItem>();
            _ticketsGrid.DataSource = _ticketCache.ToList();
        });
    }

    // ── Populate comboboxes ─────────────────────────────────────
    private void RefreshTripCombo()
    {
        if (_cbxTicketTrip is null) return;
        
        var displayList = _tripCache.Select(t => new { 
            Text = $"[{t.TrainTripId}]  {t.DepartureStation} → {t.ArrivalStation}  ({t.DepartureTime:dd/MM HH:mm})", 
            Value = t.TrainTripId 
        }).ToList();
        
        if (displayList.Count == 0)
        {
            displayList.Add(new { Text = "(Chưa có chuyến tàu)", Value = 0 });
        }

        _cbxTicketTrip.DataSource = displayList;
        _cbxTicketTrip.DisplayMember = "Text";
        _cbxTicketTrip.ValueMember = "Value";
    }

    private void RefreshCustomerCombo()
    {
        if (_cbxTicketCustomer is null) return;
        
        var displayList = _customerCache.Select(c => new { 
            Text = $"[{c.CustomerId}]  {c.FullName} - {c.PhoneNumber}", 
            Value = c.CustomerId 
        }).ToList();
        
        if (displayList.Count == 0)
        {
            displayList.Add(new { Text = "(Chưa có khách hàng)", Value = 0 });
        }

        _cbxTicketCustomer.DataSource = displayList;
        _cbxTicketCustomer.DisplayMember = "Text";
        _cbxTicketCustomer.ValueMember = "Value";
    }

    // ═══════════════════════════════════════════════════════════
    //  TICKET ACTIONS
    // ═══════════════════════════════════════════════════════════

    private async Task CreateTicketAsync()
    {
        if (_tripCache.Count == 0 || _customerCache.Count == 0)
        {
            ShowToast("⚠️  Chưa có dữ liệu chuyến tàu hoặc khách hàng.", UiTheme.Warning);
            return;
        }
        if (_cbxTicketTrip.SelectedValue is null || (int)_cbxTicketTrip.SelectedValue == 0)
        {
            ShowToast("⚠️  Vui lòng chọn chuyến tàu.", UiTheme.Warning);
            return;
        }
        if (_cbxTicketCustomer.SelectedValue is null || (int)_cbxTicketCustomer.SelectedValue == 0)
        {
            ShowToast("⚠️  Vui lòng chọn khách hàng.", UiTheme.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(_selectedSeat))
        {
            ShowToast("⚠️  Vui lòng chọn ghế từ sơ đồ.", UiTheme.Warning);
            return;
        }

        var tripId      = (int)_cbxTicketTrip.SelectedValue;
        var customerId  = (int)_cbxTicketCustomer.SelectedValue;
        var paymentMethod = _cbxPaymentMethod.SelectedItem?.ToString() ?? "";

        var payload = new
        {
            TrainTripId   = tripId,
            CustomerId    = customerId,
            SeatNumber    = _selectedSeat,
            PaymentMethod = paymentMethod
        };

        await WrapAsync(async () =>
        {
            var content  = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/tickets", content);
            var body     = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) { ShowToast($"❌  {body}", UiTheme.Danger); return; }

            ShowToast("✅  Đặt vé thành công!", UiTheme.Success);
            await LoadTicketsAsync();
            await LoadDashboardAsync();
            await OnTripSelectedAsync();
        });
    }

    private async Task OnTripSelectedAsync()
    {
        await WrapAsync(async () =>
        {
            if (_cbxTicketTrip.SelectedValue is null || (int)_cbxTicketTrip.SelectedValue == 0) return;
            var tripId = (int)_cbxTicketTrip.SelectedValue;
            var trip = _tripCache.FirstOrDefault(t => t.TrainTripId == tripId);
            if (trip == null) return;
            
            _lblTripInfo.Text = $"Ga đi: {trip.DepartureStation} ➝ Ga đến: {trip.ArrivalStation} | Khởi hành: {trip.DepartureTime:HH:mm dd/MM} | Số ghế trống: {trip.AvailableSeats} / {trip.TotalSeats}";
            _txtTicketPrice.Text = trip.BaseTicketPrice.ToString("0");
            
            await RenderSeatMapAsync(trip.TrainTripId, trip.TotalSeats);
        });
    }

    private async Task RenderSeatMapAsync(int tripId, int totalSeats)
    {
        _pnlSeatMap.Controls.Clear();
        _selectedSeat = string.Empty;

        var json = await _httpClient.GetStringAsync($"api/train-trips/{tripId}/available-seats");
        var dynamicData = JsonConvert.DeserializeObject<dynamic>(json);
        var bookedSeatsArray = dynamicData?.data?.bookedSeats?.ToObject<List<string>>() ?? new List<string>();

        for (int i = 1; i <= totalSeats; i++)
        {
            var seatName = i.ToString("D2");
            var isBooked = bookedSeatsArray.Contains(seatName);

            var btn = new Button
            {
                Text = seatName,
                Width = 40, Height = 40,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(3),
                Cursor = isBooked ? Cursors.Default : Cursors.Hand,
                BackColor = isBooked ? UiTheme.Danger : Color.White,
                ForeColor = isBooked ? Color.White : UiTheme.TextDark,
                Enabled = !isBooked
            };

            if (!isBooked)
            {
                btn.Click += (s, e) =>
                {
                    foreach (Control c in _pnlSeatMap.Controls)
                    {
                        if (c is Button b && b.Enabled && b.BackColor == UiTheme.Primary)
                        {
                            b.BackColor = Color.White;
                            b.ForeColor = UiTheme.TextDark;
                        }
                    }
                    btn.BackColor = UiTheme.Primary;
                    btn.ForeColor = Color.White;
                    _selectedSeat = seatName;
                };
            }
            _pnlSeatMap.Controls.Add(btn);
        }
    }

    private async Task DeleteTicketAsync()
    {
        if (!int.TryParse(_txtTicketActionId.Text.Trim(), out var id))
        {
            ShowToast("⚠️  Ticket ID không hợp lệ.", UiTheme.Warning); return;
        }
        if (!ConfirmDialog.Show(this, $"Bạn có chắc muốn xóa vé #{id}?", "Xóa vé", "🗑️"))
            return;

        await WrapAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync($"api/tickets/{id}");
            var body     = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) { ShowToast($"❌  {body}", UiTheme.Danger); return; }

            ShowToast("✅  Xóa vé thành công.", UiTheme.Success);
            await LoadTicketsAsync();
            await LoadDashboardAsync();
        });
    }

    private async Task CancelTicketAsync()
    {
        if (!int.TryParse(_txtTicketActionId.Text.Trim(), out var id))
        {
            ShowToast("⚠️  Ticket ID không hợp lệ.", UiTheme.Warning); return;
        }
        if (!ConfirmDialog.Show(this, $"Hủy vé #{id}? Thao tác không thể hoàn tác.", "Hủy vé", "⚠️"))
            return;

        await WrapAsync(async () =>
        {
            var payload = new { CancelReason = "Người dùng hủy trực tiếp" };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"api/tickets/{id}/cancel", content);
            var body     = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) { ShowToast($"❌  {body}", UiTheme.Danger); return; }

            ShowToast("✅  Hủy vé thành công.", UiTheme.Success);
            await LoadTicketsAsync();
            await LoadDashboardAsync();
            await OnTripSelectedAsync();
        });
    }

    private async Task PayTicketAsync()
    {
        if (!int.TryParse(_txtTicketActionId.Text.Trim(), out var id))
        {
            ShowToast("⚠️  Ticket ID không hợp lệ.", UiTheme.Warning); return;
        }
        
        var payload = new { PaymentMethod = "Cash" }; 
        
        await WrapAsync(async () =>
        {
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/tickets/{id}/pay", content);
            var body     = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode) { ShowToast($"❌  {body}", UiTheme.Danger); return; }

            ShowToast("✅  Thanh toán vé thành công.", UiTheme.Success);
            await LoadTicketsAsync();
            await LoadDashboardAsync();
        });
    }

    private async Task ExportCsvAsync()
    {
        if (!_role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            ShowToast("❌  Chỉ Admin mới được xuất báo cáo.", UiTheme.Danger); return;
        }
        await WrapAsync(async () =>
        {
            var bytes = await _httpClient.GetByteArrayAsync("api/dashboard/reports/export");
            using var dialog = new SaveFileDialog
            {
                Filter   = "CSV files (*.csv)|*.csv",
                FileName = $"ticket-report-{DateTime.Now:yyyyMMddHHmmss}.csv"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                await File.WriteAllBytesAsync(dialog.FileName, bytes);
                ShowToast("✅  Xuất CSV thành công.", UiTheme.Success);
            }
        });
    }

    // ── Search ──────────────────────────────────────────────────
    private void ApplyTicketSearch(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            _ticketsGrid.DataSource = _ticketCache.ToList();
        else
            _ticketsGrid.DataSource = _ticketCache
                .Where(x => x.TicketCode.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
    }

    // ── Logout ──────────────────────────────────────────────────
    private async Task LogoutAsync()
    {
        if (!ConfirmDialog.Show(this, "Bạn có chắc muốn đăng xuất?", "Đăng xuất", "🚪"))
            return;

        await WrapAsync(async () =>
        {
            // Clear token and user info
            TokenService.Clear();
            ShowToast("✅  Đã đăng xuất thành công.", UiTheme.Success);
            
            // Return to login after a brief delay
            await Task.Delay(500);
            ShouldReturnToLogin = true;
            Close();
        });
    }

    // ── Wrapper ─────────────────────────────────────────────────
    private async Task WrapAsync(Func<Task> action)
    {
        try
        {
            _loadingOverlay.Visible = true;
            _loadingOverlay.BringToFront();
            await action();
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"[HTTP ERROR] {httpEx.Message}");
            ShowToast($"❌ Lỗi kết nối API: {httpEx.Message}", UiTheme.Danger);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}\n{ex.StackTrace}");
            ShowToast($"❌  {ex.Message}", UiTheme.Danger);
        }
        finally
        {
            _loadingOverlay.Visible = false;
        }
    }
}
