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
using System.Windows.Shapes;

namespace ClientControl
{
    /// <summary>
    /// Interaction logic for ViewScreenWindow.xaml
    /// </summary>
    public partial class ViewScreenWindow : Window
    {
        private int userID;

        private Action closeAction;
        public int UserID { get { return userID; } }
        public ViewScreenWindow(int userID)
        {
            InitializeComponent();

            this.userID = userID;
        }

        public void SetCloseAction(Action closeAction)
        {
            this.closeAction = closeAction;
        }

        public void SetImage(BitmapSource bitmapSource)
        {
            imgScreen.Source = bitmapSource;
        }
    }
}
