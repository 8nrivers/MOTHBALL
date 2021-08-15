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
    /// Interaction logic for GameSpaceC.xaml
    /// </summary>
    public partial class GameSpaceC : Page
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

        const int MAX_P_HEALTH = 20;
        const int MAX_E_HEALTH = 38;
        int playerHealth = MAX_P_HEALTH;
        int enemyHealth = MAX_E_HEALTH;
        bool dazed;
        int playerVulnerableTimer;
        int vulnerableTimer;
        bool vulnJustUsed = false;
        bool vulnJustUsedP = false;
        int playerPoison;
        int enemyPoison;
        int turn;
        bool playerTurnReady = true;

        const int EXPOSITION_WAIT = 12000; // make this 12000

        public GameSpaceC()
        {
            InitializeComponent();
            InitializeAnimation();
            LoadCard(txtblCard1, AppServices.cards[5]);
            LoadCard(txtblCard2, AppServices.cards[8]);
            LoadCard(txtblCard3, AppServices.cards[3]);
            LoadCard(txtblCard4, AppServices.cards[7]);
            LoadCard(txtblCard5, AppServices.cards[6]);
            LoadCard(txtblCard6, AppServices.cards[1]);

            InitializeEncounter();
            BeginMusicPlayback();
        }

        readonly Window wnd = Window.GetWindow(Application.Current.MainWindow);

        static string[] enemyActionList = { "Next: Inflicts 3 Vulnerable", "Next: Attacks for 3", "Next: Inflicts 2 Poison", "Next: Attacks for 6", "Next: Heals for 5", "Next: Attacks for 3", "Time's Up!" };
        static string[] enemyActionContents = { "v3", "a3", "p2", "a6", "h5", "a3" };

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
                From = -3,
                To = 3,
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

            var floatAnimate = new DoubleAnimation
            {
                From = 160,
                To = 140,
                Duration = TimeSpan.FromSeconds(4),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = inOutCirc
            };

            imgEnemy.BeginAnimation(Canvas.TopProperty, floatAnimate);

            var battleBGScroll = new DoubleAnimation
            {
                From = -0,
                To = -1280,
                Duration = TimeSpan.FromSeconds(30),
                RepeatBehavior = RepeatBehavior.Forever
            };

            imgBattle3BG.BeginAnimation(Canvas.LeftProperty, battleBGScroll);

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
            recCard1Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard1, recCard1Bounds, 5, turn); };
            recCard2Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard2, recCard2Bounds, 8, turn); };
            recCard3Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard3, recCard3Bounds, 3, turn); };
            recCard4Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard4, recCard4Bounds, 7, turn); };
            recCard5Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard5, recCard5Bounds, 6, turn); };
            recCard6Bounds.MouseDown += delegate (object sender, MouseButtonEventArgs e) { CardClick(sender, e, txtblCard6, recCard6Bounds, 1, turn); };

            recCard1Bounds.IsEnabled = false;
            recCard2Bounds.IsEnabled = false;
            recCard3Bounds.IsEnabled = false;
            recCard4Bounds.IsEnabled = false;
            recCard5Bounds.IsEnabled = false;
            recCard6Bounds.IsEnabled = false;

            playerHealth = MAX_P_HEALTH;
            enemyHealth = MAX_E_HEALTH;
            txtPlayerHealth.Text = "Your Health: " + MAX_P_HEALTH + "/" + MAX_P_HEALTH;
            txtEnemyHealth.Text = "Enemy Health: " + MAX_E_HEALTH + "/" + MAX_E_HEALTH;
            txtNextEvent.Text = enemyActionList[0];
            ProgressBarUpdate(prgBar, 0);
            txtTurnCounter.Text = "Turn 1/6";
            HealthShake(2);
            HealthShake(3);

            dazed = false;
            imgDaze.Visibility = Visibility.Hidden;
            vulnerableTimer = 0;
            playerVulnerableTimer = 0;
            vulnJustUsed = false;
            vulnJustUsedP = false;
            imgVulnerable.Visibility = Visibility.Hidden;
            txtVulnerable.Visibility = Visibility.Hidden;
            imgVulnerableP.Visibility = Visibility.Hidden;
            txtVulnerableP.Visibility = Visibility.Hidden;
            playerPoison = 0;
            enemyPoison = 0;
            imgPoison.Visibility = Visibility.Hidden;
            txtPoison.Visibility = Visibility.Hidden;
            imgPoisonP.Visibility = Visibility.Hidden;
            txtPoisonP.Visibility = Visibility.Hidden;
            turn = 0;

            ReEnableCard(txtblCard1, recCard1Bounds);
            ReEnableCard(txtblCard2, recCard2Bounds);
            ReEnableCard(txtblCard3, recCard3Bounds);
            ReEnableCard(txtblCard4, recCard4Bounds);
            ReEnableCard(txtblCard5, recCard5Bounds);
            ReEnableCard(txtblCard6, recCard6Bounds);
        }

        async void ReEnableCard(TextBlock card, Rectangle bounds)
        {
            var returnCard = new DoubleAnimation
            {
                From = 720,
                To = 460,
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = outBack
            };
            card.BeginAnimation(Canvas.TopProperty, returnCard);
            await Task.Delay(1000);
            bounds.IsEnabled = true;
        }

        void BeginMusicPlayback()
        {
            AppServices.mPlayerC3.Open(new Uri($@"{AppDomain.CurrentDomain.BaseDirectory}\assets\Voltaic.mp3"));
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

            txtTurnCounter.Text = "Turn " + ((turn / 2) + 1) + "/6";
            ProgressBarUpdate(prgBar, turn / 2);

            if (turn > 11)
            {
                PlayerDies(0);
            }
            else if (turn % 2 != 0)
            {
                EnemyAction();
            }
            else if (playerHealth < 1)
            {
                PlayerDies(1);
            }

            playerTurnReady = true;
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
                                if (playerVulnerableTimer > 0)
                                {
                                    ScreenShake(40);
                                    playerHealth -= Int32.Parse(enemyActionContents[index][i + 1].ToString()) + 2;
                                    var vulnShake = new DoubleAnimation
                                    {
                                        From = 314,
                                        Duration = TimeSpan.FromMilliseconds(500),
                                        EasingFunction = outElastic
                                    };
                                    var vulnTShake = new DoubleAnimation
                                    {
                                        From = 354,
                                        Duration = TimeSpan.FromMilliseconds(500),
                                        EasingFunction = outElastic
                                    };
                                    imgVulnerableP.BeginAnimation(Canvas.LeftProperty, vulnShake);
                                    txtVulnerableP.BeginAnimation(Canvas.LeftProperty, vulnTShake);
                                }
                                else
                                {
                                    ScreenShake(20);
                                    playerHealth -= Int32.Parse(enemyActionContents[index][i + 1].ToString());
                                }
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
                            case 'v': // Inflict Vulnerable: 1 parameter
                                PlayMedia(2);
                                ScreenShake(5);
                                var inflictVulnShake = new DoubleAnimation
                                {
                                    From = 314,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                var vulnTextShake = new DoubleAnimation
                                {
                                    From = 354,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                playerVulnerableTimer = Int32.Parse(enemyActionContents[index][i + 1].ToString());
                                imgVulnerableP.Visibility = Visibility.Visible;
                                txtVulnerableP.Visibility = Visibility.Visible;
                                imgVulnerableP.BeginAnimation(Canvas.LeftProperty, inflictVulnShake);
                                txtVulnerableP.BeginAnimation(Canvas.LeftProperty, vulnTextShake);
                                txtVulnerableP.Text = playerVulnerableTimer.ToString();
                                vulnJustUsedP = true;
                                i++;
                                break;
                            case 'p': // Inflict Poison: 1 parameter
                                PlayMedia(2);
                                var inflictPoisonShake = new DoubleAnimation
                                {
                                    From = 244,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                var poisonTextShake = new DoubleAnimation
                                {
                                    From = 284,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                playerPoison = Int32.Parse(enemyActionContents[index][i + 1].ToString());
                                imgPoisonP.Visibility = Visibility.Visible;
                                txtPoisonP.Visibility = Visibility.Visible;
                                imgPoisonP.BeginAnimation(Canvas.LeftProperty, inflictPoisonShake);
                                txtPoisonP.BeginAnimation(Canvas.LeftProperty, poisonTextShake);
                                txtPoisonP.Text = playerPoison.ToString();
                                ScreenShake(10);
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

                var redUpdate = new ColorAnimation
                {
                    From = Color.FromRgb(186, 48, 48)
                };

                if (playerVulnerableTimer > 0 && vulnJustUsedP == false)
                {
                    playerVulnerableTimer--;
                    txtVulnerableP.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redUpdate);
                    txtVulnerableP.Text = playerVulnerableTimer.ToString();
                    if (playerVulnerableTimer == 0)
                    {
                        imgVulnerableP.Visibility = Visibility.Hidden;
                        txtVulnerableP.Visibility = Visibility.Hidden;
                    }
                }
                else if (playerVulnerableTimer == 0 && vulnJustUsedP == false)
                {
                    imgVulnerableP.Visibility = Visibility.Hidden;
                    txtVulnerableP.Visibility = Visibility.Hidden;
                }
                else
                {
                    vulnJustUsedP = false;
                }

                if (enemyPoison > 0)
                {
                    PlayMedia(0);
                    txtPoison.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redUpdate);
                    if (vulnerableTimer > 0)
                    {
                        enemyHealth -= enemyPoison + 2;
                        ScreenShake(10);
                    }
                    else
                    {
                        enemyHealth -= enemyPoison;
                        ScreenShake(5);
                    }
                    HealthShake(0);
                    enemyPoison--;
                    txtPoison.Text = enemyPoison.ToString();
                    if (enemyPoison == 0)
                    {
                        PlayMedia(6);
                        imgPoison.Visibility = Visibility.Hidden;
                        txtPoison.Visibility = Visibility.Hidden;
                    }
                }
                else if (enemyPoison == 0)
                {
                    imgPoison.Visibility = Visibility.Hidden;
                    txtPoison.Visibility = Visibility.Hidden;
                }

                txtNextEvent.Text = enemyActionList[index + 1];

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
            if (turn % 2 == 0 && playerTurnReady == true)
            {
                var cardUse = new DoubleAnimation
                {
                    From = 400,
                    To = -300,
                    Duration = TimeSpan.FromSeconds(0.5),
                    EasingFunction = inBack
                };
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

                bounds.IsEnabled = false;
                chosenCard.BeginAnimation(Canvas.TopProperty, cardUse);
                UpdateTurn();
                playerTurnReady = false;

                await Task.Delay(500);

                for (int i = 0; i < AppServices.cards[cardID].contents.Length; i++)
                {
                    switch (AppServices.cards[cardID].contents[i])
                    {
                        case 'a': // Basic Attack: 1 parameter (dmg)
                            if (vulnerableTimer > 0)
                            {
                                enemyHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString()) + 2;
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
                        case 'h': // Basic Heal: 1 parameter
                            playerHealth += Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString());
                            PlayMedia(3);
                            HealthShake(2);
                            ScreenShake(5);
                            i++;
                            break;
                        case 'v': // Inflict Vulnerable: 1 parameter (length of vuln)
                            PlayMedia(2);
                            vulnerableTimer = Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString());
                            imgVulnerable.Visibility = Visibility.Visible;
                            txtVulnerable.Visibility = Visibility.Visible;
                            imgVulnerable.BeginAnimation(Canvas.LeftProperty, vulnShake);
                            txtVulnerable.BeginAnimation(Canvas.LeftProperty, vulnTShake);
                            txtVulnerable.Text = vulnerableTimer.ToString();
                            HealthShake(4);
                            vulnJustUsed = true;
                            i++;
                            break;
                        case 'z': // Inflict Daze
                            PlayMedia(2);
                            var inflictDazeShake = new DoubleAnimation
                            {
                                From = 724,
                                Duration = TimeSpan.FromMilliseconds(500),
                                EasingFunction = outElastic
                            };
                            dazed = true;
                            imgDaze.Visibility = Visibility.Visible;
                            imgDaze.BeginAnimation(Canvas.LeftProperty, inflictDazeShake);
                            HealthShake(4);
                            break;
                        case 'p': // Inflict Poison
                            PlayMedia(2);
                            var inflictPoisonShake = new DoubleAnimation
                            {
                                From = 864,
                                Duration = TimeSpan.FromMilliseconds(500),
                                EasingFunction = outElastic
                            };
                            var poisonTextShake = new DoubleAnimation
                            {
                                From = 904,
                                Duration = TimeSpan.FromMilliseconds(500),
                                EasingFunction = outElastic
                            };
                            enemyPoison = Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString());
                            imgPoison.Visibility = Visibility.Visible;
                            txtPoison.Visibility = Visibility.Visible;
                            imgPoison.BeginAnimation(Canvas.LeftProperty, inflictPoisonShake);
                            txtPoison.BeginAnimation(Canvas.LeftProperty, poisonTextShake);
                            txtPoison.Text = enemyPoison.ToString();
                            HealthShake(4);
                            ScreenShake(10);
                            i++;
                            break;
                        case 's': // Deal damage to self
                            if (playerVulnerableTimer > 0)
                            {
                                ScreenShake(40);
                                var inflictVulnShake = new DoubleAnimation
                                {
                                    From = 314,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                var vulnTextShake = new DoubleAnimation
                                {
                                    From = 354,
                                    Duration = TimeSpan.FromMilliseconds(500),
                                    EasingFunction = outElastic
                                };
                                playerHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString()) + 2;
                                imgVulnerableP.BeginAnimation(Canvas.LeftProperty, inflictVulnShake);
                                txtVulnerableP.BeginAnimation(Canvas.LeftProperty, vulnTextShake);
                                HealthShake(0);
                                i++;
                            }
                            else
                            {
                                ScreenShake(20);
                                playerHealth -= Int32.Parse(AppServices.cards[cardID].contents[i + 1].ToString());
                            }
                            HealthShake(0);
                            i++;
                            break;
                        default:
                            ScreenShake(10);
                            break;
                    }
                }

                var redUpdate = new ColorAnimation
                {
                    From = Color.FromRgb(186, 48, 48)
                };

                if (vulnerableTimer > 0 && vulnJustUsed == false)
                {
                    vulnerableTimer--;
                    txtVulnerable.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redUpdate);
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

                if (playerPoison > 0)
                {
                    PlayMedia(0);
                    txtPoisonP.Foreground.BeginAnimation(SolidColorBrush.ColorProperty, redUpdate);
                    if (playerVulnerableTimer > 0)
                    {
                        playerHealth -= playerPoison + 2;
                        ScreenShake(40);
                    }
                    else
                    {
                        playerHealth -= playerPoison;
                        ScreenShake(20);
                    }
                    HealthShake(0);
                    playerPoison--;
                    txtPoisonP.Text = playerPoison.ToString();
                    if (playerPoison == 0)
                    {
                        PlayMedia(6);
                        imgPoisonP.Visibility = Visibility.Hidden;
                        txtPoisonP.Visibility = Visibility.Hidden;
                    }
                }
                else if (playerPoison == 0)
                {
                    imgPoisonP.Visibility = Visibility.Hidden;
                    txtPoisonP.Visibility = Visibility.Hidden;
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
                From = 520,
                To = 480,
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
            AppServices.mPlayerC3.Stop();
            txtNextEvent.Text = "You Win!";

            var deathDrop = new DoubleAnimation
            {
                From = 160,
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
            AppServices.factNumber = 3;
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

            if (reason == 1)
            {
                txtNextEvent.Text = "You Died!";
            }

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

        private void recCard1Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard1, null, 0); UpdateDecription(5); }
        private void recCard1Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard1, recCard1Bounds, 1); }
        private void recCard2Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard2, null, 0); UpdateDecription(8); }
        private void recCard2Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard2, recCard2Bounds, 1); }
        private void recCard3Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard3, null, 0); UpdateDecription(3); }
        private void recCard3Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard3, recCard3Bounds, 1); }
        private void recCard4Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard4, null, 0); UpdateDecription(7); }
        private void recCard4Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard4, recCard4Bounds, 1); }
        private void recCard5Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard5, null, 0); UpdateDecription(6); }
        private void recCard5Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard5, recCard5Bounds, 1); }
        private void recCard6Bounds_MouseEnter(object sender, MouseEventArgs e) { CardReacts(txtblCard6, null, 0); UpdateDecription(1); }
        private void recCard6Bounds_MouseLeave(object sender, MouseEventArgs e) { CardReacts(txtblCard6, recCard6Bounds, 1); }

        private void CardReacts(TextBlock card, Rectangle bounds, int state)
        {
            switch (state)
            {
                case 0:
                    PlayMedia(4);
                    var upReactE = new DoubleAnimation
                    {
                        From = 460,
                        To = 440,
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
                            From = 440,
                            To = 460,
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

        private void pagGameSpaceC_Unloaded(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["Enemy Rotation"]).Stop();
            imgBattle3BG.BeginAnimation(Canvas.LeftProperty, null);
        }
    }
}
