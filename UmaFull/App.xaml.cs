using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using Forms = System.Windows.Forms;

namespace UmaFull
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

#if !DEBUG
            // 管理者権限の確認
            Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            var pri = (WindowsPrincipal)Thread.CurrentPrincipal;

            if (!pri.IsInRole(WindowsBuiltInRole.Administrator))
            {
                var psi = new ProcessStartInfo()
                {
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Assembly.GetEntryAssembly().Location,
                    Verb = "RunAs"
                };

                if (e.Args.Length > 0)
                {
                    psi.Arguments = string.Join(" ", e.Args);
                }

                try
                {
                    Process.Start(psi);
                }
                catch (Exception)
                {
                }
                Shutdown();
            }
#endif

            // Viewを表示
            var v = new MainView();
            v.Show();
        }
    }
}