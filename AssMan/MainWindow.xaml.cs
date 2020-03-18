using AssMan.Schema;
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

        #region Key Subscription and Activation

        private IKeyboardMouseEvents m_GlobalHook;

        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            m_GlobalHook = Hook.GlobalEvents();

            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;
        }

        string search_term = "";

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            if (shown)
            {
                int code = int.Parse("232B", System.Globalization.NumberStyles.HexNumber);
                string unicodeString = char.ConvertFromUtf32(code);

                if (Char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == ' ')
                {
                    // search for assets
                    search_term += e.KeyChar.ToString();
                    search_panel.BeginAnimation(OpacityProperty, dopacity);
                    search_term_label.Content = "\"" + search_term + "\"";  

                    RenderAssets(search_term);
                } else if (e.KeyChar == '')
                {
                    search_term = search_term.Substring(0, search_term.Length - 1);
                    search_panel.BeginAnimation(OpacityProperty, dopacity);
                    search_term_label.Content = "\"" + search_term + "\"";

                    RenderAssets(search_term);
                }
            }

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

#endregion

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

            dopacity = new DoubleAnimation(1, TimeSpan.FromMilliseconds(350));
            dopacity.EasingFunction = quint;
            topAnimation = new DoubleAnimation(-Height, 0, TimeSpan.FromMilliseconds(450));
            topAnimation.EasingFunction = quint;
            dInvOpacity = new DoubleAnimation(0, TimeSpan.FromMilliseconds(350));
            dInvOpacity.EasingFunction = quint;
            topRevAnimation = new DoubleAnimation(0, -Height, TimeSpan.FromMilliseconds(450));
            topRevAnimation.EasingFunction = quint;

            dInvOpacity.Completed += DInvOpacity_Completed;

            ResolveJSONData();

            // show the assets
            RenderAssets();
        }

        private void LoadSettings()
        {
            AssManJSON.Instance = Newtonsoft.Json.JsonConvert.DeserializeObject<Schema.AssManJSON>(System.IO.File.ReadAllText(Properties.Settings.Default.AssetManJsonLocation));
        }

        private void RenderAssets(string args = "")
        {
            LoadSettings();

            if (Schema.AssManJSON.Instance != null)
            {
                // Remove previous assets
                assets_container.Children.Clear();

                foreach(var asset in Schema.AssManJSON.Instance.Assets)
                {
                    // Search the fucking assets and then render those selected
                    if (CheckAssetForArgs(asset, args))
                    {
                        var asset_item = new Controls.AssetItem();
                        asset_item.AssetTitle = asset.Title;
                        asset_item.AssetCategories = asset.Categories.ToArray();
                        asset_item.AssetImageURL = @"C:\Users\Chamuth\Desktop\sample.png";

                        assets_container.Children.Add(asset_item);
                    }
                }
            }
        }

        private bool CheckAssetForArgs(AssetBundle asset, string args)
        {
            if (args == "")
                return true;

            var titleCheck = asset.Title.Contains(args);
            var descriptionCheck = asset.Description.Contains(args);

            var catCheck = false;
            foreach (var cat in asset.Categories)
            {
                catCheck = cat.Contains(args);
                if (catCheck) break;
            }

            return (titleCheck || descriptionCheck || catCheck);
        }

        private void ResolveJSONData()
        {
            // Resolve asset man location
            if (string.IsNullOrEmpty(Properties.Settings.Default.AssetManJsonLocation))
            {
                var open = new OpenFileDialog();
                open.Title = "Browse AssetMan.json";
                open.Filter = "AssetMan.json|AssetMan.json";
                
                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    // Parse and verify
                    try
                    {
                        var content = Newtonsoft.Json.JsonConvert.DeserializeObject<Schema.AssManJSON>(System.IO.File.ReadAllText(open.FileName));

                        Properties.Settings.Default.AssetManJsonLocation = open.FileName;
                        Properties.Settings.Default.Save();
                    }
                    catch (Exception)
                    {
                        Close();
                    }
                }
                else
                {
                    Close();
                }
            }
            else
            {
                LoadSettings();
            }
        }

        private void DInvOpacity_Completed(object sender, EventArgs e)
        {
            Hide();
        }

        private void Window_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            FileDropPreview.BeginAnimation(OpacityProperty, dInvOpacity);
        }

        private void WindowContainer_PreviewDrop(object sender, System.Windows.DragEventArgs e)
        {
            FileDropPreview.BeginAnimation(OpacityProperty, dopacity);
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                AddAssets(files);
            }

            FileDropPreview.BeginAnimation(OpacityProperty, dInvOpacity);
        }

        private void AddAssets(string[] files)
        {

        }
    }
}
