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
    /// Interaction logic for MenuWindow.xaml
    /// </summary>
    public partial class MenuWindow : Window
    {
        CircleEase outCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseOut
        };

        public MenuWindow()
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
            var upReactE = new DoubleAnimation
            {
                From = 420,
                To = 400,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outCirc,
            };

            imgStart.BeginAnimation(Canvas.TopProperty, upReactE);
        }

        private void RecStartAnimBounds_MouseLeave(object sender, MouseEventArgs e)
        {
            var upReactD = new DoubleAnimation
            {
                From = 400,
                To = 420,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = outCirc,
            };

            imgStart.BeginAnimation(Canvas.TopProperty, upReactD);
        }

        private void RecExitAnimBounds_MouseEnter(object sender, MouseEventArgs e)
        {
            var exitReactE = new DoubleAnimation
            {
                From = 1170,
                To = 1180,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = outCirc,
            };

            imgExit.BeginAnimation(Canvas.LeftProperty, exitReactE);
        }

        private void RecExitAnimBounds_MouseLeave(object sender, MouseEventArgs e)
        {
            var exitReactD = new DoubleAnimation
            {
                From = 1180,
                To = 1170,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = outCirc,
            };

            imgExit.BeginAnimation(Canvas.LeftProperty, exitReactD);
        }

        private void RecStartAnimBounds_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GameWindow game = new GameWindow();
            game.ShowDialog();
        }

        private void RecExitAnimBounds_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
