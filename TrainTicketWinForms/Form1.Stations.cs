using System.Drawing.Drawing2D;

namespace TrainTicketWinForms;

public partial class Form1
{
    // ═══════════════════════════════════════════════════════════
    //  TRANG QUY ĐỊNH GA TÀU
    // ═══════════════════════════════════════════════════════════

    private Panel BuildStationsView()
    {
        var panel = ViewPanel();

        // ── Scrollable container ────────────────────────────────
        var scroll = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = UiTheme.Background,
            Padding = new Padding(0, 4, 8, 0)
        };

        // ── 1. Header banner ────────────────────────────────────
        var banner = BuildBanner();
        banner.Dock = DockStyle.Top;
        banner.Height = 110;

        // ── 2. Quick-info cards ─────────────────────────────────
        var infoRow = BuildInfoCards();
        infoRow.Dock = DockStyle.Top;
        infoRow.Height = 118;

        // ── 3. Section title: Nội quy ──────────────────────────
        var sec1Title = SectionTitle("📜  Nội quy hành khách tại ga");

        // ── 4. Regulation cards (2-column flow) ─────────────────
        var regFlow = BuildRegulationCards();
        regFlow.Dock = DockStyle.Top;
        regFlow.Height = 380;

        // ── 5. Section title: Hành lý ──────────────────────────
        var sec2Title = SectionTitle("🧳  Quy định hành lý");

        // ── 6. Baggage table ────────────────────────────────────
        var baggageCard = BuildBaggageTable();
        baggageCard.Dock = DockStyle.Top;
        baggageCard.Height = 220;

        // ── 7. Section title: Danh sách ga ─────────────────────
        var sec3Title = SectionTitle("🗺️  Thông tin các ga chính");

        // ── 8. Station info table ───────────────────────────────
        var stationCard = BuildStationTable();
        stationCard.Dock = DockStyle.Top;
        stationCard.Height = 340;

        // Add bottom spacer
        var spacer = new Panel { Height = 24, Dock = DockStyle.Top, BackColor = Color.Transparent };

        // Add in reverse (Dock=Top renders bottom-to-top order)
        scroll.Controls.Add(spacer);
        scroll.Controls.Add(stationCard);
        scroll.Controls.Add(sec3Title);
        scroll.Controls.Add(baggageCard);
        scroll.Controls.Add(sec2Title);
        scroll.Controls.Add(regFlow);
        scroll.Controls.Add(sec1Title);
        scroll.Controls.Add(infoRow);
        scroll.Controls.Add(banner);

