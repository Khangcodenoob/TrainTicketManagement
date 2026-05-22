using System.Drawing.Drawing2D;

namespace TrainTicketWinForms;

/// <summary>Custom Panel with rounded corners and a soft drop shadow.</summary>
public class ShadowPanel : Panel
{
    private const int Shadow = 5;
    private const int Radius = 12;

    public ShadowPanel()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.White;
        Padding = new Padding(18);
        Margin = new Padding(Shadow + 4);
    }

    protected override void OnPaintBackground(PaintEventArgs e) { /* suppress flicker */ }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // Draw layered shadow
        for (int i = Shadow; i >= 1; i--)
        {
            int alpha = (int)(22.0 * (Shadow - i + 1) / Shadow);
            using var sb = new SolidBrush(Color.FromArgb(alpha, 30, 30, 60));
            var r = new Rectangle(i, i + 1, Width - i * 2 - 1, Height - i * 2 - 1);
            using var sp = UiTheme.GetRoundedPath(r, Radius);
            g.FillPath(sb, sp);
        }

        // Draw card background
        var cardRect = new Rectangle(0, 0, Width - Shadow - 1, Height - Shadow - 1);
        using var cardPath = UiTheme.GetRoundedPath(cardRect, Radius);
        using var cardBrush = new SolidBrush(BackColor);
        g.FillPath(cardBrush, cardPath);

        // Draw border
        using var pen = new Pen(ColorTranslator.FromHtml("#E2E8F0"), 1);
        g.DrawPath(pen, cardPath);

        // Set clip so child controls stay inside rounded rect
        Region = new Region(cardPath);
    }
}
