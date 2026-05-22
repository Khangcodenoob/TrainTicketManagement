using System.Drawing.Drawing2D;

namespace TrainTicketWinForms;

/// <summary>Modern confirmation dialog replacing MessageBox.</summary>
public class ConfirmDialog : Form
{
    /// <summary>Shows the dialog and returns true if the user confirmed.</summary>
    public static bool Show(IWin32Window owner, string message,
        string title = "Xác nhận", string icon = "⚠️")
    {
        using var dlg = new ConfirmDialog(message, title, icon);
        return dlg.ShowDialog(owner) == DialogResult.Yes;
    }

    private ConfirmDialog(string message, string title, string icon)
    {
        Text = title;
        Size = new Size(440, 210);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.None;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        // Rounded form
        SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);

        var iconLbl = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI Emoji", 28F),
            AutoSize = true,
            Location = new Point(28, 28)
        };

        var titleLbl = new Label
        {
            Text = title,
            Font = UiTheme.HeadingFont,
            ForeColor = UiTheme.TextDark,
            AutoSize = true,
            Location = new Point(86, 28)
        };

        var msgLbl = new Label
        {
            Text = message,
            Font = UiTheme.TextFont,
            ForeColor = UiTheme.TextMuted,
            Size = new Size(360, 50),
            Location = new Point(86, 58),
            TextAlign = ContentAlignment.TopLeft
        };

        var sep = new Panel
        {
            BackColor = UiTheme.Border,
            Size = new Size(440, 1),
            Location = new Point(0, 136)
        };

        var btnNo = UiTheme.CreateSecondaryButton("✕  Hủy bỏ");
        btnNo.Location = new Point(200, 152);
        btnNo.Width = 100;
        btnNo.DialogResult = DialogResult.No;

        var btnYes = UiTheme.CreatePrimaryButton("✓  Đồng ý");
        btnYes.Location = new Point(312, 152);
        btnYes.Width = 106;
        btnYes.DialogResult = DialogResult.Yes;

        Controls.AddRange(new Control[] { iconLbl, titleLbl, msgLbl, sep, btnNo, btnYes });
        AcceptButton = btnYes;
        CancelButton = btnNo;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(UiTheme.Border, 1);
        UiTheme.DrawRoundedRect(g, pen, new Rectangle(0, 0, Width - 1, Height - 1), 12);
    }
}
