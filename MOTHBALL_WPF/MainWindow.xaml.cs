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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MOTHBALL_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CircleEase outCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseOut
        };

        public MainWindow()
        {
            InitializeComponent();

            var titleBounceAnimation = new DoubleAnimation
            {
                From = 340,
                To = 320,
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = outCirc,
            };

            var menuBGScroll = new DoubleAnimation
            {
                From = -0,
                To = -1250,
                Duration = TimeSpan.FromSeconds(20),
                RepeatBehavior = RepeatBehavior.Forever
            };

            imgTitle.BeginAnimation(HeightProperty, titleBounceAnimation);
            imgMenuBG.BeginAnimation(Canvas.LeftProperty, menuBGScroll);
        }

        private void RecStartAnimBounds_MouseEnter(object sender, MouseEventArgs e)
        {
            var startReactE = new DoubleAnimation
            {
                From = 420,
                To = 400,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outCirc,
            };

            imgStart.BeginAnimation(Canvas.TopProperty, startReactE);
        }

        private void RecStartAnimBounds_MouseLeave(object sender, MouseEventArgs e)
        {
            var startReactD = new DoubleAnimation
            {
                From = 400,
                To = 420,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = outCirc,
            };

            imgStart.BeginAnimation(Canvas.TopProperty, startReactD);
        }

        private void ImgExit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
