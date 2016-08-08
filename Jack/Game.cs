using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class Game
    {
        public Deck Deck
        {
            get;
            set;
        }

        public JackPlayer Jack
        {
            get;
            set;
        }

        public GiantPlayer Giant
        {
            get;
            set;
        }

        public IList<PlayerTurn> AllTurns
        {
            get;
            private set;
        }

        public List<IAction> AllActions
        {
            get;
            private set;
        }

        public Game()
        {
            Jack = new JackPlayer();
            Giant = new GiantPlayer();
            Deck = new Deck();
            AllTurns = new List<PlayerTurn>(50);
            AllActions = new List<IAction>(100);
        }

        public void Init()
        {
            Deck.Shuffle();
            CastleStacks = new CastleStack[5];
            int initialCardsPerCastleStack = Deck.Count / CastleStacks.Length;
            for (int i = 0; i < CastleStacks.Length; i++)
            {
                CastleStacks[i] = new CastleStack(i);
                for (int j = 0; j < initialCardsPerCastleStack; j++)
                {
                    CastleStacks[i].Add(Deck.Pop(StackEnd.Front));
                }
            }
            DiscardPile = new CardStack<Card>("Discard Pile");
            BeanstalkStacks = new CardStack<ValuedCard>[3];
            for (int i = 0; i < BeanstalkStacks.Length; i++)
            {
                BeanstalkStacks[i] = new CardStack<ValuedCard>($"Beanstalk Stack {i + 1}");
            }
            IsJacksTurn = true;
        }

        public PlayerTurn GetNextTurn()
        {
            if (null != Win)
            {
                throw new InvalidOperationException();
            }
            TurnCounter++;
            PlayerTurn ret = CurrentPlayer.GetNextTurn(this);
            AllTurns.Add(ret);
            Debug.Assert(AllTurns.Count == TurnCounter);
            IsJacksTurn = !IsJacksTurn;
            return ret;
        }

        public virtual Win CheckWinConditions()
        {
            if (ActiveBeanstalkStack == null && TurnCounter > 1)
            {
                Win = new JackWin(Jack);
                return Win;
            }
            else if (CheckGiantHorizontalWinCondition())
            {
                Win = new GiantHorizontalWin(Giant);
                return Win;
            }
            else if (CheckGiantVerticalWinCondition())
            {
                Win = new GiantVerticalWin(Giant);
                return Win;
            }
            else if (CheckGiantDiscardWinCondition())
            {
                Win = new GiantWinByDiscard(Giant);
                return Win;
            }
            else
            {
                return null;
            }
        }

        private bool CheckGiantDiscardWinCondition()
        {
            ActiveBeanstalkStackStats stats = GetActiveBeanstalkMinMax();
            if (null != stats)
            {
                if (stats.RemainingSkips < 0 && ActiveBeanstalkStack.Count < RequiredBeanstalkCards)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckGiantHorizontalWinCondition()
        {
            IEnumerable<GiantCard> frontGiantCards = CastleStacks.Where(x => x.Any()).Select(x => x.Last()).OfType<GiantCard>();
            return FeeFieFoeFum(frontGiantCards);
        }

        private bool FeeFieFoeFum(IEnumerable<GiantCard> cards)
        {
            foreach (GiantCardType gct in Enum.GetValues(typeof(GiantCardType)))
            {
                if (gct != GiantCardType.Unknown)
                {
                    if (!cards.Any(x => x.GiantCardType == gct))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckGiantVerticalWinCondition()
        {
            foreach (CastleStack castleStack in CastleStacks)
            {
                for (int i = 0; i <= castleStack.Count - 4; i++)
                {
                    IEnumerable<GiantCard> giantCards = castleStack.Skip(i).Take(4).OfType<GiantCard>();
                    if (giantCards.Count() == 4 && FeeFieFoeFum(giantCards))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Win Win
        {
            get;
            private set;
        }

        public Player WinningPlayer
        {
            get;
            private set;
        }

        public bool IsJacksTurn
        {
            get;
            private set;
        }

        public Player CurrentPlayer => IsJacksTurn ? (Player)Jack : Giant;

        public CastleStack[] CastleStacks
        {
            get;
            private set;
        }

        public CardStack<Card> DiscardPile
        {
            get;
            private set;
        }

        public virtual int RequiredBeanstalkCards => 5;

        public CardStack<ValuedCard> ActiveBeanstalkStack => BeanstalkStacks.FirstOrDefault(x => x.Count < RequiredBeanstalkCards + 1);

        public CardStack<ValuedCard>[] BeanstalkStacks
        {
            get;
            private set;
        }

        public IEnumerable<Card> CardsInPlay => CastleStacks.SelectMany(x => x);

        public int FindCastleStackIndexForCard(Card card)
        {
            for (int i = 0; i < CastleStacks.Length; i++)
            {
                if (CastleStacks[i].Find(x => Object.ReferenceEquals(x, card)) != null)
                {
                    return i;
                }
            }
            return -1;
        }

        public StackEndCardPositionDescriptor GetPositionDescriptorForCard(Card card)
        {
            int stackIndex = FindCastleStackIndexForCard(card);
            int cardIndex = CastleStacks[stackIndex].IndexOf(card);
            StackEnd end = StackEnd.Back;
            int offset = cardIndex;
            if (cardIndex > (int)Math.Truncate(CastleStacks[stackIndex].Count / 2m))
            {
                end = StackEnd.Front;
                offset = CastleStacks[stackIndex].Count - cardIndex - 1;
            }
            return new StackEndCardPositionDescriptor()
            {
                End = end,
                Stack = new CastleStackDescriptor(stackIndex),
                Offset = offset
            };
        }

        public int TurnCounter
        {
            get;
            private set;
        }

        public ActiveBeanstalkStackStats GetActiveBeanstalkMinMax()
        {
            int minimumValue = Math.Max((ActiveBeanstalkStack.LastOrDefault()?.Value ?? 0) + 1, CardsInPlay.OfType<BeanstalkCard>().Min(x => x.Value));
            int maxCardAvailable = CardsInPlay.OfType<BeanstalkCard>().Max(x => x.Value);
            int currentSkips = minimumValue - ActiveBeanstalkStack.Count - 1;
            int remainingSkips = maxCardAvailable - RequiredBeanstalkCards - currentSkips;
            int maximumValue = minimumValue + remainingSkips;
            if (ActiveBeanstalkStack.Count == RequiredBeanstalkCards)
            {
                minimumValue = maximumValue = BeanstalkCard.TreasureValue;
            }
            return new ActiveBeanstalkStackStats()
            {
                Minimum = minimumValue,
                Maximum = maximumValue,
                MaxCardAvailable = maxCardAvailable,
                CurrentSkips = currentSkips,
                RemainingSkips = remainingSkips
            };
        }

        public class ActiveBeanstalkStackStats
        {
            public int Minimum;
            public int Maximum;
            public int MaxCardAvailable;
            public int CurrentSkips;
            public int RemainingSkips;
        }

        public void BeginAction(IAction action)
        {
        }

        public void CompleteAction(IAction action)
        {
            AllActions.Add(action);
        }
    }
}
