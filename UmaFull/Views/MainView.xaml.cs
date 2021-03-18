using System;
using System.Windows;
using Forms = System.Windows.Forms;

namespace UmaFull
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// タスクバー上の通知アイコン
        /// </summary>
        private static Forms.NotifyIcon notifyIcon;

        /// <summary>
        /// メインウィンドウのステート変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            // 最小化時にタスクバーから消し、アイコン化する
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;

                if (notifyIcon == null)
                {
                    var icon = App.GetResourceStream(new Uri("Images/icon.ico", UriKind.Relative)).Stream;
                    var menu = new Forms.ContextMenuStrip();
                    menu.Items.Add(new Forms.ToolStripMenuItem("表示サイズ自動切り替え", null, (s, ev) =>
                    {
                        if (s is Forms.ToolStripMenuItem item)
                        {
                            item.Checked = !item.Checked;
                            var vm = DataContext as MainViewModel;
                            if (vm != null)
                            {
                                vm.IsCheckedAutoSwitch = item.Checked;
                            }
                        }
                    }));
                    menu.Items.Add(new Forms.ToolStripSeparator());
                    menu.Items.Add(new Forms.ToolStripMenuItem("終了", null, (s, ev) =>
                    {
                        App.Current.Shutdown();
                    }));

                    notifyIcon = new Forms.NotifyIcon
                    {
                        Visible = true,
                        Icon = new System.Drawing.Icon(icon),
                        Text = "UmaFull",
                        ContextMenuStrip = menu
                    };
                    notifyIcon.MouseClick += (s, ev) =>
                    {
                        if (ev.Button == Forms.MouseButtons.Left)
                        {
                            WindowState = WindowState.Normal;
                            Activate();
                        }
                    };
                }
            }
            else if (WindowState == WindowState.Normal)
            {
                // サイズ復元時にアイコン化を解除してタスクバーの表示を再開します
                if (notifyIcon != null)
                {
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
                ShowInTaskbar = true;
            }
        }
    }
}
