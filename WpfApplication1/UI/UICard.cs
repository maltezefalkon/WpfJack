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

        public UICard(Card card, UIParameterManager parameterManager)
        {
            Width = parameterManager.CardWidth;
            Height = parameterManager.CardHeight;
            Card = card;
            Children.Add(new Rectangle()
            {
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2d,
                Fill = new SolidColorBrush(parameterManager.GetColor(card.CardType))
            });
            Children.Add(new TextBlock()
            {
                Text = card.Abbreviation,
                Foreground = new SolidColorBrush(parameterManager.GetTextColor(card)),
                Padding = new System.Windows.Thickness(UIParameterManager.Scale / 5),
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
