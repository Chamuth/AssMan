using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssMan.Controls
{
    /// <summary>
    /// Interaction logic for AssetItem.xaml
    /// </summary>
    public partial class AssetItem : UserControl
    {
        public AssetItem()
        {
            InitializeComponent();
        }

        #region PROPS
        private string _assetTitle;

        public string AssetTitle
        {
            get { return _assetTitle; }
            set { _assetTitle = value; AssetTitle_Label.Text = value; }
        }

        private string _assetImageURL;

        public string AssetImageURL
        {
            get { return _assetImageURL; }
            set { _assetImageURL = value;
                try { AssetImage_Image.Source = new BitmapImage(new Uri(value)); } catch (Exception) { } }
        }

        public string[] _assetCategories = new string[] { };

        public string[] AssetCategories
        {
            get 
            {
                return _assetCategories; 
            }
            set
            {
                _assetCategories = value;

                // Re render categories
                categories_container.Children.Clear();
                foreach(var category in value)
                {
                    var item = new CategoryCapsule();
                    item.Category = category;
                    categories_container.Children.Add(item);
                }
            }
        }
        #endregion
    }
}
