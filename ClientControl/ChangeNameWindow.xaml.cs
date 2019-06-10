using ClientControl.model;
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
    /// ChangeNameWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ChangeNameWindow : Window
    {
        private String userName;
        private Action<String> OKAction;
        public ChangeNameWindow(String userName, Action<String> OKAction)
        {
            InitializeComponent();

            this.userName = userName;
            this.OKAction = OKAction;

            txtName.Text = userName;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            OKAction(txtName.Text);
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
