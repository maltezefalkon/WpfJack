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

        private Log Log;
        private int RunSpeed = 250;

        public MainWindow()
        {
            InitializeComponent();
            Log = new Log(LogListBox);
            Log.StaticInstance = Log;
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
            UIParameterManager = new UIParameterManager();
            DataContext = new ViewModel(UIParameterManager, Game);
            InitializeGame();
        }

        private void InitializeGame()
        {
            _currentTurn = null;
            _currentTurnActionEnumerator = null;
            Game = new Game();
            Game.Init();
            DrawCastleStacks();
            Title = "New Game";
            NextButton.IsEnabled = true;
            RunButton.IsEnabled = true;
        }

        private void DrawCastleStacks()
        {
            DrawCastleStacks(Game, UIParameterManager);
        }

        private void DrawCastleStacks(Game game, UIParameterManager mgr)
        {
            ClearCastleStacks();
            const int padding = 25;
            int x = padding;
            CardShiftAction csa = Game.AllActions.LastOrDefault() as CardShiftAction;
            Card lastCard = null;
            if (null != csa && csa.DestinationCardPosition.IsValid(Game))
            {
                lastCard = csa.DestinationCardPosition.PeekCard(Game);
            }
            foreach (CardStack<Card> stack in Game.CastleStacks)
            {
                int y = padding;
                foreach (Card card in stack)
                {
                    DrawCard(card, mgr, x, y, CastleStackCanvas, Object.ReferenceEquals(lastCard, card) ? 0 : 1);
                    y += (int)(mgr.CardHeight / 4);
                }
                x += (int)mgr.CardWidth + padding;
            }
            x = padding;
            foreach (CardStack<ValuedCard> stack in Game.BeanstalkStacks)
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
            foreach (int value in game.DiscardPile.OfType<BeanstalkCard>().Select(b => b.Value).Distinct().OrderBy(b => b))
            {
                int y = padding;
                foreach (Card card in game.DiscardPile.OfType<BeanstalkCard>().Where(b => b.Value == value))
                {
                    DrawCard(card, mgr, x, y, DiscardPileCanvas, 2);
                    y += (int)(mgr.CardHeight / 4);
                }
                x += (int)mgr.CardWidth + padding;
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

        private PlayerTurn _currentTurn = null;
        private IEnumerator<IAction> _currentTurnActionEnumerator = null;

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTurn?.Win != null)
            {
                throw new Exception();
            }
            AdvanceGameStep();
        }

        private void ShowGameWin()
        {
            if (Game.Win != null)
            {
                GameOver();
            }
        }

        private void GameOver(Exception x = null)
        {
            string text = Game.Win?.ToString() ?? x?.Message ?? "Unexpected Game Over";
            Title = text;
            Log.WriteLine(text);
            NextButton.IsEnabled = false;
            RunButton.IsEnabled = false;
        }

        protected virtual void AdvanceGameStep()
        {
            try
            { 
                if (_currentTurn == null || !_currentTurnActionEnumerator.MoveNext())
                {
                    _currentTurn = Game.GetNextTurn();
                    _currentTurnActionEnumerator = _currentTurn.GetActions(Game).GetEnumerator();
                    if (!_currentTurnActionEnumerator.MoveNext())
                    {
                        throw new Exception("Empty actions for turn");
                    }
                    string turnText = $"Turn {Game.TurnCounter}: {_currentTurn.ActingPlayer.ToString()}'s turn";
                    Title = turnText;
                    Log.WriteLine($"== {turnText} ==");
                }
                Log.WriteLine(_currentTurnActionEnumerator.Current.ToString(Game));
                _currentTurnActionEnumerator.Current.Execute(Game);
            }
            catch (Exception x)
            {
                GameOver(x);
            }
            DrawCastleStacks();
            ShowGameWin();
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            while (Game.Win == null)
            {
                AdvanceGameStep();
                await Task.Delay(RunSpeed);
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            InitializeGame();
        }

        private void RunSpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RunSpeed = GetRunSpeed((int)e.NewValue);
        }

        private int GetRunSpeed(int newValue)
        {
            switch (newValue)
            {
                case 0: return 0;
                case 1: return 50;
                case 2: return 250;
                case 3: return 500;
                case 4: return 1000;
                case 5: return 2000;
                case 6: return 3000;
                case 7: return 5000;
                case 8: return 10000;
                case 9: return 15000;
                case 10: return 30000;
                default: throw new NotImplementedException();
            }
        }
    }

    public class Log
    {
        private ListBox _listBox;
        public Log(ListBox listBox)
        {
            _listBox = listBox;
        }

        public static Log StaticInstance;

        public void WriteLine(string s)
        {
            Console.WriteLine(s);
            _listBox.Items.Add(s);
            _listBox.ScrollIntoView(s);
        }
    }
}
