using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmaFull.Properties;
using Forms = System.Windows.Forms;

namespace UmaFull
{
    #region Public Classes
    /// <summary>
    /// ウィンドウのプロセス名・ウィンドウ名を保持するクラス
    /// </summary>
    public class WindowNames
    {
        /// <summary>
        /// プロセス名
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// ウィンドウ名
        /// </summary>
        public string WindowName { get; set; }

        public WindowNames(string procName, string winName)
        {
            ProcessName = procName;
            WindowName = winName;
        }
        public WindowNames(Process proc)
        {
            ProcessName = proc.ProcessName;
            WindowName = proc.MainWindowTitle;
        }
        public override string ToString()
        {
            return $"{ProcessName} [ {WindowName} ]";
        }
    }
    #endregion

    /// <summary>
    /// メインモデル
    /// </summary>
    public class MainModel : BindableBase
    {
        #region Properties
        private Forms.Screen[] _allScreens;
        /// <summary>
        /// 接続されている全モニター
        /// </summary>
        public Forms.Screen[] AllScreens
        {
            get => _allScreens;
            private set
            {
                VerticalMonitor = value.Where(x => x.DeviceName == VerticalMonitor?.DeviceName).FirstOrDefault();
                HorizontalMonitor = value.Where(x => x.DeviceName == HorizontalMonitor?.DeviceName).FirstOrDefault();
                SetProperty(ref _allScreens, value);
            }
        }

        private Forms.Screen _verticalMonitor;
        /// <summary>
        /// 縦画面表示時の表示モニター
        /// </summary>
        public Forms.Screen VerticalMonitor
        {
            get => _verticalMonitor;
            set => SetProperty(ref _verticalMonitor, value);
        }

        private Forms.Screen _horizontalMonitor;
        /// <summary>
        /// 横画面表示時の表示モニター
        /// </summary>
        public Forms.Screen HorizontalMonitor
        {
            get => _horizontalMonitor;
            set => SetProperty(ref _horizontalMonitor, value);
        }

        private System.Drawing.Rectangle _verticalRect;
        /// <summary>
        /// 縦画面表示時の表示範囲
        /// </summary>
        public System.Drawing.Rectangle VerticalRect
        {
            get => _verticalRect;
            set => SetProperty(ref _verticalRect, value);
        }

        private System.Drawing.Rectangle _horizontalRect;
        /// <summary>
        /// 横画面表示時の表示範囲
        /// </summary>
        public System.Drawing.Rectangle HorizontalRect
        {
            get => _horizontalRect;
            set => SetProperty(ref _horizontalRect, value);
        }

        private ObservableCollection<WindowNames> _targetWindows;
        /// <summary>
        /// 対象のウィンドウ
        /// </summary>
        public ObservableCollection<WindowNames> TargetWindows
        {
            get => _targetWindows;
            set => SetProperty(ref _targetWindows, value);
        }

        private WindowNames _selectedTargetWindow;
        /// <summary>
        /// 選択中の対象のウィンドウ
        /// </summary>
        public WindowNames SelectedTargetWindow
        {
            get => _selectedTargetWindow;
            set => SetProperty(ref _selectedTargetWindow, value);
        }

        private bool _isCheckedAutoSwitch;
        /// <summary>
        /// 表示サイズ自動切り替え有効フラグ
        /// </summary>
        public bool IsCheckedAutoSwitch
        {
            get => _isCheckedAutoSwitch;
            set => SetProperty(ref _isCheckedAutoSwitch, value);
        }

        #endregion

        public MainModel()
        {
            // 全ディスプレイを取得
            AllScreens = Forms.Screen.AllScreens;

            // ターゲットになるウィンドウを列挙する
            TargetWindows = new ObservableCollection<WindowNames>();
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.MainWindowTitle?.Length > 0)
                {
                    TargetWindows.Add(new WindowNames(proc));
                }
            }

            // 設定を読み込み
            LoadSettings();

