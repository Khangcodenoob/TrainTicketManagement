namespace TrainTicketWinForms;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        SuspendLayout();
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1280, 760);
        MinimumSize = new Size(1080, 680);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Quản lý bán vé tàu hỏa";
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        ResumeLayout(false);
    }

    #endregion
}
