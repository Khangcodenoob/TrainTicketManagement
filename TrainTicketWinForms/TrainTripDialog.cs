using System.Drawing.Drawing2D;
using System.Linq;
using TrainTicketWinForms.Models;

namespace TrainTicketWinForms.Dialogs;

public sealed class TrainTripDialog : Form
{
    private readonly TextBox _txtTrainCode;
    private readonly ComboBox _cbxRoute;
    private readonly DateTimePicker _dtDeparture;
    private readonly DateTimePicker _dtArrival;
    private readonly TextBox _txtTotalSeats;
    private readonly TextBox _txtAvailableSeats;
    private readonly TextBox _txtPrice;
    private readonly ComboBox _cbxStatus;

    public TrainTripItem? Result { get; private set; }

    public TrainTripDialog(List<RouteItem> routes, TrainTripItem? trip = null)
    {
        Text = trip is null ? "Thêm chuyến tàu" : "Sửa chuyến tàu";
        Size = new Size(620, 460);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var lblTrainCode = FieldLabel("Mã tàu");
        _txtTrainCode = UiTheme.CreateInput("Nhập mã tàu...");

        var lblRoute = FieldLabel("Tuyến");
        _cbxRoute = new ComboBox
        {
            FlatStyle = FlatStyle.Flat,
            Font = UiTheme.TextFont,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.White,
            Width = 260
        };

        var routeItems = routes.Select(r => new { r.RouteId, Text = $"[{r.RouteId}] {r.DepartureStation} → {r.ArrivalStation}" }).ToList();
        _cbxRoute.DataSource = routeItems;
        _cbxRoute.DisplayMember = "Text";
        _cbxRoute.ValueMember = "RouteId";
        if (routeItems.Count == 0)
        {
            _cbxRoute.Items.Clear();
            _cbxRoute.Items.Add("(Chưa có tuyến)");
            _cbxRoute.SelectedIndex = 0;
        }

        var lblDeparture = FieldLabel("Khởi hành");
        _dtDeparture = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = UiTheme.TextFont, Width = 220 };

        var lblArrival = FieldLabel("Đến nơi");
        _dtArrival = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm", Font = UiTheme.TextFont, Width = 220 };

        var lblTotalSeats = FieldLabel("Tổng ghế");
        _txtTotalSeats = UiTheme.CreateInput("Số ghế...");

        var lblAvailableSeats = FieldLabel("Ghế trống");
        _txtAvailableSeats = UiTheme.CreateInput("Ghế còn...");

        var lblPrice = FieldLabel("Giá vé (VND)");
        _txtPrice = UiTheme.CreateInput("Nhập giá...");

        var lblStatus = FieldLabel("Trạng thái");
        _cbxStatus = new ComboBox
        {
            FlatStyle = FlatStyle.Flat,
            Font = UiTheme.TextFont,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.White,
            Width = 220
        };
        _cbxStatus.Items.AddRange(new object[] { "Scheduled", "Departed", "Completed", "Cancelled" });
        _cbxStatus.SelectedIndex = 0;

        var btnOk = UiTheme.CreatePrimaryButton("Lưu");
        btnOk.Location = new Point(430, 360);
        btnOk.Click += (_, _) => OnSubmit();

        var btnCancel = UiTheme.CreateSecondaryButton("Hủy");
        btnCancel.Location = new Point(530, 360);
        btnCancel.Click += (_, _) => Close();

        lblTrainCode.SetBounds(24, 24, 120, 20);
        _txtTrainCode.SetBounds(24, 46, 260, 34);
        lblRoute.SetBounds(320, 24, 120, 20);
        _cbxRoute.SetBounds(320, 46, 260, 34);
        lblDeparture.SetBounds(24, 96, 120, 20);
        _dtDeparture.SetBounds(24, 118, 260, 34);
        lblArrival.SetBounds(320, 96, 120, 20);
        _dtArrival.SetBounds(320, 118, 260, 34);
        lblTotalSeats.SetBounds(24, 168, 120, 20);
        _txtTotalSeats.SetBounds(24, 190, 120, 34);
        lblAvailableSeats.SetBounds(160, 168, 120, 20);
        _txtAvailableSeats.SetBounds(160, 190, 120, 34);
        lblPrice.SetBounds(296, 168, 120, 20);
        _txtPrice.SetBounds(296, 190, 140, 34);
        lblStatus.SetBounds(460, 168, 120, 20);
        _cbxStatus.SetBounds(460, 190, 120, 34);

        Controls.AddRange(new Control[]
        {
            lblTrainCode, _txtTrainCode,
            lblRoute, _cbxRoute,
            lblDeparture, _dtDeparture,
            lblArrival, _dtArrival,
            lblTotalSeats, _txtTotalSeats,
            lblAvailableSeats, _txtAvailableSeats,
            lblPrice, _txtPrice,
            lblStatus, _cbxStatus,
            btnOk, btnCancel
        });

        if (trip is not null)
        {
            _txtTrainCode.Text = trip.TrainCode;
            if (routeItems.Any())
                _cbxRoute.SelectedValue = trip.RouteId;
            _dtDeparture.Value = trip.DepartureTime;
            _dtArrival.Value = trip.ArrivalTime;
            _txtTotalSeats.Text = trip.TotalSeats.ToString();
            _txtAvailableSeats.Text = trip.AvailableSeats.ToString();
            _txtPrice.Text = trip.BaseTicketPrice.ToString("0.##");
            _cbxStatus.SelectedItem = trip.Status;
            Result = trip;
        }
    }

    private void OnSubmit()
    {
        if (string.IsNullOrWhiteSpace(_txtTrainCode.Text))
        {
            MessageBox.Show("Mã tàu không được để trống.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (_cbxRoute.SelectedItem is null || _cbxRoute.SelectedValue is null || _cbxRoute.SelectedValue.ToString() == "(Chưa có tuyến)")
        {
            MessageBox.Show("Vui lòng chọn tuyến tàu.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (_dtArrival.Value <= _dtDeparture.Value)
        {
            MessageBox.Show("Thời gian đến phải lớn hơn thời gian khởi hành.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!int.TryParse(_txtTotalSeats.Text.Trim(), out var totalSeats) || totalSeats <= 0)
        {
            MessageBox.Show("Tổng ghế phải là số nguyên dương.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!int.TryParse(_txtAvailableSeats.Text.Trim(), out var availableSeats) || availableSeats < 0 || availableSeats > totalSeats)
        {
            MessageBox.Show("Ghế trống phải lớn hơn hoặc bằng 0 và nhỏ hơn hoặc bằng tổng ghế.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!decimal.TryParse(_txtPrice.Text.Trim(), out var price) || price <= 0)
        {
            MessageBox.Show("Giá vé phải là số dương.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new TrainTripItem
        {
            TrainCode = _txtTrainCode.Text.Trim(),
            RouteId = (int)_cbxRoute.SelectedValue,
            DepartureStation = string.Empty,
            ArrivalStation = string.Empty,
            DepartureTime = _dtDeparture.Value,
            ArrivalTime = _dtArrival.Value,
            TotalSeats = totalSeats,
            AvailableSeats = availableSeats,
            BaseTicketPrice = price,
            Status = _cbxStatus.SelectedItem?.ToString() ?? string.Empty
        };

        DialogResult = DialogResult.OK;
        Close();
    }

    private static Label FieldLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = UiTheme.TextFont,
        ForeColor = UiTheme.TextMuted
    };

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(UiTheme.Border, 1);
        UiTheme.DrawRoundedRect(g, pen, new Rectangle(0, 0, Width - 1, Height - 1), 12);
    }
}
