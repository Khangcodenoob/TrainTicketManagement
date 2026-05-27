using System.Drawing.Drawing2D;
using TrainTicketWinForms.Models;

namespace TrainTicketWinForms.Dialogs;

public sealed class RouteDialog : Form
{
    private readonly TextBox _txtDeparture;
    private readonly TextBox _txtArrival;
    private readonly TextBox _txtDistance;
    private readonly ComboBox _cbxStatus;

    public RouteItem? Result { get; private set; }

    public RouteDialog(RouteItem? route = null)
    {
        Text = route is null ? "Thêm tuyến tàu" : "Sửa tuyến tàu";
        Size = new Size(520, 320);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var lblDeparture = FieldLabel("Ga đi");
        _txtDeparture = UiTheme.CreateInput("Nhập ga đi...");

        var lblArrival = FieldLabel("Ga đến");
        _txtArrival = UiTheme.CreateInput("Nhập ga đến...");

        var lblDistance = FieldLabel("Khoảng cách (km)");
        _txtDistance = UiTheme.CreateInput("Nhập khoảng cách...");

        var lblStatus = FieldLabel("Trạng thái");
        _cbxStatus = new ComboBox
        {
            FlatStyle = FlatStyle.Flat,
            Font = UiTheme.TextFont,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Color.White,
            Width = 220
        };
        _cbxStatus.Items.AddRange(new object[] { "Active", "Inactive", "Cancelled" });
        _cbxStatus.SelectedIndex = 0;

        var btnOk = UiTheme.CreatePrimaryButton("Lưu");
        btnOk.Location = new Point(310, 220);
        btnOk.Click += (_, _) => OnSubmit();

        var btnCancel = UiTheme.CreateSecondaryButton("Hủy");
        btnCancel.Location = new Point(420, 220);
        btnCancel.Click += (_, _) => Close();

        lblDeparture.SetBounds(24, 24, 120, 20);
        _txtDeparture.SetBounds(24, 46, 440, 34);
        lblArrival.SetBounds(24, 96, 120, 20);
        _txtArrival.SetBounds(24, 118, 440, 34);
        lblDistance.SetBounds(24, 168, 120, 20);
        _txtDistance.SetBounds(24, 190, 220, 34);
        lblStatus.SetBounds(260, 168, 120, 20);
        _cbxStatus.SetBounds(260, 190, 204, 34);

        Controls.AddRange(new Control[]
        {
            lblDeparture, _txtDeparture,
            lblArrival, _txtArrival,
            lblDistance, _txtDistance,
            lblStatus, _cbxStatus,
            btnOk, btnCancel
        });

        if (route is not null)
        {
            _txtDeparture.Text = route.DepartureStation;
            _txtArrival.Text = route.ArrivalStation;
            _txtDistance.Text = route.DistanceKm.ToString("0.##");
            _cbxStatus.SelectedItem = route.Status;
            Result = route;
        }
    }

    private void OnSubmit()
    {
        if (string.IsNullOrWhiteSpace(_txtDeparture.Text))
        {
            MessageBox.Show("Ga đi không được để trống.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(_txtArrival.Text))
        {
            MessageBox.Show("Ga đến không được để trống.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!decimal.TryParse(_txtDistance.Text.Trim(), out var distance) || distance <= 0)
        {
            MessageBox.Show("Khoảng cách phải là số dương.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new RouteItem
        {
            DepartureStation = _txtDeparture.Text.Trim(),
            ArrivalStation = _txtArrival.Text.Trim(),
            DistanceKm = distance,
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
