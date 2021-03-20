using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using Forms = System.Windows.Forms;

namespace UmaFull
{
    public class MainViewModel : BindableBase
    {
        #region Properties
        /// <summary>
        /// 全モニター
        /// </summary>
        public IEnumerable<Forms.Screen> AllMonitors { get; private set; }

        private Forms.Screen _selectedVerticalMonitor;
        /// <summary>
        /// 選択中の縦画面表示時の表示モニター
        /// </summary>
        public Forms.Screen SelectedVerticalMonitor
        {
            get => _selectedVerticalMonitor;
            set => SetProperty(ref _selectedVerticalMonitor, value);
        }

        private Forms.Screen _selectedHorizontalMonitor;
        /// <summary>
        /// 選択中の横画面表示時の表示モニター
        /// </summary>
        public Forms.Screen SelectedHorizontalMonitor
        {
            get => _selectedHorizontalMonitor;
            set => SetProperty(ref _selectedHorizontalMonitor, value);
        }

        /// <summary>
        /// 縦画面表示の表示座標
        /// </summary>
        public Rectangle VerticalRect
        {
            get => new Rectangle(VerticalRectX, VerticalRectY, VerticalRectWidth, VerticalRectHeight);
            set
            {
                VerticalRectX = value.X;
                VerticalRectY = value.Y;
                VerticalRectWidth = value.Width;
                VerticalRectHeight = value.Height;
                OnPropertyChanged();
            }
        }

        private int _verticalRectX;
        /// <summary>
        /// 縦画面表示の表示座標X
        /// </summary>
        public int VerticalRectX
        {
            get => _verticalRectX;
            set
            {
                SetProperty(ref _verticalRectX, value);
                OnPropertyChanged("VerticalRect");
            }
        }

        private int _verticalRectY;
        /// <summary>
        /// 縦画面表示の表示座標Y
        /// </summary>
        public int VerticalRectY
        {
            get => _verticalRectY;
            set
            {
                SetProperty(ref _verticalRectY, value);
                OnPropertyChanged("VerticalRect");
            }
        }

        private int _verticalRectWidth;
        /// <summary>
        /// 縦画面表示の表示座標幅
        /// </summary>
        public int VerticalRectWidth
        {
            get => _verticalRectWidth;
            set
            {
                SetProperty(ref _verticalRectWidth, value);
                OnPropertyChanged("VerticalRect");
            }
        }

        private int _verticalRectHeight;
        /// <summary>
        /// 縦画面表示の表示座標高さ
        /// </summary>
        public int VerticalRectHeight
        {
            get => _verticalRectHeight;
            set
            {
                SetProperty(ref _verticalRectHeight, value);
                OnPropertyChanged("VerticalRect");
            }
        }

        /// <summary>
        /// 横画面表示の表示座標
        /// </summary>
        public Rectangle HorizontalRect
        {
            get => new Rectangle(HorizontalRectX, HorizontalRectY, HorizontalRectWidth, HorizontalRectHeight);
            set
            {
                HorizontalRectX = value.X;
                HorizontalRectY = value.Y;
                HorizontalRectWidth = value.Width;
                HorizontalRectHeight = value.Height;
                OnPropertyChanged();
            }
        }

        private int _horizontalRectX;
        /// <summary>
        /// 横画面表示の表示座標X
        /// </summary>
        public int HorizontalRectX
        {
            get => _horizontalRectX;
            set
            {
                SetProperty(ref _horizontalRectX, value);
                OnPropertyChanged("HorizontalRect");
            }
        }

        private int _horizontalRectY;
        /// <summary>
        /// 横画面表示の表示座標Y
        /// </summary>
        public int HorizontalRectY
        {
            get => _horizontalRectY;
            set
            {
                SetProperty(ref _horizontalRectY, value);
                OnPropertyChanged("HorizontalRect");
            }
        }

        private int _horizontalRectWidth;
        /// <summary>
        /// 横画面表示の表示座標幅
        /// </summary>
        public int HorizontalRectWidth
        {
            get => _horizontalRectWidth;
            set
            {
                SetProperty(ref _horizontalRectWidth, value);
                OnPropertyChanged("HorizontalRect");
            }
        }

