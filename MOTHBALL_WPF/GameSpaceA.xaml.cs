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
using MOTHBALL_WPF.Properties;

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

        BackEase outBack = new BackEase
        {
            EasingMode = EasingMode.EaseOut
        };

        ElasticEase outElastic = new ElasticEase
        {
            EasingMode = EasingMode.EaseOut
        };

        const int MAX_P_HEALTH = 15;
        const int MAX_E_HEALTH = 25;
        int playerHealth = MAX_P_HEALTH;
        int enemyHealth = MAX_E_HEALTH;
        bool dazed;
        int vulnerableTimer;
        bool vulnJustUsed = false;
        int turn;

        const int EXPOSITION_WAIT = 12000; // set to 12000 on release

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
            BeginMusicPlayback();
        }

        readonly Window wnd = Window.GetWindow(Application.Current.MainWindow);

        static string[] enemyActionList = { "Next: Attacks for 5", "Next: Heals for 5", "Next: Attacks for 6", "Next: Heals for 2", "Next: Attacks for 4", "Time's Up!" };
        static string[] enemyActionContents = { "a5", "h5", "a6", "h2", "a4" };

        private async void InitializeAnimation()
        {
            txtExposition2.Opacity = 0;

            var transitionSlideOut = new DoubleAnimation
            {
                From = 0,
                To = -1280,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = inCirc
            };
            
            imgTransition.BeginAnimation(Canvas.LeftProperty, transitionSlideOut);

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

            Canvas.SetLeft(recExposition, -1);
            Canvas.SetLeft(txtExposition, 190);
            Canvas.SetLeft(txtExposition2, 190);

            await Task.Delay(EXPOSITION_WAIT);

            var banishExposition = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = inOutCirc
            };
            txtExposition.BeginAnimation(OpacityProperty, banishExposition);

            var actuallyIWantItBack = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = inOutCirc
            };
            txtExposition2.BeginAnimation(OpacityProperty, actuallyIWantItBack);

            await Task.Delay(EXPOSITION_WAIT);

            recExposition.BeginAnimation(OpacityProperty, banishExposition);
            txtExposition2.BeginAnimation(OpacityProperty, banishExposition);
            await Task.Delay(1000);
            recExposition.Visibility = Visibility.Hidden;
            txtExposition.Visibility = Visibility.Hidden;
            txtExposition2.Visibility = Visibility.Hidden;
        }

        private void LoadCard(TextBlock item, AppServices.Cards card)
        {
            item.Text = card.name;
            Canvas.SetTop(item, 720);
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

            recCard1Bounds.IsEnabled = false;
            recCard2Bounds.IsEnabled = false;
            recCard3Bounds.IsEnabled = false;
            recCard4Bounds.IsEnabled = false;
            recCard5Bounds.IsEnabled = false;

            playerHealth = MAX_P_HEALTH;
            enemyHealth = MAX_E_HEALTH;
            txtPlayerHealth.Text = "Your Health: " + MAX_P_HEALTH + "/" + MAX_P_HEALTH;
            txtEnemyHealth.Text = "Enemy Health: " + MAX_E_HEALTH + "/" + MAX_E_HEALTH;
            txtNextEvent.Text = enemyActionList[0];
            ProgressBarUpdate(prgBar, 0);
            txtTurnCounter.Text = "Turn 1/5";
            HealthShake(2);
            HealthShake(3);

            dazed = false;
            imgDaze.Visibility = Visibility.Hidden;
            vulnerableTimer = 0;
            vulnJustUsed = false;
            imgVulnerable.Visibility = Visibility.Hidden;
            txtVulnerable.Visibility = Visibility.Hidden;
            turn = 0;

            ReEnableCard(txtblCard1, recCard1Bounds);
            ReEnableCard(txtblCard2, recCard2Bounds);
            ReEnableCard(txtblCard3, recCard3Bounds);
            ReEnableCard(txtblCard4, recCard4Bounds);
            ReEnableCard(txtblCard5, recCard5Bounds);
        }

        async void ReEnableCard(TextBlock card, Rectangle bounds)
        {
            var returnCard = new DoubleAnimation
            {
                From = 720,
                To = 420,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = outBack
            };
            card.BeginAnimation(Canvas.TopProperty, returnCard);
            await Task.Delay(1000);
            bounds.IsEnabled = true;
        }

        void BeginMusicPlayback()
        {
            AppServices.mPlayerC3.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\Eternity.mp3"));
            AppServices.mPlayerC3.MediaEnded += new EventHandler(Music_Ended);
            AppServices.mPlayerC3.Play();
        }

        void Music_Ended(object sender, EventArgs e)
        {
            AppServices.mPlayerC3.Position = TimeSpan.Zero;
            AppServices.mPlayerC3.Play();
        }

        void UpdateTurn()
        {
            turn += 1;

            txtTurnCounter.Text = "Turn " + ((turn / 2) + 1) + "/5";
            ProgressBarUpdate(prgBar, turn / 2);

            if (turn > 9)
            {
                PlayerDies(0);
            }
            else if (turn % 2 != 0)
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

            if (enemyHealth > 0)
            {
                if (dazed != true)
                {
                    for (int i = 0; i < enemyActionContents[index].Length; i++)
                    {
                        switch (enemyActionContents[index][i])
                        {
                            case 'a': // Basic Attack: 1 parameter
                                playerHealth -= Int32.Parse(enemyActionContents[index][i + 1].ToString());
                                ScreenShake(20);
                                HealthShake(0);
                                i++;
                                break;
                            case 'h': // Basic Heal: 1 parameter
                                enemyHealth += Int32.Parse(enemyActionContents[index][i + 1].ToString());
                                PlayMedia(3);
                                ScreenShake(5);
                                HealthShake(3);
                                i++;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    PlayMedia(6);
                    ScreenShake(5);
                    dazed = false;
                    imgDaze.Visibility = Visibility.Hidden;
                }

                txtNextEvent.Text = enemyActionList[index + 1];

                var redUpdate = new ColorAnimation
                {
                    From = Color.FromRgb(186, 48, 48)
                };

                txtTurnCounter.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redUpdate);
                txtNextEvent.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redUpdate);
                UpdateTurn();
            }
            else
            {
                EnemyDies();
            }
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
                            if (vulnerableTimer > 0)
                            {
                                enemyHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString()) + 2;
                                var vulnShake = new DoubleAnimation
                                {
                                    From = 794,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                var vulnTShake = new DoubleAnimation
                                {
                                    From = 834,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                imgVulnerable.BeginAnimation(Canvas.LeftProperty, vulnShake);
                                txtVulnerable.BeginAnimation(Canvas.LeftProperty, vulnTShake);
                            }
                            else
                            {
                                enemyHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString());
                            }
                            HealthShake(1);
                            ScreenShake(10);
                            i++;
                            break;
                        case 'm': // Multiple Attack: 2 parameters (amount, dmg)
                            for (int j = 0; j < Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString()); j++)
                            {
                                if (vulnerableTimer > 0)
                                {
                                    enemyHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 2].ToString()) + 2;
                                    var vulnShake = new DoubleAnimation
                                    {
                                        From = 794,
                                        Duration = TimeSpan.FromMilliseconds(500),
                                        EasingFunction = outElastic
                                    };
                                    var vulnTShake = new DoubleAnimation
                                    {
                                        From = 834,
                                        Duration = TimeSpan.FromMilliseconds(500),
                                        EasingFunction = outElastic
                                    };
                                    imgVulnerable.BeginAnimation(Canvas.LeftProperty, vulnShake);
                                    txtVulnerable.BeginAnimation(Canvas.LeftProperty, vulnTShake);
                                }
                                else
                                {
                                    enemyHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 2].ToString());
                                }
                                HealthShake(1);
                                ScreenShake(5);
                                await Task.Delay(250);
                            }
                            i += 2;
                            break;
                        case 'd': // Basic Defend: 1 parameter (defense points added)
                            break;
                        case 'v': // Inflict Vulnerable: 1 parameter (length of vuln)
                            PlayMedia(2);
                            var inflictVulnShake = new DoubleAnimation
                            {
                                From = 794,
                                Duration = TimeSpan.FromMilliseconds(500),
                                EasingFunction = outElastic
                            };
                            var vulnTextShake = new DoubleAnimation
                            {
                                From = 834,
                                Duration = TimeSpan.FromMilliseconds(500),
                                EasingFunction = outElastic
                            };
                            vulnerableTimer = Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString());
                            imgVulnerable.Visibility = Visibility.Visible;
                            txtVulnerable.Visibility = Visibility.Visible;
                            imgVulnerable.BeginAnimation(Canvas.LeftProperty, inflictVulnShake);
                            txtVulnerable.BeginAnimation(Canvas.LeftProperty, vulnTextShake);
                            txtVulnerable.Text = vulnerableTimer.ToString();
                            HealthShake(4);
                            vulnJustUsed = true;
                            i++;
                            break;
                        case 'z': // Inflict Daze
                            PlayMedia(2);
                            var inflictDazeShake = new DoubleAnimation
                            {
                                From = 794,
                                Duration = TimeSpan.FromMilliseconds(500),
                                EasingFunction = outElastic
                            };
                            dazed = true;
                            imgDaze.Visibility = Visibility.Visible;
                            imgDaze.BeginAnimation(Canvas.LeftProperty, inflictDazeShake);
                            HealthShake(4);
                            break;
                        default:
                            ScreenShake(10);
                            break;
                    }   
                }

                if (vulnerableTimer > 0 && vulnJustUsed == false)
                {
                    vulnerableTimer--;
                    txtVulnerable.Text = vulnerableTimer.ToString();
                    if (vulnerableTimer == 0)
                    {
                        imgVulnerable.Visibility = Visibility.Hidden;
                        txtVulnerable.Visibility = Visibility.Hidden;
                    }
                }
                else if (vulnerableTimer == 0 && vulnJustUsed == false)
                {
                    imgVulnerable.Visibility = Visibility.Hidden;
                    txtVulnerable.Visibility = Visibility.Hidden;
                }
                else
                {
                    vulnJustUsed = false;
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

            var greenHealth = new ColorAnimation
            {
                From = Color.FromRgb(32, 210, 32)
            };

            switch (toShake)
            {
                case 0: // player damage
                    PlayMedia(1);
                    txtPlayerHealth.Text = "Your Health: " + playerHealth + "/" + MAX_P_HEALTH;
                    txtPlayerHealth.BeginAnimation(Canvas.LeftProperty, shakeHealth);
                    txtPlayerHealth.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redHealth);
                    break;
                case 1: // enemy damage
                    PlayMedia(0);
                    txtEnemyHealth.Text = "Enemy Health: " + enemyHealth + "/" + MAX_E_HEALTH;
                    txtEnemyHealth.BeginAnimation(Canvas.LeftProperty, shakeHealth);
                    txtEnemyHealth.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redHealth);
                    imgEnemy.BeginAnimation(Canvas.LeftProperty, shakeEnemy);
                    break;
                case 2: // player heal
                    txtPlayerHealth.Text = "Your Health: " + playerHealth + "/" + MAX_P_HEALTH;
                    txtPlayerHealth.BeginAnimation(Canvas.LeftProperty, shakeHealth);
                    txtPlayerHealth.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, greenHealth);
                    break;
                case 3: // enemy heal
                    txtEnemyHealth.Text = "Enemy Health: " + enemyHealth + "/" + MAX_E_HEALTH;
                    txtEnemyHealth.BeginAnimation(Canvas.LeftProperty, shakeHealth);
                    txtEnemyHealth.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, greenHealth);
                    break;
                case 4: // enemy status applied
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

        async void EnemyDies()
        {
            PlayMedia(5);
            ScreenShake(50);
            ProgressBarUpdate(prgBar, 0);
            txtNextEvent.Text = "You Win!";

            var deathDrop = new DoubleAnimation
            {
                From = 40,
                To = 720,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = inCirc
            };

            var deathOpacity = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromSeconds(2),
                EasingFunction = outCirc
            };

            imgEnemy.BeginAnimation(Canvas.TopProperty, deathDrop);
            imgEnemy.BeginAnimation(OpacityProperty, deathOpacity);

            await Task.Delay(2000);

            var wndTransitionStart = new DoubleAnimation
            {
                From = 320,
                To = -1280,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = inCirc
            };

            var transitionSlideIn = new DoubleAnimation
            {
                From = 1280,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = outCirc
            };

            imgTransition.BeginAnimation(Canvas.LeftProperty, transitionSlideIn);
            wnd.BeginAnimation(Window.LeftProperty, wndTransitionStart);

            await Task.Delay(1000);
            AppServices.factNumber = 1;
            Page transitionScreen = new TransitionScreen();
            this.NavigationService.Navigate(transitionScreen);
        }

        async void PlayerDies(int reason)
        {
            var shakeScreen = new DoubleAnimation
            {
                From = 420,
                To = 320,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = outElastic
            };

            wnd.BeginAnimation(Window.LeftProperty, shakeScreen);
            await Task.Delay(2000);
            PlayMedia(3);
            InitializeEncounter();
        }

        void PlayMedia(int snd)
        {
            switch (snd)
            {
                case 0: // enemy damage
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\enemyHurt.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                case 1: // player damage
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\playerHurt.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                case 2: // debuff inflicted
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\debuff.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                case 3: // heal
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\heal.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                case 4: // mouseover card
                    AppServices.mPlayerC2.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\cardSlide.wav"));
                    AppServices.mPlayerC2.Play();
                    break;
                case 5: // enemy defeated
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\defeated.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                case 6: // status wears off
                    AppServices.mPlayerC1.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\debuffExpire.wav"));
                    AppServices.mPlayerC1.Play();
                    break;
                default:
                    break;
            }
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
                    PlayMedia(4);
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

        private void pagGameSpaceA_Unloaded(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["Enemy Rotation"]).Stop();
            imgBattle1BG.BeginAnimation(Canvas.LeftProperty, null);
        }
    }
}
