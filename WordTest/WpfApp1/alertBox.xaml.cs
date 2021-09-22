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
using System.Windows.Threading;

namespace WordTest
{
    /// <summary>
    /// alertBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class alertBox : UserControl
    {
        DispatcherTimer timer = new DispatcherTimer();
        int life = 80;
        public bool hide = false;

        public alertBox(string text)
        {
            InitializeComponent();
            Set(text);
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Tick += new EventHandler(Tick);
            timer.Start();
        }

        public alertBox(string text, bool visible)
        {
            InitializeComponent();
            if (visible == true)
            {
                textBox.Visibility = Visibility.Visible;
            }
            
            Set(text);
        }

        void Tick(object sender, EventArgs e)
        {
            if (life >= 0)
            {
                life--;
            }
            else
            {
                Box.Opacity -= 0.03;
            }

            if (Box.Opacity <= 0)
            {
                hide = true;
                timer.Stop();
            }
        }

        public void Set(string text)
        {
            this.text.Text = text;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            pinRect.Fill = new SolidColorBrush(Color.FromArgb(50, 255, 255, 118));
        }

        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            timer.Start();
            pinRect.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            delRect.Fill = new SolidColorBrush(Color.FromArgb(50, 251, 117, 117));
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            delRect.Fill = new SolidColorBrush(Color.FromArgb(0, 251, 117, 117));
        }

        private void delRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Box.Opacity = 0;
        }
    }
}