            // 監視のメインタスク
            Task.Run(async () =>
            {
                try
                {
                    SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;

                    while (true)
                    {
                        if (IsCheckedAutoSwitch)
                        {
                            if (CheckWindow())
                            {
                                // サイズ変更後
                            }
                        }
                        await Task.Delay(500);
                    }
                }
                finally
                {
                    SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
                }
            });
        }

        /// <summary>
        /// ディスプレイ情報が変更された時に取得し直すためのイベントハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            AllScreens = Forms.Screen.AllScreens;
        }

        /// <summary>
        /// ウィンドウのチェック
        /// </summary>
        /// <returns></returns>
        private bool CheckWindow()
        {
            if (SelectedTargetWindow == null)
            {
                return false;
            }
            var proc = Process.GetProcesses().Where(
                x => x.ProcessName == SelectedTargetWindow.ProcessName &&
                x.MainWindowTitle == SelectedTargetWindow.WindowName).FirstOrDefault();
            if (proc == null)
            {
                return false;
            }
            var sb = new StringBuilder(256);
            NativeMethods.GetClassName(proc.MainWindowHandle, sb, sb.Capacity);
            var className = sb.ToString();

            // ウィンドウを探す
            var hWnd = NativeMethods.FindWindow(className, SelectedTargetWindow.WindowName);
            if (hWnd == IntPtr.Zero)
            {
                return false;
            }

            // ウィンドウ位置を取得
            if (!NativeMethods.GetWindowRect(hWnd, out NativeMethods.RECT rect))
            {
                return false;
            }

            // 現在縦画面か横画面化を判定
            var isHorizontal = (rect.Right - rect.Left) > (rect.Bottom - rect.Top);

            // サイズ変更をチェック
            if (!isHorizontal)
            {
                if ((rect.Right - rect.Left) != VerticalRect.Width || (rect.Bottom - rect.Top) != VerticalRect.Height)
                {
                    // タイトルバーを取り除いてサイズ変更
                    NativeMethods.RemoveTitleBar(hWnd);
                    NativeMethods.MoveWindow(hWnd, VerticalRect.Left, VerticalRect.Top, VerticalRect.Width, VerticalRect.Height, true);
                    return true;
                }
            }
            else
            {
                if ((rect.Right - rect.Left) != HorizontalRect.Width || (rect.Bottom - rect.Top) != HorizontalRect.Height)
                {
                    // タイトルバーを取り除いてサイズ変更
                    NativeMethods.RemoveTitleBar(hWnd);
                    NativeMethods.MoveWindow(hWnd, HorizontalRect.Left, HorizontalRect.Top, HorizontalRect.Width, HorizontalRect.Height, true);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 設定の読み込み
        /// </summary>
        internal void LoadSettings()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
            VerticalRect = Settings.Default.VerticalRect;
            HorizontalRect = Settings.Default.HorizontalRect;
            VerticalMonitor = AllScreens.Where(x => x.DeviceName == Settings.Default.VerticalMonitorName).FirstOrDefault();
            HorizontalMonitor = AllScreens.Where(x => x.DeviceName == Settings.Default.HorizontalMonitorName).FirstOrDefault();
            SelectedTargetWindow = TargetWindows.Where(x => x.ProcessName == Settings.Default.TargetWindowProcess && x.WindowName == Settings.Default.TargetWindowName).FirstOrDefault();
            IsCheckedAutoSwitch = Settings.Default.IsCheckedAutoSwitch;
        }

        /// <summary>
        /// 設定の保存
        /// </summary>
        internal void SaveSettings()
        {
            // 設定を保存
            Settings.Default.VerticalRect = VerticalRect;
            Settings.Default.HorizontalRect = HorizontalRect;
            Settings.Default.VerticalMonitorName = VerticalMonitor?.DeviceName;
            Settings.Default.HorizontalMonitorName = HorizontalMonitor?.DeviceName;
            Settings.Default.TargetWindowProcess = SelectedTargetWindow?.ProcessName;
            Settings.Default.TargetWindowName = SelectedTargetWindow?.WindowName;
            Settings.Default.IsCheckedAutoSwitch = IsCheckedAutoSwitch;
            Settings.Default.Save();
        }
    }
}
