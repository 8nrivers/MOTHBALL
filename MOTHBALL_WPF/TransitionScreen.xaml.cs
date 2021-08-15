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
    /// Interaction logic for TransitionScreen.xaml
    /// </summary>
    public partial class TransitionScreen : Page
    {
        CircleEase outCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseOut
        };

        CircleEase inCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseIn
        };

        readonly Window wnd = Window.GetWindow(Application.Current.MainWindow);

        public TransitionScreen()
        {
            InitializeComponent();
            InitializeAnimation();
            DisplayFact();
        }

        async void InitializeAnimation()
        {
            var wndTransitionEnd = new DoubleAnimation
            {
                From = 2460,
                To = 320,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = outCirc
            };

            wnd.BeginAnimation(Window.LeftProperty, wndTransitionEnd);

            var menuBGScroll = new DoubleAnimation
            {
                From = -0,
                To = -1250,
                Duration = TimeSpan.FromSeconds(30),
                RepeatBehavior = RepeatBehavior.Forever
            };

            imgMenuBG.BeginAnimation(Canvas.LeftProperty, menuBGScroll);

            var transitionSlideOut = new DoubleAnimation
            {
                From = 0,
                To = -1280,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = inCirc
            };

            imgTransition.BeginAnimation(Canvas.LeftProperty, transitionSlideOut);

            await Task.Delay(3000);

            var transitionSlideIn = new DoubleAnimation
            {
                From = 1280,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = outCirc
            };

            imgTransition.BeginAnimation(Canvas.LeftProperty, transitionSlideIn);
        }

        private async void DisplayFact()
        {
            await Task.Delay(5000);

            switch (AppServices.factNumber)
            {
                case 0:
                    Page gamespaceA = new GameSpaceA();
                    this.NavigationService.Navigate(gamespaceA);
                    break;
                case 1:
                    Page gamespaceB = new GameSpaceB();
                    this.NavigationService.Navigate(gamespaceB);
                    break;
                default:
                    break;
            }
        }

        private void PagMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            imgMenuBG.BeginAnimation(Canvas.LeftProperty, null);
        }
    }
}
