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
    /// Interaction logic for CreditsScreen.xaml
    /// </summary>
    public partial class CreditsScreen : Page
    {
        CircleEase outCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseOut
        };

        public CreditsScreen()
        {
            InitializeComponent();
            var menuBGScroll = new DoubleAnimation
            {
                From = -0,
                To = -1250,
                Duration = TimeSpan.FromSeconds(30),
                RepeatBehavior = RepeatBehavior.Forever
            };
            imgMenuBG.BeginAnimation(Canvas.LeftProperty, menuBGScroll);
            txtCredits.Text = "KEVIN MACLEOD: In order of appearance: \"Airship Serenity\", \"Eternity\", \"Desert of Lost Souls\", \"Voltaic\", \"One Sly Move\", Kevin MacLeod(incompetech.com) \n Licensed under Creative Commons: By Attribution 4.0 License \n http://creativecommons.org/licenses/by/4.0/ \n \n jsfxr, sound effects \n \n http://www.04.jp.org/, 04b03 font";
        }

        void PlayMedia(int snd)
        {
            switch (snd)
            {
                case 0: // menu select
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\menuBack.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                case 1: // menu hover
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\menuSelect.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                default:
                    break;
            }
        }

        private void RecExitAnimBounds_MouseEnter(object sender, MouseEventArgs e)
        {
            PlayMedia(1);
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

        private void RecExitAnimBounds_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlayMedia(0);
            Page menuScreen = new MenuScreen();
            this.NavigationService.Navigate(menuScreen);
        }
    }
}