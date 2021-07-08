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
    /// Interaction logic for GameSpaceA.xaml
    /// </summary>
    public partial class GameSpaceA : Page
    {
        CircleEase inOutCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseInOut
        };

        CircleEase inCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseIn
        };

        CircleEase outCirc = new CircleEase
        {
            EasingMode = EasingMode.EaseOut
        };

        public GameSpaceA()
        {
            InitializeComponent();
            InitializeAnimation();
            InitializeEncounter();
        }

        int turn = 0;

        private void InitializeAnimation()
        {
            var transitionSlideOut = new DoubleAnimation
            {
                From = 0,
                To = -1280,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = inCirc
            };

            imgTransition.BeginAnimation(Canvas.LeftProperty, transitionSlideOut);

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = 100,
                Duration = TimeSpan.FromSeconds(8),
                EasingFunction = inCirc
            };

            imgBar.BeginAnimation(OpacityProperty, fadeIn);
            prgBar.BeginAnimation(OpacityProperty, fadeIn);
            imgEnemy.BeginAnimation(OpacityProperty, fadeIn);
            txtTurnCounter.BeginAnimation(OpacityProperty, fadeIn);

            Storyboard enemyRotate = new Storyboard();

            var rotateAnimate = new DoubleAnimation
            {
                From = -1,
                To = 1,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = inOutCirc
            };

            Storyboard.SetTarget(rotateAnimate, imgEnemy);
            Storyboard.SetTargetProperty(rotateAnimate, new PropertyPath("(UIElement.RenderTransform).(RotateTransform.Angle)"));
            enemyRotate.Children.Add(rotateAnimate);

            Resources.Add("Enemy Rotation", enemyRotate);
            ((Storyboard)Resources["Enemy Rotation"]).Begin();

            var battleBGScroll = new DoubleAnimation
            {
                From = -0,
                To = -1190,
                Duration = TimeSpan.FromSeconds(30),
                RepeatBehavior = RepeatBehavior.Forever
            };

            imgBattle1BG.BeginAnimation(Canvas.LeftProperty, battleBGScroll);
        }

        private void InitializeEncounter()
        {

            if (turn / 2 == 0) // player's turn
            {

            }
            else if (turn == 5) // fail state
            {

            }
            else // enemy's turn
            {

            }

            turn += 1;
        }

        private void TxtblCard1_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard1, 0); }
        private void TxtblCard1_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard1, 1); }
        private void TxtblCard2_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard2, 0); }
        private void TxtblCard2_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard2, 1); }
        private void TxtblCard3_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard3, 0); }
        private void TxtblCard3_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard3, 1); }
        private void TxtblCard4_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard4, 0); }
        private void TxtblCard4_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard4, 1); }
        private void TxtblCard5_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard5, 0); }
        private void TxtblCard5_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard5, 1); }

        private void CardReacts(TextBlock card, int state)
        {
            switch (state)
            {
                case 0:
                    var upReactE = new DoubleAnimation
                    {
                        From = 420,
                        To = 400,
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = outCirc,
                    };

                    card.BeginAnimation(Canvas.TopProperty, upReactE);
                    break;
                case 1:
                    var upReactD = new DoubleAnimation
                    {
                        From = 400,
                        To = 420,
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = outCirc,
                    };

                    card.BeginAnimation(Canvas.TopProperty, upReactD);
                    break;
                default:
                    break;
            }
        }
    }
}
