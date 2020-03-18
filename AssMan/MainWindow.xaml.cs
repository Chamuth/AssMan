using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private IKeyboardMouseEvents m_GlobalHook;

        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine("KeyPress: \t{0}", e.KeyChar);

            if (e.KeyChar == '~')
            {
                ToggleWindow();
            }
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            Console.WriteLine("MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp);

            // uncommenting the following line will suppress the middle mouse button click
            // if (e.Buttons == MouseButtons.Middle) { e.Handled = true; }
        }

        public void Unsubscribe()
        {
            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress -= GlobalHookKeyPress;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }

        bool shown = false;

        DoubleAnimation dopacity, topAnimation, dInvOpacity, topRevAnimation;

        private void ToggleWindow()
        {
            if (!shown)
            {
                Show();
                // Fade in the nigga
                BeginAnimation(OpacityProperty, dopacity);
                // pan in animation
                BeginAnimation(TopProperty, topAnimation);
                // Center the window
                Left = (SystemParameters.PrimaryScreenWidth / 2) - (Width / 2);
            }
            else
            {
                // Fade in the nigga
                BeginAnimation(OpacityProperty, dInvOpacity);
                // pan in animation
                BeginAnimation(TopProperty, topRevAnimation);
                // Center the window
                Left = (SystemParameters.PrimaryScreenWidth / 2) - (Width / 2);
            }

            // negate this shit
            shown = !shown;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Subscribe();

            //Hide the nigga
            Opacity = 0;
            Hide();

            var quint = new QuinticEase();

            dopacity = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(350));
            dopacity.EasingFunction = quint;
            topAnimation = new DoubleAnimation(-Height, 0, TimeSpan.FromMilliseconds(450));
            topAnimation.EasingFunction = quint;
            dInvOpacity = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(350));
            dInvOpacity.EasingFunction = quint;
            topRevAnimation = new DoubleAnimation(0, -Height, TimeSpan.FromMilliseconds(450));
            topRevAnimation.EasingFunction = quint;

            dInvOpacity.Completed += DInvOpacity_Completed;
        }

        private void DInvOpacity_Completed(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