        private int _horizontalRectHeight;
        /// <summary>
        /// 横画面表示の表示座標高さ
        /// </summary>
        public int HorizontalRectHeight
        {
            get => _horizontalRectHeight;
            set
            {
                SetProperty(ref _horizontalRectHeight, value);
                OnPropertyChanged("HorizontalRect");
            }
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
        /// 表示サイズ自動切り替えボタンの押下状態
        /// </summary>
        public bool IsCheckedAutoSwitch
        {
            get => _isCheckedAutoSwitch;
            set => SetProperty(ref _isCheckedAutoSwitch, value);
        }

        /// <summary>
        /// メインモデル
        /// </summary>
        public MainModel MainModel { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// 縦画面時に9:16でフルスクリーンになるように座標を登録コマンド
        /// </summary>
        public DelegateCommand VerticalFullScreenCommand { get; private set; }

        /// <summary>
        /// 縦画面時に16:9でフルスクリーンになるように座標を登録コマンド
        /// </summary>
        public DelegateCommand HorizontalFullScreenCommand { get; private set; }

        /// <summary>
        /// 設定の保存コマンド
        /// </summary>
        public DelegateCommand ApplyCommand { get; private set; }

        /// <summary>
        /// 表示サイズ自動切り替えコマンド
        /// </summary>
        public DelegateCommand AutoSwitchCommand { get; private set; }

        /// <summary>
        /// 対象ウィンドウ一覧のリロードコマンド
        /// </summary>
        public DelegateCommand ReloadWindowCommand { get; private set; }
        #endregion

        public MainViewModel()
        {
            MainModel = new MainModel();

            AllMonitors = MainModel.AllScreens;
            TargetWindows = MainModel.TargetWindows;

            VerticalRect = MainModel.VerticalRect;
            HorizontalRect = MainModel.HorizontalRect;
            SelectedVerticalMonitor = MainModel.VerticalMonitor;
            SelectedHorizontalMonitor = MainModel.HorizontalMonitor;
            SelectedTargetWindow = MainModel.SelectedTargetWindow;
            IsCheckedAutoSwitch = MainModel.IsCheckedAutoSwitch;

            // ModelのPropertyChanged実装
            MainModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainModel.AllScreens))
                {
                    AllMonitors = MainModel.AllScreens;
                    SelectedVerticalMonitor = MainModel.VerticalMonitor;
                    SelectedHorizontalMonitor = MainModel.HorizontalMonitor;
                }
                else if (e.PropertyName == nameof(MainModel.VerticalRect))
                {
                    VerticalRect = MainModel.VerticalRect;
                }
                else if (e.PropertyName == nameof(MainModel.HorizontalRect))
                {
                    HorizontalRect = MainModel.HorizontalRect;
                }
                else if (e.PropertyName == nameof(MainModel.VerticalMonitor))
                {
                    SelectedVerticalMonitor = MainModel.VerticalMonitor;
                }
                else if (e.PropertyName == nameof(MainModel.HorizontalMonitor))
                {
                    SelectedHorizontalMonitor = MainModel.HorizontalMonitor;
                }
                else if (e.PropertyName == nameof(MainModel.TargetWindows))
                {
                    TargetWindows = MainModel.TargetWindows;
                }
                else if (e.PropertyName == nameof(MainModel.SelectedTargetWindow))
                {
                    SelectedTargetWindow = MainModel.SelectedTargetWindow;
                }
                else if (e.PropertyName == nameof(MainModel.IsCheckedAutoSwitch))
                {
                    IsCheckedAutoSwitch = MainModel.IsCheckedAutoSwitch;
                }
            };

            // ViewModelのPropertyChanged実装
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(VerticalRect))
                {
                    IsCheckedAutoSwitch = false;
                    MainModel.VerticalRect = VerticalRect;
                }
                else if (e.PropertyName == nameof(HorizontalRect))
                {
                    IsCheckedAutoSwitch = false;
                    MainModel.HorizontalRect = HorizontalRect;
                }
                else if (e.PropertyName == nameof(SelectedVerticalMonitor))
                {
                    MainModel.VerticalMonitor = SelectedVerticalMonitor;
                }
                else if (e.PropertyName == nameof(SelectedHorizontalMonitor))
                {
                    MainModel.HorizontalMonitor = SelectedHorizontalMonitor;
                }
                else if (e.PropertyName == nameof(SelectedTargetWindow))
                {
                    MainModel.SelectedTargetWindow = SelectedTargetWindow;
                }
                else if (e.PropertyName == nameof(IsCheckedAutoSwitch))
                {
                    MainModel.IsCheckedAutoSwitch = IsCheckedAutoSwitch;
                }
            };

            // 各コマンド実装
            VerticalFullScreenCommand = new DelegateCommand(() =>
            {
                // 9:16でフルスクリーンになるように座標を登録
                var screen = MainModel.AllScreens?.Where(x => SelectedVerticalMonitor?.DeviceName == x.DeviceName).FirstOrDefault();
                if (screen != null)
                {
                    var rect = screen.Bounds;
                    var width = (rect.Height * 9) / 16;
                    rect.X += (rect.Width - width) / 2;
                    rect.Width = width;
                    VerticalRect = rect;
                }
            }, () =>
            {
                return !string.IsNullOrEmpty(SelectedVerticalMonitor?.DeviceName);
            });

            HorizontalFullScreenCommand = new DelegateCommand(() =>
            {
                // 16:9でフルスクリーンになるように座標を登録
                var screen = MainModel.AllScreens?.Where(x => SelectedHorizontalMonitor?.DeviceName == x.DeviceName).FirstOrDefault();
                if (screen != null)
                {
                    var rect = screen.Bounds;
                    var height = (rect.Width * 9) / 16;
                    rect.Y += (rect.Height - height) / 2;
                    rect.Height = height;
                    HorizontalRect = rect;
                }
            }, () =>
            {
                return !string.IsNullOrEmpty(SelectedHorizontalMonitor?.DeviceName);
            });

            ApplyCommand = new DelegateCommand(() =>
            {
                MainModel.SaveSettings();
                MessageBox.Show("設定を保存しました");
            });

            AutoSwitchCommand = new DelegateCommand(() =>
            {
                if (IsCheckedAutoSwitch)
                {
                    // 自動切り替え有効化
                }
                else
                {
                    // 自動切り替え無効化

                }
            });

            ReloadWindowCommand = new DelegateCommand(() =>
            {
                var wins = new ObservableCollection<WindowNames>();
                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.MainWindowTitle?.Length > 0)
                    {
                        wins.Add(new WindowNames(proc));
                    }
                }
                var target = wins.Where(x => x.ProcessName == SelectedTargetWindow?.ProcessName && x.WindowName == SelectedTargetWindow?.WindowName).FirstOrDefault();
                TargetWindows = wins;
                SelectedTargetWindow = target;
            });
        }
    }
}