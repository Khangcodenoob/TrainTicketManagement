using System.Drawing.Drawing2D;
using TrainTicketWinForms.Models;

namespace TrainTicketWinForms;

public partial class Form1
{
    // ═══════════════════════════════════════════════════════════
    //  VIEWS
    // ═══════════════════════════════════════════════════════════

    private void BuildViews()
    {
        _dashboardView  = BuildDashboardView();
        _routesView     = BuildRoutesView();
        _tripsView      = BuildTripsView();
        _customersView  = BuildCustomersView();
        _ticketsView    = BuildTicketsView();
        _stationsView   = BuildStationsView();

        foreach (var v in new[] { _dashboardView, _routesView, _tripsView, _customersView, _ticketsView, _stationsView })
            _contentPanel.Controls.Add(v);

        // Register icon mapping for auto-bound DataGridViews
        RegisterGridIconEvents();
    }

    // ── Dashboard ───────────────────────────────────────────────
    private Panel BuildDashboardView()
    {
        var panel = ViewPanel();

        // ── Stat cards row ──
        var cardsFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 130,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };

        var card1 = StatCard("🎫  Tổng số vé",       UiTheme.Primary,  out _lblTotalTickets);
        var card2 = StatCard("💰  Doanh thu (VND)",  UiTheme.Success,  out _lblRevenue);
        var card3 = StatCard("🚆  Tổng chuyến tàu",  UiTheme.Accent,   out _lblTrips);
        var card4 = StatCard("👥  Tổng khách hàng",  UiTheme.Warning,  out _lblCustomers);

        cardsFlow.Controls.AddRange(new Control[] { card1, card2, card3, card4 });

