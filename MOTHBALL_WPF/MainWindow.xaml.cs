using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            winMenu.Left = 320;
            winMenu.Top = 180;
            AppServices.factNumber = 0;
            
            string jsonfile = AppServices.assembly.GetManifestResourceNames().Single(str => str.EndsWith("cards.json"));

            string json;

            using (var reader = new StreamReader(AppServices.assembly.GetManifestResourceStream(jsonfile)))
            {
                json = reader.ReadToEnd();
            }

            AppServices.cards = JsonConvert.DeserializeObject<List<AppServices.Cards>>(json);

            frmContent.Content = new MenuScreen();
        }
    }
}