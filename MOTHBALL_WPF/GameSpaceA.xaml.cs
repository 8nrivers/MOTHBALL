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
using System.Windows.Threading;

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

        BackEase inBack = new BackEase
        {
            EasingMode = EasingMode.EaseIn
        };

        ElasticEase outElastic = new ElasticEase
        {
            EasingMode = EasingMode.EaseOut
        };

        int playerHealth = 100;
        int enemyHealth = 100;
        int turn = 0;

        DispatcherTimer effectWait = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500)
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

        readonly Window wnd = Window.GetWindow(Application.Current.MainWindow);

        static string[] enemyActionList = { "Next: Attacks for 5", ".", ".", ".", ".", "." };
        static string[] enemyActionContents = { "a5", "h2", ".", ".", "." };

        private void InitializeAnimation()
        {
            var wndTransitionEnd = new DoubleAnimation
            {
                From = 2460,
                To = 320,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = outCirc
            };

            wnd.BeginAnimation(Window.LeftProperty, wndTransitionEnd);

            var transitionSlideOut = new DoubleAnimation
            {
                From = 0,
                To = -1280,
                Duration = TimeSpan.FromMilliseconds(1000),
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
            recCard1Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard1, recCard1Bounds, 0, turn); };
            recCard2Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard2, recCard2Bounds, 1, turn); };
            recCard3Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard3, recCard3Bounds, 2, turn); };
            recCard4Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard4, recCard4Bounds, 3, turn); };
            recCard5Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard5, recCard5Bounds, 4, turn); };

            txtNextEvent.Text = enemyActionList[0];
        }

        void UpdateTurn()
        {
            turn += 1;

            txtTurnCounter.Text = "Turn " + ((turn / 2) + 1) + "/5";
            ProgressBarUpdate(prgBar, turn / 2);

            if (turn % 2 != 0)
            {
                EnemyAction();
            }
        }

        void ProgressBarUpdate(ProgressBar bar, int position)
        {
            var animateUpdate = new DoubleAnimation
            {
                To = position,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outElastic
            };

            bar.BeginAnimation(ProgressBar.ValueProperty, animateUpdate);
        }

        async void EnemyAction()
        {
            int index = (turn - 1) / 2;
            
            await Task.Delay(2000);

            for (int i = 0; i < enemyActionContents[index].Length; i++)
            {
                switch (enemyActionContents[index][i])
                {
                    case 'a': // Basic Attack: 1 parameter
                        playerHealth -= Int32.Parse(enemyActionContents[index][i + 1].ToString());
                        txtPlayerHealth.Text = "Your Health: " + playerHealth + "/100";
                        HealthShake(0);
                        i++;
                        break;
                    case 'h': // Basic Heal: 1 parameter
                        enemyHealth += Int32.Parse(enemyActionContents[index][i + 1].ToString());
                        txtEnemyHealth.Text = "Enemy Health: " + enemyHealth + "/100";
                        i++;
                        break;
                    default:
                        break;
                }
            }

            ScreenShake(20);
            txtNextEvent.Text = enemyActionList[index + 1];

            var redUpdate = new ColorAnimation
            {
                From = Color.FromRgb(186, 48, 48)
            };

            txtTurnCounter.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redUpdate);
            UpdateTurn();
        }

        async void CardClick(object sender, MouseButtonEventArgs e, TextBlock chosenCard, Rectangle bounds, int cardID, int turn)
        {
            if (turn % 2 == 0)
            {
                var cardUse = new DoubleAnimation
                {
                    From = 400,
                    To = -300,
                    Duration = TimeSpan.FromSeconds(0.5),
                    EasingFunction = inBack
                };

                bounds.IsEnabled = false;
                chosenCard.BeginAnimation(Canvas.TopProperty, cardUse);
                UpdateTurn();

                await Task.Delay(500);

                for (int i = 0; i < AppServices.cards[cardID].contents.Length; i++)
                {
                    switch (AppServices.cards[cardID].contents[i])
                    {
                        case 'a': // Basic Attack: 1 parameter (dmg)
                            enemyHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString());
                            txtEnemyHealth.Text = "Enemy Health: " + enemyHealth + "/100";
                            HealthShake(1);
                            ScreenShake(10);
                            i++;
                            break;
                        case 'm': // Multiple Attack: 2 parameters (amount, dmg)
                            for (int j = 0; j < Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString()); j++)
                            {
                                enemyHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 2].ToString());
                                txtEnemyHealth.Text = "Enemy Health: " + enemyHealth + "/100";
                                HealthShake(1);
                                ScreenShake(5);
                                await Task.Delay(250);
                            }
                            i += 2;
                            break;
                        case 'd': // Basic Defend: 1 parameter (defense points added)
                            break;
                        case 'w': // Weaken: 1 parameter (length of weaken)
                            break;
                        case 'z': // Daze
                            break;
                            // add more
                        default:
                            ScreenShake(10);
                            break;
                    }
                }
            }
        }

        void HealthShake(int toShake)
        {
            var shakeHealth = new DoubleAnimation
            {
                From = 42,
                To = 22,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outElastic
            };

            var shakeEnemy = new DoubleAnimation
            {
                From = 440,
                To = 400,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outElastic
            };

            var redHealth = new ColorAnimation
            {
                From = Color.FromRgb(186, 48, 48)
            };

            switch (toShake)
            {
                case 0: // player
                    txtPlayerHealth.BeginAnimation(Canvas.LeftProperty, shakeHealth);
                    txtPlayerHealth.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redHealth);
                    break;
                case 1: // enemy
                    txtEnemyHealth.BeginAnimation(Canvas.LeftProperty, shakeHealth);
                    txtEnemyHealth.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redHealth);
                    imgEnemy.BeginAnimation(Canvas.LeftProperty, shakeEnemy);
                    break;
                default:
                    break;
            }
        }

        void ScreenShake(int intensity)
        {
            var shakeScreen = new DoubleAnimation
            {
                From = 320 + intensity,
                To = 320,
                Duration = TimeSpan.FromMilliseconds(500),
                EasingFunction = outElastic
            };

            wnd.BeginAnimation(Window.LeftProperty, shakeScreen);
        }

        private void recCard1Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard1, null, 0); UpdateDecription(0); }
        private void recCard1Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard1, recCard1Bounds, 1); }
        private void recCard2Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard2, null, 0); UpdateDecription(1); }
        private void recCard2Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard2, recCard2Bounds, 1); }
        private void recCard3Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard3, null, 0); UpdateDecription(2); }
        private void recCard3Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard3, recCard3Bounds, 1); }
        private void recCard4Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard4, null, 0); UpdateDecription(3); }
        private void recCard4Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard4, recCard4Bounds, 1); }
        private void recCard5Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard5, null, 0); UpdateDecription(4); }
        private void recCard5Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard5, recCard5Bounds, 1); }

        private void CardReacts(TextBlock card, Rectangle bounds, int state)
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
                    if (bounds.IsEnabled == true)
                    {
                        var upReactD = new DoubleAnimation
                        {
                            From = 400,
                            To = 420,
                            Duration = TimeSpan.FromMilliseconds(500),
                            EasingFunction = outCirc
                        };

                        card.BeginAnimation(Canvas.TopProperty, upReactD);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
