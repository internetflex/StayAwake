using System;
using System.Windows.Forms;

namespace SystemTray
{
    public class MyApplicationContext : ApplicationContext
    {
        private IntrayConfig _configWindow;
        private NotifyIcon _notifyIcon;
        private Timer _timer;
        private bool _balloonTipVisible = false;

        public MyApplicationContext()
        {
            MenuItem configMenuItem = new MenuItem("Configuration", ShowConfig);
            MenuItem exitMenuItem = new MenuItem("Exit", Exit);

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Resource.InTray;
            _notifyIcon.ContextMenu = new ContextMenu(new[] { configMenuItem, exitMenuItem });
            _notifyIcon.Visible = true;
            _notifyIcon.BalloonTipTitle = "Stay Awake Tool";
            _notifyIcon.BalloonTipText = "Active";
            _notifyIcon.ShowBalloonTip(100);
            _notifyIcon.MouseMove += _notifyIcon_MouseMove;
            _notifyIcon.BalloonTipClosed += _notifyIcon_BalloonTipClosed;

            _configWindow = new IntrayConfig();

            SetupMouseMovementTimer();
            SetUpExecutionState();
            _timer.Start();
        }

        private void _notifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            _balloonTipVisible = false;
        }

        private void _notifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_balloonTipVisible)
            {
                _balloonTipVisible = true;
                _notifyIcon.ShowBalloonTip(500);
            }
        }

        private void SetUpExecutionState()
        {
            Win32.SetThreadExecutionState(EXECUTION_STATE.ES_AWAYMODE_REQUIRED 
                                          | EXECUTION_STATE.ES_SYSTEM_REQUIRED 
                                          | EXECUTION_STATE.ES_AWAYMODE_REQUIRED 
                                          | EXECUTION_STATE.ES_CONTINUOUS);
        }

        private void ResetExecutionState()
        {
            Win32.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        private void SetupMouseMovementTimer()
        {
            int flip = 1;

            _timer = new Timer();
            _timer.Interval = (int)(TimeSpan.TicksPerSecond * _configWindow.WaitInterval / TimeSpan.TicksPerMillisecond);
            _timer.Tick += (sender, args) =>
                {
                    Win32.MoveMouseRelative(flip, flip);
                    flip *= -1;
                };
        }

        private void ShowConfig(object sender, EventArgs e)
        {
            if (_configWindow.Visible)
            {
                _configWindow.Activate();
            }
            else
            {
                _configWindow.ShowDialog();
            }

            _balloonTipVisible = false;
            _timer.Stop();
            SetupMouseMovementTimer();

            if (_configWindow.StayAwakeActive)
            {
                _notifyIcon.BalloonTipText = "Active";
                _timer.Start();
            }
            else
                _notifyIcon.BalloonTipText = "Inactive";
        }

        private void Exit(object sender, EventArgs e)
        {
            _timer.Stop();
            _notifyIcon.Visible = false;
            ResetExecutionState();
            Application.Exit();            
        }
    }
}
