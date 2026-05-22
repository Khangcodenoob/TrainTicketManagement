namespace TrainTicketWinForms;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        while (true)
        {
            using var loginForm = new LoginForm();
            if (loginForm.ShowDialog() != DialogResult.OK || loginForm.LoginData is null)
            {
                break;
            }

            using var mainForm = new Form1(
                loginForm.LoginData.Role,
                loginForm.LoginData.UserName);
            Application.Run(mainForm);

            if (!mainForm.ShouldReturnToLogin)
            {
                break;
            }
        }
    }    
}