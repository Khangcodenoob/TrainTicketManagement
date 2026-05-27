using System.Drawing.Drawing2D;
using TrainTicketWinForms.Models;

namespace TrainTicketWinForms.Dialogs;

public sealed class CustomerDialog : Form
{
    private readonly TextBox _txtFullName;
    private readonly TextBox _txtPhone;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtIdentity;
    private readonly TextBox _txtAddress;

    public CustomerItem? Result { get; private set; }

    public CustomerDialog(CustomerItem? customer = null)
    {
        Text = customer is null ? "Thêm khách hàng" : "Sửa khách hàng";
        Size = new Size(560, 420);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = Color.White;

        var lblName = FieldLabel("Họ tên");
        _txtFullName = UiTheme.CreateInput("Nhập họ tên...");

        var lblPhone = FieldLabel("Số điện thoại");
        _txtPhone = UiTheme.CreateInput("Nhập số điện thoại...");

        var lblEmail = FieldLabel("Email");
        _txtEmail = UiTheme.CreateInput("Nhập email...");

        var lblIdentity = FieldLabel("CMND/CCCD");
        _txtIdentity = UiTheme.CreateInput("Nhập CMND/CCCD...");

        var lblAddress = FieldLabel("Địa chỉ");
        _txtAddress = UiTheme.CreateInput("Nhập địa chỉ...");

        var btnOk = UiTheme.CreatePrimaryButton("Lưu");
        btnOk.Location = new Point(370, 320);
        btnOk.Click += (_, _) => OnSubmit();

        var btnCancel = UiTheme.CreateSecondaryButton("Hủy");
        btnCancel.Location = new Point(470, 320);
        btnCancel.Click += (_, _) => Close();

        lblName.SetBounds(24, 24, 120, 20);
        _txtFullName.SetBounds(24, 46, 500, 34);
        lblPhone.SetBounds(24, 96, 120, 20);
        _txtPhone.SetBounds(24, 118, 240, 34);
        lblEmail.SetBounds(290, 96, 120, 20);
        _txtEmail.SetBounds(290, 118, 234, 34);
        lblIdentity.SetBounds(24, 168, 120, 20);
        _txtIdentity.SetBounds(24, 190, 240, 34);
        lblAddress.SetBounds(290, 168, 120, 20);
        _txtAddress.SetBounds(290, 190, 234, 34);

        Controls.AddRange(new Control[]
        {
            lblName, _txtFullName,
            lblPhone, _txtPhone,
            lblEmail, _txtEmail,
            lblIdentity, _txtIdentity,
            lblAddress, _txtAddress,
            btnOk, btnCancel
        });

        if (customer is not null)
        {
            _txtFullName.Text = customer.FullName;
            _txtPhone.Text = customer.PhoneNumber;
            _txtEmail.Text = customer.Email;
            _txtIdentity.Text = customer.IdentityNumber;
            _txtAddress.Text = customer.Address ?? string.Empty;
            Result = customer;
        }
    }

    private void OnSubmit()
    {
        if (string.IsNullOrWhiteSpace(_txtFullName.Text))
        {
            MessageBox.Show("Tên khách hàng không được để trống.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(_txtPhone.Text))
        {
            MessageBox.Show("Số điện thoại không được để trống.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(_txtEmail.Text))
        {
            MessageBox.Show("Email không được để trống.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(_txtIdentity.Text))
        {
            MessageBox.Show("CMND/CCCD không được để trống.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new CustomerItem
        {
            FullName = _txtFullName.Text.Trim(),
            PhoneNumber = _txtPhone.Text.Trim(),
            Email = _txtEmail.Text.Trim(),
            IdentityNumber = _txtIdentity.Text.Trim(),
            Address = string.IsNullOrWhiteSpace(_txtAddress.Text) ? null : _txtAddress.Text.Trim()
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
