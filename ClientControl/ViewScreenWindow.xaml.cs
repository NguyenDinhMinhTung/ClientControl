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

        private Image[,] imagelist;
        public int UserID { get { return userID; } }
        public ViewScreenWindow(int userID)
        {
            InitializeComponent();

            this.userID = userID;

            imagelist = new Image[15, 15];

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    imagelist[i, j] = new Image();
                    Canvas.SetTop(imagelist[i, j], 200 * j);
                    Canvas.SetLeft(imagelist[i, j], 200 * i);
                    imageCanvas.Children.Add((imagelist[i, j]));
                }
            }
        }

        public void SetCloseAction(Action closeAction)
        {
            this.closeAction = closeAction;
        }

        public void SetImage(BitmapSource bitmapSource, int x, int y)
        {
            imageCanvas.Dispatcher.Invoke(() => {
                imagelist[x, y].Source = bitmapSource;
            });
            
        }
    }
}
