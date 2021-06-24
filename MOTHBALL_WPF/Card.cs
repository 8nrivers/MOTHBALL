using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOTHBALL_WPF
{
    public class Card
    {
        /// <summary>
        /// The name of the card.
        /// </summary>
        public string name;

        /// <summary>
        /// The position of the card in the player's hand.
        /// </summary>
        public int position;

        /// <summary>
        /// The written description on the card. Should accurately describe its contents.
        /// </summary>
        public string description;

        /// <summary>
        /// The functional contents of the card.
        /// Index 0: Identity of card
        ///     0: Basic Attack
        ///     1: Multiple Attack
        ///     2: Defense
        ///     3: Toxin (damage over time)
        ///     4: Heal
        /// Index 1: Value of effect
        /// Index 2: Secondary effect type
        ///     0: # Attacks [FOR MULTI-ATTACK]
        ///     1: Toxin
        ///     2: Bleed (increase damage from single blows)
        ///     3: Damage Self
        /// Index 3: Secondary effect value
        /// </summary>
        public string[] contents = new string[4];

        /// <summary>
        /// Applies effects according to card contents.
        /// </summary>
        public void Consume()
        {
            switch (contents[0])
            {
                default:
                    break;
            }
        }
    }
}