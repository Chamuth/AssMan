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
    /// Interaction logic for CategoryCapsule.xaml
    /// </summary>
    public partial class CategoryCapsule : UserControl
    {
        public CategoryCapsule()
        {
            InitializeComponent();
        }

        private string _category = "Weapons";

        public string Category
        {
            get { return _category; }
            set { _category = value; category_name.Content = value; }
        }

    }
}
