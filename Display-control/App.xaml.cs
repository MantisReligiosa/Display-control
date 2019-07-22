using DataExchange;
using DataExchange.DTO;
using DataExchange.Requests;
using DataExchange.Responces;
using Display_control.Blocks;
using Display_control.Properties;
using Nancy.Hosting.Self;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Web;
using Media = System.Windows.Media;

namespace Display_control
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window _window;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CreateNotifyIcon();

            try
            {
                RegisterHandlers();
                StartNancy();

                _window = new Window()
                {
                    Visibility = Visibility.Hidden,
                    ShowInTaskbar = false,
                    WindowStyle = WindowStyle.None,
                    Cursor = Cursors.None,
                    ResizeMode = ResizeMode.NoResize,
#if !DEBUG
                    Topmost = true
#endif
                };
                _notifyIcon.ShowBalloonTip(1, "Display-control", "Сервер запущен и готов к работе", System.Windows.Forms.ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                _notifyIcon.ShowBalloonTip(1, "Display-control", ex.Message, System.Windows.Forms.ToolTipIcon.Error);
            }
        }

        private static void StartNancy()
        {
            var hostConfiguration = new HostConfiguration
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true }
            };
            var port = Settings.Default.ServerPort;
            var uri = new Uri($"http://localhost:{port}");
            var bootstrapper = new Bootstrapper();
            var nancyHost = new NancyHost(bootstrapper, hostConfiguration, uri);
            nancyHost.Start();
        }

        private void RegisterHandlers()
        {
            var broker = Broker.GetBroker();
            broker.RegisterHandler<GetScreenSizeRequest>(request =>
            {
                var screens = System.Windows.Forms.Screen.AllScreens;
                var responce = new GetScreenSizeResponce
                {
                    Height = (int)SystemParameters.VirtualScreenHeight,
                    Width = (int)SystemParameters.VirtualScreenWidth,
                    Screens = screens.Select(s => new ScreenSize
                    {
                        Left = s.Bounds.X,
                        Top = s.Bounds.Y,
                        Width = s.Bounds.Width,
                        Height = s.Bounds.Height
                    })
                };
                return responce;
            });
            broker.RegisterHandler<GetFontsRequest>(request =>
            {
                return new GetFontsResponce
                {
                    Fonts = System.Drawing.FontFamily.Families.Select(f => f.Name)
                };
            });
            broker.RegisterHandler<StartShowRequest>(request =>
            {
                var requestData = request as StartShowRequest;
                Dispatcher.Invoke(() =>
                {
                    _window.Height = requestData.Screens.Displays.Max(d => d.Top + d.Height) - requestData.Screens.Displays.Min(d => d.Top);
                    _window.Width = requestData.Screens.Displays.Max(d => d.Left + d.Width) - requestData.Screens.Displays.Min(d => d.Left);
                    _window.Left = requestData.Screens.Displays.Min(d => d.Left);
                    _window.Top = requestData.Screens.Displays.Min(d => d.Top);
                    var bc = new Media.BrushConverter();
                    var border = new Border();
                    if (ColorConverter.TryToParseRGB(requestData.Background, out string colorHex))
                    {
                        border.Background = (Media.Brush)bc.ConvertFrom(colorHex);
                    }
                    var canvas = new Canvas();


                    var blockBuilder = new BlockBuilder();
                    foreach (var block in requestData.Blocks)
                    {
                        var element = blockBuilder.BuildElement(block);
                        if (element != null)
                        {
                            canvas.Children.Add(element);
                        }
                    }

                    border.Child = canvas;
                    _window.Content = border;
                    _window.Show();
                });
                return null;
            });
            broker.RegisterHandler<StopShowRequest>(request =>
            {
                Dispatcher.Invoke(() =>
                {
                    _window.Visibility = Visibility.Hidden;
                });
                return null;
            });
            broker.RegisterHandler<GetVersionRequest>(request =>
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                return new GetVersionResponce
                {
                    Major = version.Major,
                    Minor = version.Minor,
                    Build = version.Build
                };
            });
        }

        private void CreateNotifyIcon()
        {
            _notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = Display_control.Properties.Resources.Logo,
                Visible = true,
                ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip()
            };
            _notifyIcon.ContextMenuStrip.Items.Add("Выход").Click += (s, args) => Current.Shutdown();
        }
    }
}
