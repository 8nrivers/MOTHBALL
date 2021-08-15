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

            await Task.Delay(8000);

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
            switch (AppServices.factNumber)
            {
                case 0:
                    txtFact.Text = "Did you know? Since the Industrial Revolution, 692 classified species of animals and plants have gone extinct.";
                    break;
                case 1:
                    txtFact.Text = "Did you know? In 1890, a visitor to England wrote \"The heavens were black with smoke, and the smother of the mills, to one whose lungs were unaccustomed to breathing sulphurised air, made itself felt.\"";
                    break;
                case 2:
                    txtFact.Text = "Did you know? During the Industrial Revolution, moths adapted to the sooty and blackened environment by evolving black coats to hide from predators.";
                    break;
                case 3:
                    txtFact.Text = "Did you know? Your actions have doomed the ecosystem for generations to come.";
                    break;
                default:
                    break;
            }

            await Task.Delay(9000);

            switch (AppServices.factNumber)
            {
                case 0:
                    Page gamespaceA = new GameSpaceC();
                    this.NavigationService.Navigate(gamespaceA);
                    break;
                case 1:
                    Page gamespaceB = new GameSpaceB();
                    this.NavigationService.Navigate(gamespaceB);
                    break;
                case 2:
                    Page gamespaceC = new GameSpaceC();
                    this.NavigationService.Navigate(gamespaceC);
                    break;
                case 3:
                    var fadeOutFact = new DoubleAnimation
                    {
                        To = 0,
                        Duration = TimeSpan.FromSeconds(3)
                    };
                    txtFact.BeginAnimation(OpacityProperty, fadeOutFact);
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
