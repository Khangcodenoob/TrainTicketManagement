using System.Drawing.Drawing2D;

namespace TrainTicketWinForms;

public static class UiTheme
{
    // ── Color Palette ──────────────────────────────────────────
    public static readonly Color Primary        = ColorTranslator.FromHtml("#2563EB");
    public static readonly Color PrimaryHover   = ColorTranslator.FromHtml("#1D4ED8");
    public static readonly Color Accent         = ColorTranslator.FromHtml("#6366F1");
    public static readonly Color SidebarBg      = ColorTranslator.FromHtml("#0F172A");
    public static readonly Color SidebarHover   = ColorTranslator.FromHtml("#1E293B");
    public static readonly Color SidebarActive  = ColorTranslator.FromHtml("#1E3A5F");
    public static readonly Color Secondary      = ColorTranslator.FromHtml("#1E293B");
    public static readonly Color Background     = ColorTranslator.FromHtml("#F1F5F9");
    public static readonly Color Surface        = Color.White;
    public static readonly Color Success        = ColorTranslator.FromHtml("#22C55E");
    public static readonly Color SuccessLight   = ColorTranslator.FromHtml("#DCFCE7");
    public static readonly Color Danger         = ColorTranslator.FromHtml("#EF4444");
    public static readonly Color DangerLight    = ColorTranslator.FromHtml("#FEE2E2");
    public static readonly Color Warning        = ColorTranslator.FromHtml("#F59E0B");
    public static readonly Color WarningLight   = ColorTranslator.FromHtml("#FEF3C7");
    public static readonly Color Border         = ColorTranslator.FromHtml("#E2E8F0");
    public static readonly Color TextMuted      = ColorTranslator.FromHtml("#64748B");
    public static readonly Color TextDark       = ColorTranslator.FromHtml("#0F172A");

    // ── Fonts ──────────────────────────────────────────────────
    public static readonly Font TitleFont      = new("Segoe UI", 15F, FontStyle.Bold);
    public static readonly Font HeadingFont    = new("Segoe UI", 11F, FontStyle.Bold);
    public static readonly Font TextFont       = new("Segoe UI", 9.5F, FontStyle.Regular);
    public static readonly Font SmallFont      = new("Segoe UI", 8.5F, FontStyle.Regular);
    public static readonly Font LargeNumFont   = new("Segoe UI", 26F, FontStyle.Bold);
    public static readonly Font EmojiFont      = new("Segoe UI Emoji", 14F);

    // ── Chart Colors ───────────────────────────────────────────
    public static readonly Color[] ChartColors =
    [
        ColorTranslator.FromHtml("#3B82F6"),
        ColorTranslator.FromHtml("#22C55E"),
        ColorTranslator.FromHtml("#F59E0B"),
        ColorTranslator.FromHtml("#EF4444"),
        ColorTranslator.FromHtml("#8B5CF6"),
    ];

