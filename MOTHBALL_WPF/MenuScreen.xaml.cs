using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
using System.Windows.Threading;

namespace MOTHBALL_WPF
{
    /// <summary>
    /// Interaction logic for MenuScreen.xaml
    /// </summary>

    public partial class MenuScreen : Page
    {
        CircleEase outCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseOut
        };

        int transitionTime = 0;

        DispatcherTimer transitionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
        };

        public MenuScreen()
        {
            InitializeComponent();
            transitionTimer.Tick += TransitionTimer_Tick;
            InitializeAnimation();
        }

        private void InitializeAnimation()
        {
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
                Duration = TimeSpan.FromSeconds(30),
                RepeatBehavior = RepeatBehavior.Forever
            };

            Storyboard titleRotate = new Storyboard();

            var titleRotation = new DoubleAnimation
            {
                From = 0,
                To = 3,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = outCirc,
            };

            Storyboard.SetTarget(titleRotation, imgTitle);
            Storyboard.SetTargetProperty(titleRotation, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            titleRotate.Children.Add(titleRotation);

            titleRotate.Begin();

            imgTitle.BeginAnimation(HeightProperty, titleBounceAnimation);
            imgMenuBG.BeginAnimation(Canvas.LeftProperty, menuBGScroll);
        }

        private void RecStartAnimBounds_MouseEnter(object sender, MouseEventArgs e)
        {
            var upReactE = new DoubleAnimation
            {
                From = 400,
                To = 380,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outCirc,
            };
            
            imgStart.BeginAnimation(Canvas.TopProperty, upReactE);
        }

        private void RecStartAnimBounds_MouseLeave(object sender, MouseEventArgs e)
        {
            var upReactD = new DoubleAnimation
            {
                From = 380,
                To = 400,
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
                Duration = TimeSpan.FromMilliseconds(500),
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
            var transitionSlideIn = new DoubleAnimation
            {
                From = 1280,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outCirc
            };

            imgTransition.BeginAnimation(Canvas.LeftProperty, transitionSlideIn);
            transitionTimer.Start();
        }

        private void TransitionTimer_Tick(object sender, EventArgs e)
        {
            transitionTime += 1;
            Console.WriteLine(transitionTime);

            if (transitionTime >= 1)
            {
                Page gamespace = new GameSpaceA();
                transitionTimer.Stop();
                this.NavigationService.Navigate(gamespace);
            }
        }

        private void RecExitAnimBounds_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PagMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            //((Storyboard)Resources["Title Rotation"]).Stop();
            imgTitle.BeginAnimation(HeightProperty, null);
            imgMenuBG.BeginAnimation(Canvas.LeftProperty, null);
        }

        private void RecCreditsAnimBounds_MouseEnter(object sender, MouseEventArgs e)
        {
            var upReactE = new DoubleAnimation
            {
                From = 520,
                To = 500,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outCirc,
            };

            imgCredits.BeginAnimation(Canvas.TopProperty, upReactE);
        }

        private void RecCreditsAnimBounds_MouseLeave(object sender, MouseEventArgs e)
        {
            var upReactD = new DoubleAnimation
            {
                From = 500,
                To = 520,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = outCirc,
            };

            imgCredits.BeginAnimation(Canvas.TopProperty, upReactD);
        }
    }
}