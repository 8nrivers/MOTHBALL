using System;
using System.Collections.Generic;
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

        BackEase outBack = new BackEase
        {
            EasingMode = EasingMode.EaseOut
        };

        public GameSpaceA()
        {
            InitializeComponent();
            InitializeAnimation();
            LoadCard(txtblCard1, AppServices.cards[0]);
            LoadCard(txtblCard2, AppServices.cards[1]);
            LoadCard(txtblCard3, AppServices.cards[2]);
            LoadCard(txtblCard4, AppServices.cards[3]);
            LoadCard(txtblCard5, AppServices.cards[4]);

            InitializeEncounter();
        }

        const string ENEMY_ACTION_LIST = ".|.|.|.|.";

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

        private void LoadCard(TextBlock item, AppServices.Cards card)
        {
            item.Text = card.name;
        }

        private void UpdateDecription(int id)
        {
            txtCardName.Text = AppServices.cards[id].name;
            txtDescription.Text = AppServices.cards[id].description;
        }

        private void InitializeEncounter()
        {
            var playerHealth = 100;
            var enemyHealth = 100;
            var turn = 0;

            txtblCard1.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, 0, turn); };
            txtblCard2.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, 1, turn); };
            txtblCard3.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, 2, turn); };
            txtblCard4.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, 3, turn); };
            txtblCard5.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, 4, turn); };

            var enemyActions = ENEMY_ACTION_LIST.Split('|');

            while (true)
            {
                if (turn / 2 == 0) // player's turn
                {

                }
                else if (turn == 5) // win/fail state
                {
                    break;
                }
                else // enemy's turn
                {
                    txtNextEvent.Text = "Next Action: " + enemyActions[(turn - 1) / 2];
                }

                turn += 1;
            }
        }

        void CardClick(object sender, MouseButtonEventArgs e, int card, int turn)
        {
            if (turn / 2 == 0)
            {
                switch (card)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    default:
                        break;
                }
            }
            else
            {

            }
        }

        private void TxtblCard1_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard1, 0); UpdateDecription(0); }
        private void TxtblCard1_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard1, 1); }
        private void TxtblCard2_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard2, 0); UpdateDecription(1); }
        private void TxtblCard2_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard2, 1); }
        private void TxtblCard3_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard3, 0); UpdateDecription(2); }
        private void TxtblCard3_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard3, 1); }
        private void TxtblCard4_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard4, 0); UpdateDecription(3); }
        private void TxtblCard4_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard4, 1); }
        private void TxtblCard5_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard5, 0); UpdateDecription(4); }
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
                        EasingFunction = outCirc
                    };

                    card.BeginAnimation(Canvas.TopProperty, upReactE);
                    break;

                case 1:
                    var upReactD = new DoubleAnimation
                    {
                        From = 400,
                        To = 420,
                        Duration = TimeSpan.FromMilliseconds(500),
                        EasingFunction = outCirc
                    };

                    card.BeginAnimation(Canvas.TopProperty, upReactD);
                    break;

                default:
                    break;
            }
        }

        private void CardUsed(TextBlock card)
        {
            var cardUse = new DoubleAnimation
            {
                From = 420,
                To = 730,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = outBack
            };

            card.BeginAnimation(Canvas.TopProperty, cardUse);
        }

        private void TxtblCard1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CardUsed(txtblCard1);
        }
    }
}