    // ── Sidebar Button ─────────────────────────────────────────
    public static Button CreateSidebarButton(string icon, string label)
    {
        var btn = new Button
        {
            Text = $"  {icon}  {label}",
            Dock = DockStyle.Top,
            Height = 52,
            FlatStyle = FlatStyle.Flat,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0),
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            ForeColor = ColorTranslator.FromHtml("#94A3B8"),
            BackColor = SidebarBg,
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = SidebarHover }
        };
        return btn;
    }

    // ── Primary Button ─────────────────────────────────────────
    public static Button CreatePrimaryButton(string text)
    {
        return new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            Height = 38,
            Width = 130,
            BackColor = Primary,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = PrimaryHover }
        };
    }

    // ── Secondary Button ───────────────────────────────────────
    public static Button CreateSecondaryButton(string text)
    {
        return new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            Height = 38,
            Width = 120,
            BackColor = ColorTranslator.FromHtml("#E2E8F0"),
            ForeColor = Secondary,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = ColorTranslator.FromHtml("#CBD5E1") }
        };
    }

    // ── Danger Button ──────────────────────────────────────────
    public static Button CreateDangerButton(string text)
    {
        return new Button
        {
            Text = text,
            FlatStyle = FlatStyle.Flat,
            Height = 38,
            Width = 120,
            BackColor = Danger,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            Cursor = Cursors.Hand,
            FlatAppearance = { BorderSize = 0, MouseOverBackColor = ColorTranslator.FromHtml("#DC2626") }
        };
    }

    // ── Text Input ─────────────────────────────────────────────
    public static TextBox CreateInput(string placeholder = "")
    {
        var tb = new TextBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            Font = TextFont,
            BackColor = Color.White,
            ForeColor = TextDark,
            Height = 34,
        };
        if (!string.IsNullOrEmpty(placeholder))
            tb.PlaceholderText = placeholder;
        return tb;
    }

    // ── ComboBox ───────────────────────────────────────────────
    public static ComboBox CreateComboBox()
    {
        return new ComboBox
        {
            FlatStyle = FlatStyle.Flat,
            Font = TextFont,
            BackColor = Color.White,
            ForeColor = TextDark,
            DropDownStyle = ComboBoxStyle.DropDownList,
        };
    }

    // ── Status Badge Label ─────────────────────────────────────
    public static Label CreateBadge(string text, Color backColor, Color foreColor)
    {
        return new Label
        {
            Text = text,
            AutoSize = false,
            Width = 88,
            Height = 22,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = SmallFont,
            ForeColor = foreColor,
            BackColor = backColor,
        };
    }

    // ── Style DataGridView ─────────────────────────────────────
    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.RowHeadersVisible = false;
        grid.EnableHeadersVisualStyles = false;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Border;

        grid.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8FAFC");
        grid.ColumnHeadersDefaultCellStyle.ForeColor = TextMuted;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
        grid.ColumnHeadersHeight = 44;

        grid.DefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
        grid.DefaultCellStyle.Font = TextFont;
        grid.DefaultCellStyle.ForeColor = TextDark;
        grid.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EFF6FF");
        grid.DefaultCellStyle.SelectionForeColor = Primary;

        grid.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FAFBFC");
        grid.RowTemplate.Height = 44;
    }

    // ── GDI+ Helpers ───────────────────────────────────────────
    public static void FillRoundedRect(Graphics g, Brush brush, Rectangle rect, int radius)
    {
        using var path = GetRoundedPath(rect, radius);
        g.FillPath(brush, path);
    }

    public static void DrawRoundedRect(Graphics g, Pen pen, Rectangle rect, int radius)
    {
        using var path = GetRoundedPath(rect, radius);
        g.DrawPath(pen, path);
    }

    public static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        int d = radius * 2;
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    // ── Bar Chart Painter ──────────────────────────────────────
    /// <summary>Draws a simple vertical bar chart on a Panel's Paint event.</summary>
    public static void DrawBarChart(Graphics g, int width, int height,
        string[] labels, double[] values, Color[] colors)
    {
        if (labels.Length == 0) return;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        const int padLeft = 48, padBottom = 40, padTop = 20, padRight = 16;
        int chartH = height - padBottom - padTop;
        int chartW = width - padLeft - padRight;
        double maxVal = values.Length > 0 ? Math.Max(values.Max(), 1) : 1;

        // Grid lines
        using var gridPen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1);
        using var labelFont = new Font("Segoe UI", 8F);
        using var labelBrush = new SolidBrush(TextMuted);

        for (int i = 0; i <= 4; i++)
        {
            int y = padTop + (int)(chartH * (1.0 - i / 4.0));
            g.DrawLine(gridPen, padLeft, y, padLeft + chartW, y);
            g.DrawString(((int)(maxVal * i / 4)).ToString(), labelFont, labelBrush,
                2, y - 8);
        }

        // Bars
        int barCount = labels.Length;
        int slotW = chartW / barCount;
        int barW = Math.Max(20, slotW - 20);

        for (int i = 0; i < barCount; i++)
        {
            int barH = (int)(chartH * (values[i] / maxVal));
            int x = padLeft + i * slotW + (slotW - barW) / 2;
            int y = padTop + chartH - barH;

            Color c = colors[i % colors.Length];
            using var barBrush = new LinearGradientBrush(
                new Rectangle(x, y, barW, Math.Max(barH, 1)),
                c, Color.FromArgb(180, c), LinearGradientMode.Vertical);
            var barRect = new Rectangle(x, y, barW, barH);
            FillRoundedRect(g, barBrush, barRect, 4);

            // Value on top
            if (barH > 16)
            {
                string valStr = values[i] >= 1000
                    ? $"{values[i] / 1000.0:F1}k"
                    : ((int)values[i]).ToString();
                using var valFont = new Font("Segoe UI", 8F, FontStyle.Bold);
                using var valBrush = new SolidBrush(TextDark);
                var sz = g.MeasureString(valStr, valFont);
                g.DrawString(valStr, valFont, valBrush, x + (barW - sz.Width) / 2, y - sz.Height - 2);
            }

            // Label below
            using var lbFont = new Font("Segoe UI", 8.5F);
            var lbSz = g.MeasureString(labels[i], lbFont);
            g.DrawString(labels[i], lbFont, labelBrush,
                x + (barW - lbSz.Width) / 2,
                padTop + chartH + 6);
        }
    }
}
