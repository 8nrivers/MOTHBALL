using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using MOTHBALL_WPF.Properties;
using System.Reflection;
using System.Windows.Media;

namespace MOTHBALL_WPF
{
    public static class AppServices
    {
        public static Assembly assembly = Assembly.GetExecutingAssembly();

        public class Cards
        {
            /// <summary>
            /// The ID number of the card.
            /// </summary>
            public int id;

            /// <summary>
            /// The name of the card.
            /// </summary>
            public string name;

            /// <summary>
            /// The written description of the card.
            /// </summary>
            public string description;

            /// <summary>
            /// The contents of the card.
            /// </summary>
            public string contents;
            // parser reads each character of the string to determine what will occur
        }

        public static List<Cards> cards { get; set; }
        public static int cutsceneNumber { get; set; }

        public static SoundPlayer enemyHurt = new SoundPlayer(Resources.enemyHurt);

        //public static MediaPlayer mPlayer = new MediaPlayer();
    }
}
