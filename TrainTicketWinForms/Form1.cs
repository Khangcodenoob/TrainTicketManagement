using System.Drawing.Drawing2D;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TrainTicketWinForms.Models;
using TrainTicketWinForms.Services;

namespace TrainTicketWinForms;

public partial class Form1 : Form
{
    private readonly HttpClient _httpClient = ApiService.Client;
    private readonly string _role;
    private readonly string _userName;
    private bool _handlingUnauthorized;
    private readonly ErrorProvider _errorProvider = new();
    private List<TicketItem> _ticketCache = new();
    private List<TrainTripItem> _tripCache = new();
    private List<CustomerItem> _customerCache = new();
    private List<RouteItem> _routeCache = new();

    // ── Layout panels ───────────────────────────────────────────
    private Panel _sidebar = null!;
    private Panel _topbar = null!;
    private Panel _contentPanel = null!;

    // ── Toast ───────────────────────────────────────────────────
    private Panel _toastPanel = null!;
    private Label _toastLabel = null!;
    private System.Windows.Forms.Timer _toastTimer = null!;

    // ── Loading overlay ─────────────────────────────────────────
    private Panel _loadingOverlay = null!;

    // ── Topbar controls ─────────────────────────────────────────
    private Label _titleLabel = null!;
    private TextBox _globalSearchBox = null!;

    // ── Sidebar buttons ─────────────────────────────────────────
    private Button _btnDashboard = null!;
    private Button _btnRoutes    = null!;
    private Button _btnTrips     = null!;
    private Button _btnCustomers = null!;
    private Button _btnTickets   = null!;
    private Button _btnStations  = null!;

    // ── Views ───────────────────────────────────────────────────
    private Panel _dashboardView  = null!;
    private Panel _routesView     = null!;
    private Panel _tripsView      = null!;
    private Panel _customersView  = null!;
    private Panel _ticketsView    = null!;
    private Panel _stationsView   = null!;

    // ── Dashboard ───────────────────────────────────────────────
    private Label _lblTotalTickets = null!;
    private Label _lblRevenue      = null!;
    private Label _lblTrips        = null!;
    private Label _lblCustomers    = null!;
    private Panel _chartPanel      = null!;
    private double[] _chartValues  = [];
    private string[] _chartLabels  = [];

    // ── Grids ───────────────────────────────────────────────────
    private DataGridView _routesGrid    = null!;
    private DataGridView _tripsGrid     = null!;
    private DataGridView _customersGrid = null!;
    private DataGridView _ticketsGrid   = null!;

    // ── Trip filter ─────────────────────────────────────────────
    private ComboBox     _txtTripFrom   = null!;
    private ComboBox     _txtTripTo     = null!;
    private DateTimePicker _dtTripDate  = null!;

    private ComboBox _cbxTicketTrip      = null!;
    private ComboBox _cbxTicketCustomer  = null!;
    private Label    _lblTripInfo        = null!;
    private FlowLayoutPanel _pnlSeatMap  = null!;
    private TextBox  _txtTicketPrice     = null!;
    private ComboBox _cbxPaymentMethod   = null!;
    private TextBox  _txtTicketSearch    = null!;
    private TextBox  _txtTicketActionId  = null!;
    private string   _selectedSeat       = string.Empty;

    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public bool ShouldReturnToLogin { get; private set; }

    public Form1(string role, string userName)
    {
        _role = role;
        _userName = userName;
        InitializeComponent();
        BuildLayout();
    }

    // ═══════════════════════════════════════════════════════════
    //  LAYOUT
    // ═══════════════════════════════════════════════════════════

    private void BuildLayout()
    {
        Controls.Clear();
        MinimumSize = new Size(1200, 700);

        _sidebar      = new Panel { Dock = DockStyle.Left,  Width  = 260, BackColor = UiTheme.SidebarBg };
        _topbar       = new Panel { Dock = DockStyle.Top,   Height = 68,  BackColor = Color.White };
        _contentPanel = new Panel { Dock = DockStyle.Fill,  BackColor = UiTheme.Background, Padding = new Padding(20) };

        // Topbar bottom border
        _topbar.Paint += (_, e) =>
        {
            using var pen = new Pen(UiTheme.Border, 1);
            e.Graphics.DrawLine(pen, 0, _topbar.Height - 1, _topbar.Width, _topbar.Height - 1);
        };

        BuildSidebar();
        BuildTopbar();
        BuildViews();
        BuildToast();
        BuildLoadingOverlay();

        Controls.Add(_contentPanel);
        Controls.Add(_topbar);
        Controls.Add(_sidebar);

        BackColor = UiTheme.Background;
        Font = UiTheme.TextFont;
        Text = "🚂 Quản lý Bán Vé Tàu Hỏa";
        WindowState = FormWindowState.Maximized;
    }

