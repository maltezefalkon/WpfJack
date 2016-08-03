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
            DataContext = new ViewModel(UIParameterManager, Game);
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
                    DrawCard(card, mgr, x, y, CastleStackCanvas, 1);
                    y += (int)(mgr.CardHeight / 4);
                }
                x += (int)mgr.CardWidth + padding;
            }
            x = padding;
            foreach (CardStack<BeanstalkCard> stack in game.BeanstalkStacks)
            {
                int y = padding;
                foreach (Card card in stack)
                {
                    DrawCard(card, mgr, x, y, BeanstalkStackCanvas, 0);
                    y += (int)(mgr.CardHeight / 4);
                }
                x += (int)mgr.CardWidth + padding;
            }
            x = padding;
            int yy = padding;
            foreach (Card card in game.DiscardPile)
            {
                DrawCard(card, mgr, x, yy, DiscardPileCanvas, 2);
                yy += (int)(mgr.CardHeight / 4);
            }
        }

        private void DrawCard(Card card, UIParameterManager mgr, int x, int y, Canvas canvas, int darkness)
        {
            UICard uiCard = UICards.ContainsKey(card) ? UICards[card] : null;
            if (null == uiCard)
            {
                uiCard = new UICard(card, mgr, darkness);
                canvas.Children.Add(uiCard);
                UICards[card] = uiCard;
            }
            Canvas.SetLeft(uiCard, x);
            Canvas.SetTop(uiCard, y);
        }

        private void ClearCastleStacks()
        {
            CastleStackCanvas.Children.Clear();
            BeanstalkStackCanvas.Children.Clear();
            DiscardPileCanvas.Children.Clear();
            UICards.Clear();
        }

        private int _turnCounter = 0;
        private PlayerTurn _currentTurn = null;
        private IEnumerator<IAction> _currentTurnActionEnumerator = null;
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTurn == null || !_currentTurnActionEnumerator.MoveNext())
            {
                _turnCounter++;
                _currentTurn = Game.GetNextTurn();
                _currentTurnActionEnumerator = _currentTurn.GetActions(Game).GetEnumerator();
                if (!_currentTurnActionEnumerator.MoveNext())
                {
                    throw new Exception("Empty actions for turn");
                }
                this.Title = $"Turn {_turnCounter}: {_currentTurn.ActingPlayer.ToString()}'s turn";
            }
            Console.WriteLine("# " + _currentTurnActionEnumerator.Current.ToString());
            _currentTurnActionEnumerator.Current.Execute(Game);
            DrawCastleStacks();
        }
    }
}
