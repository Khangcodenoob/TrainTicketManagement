using System.Drawing.Drawing2D;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TrainTicketWinForms.Models;
using TrainTicketWinForms.Services;

namespace TrainTicketWinForms;

public class LoginForm : Form
{
    private readonly TextBox _txtUserName;
    private readonly TextBox _txtPassword;
    private readonly Button  _btnLogin;
    private readonly Label   _lblStatus = new() { AutoSize = true };

    public LoginResponse? LoginData { get; private set; }

    public LoginForm()
    {
        _txtUserName = UiTheme.CreateInput();
        _txtPassword = UiTheme.CreateInput();
        _btnLogin    = UiTheme.CreatePrimaryButton("  Đăng nhập");

        Text = "Đăng nhập — Quản lý Vé Tàu Hỏa";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.None;
        Size = new Size(900, 560);
        BackColor = Color.White;

        // ── Left gradient panel ─────────────────────────────────
        var leftPanel = new Panel { Dock = DockStyle.Left, Width = 400 };
        leftPanel.Paint += (s, e) =>
        {
            var p = (Panel)s!;
            PaintGradientLeft(e.Graphics, p.Width, p.Height);
        };

        var lblTrain = new Label
        {
            Text = "🚂",
            Font = new Font("Segoe UI Emoji", 52F),
            AutoSize = true, BackColor = Color.Transparent,
            Location = new Point(70, 100)
        };
        var lblTitle = new Label
        {
            Text = "Quản lý\nBán Vé Tàu Hỏa",
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            AutoSize = true, BackColor = Color.Transparent,
            ForeColor = Color.White, Location = new Point(60, 210)
        };
        var lblSub = new Label
        {
            Text = "Hệ thống quản lý vé tàu hỏa\nhiện đại và chuyên nghiệp.",
            Font = new Font("Segoe UI", 11F),
            AutoSize = true, BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(190, 255, 255, 255),
            Location = new Point(60, 318)
        };
        var lblVer = new Label
        {
            Text = "© 2026 TrainTicket System v2.0",
            Font = new Font("Segoe UI", 8.5F),
            AutoSize = true, BackColor = Color.Transparent,
            ForeColor = Color.FromArgb(110, 255, 255, 255),
            Location = new Point(60, 510)
        };
        var btnClose = new Button
        {
            Text = "✕", Size = new Size(30, 30),
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            BackColor = Color.Transparent, ForeColor = Color.White,
            Font = new Font("Segoe UI", 11F),
            Location = new Point(362, 10)
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (_, _) => Application.Exit();
        leftPanel.Controls.AddRange(new Control[] { lblTrain, lblTitle, lblSub, lblVer, btnClose });

        // ── Right panel ─────────────────────────────────────────
        var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        rightPanel.MouseDown += (_, me) =>
        {
            if (me.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        };
        BuildRightPanel(rightPanel);

        Controls.Add(rightPanel);
        Controls.Add(leftPanel);

        // ── Fade-in ─────────────────────────────────────────────
        Opacity = 0;
        var timer = new System.Windows.Forms.Timer { Interval = 15 };
        int step = 0;
        timer.Tick += (_, _) =>
        {
            Opacity = Math.Min(1.0, ++step * 0.06);
            if (Opacity >= 1.0) timer.Stop();
        };
        Load += (_, _) => timer.Start();

        _txtUserName.Text = "admin";
        _txtPassword.Text = "admin123";
    }

    private void BuildRightPanel(Panel right)
    {
        var lblWelcome = new Label
        {
            Text = "Chào mừng trở lại! 👋",
            Font = new Font("Segoe UI", 19F, FontStyle.Bold),
            ForeColor = UiTheme.TextDark,
            AutoSize = true, Location = new Point(70, 80)
        };
        var lblHint = new Label
        {
            Text = "Vui lòng đăng nhập để tiếp tục.",
            Font = UiTheme.TextFont, ForeColor = UiTheme.TextMuted,
            AutoSize = true, Location = new Point(70, 122)
        };

        var lblUser = FieldLabel("Tên đăng nhập", 70, 166);
        _txtUserName.SetBounds(70, 190, 350, 36);
        _txtUserName.Font = UiTheme.HeadingFont;

        var lblPass = FieldLabel("Mật khẩu", 70, 245);
        _txtPassword.SetBounds(70, 269, 310, 36);
        _txtPassword.Font = UiTheme.HeadingFont;
        _txtPassword.UseSystemPasswordChar = true;

        var btnShowPass = new Button
        {
            Text = "👁️",
            Font = new Font("Segoe UI Emoji", 10F),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = UiTheme.TextMuted,
            Cursor = Cursors.Hand,
            Size = new Size(36, 36),
            Location = new Point(384, 269)
        };
        btnShowPass.FlatAppearance.BorderSize = 1;
        btnShowPass.FlatAppearance.BorderColor = UiTheme.Border;
        btnShowPass.Click += (_, _) =>
        {
            _txtPassword.UseSystemPasswordChar = !_txtPassword.UseSystemPasswordChar;
            btnShowPass.Text = _txtPassword.UseSystemPasswordChar ? "👁️" : "🙈";
        };

        _btnLogin.SetBounds(70, 332, 350, 46);
        _btnLogin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        _btnLogin.Click += BtnLogin_Click;

        _lblStatus.SetBounds(70, 390, 350, 24);
        _lblStatus.Font = UiTheme.TextFont;

        var divider = new Panel
        {
            BackColor = UiTheme.Border,
            Size = new Size(350, 1),
            Location = new Point(70, 430)
        };
        var lblFooter = new Label
        {
            Text = "Hệ thống quản lý vé tàu hỏa nội bộ",
            Font = UiTheme.SmallFont, ForeColor = UiTheme.TextMuted,
            AutoSize = true, Location = new Point(70, 442)
        };

        right.Controls.AddRange(new Control[]
        {
            lblWelcome, lblHint,
            lblUser, _txtUserName,
            lblPass, _txtPassword, btnShowPass,
            _btnLogin, _lblStatus,
            divider, lblFooter
        });
    }

    private static Label FieldLabel(string text, int x, int y) => new()
    {
        Text = text, Font = UiTheme.TextFont, ForeColor = UiTheme.TextMuted,
        AutoSize = true, Location = new Point(x, y)
    };

    private static void PaintGradientLeft(Graphics g, int w, int h)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var brush = new LinearGradientBrush(
            new Rectangle(0, 0, w, h),
            ColorTranslator.FromHtml("#1E3A5F"),
            ColorTranslator.FromHtml("#6366F1"),
            LinearGradientMode.ForwardDiagonal);
        g.FillRectangle(brush, 0, 0, w, h);

        using var cb = new SolidBrush(Color.FromArgb(25, 255, 255, 255));
        g.FillEllipse(cb, -80, -80, 280, 280);
        g.FillEllipse(cb, w - 120, h - 140, 220, 220);
        g.FillEllipse(cb, w - 70, 30, 140, 140);
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        try
        {
            _btnLogin.Enabled = false;
            _btnLogin.Text = "  Đang đăng nhập...";
            _lblStatus.Text = string.Empty;

            var payload = new { UserName = _txtUserName.Text.Trim(), Password = _txtPassword.Text };
            var content = new StringContent(
                JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = await ApiService.Client.PostAsync("api/auth/login", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _lblStatus.ForeColor = UiTheme.Danger;
                _lblStatus.Text = "❌  Đăng nhập thất bại. Kiểm tra lại tài khoản.";
                return;
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var result = JsonConvert.DeserializeObject<ApiResponse<LoginResponse>>(body, settings);
            if (result?.Data is null)
            {
                _lblStatus.ForeColor = UiTheme.Danger;
                _lblStatus.Text = "❌  Dữ liệu đăng nhập không hợp lệ.";
                return;
            }

            var accessToken = result.Data.AccessToken?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                _lblStatus.ForeColor = UiTheme.Danger;
                _lblStatus.Text = "Dang nhap thanh cong nhung khong nhan duoc access token.";
                return;
            }

            result.Data.AccessToken = accessToken;
            AuthService.SetSession(result.Data);
            LoginData = result.Data;
            _lblStatus.ForeColor = UiTheme.Success;
            _lblStatus.Text = "✅  Đăng nhập thành công!";
            DialogResult = DialogResult.OK;
            await Task.Delay(300);
            Close();
        }
        catch (Exception ex)
        {
            _lblStatus.ForeColor = UiTheme.Danger;
            _lblStatus.Text = $"❌  {ex.Message}";
        }
        finally
        {
            _btnLogin.Enabled = true;
            _btnLogin.Text = "  Đăng nhập";
        }
    }
}

internal static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    internal static extern bool ReleaseCapture();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
}