    // ── Sidebar ─────────────────────────────────────────────────
    private void BuildSidebar()
    {
        // Logo area
        var logoPanel = new Panel
        {
            Dock = DockStyle.Top, Height = 76,
            BackColor = ColorTranslator.FromHtml("#080F1E")
        };
        logoPanel.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var brush = new LinearGradientBrush(
                new Rectangle(0, 0, logoPanel.Width, logoPanel.Height),
                ColorTranslator.FromHtml("#080F1E"),
                ColorTranslator.FromHtml("#0F172A"),
                LinearGradientMode.Vertical);
            g.FillRectangle(brush, 0, 0, logoPanel.Width, logoPanel.Height);

            using var font1 = new Font("Segoe UI Emoji", 20F);
            using var font2 = new Font("Segoe UI", 11F, FontStyle.Bold);
            using var font3 = new Font("Segoe UI", 8F);
            using var wb = new SolidBrush(Color.White);
            using var gb = new SolidBrush(ColorTranslator.FromHtml("#94A3B8"));
            g.DrawString("🚂", font1, wb, 14, 14);
            g.DrawString("TrainTicket", font2, wb, 58, 16);
            g.DrawString("Quản lý bán vé tàu hỏa", font3, gb, 58, 42);
        };

        // Nav buttons
        _btnDashboard = UiTheme.CreateSidebarButton("📊", "Bảng điều khiển");
        _btnRoutes    = UiTheme.CreateSidebarButton("🗺️", "Tuyến tàu");
        _btnTrips     = UiTheme.CreateSidebarButton("🚆", "Chuyến tàu");
        _btnCustomers = UiTheme.CreateSidebarButton("👥", "Khách hàng");
        _btnTickets   = UiTheme.CreateSidebarButton("🎫", "Quản lý vé");
        _btnStations  = UiTheme.CreateSidebarButton("📋", "Quy định ga tàu");

        _btnDashboard.Click += (_, _) => ActivateMenu(_btnDashboard, _dashboardView, "📊  Bảng điều khiển");
        _btnRoutes.Click    += (_, _) => ActivateMenu(_btnRoutes,    _routesView,    "🗺️  Tuyến tàu");
        _btnTrips.Click     += (_, _) => ActivateMenu(_btnTrips,     _tripsView,     "🚆  Chuyến tàu");
        _btnCustomers.Click += (_, _) => ActivateMenu(_btnCustomers, _customersView, "👥  Khách hàng");
        _btnTickets.Click   += (_, _) => ActivateMenu(_btnTickets,   _ticketsView,   "🎫  Quản lý vé");
        _btnStations.Click  += (_, _) => ActivateMenu(_btnStations,  _stationsView,  "📋  Quy định ga tàu");

        // User info at bottom
        var userPanel = new Panel
        {
            Dock = DockStyle.Bottom, Height = 64,
            BackColor = ColorTranslator.FromHtml("#080F1E")
        };
        var lblUser = new Label
        {
            Text = $"  👤  {_userName}",
            Dock = DockStyle.Top, Height = 32,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft
        };
        var lblRole = new Label
        {
            Text = $"      {_role}",
            Dock = DockStyle.Fill,
            Font = UiTheme.SmallFont,
            ForeColor = ColorTranslator.FromHtml("#94A3B8"),
            TextAlign = ContentAlignment.MiddleLeft
        };
        userPanel.Controls.Add(lblRole);
        userPanel.Controls.Add(lblUser);