        // ── Bar chart area ──
        var chartCard = new ShadowPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 12, 8, 0) };
        var chartTitle = new Label
        {
            Text = "📊  Thống kê trạng thái vé",
            Dock = DockStyle.Top, Height = 36,
            Font = UiTheme.HeadingFont,
            ForeColor = UiTheme.TextDark,
            Padding = new Padding(2, 8, 0, 0)
        };

        _chartPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        _chartPanel.Paint += (_, e) =>
        {
            if (_chartValues.Length > 0)
                UiTheme.DrawBarChart(e.Graphics, _chartPanel.Width, _chartPanel.Height,
                    _chartLabels, _chartValues, UiTheme.ChartColors);
        };

        chartCard.Controls.Add(_chartPanel);
        chartCard.Controls.Add(chartTitle);

        panel.Controls.Add(chartCard);
        panel.Controls.Add(cardsFlow);
        return panel;
    }

    private static ShadowPanel StatCard(string title, Color accentColor, out Label valueLabel)
    {
        var card = new ShadowPanel
        {
            Width = 240,
            Height = 114,
            Margin = new Padding(0, 0, 14, 0)
        };

        // Left accent bar
        card.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var b = new SolidBrush(accentColor);
            e.Graphics.FillRectangle(b, 0, 0, 5, card.Height - 6);
        };

        var lblTitle = new Label
        {
            Text = title,
            Dock = DockStyle.Top, Height = 28,
            Font = new Font("Segoe UI", 9F),
            ForeColor = UiTheme.TextMuted,
            Padding = new Padding(14, 0, 0, 0)
        };

        valueLabel = new Label
        {
            Text = "—",
            Dock = DockStyle.Fill,
            Font = UiTheme.LargeNumFont,
            ForeColor = UiTheme.TextDark,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(14, 0, 0, 0)
        };

        card.Controls.Add(valueLabel);
        card.Controls.Add(lblTitle);
        return card;
    }

    // ── Routes ──────────────────────────────────────────────────
    private Panel BuildRoutesView()
    {
        var panel = ViewPanel();
        var toolbar = SectionToolbar("🗺️  Danh sách tuyến tàu", async (_, _) => await LoadRoutesAsync());
        _routesGrid = CreateGrid();
        panel.Controls.Add(_routesGrid);
        panel.Controls.Add(toolbar);
        return panel;
    }

    // ── Trips ───────────────────────────────────────────────────
    private Panel BuildTripsView()
    {
        var panel = ViewPanel();

        var filterCard = new ShadowPanel
        {
            Dock = DockStyle.Top, Height = 80,
            Margin = new Padding(0, 0, 0, 12)
        };

        // ── Station ComboBoxes ──────────────────────────────────
        var lblFrom = FieldLabel("Ga đi");
        var lblTo   = FieldLabel("Ga đến");
        lblFrom.SetBounds(16, 8, 80, 18);
        lblTo.SetBounds(230, 8, 80, 18);

        _txtTripFrom = CreateStationCombo();
        _txtTripFrom.SetBounds(16, 28, 200, 32);

        _txtTripTo = CreateStationCombo();
        _txtTripTo.SetBounds(230, 28, 200, 32);

        // ── Date picker ─────────────────────────────────────────
        var lblDate = FieldLabel("Ngày khởi hành");
        lblDate.SetBounds(444, 8, 120, 18);
        _dtTripDate = new DateTimePicker
        {
            Format = DateTimePickerFormat.Short,
            Font = UiTheme.TextFont
        };
        _dtTripDate.SetBounds(444, 28, 130, 32);

        // ── Buttons ─────────────────────────────────────────────
        var btnSearch = UiTheme.CreatePrimaryButton("🔍  Tìm");
        btnSearch.SetBounds(588, 26, 110, 36);
        btnSearch.Click += async (_, _) => await LoadTripsAsync(true);

        var btnRefresh = UiTheme.CreateSecondaryButton("↺  Tải lại");
        btnRefresh.SetBounds(706, 26, 110, 36);
        btnRefresh.Click += async (_, _) => await LoadTripsAsync();

        filterCard.Controls.AddRange(new Control[]
            { lblFrom, _txtTripFrom, lblTo, _txtTripTo, lblDate, _dtTripDate, btnSearch, btnRefresh });

        _tripsGrid = CreateGrid();
        panel.Controls.Add(_tripsGrid);
        panel.Controls.Add(filterCard);
        return panel;
    }

    private static readonly string[] VietnamTrainStations =
    [
        "Ga An Thái", "Ga Bắc Giang", "Ga Bầu Cá", "Ga Biên Hòa", "Ga Bình Định",
        "Ga Bình Dương", "Ga Bình Phước", "Ga Bình Triệu", "Ga Cao Bằng", "Ga Cao Lãnh",
        "Ga Cẩm Phả", "Ga Cửa Ông", "Ga Châu Đốc", "Ga Chí Linh", "Ga Cần Thơ",
        "Ga Đà Lạt", "Ga Đà Nẵng", "Ga Đông Anh", "Ga Đông Hà", "Ga Đồng Hới",
        "Ga Đông Triều", "Ga Diêu Trì", "Ga Dĩ An", "Ga Gia Lâm", "Ga Giáp Bát",
        "Ga Hà Giang", "Ga Hạ Long", "Ga Hải Dương", "Ga Hải Phòng", "Ga Hà Nội",
        "Ga Hòa Bình", "Ga Huế", "Ga Hưng Yên", "Ga Khánh Hòa", "Ga Kon Tum",
        "Ga Lạng Sơn", "Ga Lào Cai", "Ga Long Biên", "Ga Long Thành", "Ga Mỹ Đình",
        "Ga Mỹ Tho", "Ga Nam Định", "Ga Nha Trang", "Ga Ninh Bình", "Ga Phan Rang",
        "Ga Phan Thiết", "Ga Phủ Lý", "Ga Pleiku", "Ga Quảng Ngãi", "Ga Quy Nhơn",
        "Ga Sài Gòn", "Ga Tam Kỳ", "Ga Tân Ấp", "Ga Thanh Hóa", "Ga Thủ Dầu Một",
        "Ga Tuy Hòa", "Ga Uông Bí", "Ga Vinh", "Ga Việt Trì", "Ga Yên Bái",
    ];

    private static ComboBox CreateStationCombo()
    {
        var cb = new ComboBox
        {
            FlatStyle = FlatStyle.Flat,
            Font = UiTheme.TextFont,
            BackColor = Color.White,
            ForeColor = UiTheme.TextDark,
            DropDownStyle = ComboBoxStyle.DropDown,   // allows typing
            DropDownHeight = 260,
            AutoCompleteMode = AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = AutoCompleteSource.ListItems,
        };
        cb.Items.AddRange(VietnamTrainStations.Cast<object>().ToArray());
        return cb;
    }


    // ── Customers ───────────────────────────────────────────────
    private Panel BuildCustomersView()
    {
        var panel = ViewPanel();

        // Build toolbar manually so we can place search + refresh without overlap
        var toolbar = new Panel
        {
            Dock = DockStyle.Top, Height = 56,
            BackColor = Color.Transparent
        };

        var lblTitle = new Label
        {
            Text = "👥  Danh sách khách hàng",
            Font = UiTheme.HeadingFont,
            AutoSize = true,
            ForeColor = UiTheme.TextDark,
            Location = new Point(0, 16)
        };

        // Search box — fixed position after a generous gap from title
        var searchBox = UiTheme.CreateInput("🔍  Tìm theo tên / email...");
        searchBox.SetBounds(310, 14, 240, 30);
        searchBox.TextChanged += (_, _) =>
        {
            var kw = searchBox.Text.Trim();
            _customersGrid.DataSource = string.IsNullOrWhiteSpace(kw)
                ? _customerCache.ToList()
                : _customerCache.Where(c =>
                    c.FullName.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                    c.Email.Contains(kw, StringComparison.OrdinalIgnoreCase)).ToList();
        };

        // Refresh button — placed right after search box
        var btnRefresh = UiTheme.CreateSecondaryButton("↺  Tải lại");
        btnRefresh.SetBounds(562, 13, 110, 32);
        btnRefresh.Click += async (_, _) => await LoadCustomersAsync();

        toolbar.Controls.AddRange(new Control[] { lblTitle, searchBox, btnRefresh });

        _customersGrid = CreateGrid();
        panel.Controls.Add(_customersGrid);
        panel.Controls.Add(toolbar);
        return panel;
    }

    // ── Tickets ─────────────────────────────────────────────────
    private Panel BuildTicketsView()
    {
        var panel = ViewPanel();

        // ── Form card ──
        var formCard = new ShadowPanel
        {
            Dock = DockStyle.Top, Height = 280,
            Margin = new Padding(0, 0, 0, 12)
        };

        // Block 1: Select Trip & Customer
        var l1 = FieldLabel("1. Chọn chuyến tàu");
        var l2 = FieldLabel("2. Chọn khách hàng");
        _cbxTicketTrip     = UiTheme.CreateComboBox();
        _cbxTicketCustomer = UiTheme.CreateComboBox();

        l1.SetBounds(16, 16, 180, 20); _cbxTicketTrip.SetBounds(16, 38, 260, 32);
        l2.SetBounds(300, 16, 180, 20); _cbxTicketCustomer.SetBounds(300, 38, 200, 32);

        _lblTripInfo = new Label
        {
            Text = "Thông tin chuyến: ...",
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = UiTheme.TextMuted,
            AutoSize = true,
            Location = new Point(16, 80)
        };

        // Block 2: Seat Map
        var l3 = FieldLabel("3. Chọn ghế");
        l3.SetBounds(520, 16, 100, 20);
        
        _pnlSeatMap = new FlowLayoutPanel
        {
            BackColor = Color.WhiteSmoke,
            AutoScroll = true,
            WrapContents = true,
            BorderStyle = BorderStyle.FixedSingle
        };
        _pnlSeatMap.SetBounds(520, 38, 300, 180);

        // Block 3: Payment & Create
        var l4 = FieldLabel("4. Giá vé (VND)");
        var l5 = FieldLabel("Phương thức TT");
        
        _txtTicketPrice = UiTheme.CreateInput();
        _txtTicketPrice.ReadOnly = true;
        _txtTicketPrice.BackColor = Color.WhiteSmoke;
        
        _cbxPaymentMethod = UiTheme.CreateComboBox();
        _cbxPaymentMethod.Items.AddRange(new object[] { "Cash", "BankTransfer", "Card", "EWallet" });
        _cbxPaymentMethod.SelectedIndex = 0;

        l4.SetBounds(16, 130, 120, 20); _txtTicketPrice.SetBounds(16, 152, 130, 32);
        l5.SetBounds(160, 130, 120, 20); _cbxPaymentMethod.SetBounds(160, 152, 130, 32);

        var btnCreate = UiTheme.CreatePrimaryButton("🎫  Đặt vé");
        btnCreate.SetBounds(310, 148, 120, 36);
        btnCreate.Click += async (_, _) => await CreateTicketAsync();

        // Block 4: Actions & Search toolbar
        var lSearch = FieldLabel("Tìm mã vé/Tên KH");
        var lAction = FieldLabel("Ticket ID (thao tác)");

        _txtTicketSearch   = UiTheme.CreateInput("Nhập từ khóa...");
        _txtTicketActionId = UiTheme.CreateInput("ID vé...");

        lSearch.SetBounds(16, 210, 150, 20);  _txtTicketSearch.SetBounds(16, 232, 200, 32);
        lAction.SetBounds(228, 210, 160, 20); _txtTicketActionId.SetBounds(228, 232, 90, 32);

        _txtTicketSearch.TextChanged += (_, _) => ApplyTicketSearch(_txtTicketSearch.Text.Trim());

        var btnPay = UiTheme.CreatePrimaryButton("💳 TT");
        var btnCancel  = UiTheme.CreateDangerButton("✕ Hủy");
        var btnRefresh = UiTheme.CreateSecondaryButton("↺ Tải lại");

        btnPay.SetBounds(330, 230, 90, 36);
        btnCancel.SetBounds(430, 230, 90, 36);
        btnRefresh.SetBounds(530, 230, 100, 36);

        btnPay.Click += async (_, _) => await PayTicketAsync();
        btnCancel.Click  += async (_, _) => await CancelTicketAsync();
        btnRefresh.Click += async (_, _) => 
        {
            await LoadTripsAsync();
            await LoadCustomersAsync();
            await LoadTicketsAsync();
        };

        formCard.Controls.AddRange(new Control[]
        {
            l1, _cbxTicketTrip, l2, _cbxTicketCustomer, _lblTripInfo,
            l3, _pnlSeatMap,
            l4, _txtTicketPrice, l5, _cbxPaymentMethod, btnCreate,
            lSearch, _txtTicketSearch, lAction, _txtTicketActionId,
            btnPay, btnCancel, btnRefresh
        });

        _ticketsGrid = CreateGrid();
        panel.Controls.Add(_ticketsGrid);
        panel.Controls.Add(formCard);

        _cbxTicketTrip.SelectedIndexChanged += async (_, _) => await OnTripSelectedAsync();

        return panel;
    }

    // ── Helpers ─────────────────────────────────────────────────
    private static Panel ViewPanel() => new()
    {
        Dock = DockStyle.Fill,
        BackColor = UiTheme.Background,
        Visible = false
    };

    private static Label FieldLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = new Font("Segoe UI", 8.5F),
        ForeColor = UiTheme.TextMuted
    };

    private static Panel SectionToolbar(string title, EventHandler refreshHandler)
    {
        var top = new Panel
        {
            Dock = DockStyle.Top, Height = 56,
            BackColor = Color.Transparent
        };
        var lbl = new Label
        {
            Text = title,
            Font = UiTheme.HeadingFont,
            AutoSize = true,
            ForeColor = UiTheme.TextDark,
            Location = new Point(0, 16)
        };
        var btn = UiTheme.CreateSecondaryButton("↺  Tải lại");
        btn.Location = new Point(260, 12);
        btn.Click += refreshHandler;
        top.Controls.Add(lbl);
        top.Controls.Add(btn);
        return top;
    }

    private static DataGridView CreateGrid()
    {
        var grid = new DataGridView { Dock = DockStyle.Fill };
        UiTheme.StyleGrid(grid);
        return grid;
    }

    private void ShowToast(string message, Color backColor)
    {
        _toastPanel.BackColor = backColor;
        _toastLabel.Text = message;
        _toastPanel.Visible = true;
        _toastPanel.BringToFront();
        _toastTimer.Stop();
        _toastTimer.Start();
    }
}
