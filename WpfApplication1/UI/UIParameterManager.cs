using Jack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApplication1.UI
{
    public class UIParameterManager
    {
        public const double Scale = 30;
        public double CardWidth = 2.5d * Scale;
        public double CardHeight = 3.5d * Scale;

        public Color GetTextColor(Card c)
        {
            switch (c.CardType)
            {
                case CardType.Beanstalk:
                    return Colors.White;
                case CardType.Giant:
                    return Colors.Black;
                case CardType.Treasure:
                    return Colors.Yellow;
                default:
                    return Colors.Black;
            }
        }

        public Color GetColor(CardType c)
        {
            switch (c)
            {
                case CardType.Beanstalk:
                    return Colors.Green;
                case CardType.Giant:
                    return Colors.Red;
                case CardType.Treasure:
                    return Colors.Blue;
                default:
                    return Colors.White;
            }
        }
    }
}
