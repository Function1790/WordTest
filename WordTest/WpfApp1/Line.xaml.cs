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

namespace WpfApp1
{
    /// <summary>
    /// Line.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Line : UserControl
    {
        public Line()
        {
            InitializeComponent();
        }

        public void Set(Word word)
        {
            Eng.Text = word.Eng;
            Kor.Text = word.Kor;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}
