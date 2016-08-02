using System;
using System.Collections.Generic;
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

        public Game()
        {
            Jack = new JackPlayer();
            Giant = new GiantPlayer();
            Deck = new Deck();
        }

        public void Init()
        {
            Deck.Shuffle();
            CastleStacks = new CardStack[5];
            int initialCardsPerCastleStack = Deck.Count / CastleStacks.Length;
            for (int i = 0; i < CastleStacks.Length; i++)
            {
                CastleStacks[i] = new CardStack($"Castle Stack {i + 1}");
                for (int j = 0; j < initialCardsPerCastleStack; j++)
                {
                    CastleStacks[i].Add(Deck.Pop(StackEnd.Front));
                }
            }
            DiscardPile = new CardStack("Discard Pile");
            BeanstalkStacks = new CardStack<BeanstalkCard>[3];
            for (int i = 0; i < BeanstalkStacks.Length; i++)
            {
                BeanstalkStacks[i] = new CardStack<BeanstalkCard>($"Beanstalk Stack {i + 1}");
            }
            IsJacksTurn = true;
        }

        public PlayerTurn GetNextTurn()
        {
            PlayerTurn ret = CurrentPlayer.GetNextTurn(this);
            IsJacksTurn = !IsJacksTurn;
            return ret;
        }

        public bool IsJacksTurn
        {
            get;
            private set;
        }

        public Player CurrentPlayer => IsJacksTurn ? (Player)Jack : Giant;

        public CardStack[] CastleStacks
        {
            get;
            private set;
        }

        public CardStack DiscardPile
        {
            get;
            private set;
        }

        public const int RequiredBeanstalkCards = 6;

        public CardStack<BeanstalkCard> ActiveBeanstalkStack => BeanstalkStacks.FirstOrDefault(x => x.Count < RequiredBeanstalkCards + 1);

        public CardStack<BeanstalkCard>[] BeanstalkStacks
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

        public ICardPositionDescriptor<Card> GetPositionDescriptorForCard(Card card)
        {
            int stackIndex = FindCastleStackIndexForCard(card);
            int cardIndex = CastleStacks[stackIndex].IndexOf(card);
            if (cardIndex == CastleStacks[stackIndex].FrontIndex)
            {
                return new StackEndCardPositionDescriptor()
                {
                    End = StackEnd.Front,
                    Stack = new CastleStackDescriptor(stackIndex)
                };
            }
            else if (cardIndex == CastleStacks[stackIndex].BackIndex)
            {
                return new StackEndCardPositionDescriptor()
                {
                    End = StackEnd.Back,
                    Stack = new CastleStackDescriptor(stackIndex)
                };
            }
            else
            {
                return new RandomCardPositionDescriptor<Card>()
                {
                    Stack = new CastleStackDescriptor(stackIndex),
                    Index = cardIndex
                };
            }
        }
    }
}