        _sidebar.Controls.Add(userPanel);
        _sidebar.Controls.Add(_btnStations);
        _sidebar.Controls.Add(_btnTickets);
        _sidebar.Controls.Add(_btnCustomers);
        _sidebar.Controls.Add(_btnTrips);
        _sidebar.Controls.Add(_btnRoutes);
        _sidebar.Controls.Add(_btnDashboard);
        _sidebar.Controls.Add(logoPanel);
    }

    // ── Topbar ──────────────────────────────────────────────────
    private void BuildTopbar()
    {
        _titleLabel = new Label
        {
            Text = "📊  Bảng điều khiển",
            AutoSize = true,
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            Location = new Point(20, 20),
            ForeColor = UiTheme.TextDark
        };

        _globalSearchBox = UiTheme.CreateInput("🔍  Tìm nhanh theo mã vé...");
        _globalSearchBox.Width = 280;
        _globalSearchBox.Location = new Point(340, 19);
        _globalSearchBox.TextChanged += (_, _) => ApplyTicketSearch(_globalSearchBox.Text.Trim());

        var lblUserInfo = new Label
        {
            AutoSize = true,
            Font = UiTheme.SmallFont,
            ForeColor = UiTheme.TextMuted,
            Text = $"{_userName}  •  {_role}",
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
        };

        var btnLogout = UiTheme.CreateDangerButton("🚪  Đăng xuất");
        btnLogout.Width = 120;
        btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnLogout.Click += async (_, _) => await LogoutAsync();

        _topbar.Controls.Add(_titleLabel);
        _topbar.Controls.Add(_globalSearchBox);
        _topbar.Controls.Add(lblUserInfo);
        _topbar.Controls.Add(btnLogout);
        _topbar.Resize += (_, _) =>
        {
            btnLogout.Left = _topbar.Width - btnLogout.Width - 10;
            btnLogout.Top = (_topbar.Height - btnLogout.Height) / 2;
            lblUserInfo.Left = btnLogout.Left - lblUserInfo.Width - 15;
            lblUserInfo.Top  = (_topbar.Height - lblUserInfo.Height) / 2;
        };
    }

    // ── Active menu highlight ───────────────────────────────────
    private void ActivateMenu(Button menuButton, Panel view, string title)
    {
        var allButtons = new[] { _btnDashboard, _btnRoutes, _btnTrips, _btnCustomers, _btnTickets, _btnStations };
        foreach (var b in allButtons)
        {
            b.BackColor = UiTheme.SidebarBg;
            b.ForeColor = ColorTranslator.FromHtml("#94A3B8");
            b.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
        }

        foreach (Control c in _contentPanel.Controls)
            c.Visible = false;

        _titleLabel.Text = title;
        menuButton.BackColor = UiTheme.SidebarActive;
        menuButton.ForeColor = Color.White;
        menuButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        view.Visible = true;
        view.BringToFront();
    }

    // ── Toast ───────────────────────────────────────────────────
    private void BuildToast()
    {
        _toastPanel = new Panel
        {
            Size = new Size(360, 50),
            Visible = false,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        _toastPanel.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(_toastPanel.BackColor);
            UiTheme.FillRoundedRect(g, brush, new Rectangle(0, 0, _toastPanel.Width, _toastPanel.Height), 8);
        };

        _toastLabel = new Label
        {
            Dock = DockStyle.Fill,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(14, 0, 14, 0),
            BackColor = Color.Transparent
        };
        _toastPanel.Controls.Add(_toastLabel);
        Controls.Add(_toastPanel);

        Resize += (_, _) =>
        {
            _toastPanel.Left = ClientSize.Width  - _toastPanel.Width  - 24;
            _toastPanel.Top  = ClientSize.Height - _toastPanel.Height - 24;
        };

        _toastTimer = new System.Windows.Forms.Timer { Interval = 2800 };
        _toastTimer.Tick += (_, _) => { _toastPanel.Visible = false; _toastTimer.Stop(); };
    }

    // ── Loading overlay ─────────────────────────────────────────
    private void BuildLoadingOverlay()
    {
        _loadingOverlay = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(120, 248, 250, 252),
            Visible = false
        };
        var lbl = new Label
        {
            AutoSize = true,
            Font = UiTheme.HeadingFont,
            Text = "⏳  Đang tải dữ liệu...",
            ForeColor = UiTheme.Primary
        };
        _loadingOverlay.Controls.Add(lbl);
        _loadingOverlay.Resize += (_, _) =>
        {
            lbl.Left = (_loadingOverlay.Width - lbl.Width) / 2;
            lbl.Top  = (_loadingOverlay.Height - lbl.Height) / 2;
        };
        Controls.Add(_loadingOverlay);
        _loadingOverlay.BringToFront();
    }
}
