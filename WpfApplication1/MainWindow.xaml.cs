using Jack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.UI;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<Card, UICard> UICards = new Dictionary<Card, UICard>();

        public MainWindow()
        {
            Game = new Game();
            InitializeComponent();
        }

        public Game Game
        {
            get;
            private set;
        }

        public UIParameterManager UIParameterManager
        {
            get;
            private set;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Game.Init();
            UIParameterManager = new UIParameterManager();
            DrawCastleStacks();
        }

        private void DrawCastleStacks()
        {
            DrawCastleStacks(Game, UIParameterManager);
        }

        private void DrawCastleStacks(Game game, UIParameterManager mgr)
        {
            ClearCastleStacks();
            const int padding = 50;
            int x = padding;
            foreach (CardStack stack in game.CastleStacks)
            {
                int y = padding;
                foreach (Card card in stack)
                {
                    DrawCard(card, mgr, x, y, CastleStackCanvas);
                    y += (int)(mgr.CardHeight / 4);
                }
                x += (int)mgr.CardWidth + padding;
            }
            foreach (CardStack<BeanstalkCard> stack in game.BeanstalkStacks)
            {
                int y = padding;
                foreach (Card card in stack)
                {
                    DrawCard(card, mgr, x, y, BeanstalkStackCanvas);
                    y += (int)(mgr.CardHeight / 4);
                }
            }
        }

        private void DrawCard(Card card, UIParameterManager mgr, int x, int y, Canvas castleStackCanvas)
        {
            UICard uiCard = new UICard(card, mgr);
            CastleStackCanvas.Children.Add(uiCard);
            Canvas.SetLeft(uiCard, x);
            Canvas.SetTop(uiCard, y);
            UICards[card] = uiCard;
        }

        private void ClearCastleStacks()
        {
            CastleStackCanvas.Children.Clear();
            UICards.Clear();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PlayerTurn turn = Game.GetNextTurn();
            foreach (IAction action in turn.Actions)
            {
                action.Execute(Game);
            }
            DrawCastleStacks();
        }
    }
}
