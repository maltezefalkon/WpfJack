using Jack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApplication1.UI
{
    public class UICard : System.Windows.Controls.Grid
    {

        public UICard(Card card, UIParameterManager parameterManager, int darkLevels = 0)
        {
            Width = parameterManager.CardWidth;
            Height = parameterManager.CardHeight;
            Card = card;
            Color c = parameterManager.GetColor(card.CardType);
            for (int i = 0; i < darkLevels; i++)
            {
                c = parameterManager.Darken(c, 0.4);
            }
            Children.Add(new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2d,
                Fill = new SolidColorBrush(c)
            });
            Children.Add(new TextBlock()
            {
                Text = card.Abbreviation,
                Foreground = new SolidColorBrush(parameterManager.GetTextColor(card)),
                Padding = new Thickness(parameterManager.CardWidth / 100d),
                FontWeight = FontWeights.Bold
            });
        }

        public Card Card
        {
            get;
            private set;
        }

    }
}