        panel.Controls.Add(scroll);
        return panel;
    }

    // ── Banner ──────────────────────────────────────────────────
    private static Panel BuildBanner()
    {
        var banner = new Panel { Margin = new Padding(0, 0, 0, 14) };
        banner.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var br = new LinearGradientBrush(
                new Rectangle(0, 0, banner.Width, banner.Height),
                ColorTranslator.FromHtml("#1E3A5F"),
                ColorTranslator.FromHtml("#2563EB"),
                LinearGradientMode.Horizontal);
            UiTheme.FillRoundedRect(g, br, new Rectangle(0, 0, banner.Width - 1, banner.Height - 1), 12);

            // Decorative circle
            using var cb = new SolidBrush(Color.FromArgb(20, 255, 255, 255));
            g.FillEllipse(cb, banner.Width - 160, -40, 200, 200);
            g.FillEllipse(cb, banner.Width - 80, 20, 120, 120);

            using var fIcon = new Font("Segoe UI Emoji", 30F);
            using var fTitle = new Font("Segoe UI", 18F, FontStyle.Bold);
            using var fSub = new Font("Segoe UI", 10F);
            using var wb = new SolidBrush(Color.White);
            using var sb = new SolidBrush(Color.FromArgb(200, 255, 255, 255));

            g.DrawString("🚉", fIcon, wb, 20, 22);
            g.DrawString("Quy định & Thông tin Ga Tàu Hỏa", fTitle, wb, 80, 26);
            g.DrawString("Tổng hợp nội quy, quy định hành lý và thông tin vận hành tại các ga tàu hỏa Việt Nam.", fSub, sb, 82, 66);
        };
        return banner;
    }

    // ── Quick info cards ────────────────────────────────────────
    private static Panel BuildInfoCards()
    {
        var flow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            BackColor = Color.Transparent,
            Padding = new Padding(0),
            Margin = new Padding(0, 0, 0, 8)
        };

        flow.Controls.Add(QuickInfoCard("🕐", "Giờ hoạt động", "05:00 – 22:00\nHằng ngày", UiTheme.Primary));
        flow.Controls.Add(QuickInfoCard("⏱️", "Thời gian check-in", "Trước 30 phút\nGiờ tàu khởi hành", UiTheme.Accent));
        flow.Controls.Add(QuickInfoCard("🧳", "Hành lý miễn cước", "Tối đa 30 kg\nMỗi hành khách", UiTheme.Success));
        flow.Controls.Add(QuickInfoCard("🎫", "Đổi/Hoàn vé", "Trước 24 giờ\nKhởi hành", UiTheme.Warning));
        flow.Controls.Add(QuickInfoCard("🚭", "Khu vực cấm", "Cấm hút thuốc\nTrên toàn bộ ga", UiTheme.Danger));

        return flow;
    }

    private static ShadowPanel QuickInfoCard(string icon, string title, string value, Color accent)
    {
        var card = new ShadowPanel { Width = 195, Height = 102, Margin = new Padding(0, 0, 14, 0) };
        card.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var b = new SolidBrush(Color.FromArgb(30, accent.R, accent.G, accent.B));
            e.Graphics.FillRectangle(b, 0, 0, 5, card.Height - 6);
            using var ab = new SolidBrush(accent);
            e.Graphics.FillRectangle(ab, 0, 0, 4, card.Height - 6);
        };

        var lblIcon = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI Emoji", 18F),
            AutoSize = true,
            Location = new Point(14, 10)
        };
        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
            ForeColor = UiTheme.TextMuted,
            AutoSize = true,
            Location = new Point(14, 48)
        };
        var lblVal = new Label
        {
            Text = value,
            Font = new Font("Segoe UI", 8F),
            ForeColor = UiTheme.TextDark,
            AutoSize = true,
            Location = new Point(14, 66)
        };
        card.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblVal });
        return card;
    }

    // ── Section title ───────────────────────────────────────────
    private static Panel SectionTitle(string text)
    {
        var p = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.Transparent };
        p.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(UiTheme.Primary, 3);
            g.DrawLine(pen, 0, 36, 60, 36);
        };
        var lbl = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = UiTheme.TextDark,
            AutoSize = true,
            Location = new Point(0, 8)
        };
        p.Controls.Add(lbl);
        return p;
    }

    // ── Regulation cards ────────────────────────────────────────
    private static Panel BuildRegulationCards()
    {
        var flow = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Color.Transparent,
            Padding = new Padding(0),
            Margin = new Padding(0, 0, 0, 8)
        };

        var regs = new (string Icon, string Title, string Body, Color Color)[]
        {
            ("🎫", "Mua vé & Lên tàu",
             "• Hành khách phải xuất trình vé hợp lệ khi lên tàu.\n• Vé mua online cần kích hoạt tại quầy hoặc app.\n• Trẻ em dưới 6 tuổi được miễn vé, ngồi cùng ghế phụ huynh.\n• Không chuyển nhượng vé cho người khác.",
             UiTheme.Primary),

            ("🆔", "Giấy tờ tuỳ thân",
             "• Mang theo CCCD/Hộ chiếu khi đi tàu đường dài.\n• Trẻ em cần giấy khai sinh hoặc hộ chiếu.\n• Người nước ngoài cần hộ chiếu còn hiệu lực.\n• Kiểm tra giấy tờ được thực hiện trước khi lên tàu.",
             UiTheme.Accent),

            ("🔇", "Trật tự & Văn minh",
             "• Giữ yên lặng, không gây ồn ào từ 22:00 – 06:00.\n• Không sử dụng loa ngoài, nhạc chuông to.\n• Không xả rác, giữ vệ sinh khu vực ghế ngồi.\n• Ưu tiên chỗ ngồi cho người già, trẻ em, phụ nữ mang thai.",
             UiTheme.Success),

            ("🚫", "Các hành vi bị cấm",
             "• Cấm mang chất nổ, chất lỏng dễ cháy lên tàu.\n• Cấm hút thuốc trên toàn bộ tàu và sân ga.\n• Cấm mang động vật sống (trừ thú cưng có lồng).\n• Cấm bán hàng rong, quảng cáo trái phép trong ga.",
             UiTheme.Danger),

            ("🔄", "Đổi & Hoàn vé",
             "• Đổi vé: Trước giờ tàu 24h, phí 20% giá vé.\n• Hoàn vé: Trước 48h, hoàn 75% giá vé.\n• Vé giảm giá và khuyến mãi không được hoàn trả.\n• Liên hệ quầy vé hoặc tổng đài 1900-1717.",
             UiTheme.Warning),

            ("♿", "Hỗ trợ đặc biệt",
             "• Người khuyết tật được hỗ trợ miễn phí lên xuống tàu.\n• Ưu tiên khu vực ghế xe lăn toa đầu & cuối.\n• Phụ nữ mang thai được ưu tiên kiểm tra vé riêng.\n• Liên hệ nhân viên ga để được hỗ trợ.",
             ColorTranslator.FromHtml("#8B5CF6")),
        };

        foreach (var (icon, title, body, color) in regs)
            flow.Controls.Add(RegCard(icon, title, body, color));

        return flow;
    }

    private static ShadowPanel RegCard(string icon, string title, string body, Color accent)
    {
        var card = new ShadowPanel { Width = 470, Height = 164, Margin = new Padding(0, 0, 16, 14) };

        // Accent top bar
        card.Paint += (_, e) =>
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var b = new SolidBrush(accent);
            var r = new Rectangle(0, 0, card.Width - 6, 4);
            e.Graphics.FillRectangle(b, r);
        };

        var lblIcon = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI Emoji", 16F),
            AutoSize = true,
            Location = new Point(14, 14)
        };
        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
            ForeColor = UiTheme.TextDark,
            AutoSize = true,
            Location = new Point(52, 18)
        };
        var lblBody = new Label
        {
            Text = body,
            Font = new Font("Segoe UI", 8.5F),
            ForeColor = UiTheme.TextMuted,
            Size = new Size(420, 114),
            Location = new Point(14, 48),
            TextAlign = ContentAlignment.TopLeft
        };
        card.Controls.AddRange(new Control[] { lblIcon, lblTitle, lblBody });
        return card;
    }

    // ── Baggage table ───────────────────────────────────────────
    private static ShadowPanel BuildBaggageTable()
    {
        var card = new ShadowPanel { Margin = new Padding(0, 0, 0, 8) };

        var grid = new DataGridView { Dock = DockStyle.Fill };
        UiTheme.StyleGrid(grid);
        grid.ColumnHeadersHeight = 40;

        grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "👤  Loại hành khách",  FillWeight = 30 },
            new DataGridViewTextBoxColumn { HeaderText = "🧳  Miễn cước (kg)",   FillWeight = 20 },
            new DataGridViewTextBoxColumn { HeaderText = "💰  Phụ thu (VND/kg)",  FillWeight = 20 },
            new DataGridViewTextBoxColumn { HeaderText = "📐  Kích thước tối đa", FillWeight = 30 }
        );

        object[][] rows =
        [
            ["Người lớn (≥ 12 tuổi)",  "30 kg", "5.000 VND/kg",  "100 × 60 × 40 cm"],
            ["Trẻ em (6–11 tuổi)",     "15 kg", "5.000 VND/kg",  "80 × 50 × 30 cm"],
            ["Trẻ em (dưới 6 tuổi)",   "10 kg", "Miễn phí",      "Hành lý gọn nhẹ"],
            ["Hàng hóa thông thường",  "—",     "8.000 VND/kg",  "Thỏa thuận với ga"],
            ["Hàng cồng kềnh/đặc biệt","—",     "Theo giá dịch vụ", "Liên hệ ga trước"],
        ];

        foreach (var r in rows)
            grid.Rows.Add(r);

        card.Controls.Add(grid);
        return card;
    }

    // ── Station info table ──────────────────────────────────────
    private static ShadowPanel BuildStationTable()
    {
        var card = new ShadowPanel { Margin = new Padding(0, 0, 0, 8) };

        var grid = new DataGridView { Dock = DockStyle.Fill };
        UiTheme.StyleGrid(grid);

        grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { HeaderText = "🚨  Tên ga",           FillWeight = 22 },
            new DataGridViewTextBoxColumn { HeaderText = "📍  Tỉnh/TP",          FillWeight = 18 },
            new DataGridViewTextBoxColumn { HeaderText = "🏢  Loại ga",          FillWeight = 14 },
            new DataGridViewTextBoxColumn { HeaderText = "🕒  Giờ mở cửa",       FillWeight = 16 },
            new DataGridViewTextBoxColumn { HeaderText = "🚉  Số ke ga",         FillWeight = 12 },
            new DataGridViewTextBoxColumn { HeaderText = "✨  Tiện ích",         FillWeight = 18 }
        );

        object[][] rows =
        [
            ["Ga Hà Nội",       "Hà Nội",       "Ga trung tâm", "05:00–22:30", "5 ke",  "WiFi, Phòng chờ VIP, ATM, Nhà hàng"],
            ["Ga Sài Gòn",      "TP. HCM",      "Ga trung tâm", "05:00–22:30", "4 ke",  "WiFi, Phòng chờ VIP, ATM, Nhà hàng"],
            ["Ga Đà Nẵng",      "Đà Nẵng",      "Ga lớn",       "05:30–22:00", "3 ke",  "WiFi, Phòng chờ, ATM"],
            ["Ga Huế",          "Thừa Thiên Huế","Ga lớn",       "05:30–21:00", "2 ke",  "WiFi, Phòng chờ, Quán cà phê"],
            ["Ga Nha Trang",    "Khánh Hòa",    "Ga lớn",       "05:30–21:30", "3 ke",  "WiFi, Phòng chờ, ATM"],
            ["Ga Vinh",         "Nghệ An",      "Ga lớn",       "05:00–21:00", "3 ke",  "WiFi, Phòng chờ, Nhà hàng"],
            ["Ga Đồng Hới",     "Quảng Bình",   "Ga vừa",       "06:00–20:00", "2 ke",  "Phòng chờ, Quán ăn"],
            ["Ga Giáp Bát",     "Hà Nội",       "Ga vừa",       "05:30–21:00", "3 ke",  "WiFi, Phòng chờ"],
            ["Ga Biên Hòa",     "Đồng Nai",     "Ga vừa",       "05:30–21:00", "2 ke",  "Phòng chờ, ATM"],
            ["Ga Lào Cai",      "Lào Cai",      "Ga biên giới", "06:00–20:00", "2 ke",  "Phòng chờ, Dịch vụ hải quan"],
            ["Ga Lạng Sơn",     "Lạng Sơn",     "Ga biên giới", "06:00–19:00", "2 ke",  "Phòng chờ, Dịch vụ hải quan"],
            ["Ga Hải Phòng",    "Hải Phòng",    "Ga lớn",       "05:30–21:00", "2 ke",  "WiFi, Phòng chờ, ATM"],
        ];

        foreach (var r in rows)
        {
            int idx = grid.Rows.Add(r);
            // Color-code by station type
            var type = r[2].ToString()!;
            grid.Rows[idx].DefaultCellStyle.BackColor = type switch
            {
                "Ga trung tâm" => ColorTranslator.FromHtml("#EFF6FF"),
                "Ga biên giới" => ColorTranslator.FromHtml("#FEF3C7"),
                _ => Color.White
            };
        }

        card.Controls.Add(grid);
        return card;
    }
}
