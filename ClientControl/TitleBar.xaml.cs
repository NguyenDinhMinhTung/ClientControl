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

namespace ClientControl
{
    /// <summary>
    /// TitleBar.xaml の相互作用ロジック
    /// </summary>
    public partial class TitleBar : UserControl
    {
        Window parentWindow;
        public TitleBar()
        {
            InitializeComponent();
        }

        private Window getParentWindow()
        {
            FrameworkElement frameworkElement = this as FrameworkElement;

            while (frameworkElement.Parent != null)
            {
                frameworkElement = frameworkElement.Parent as FrameworkElement;
            }

            return frameworkElement as Window;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (parentWindow != null)
            {
                parentWindow.Close();
            }
        }

        private void TitleBarUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            parentWindow = getParentWindow();
            if (parentWindow == null) return;

            parentWindow.MouseMove += new MouseEventHandler((o, ex) =>
            {
                if (ex.LeftButton == MouseButtonState.Pressed)
                {
                    parentWindow.DragMove();
                }
            });
        }
    }
}
