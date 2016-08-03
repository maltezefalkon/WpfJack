using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class JackPlayer : Player
    {
        public override PlayerTurn GetNextTurn(Game game)
        {
            return new JackPlayerTurn(this);
        }

        public override string ToString()
        {
            return "Jack";
        }
    }

    public class JackPlayerTurn : PlayerTurn
    {
        public JackPlayerTurn(JackPlayer player)
            : base(player)
        {

        }

        public override IEnumerable<IAction> GetActions(Game game)
        {
            for (int i = 0; i< 3; i++)
            {
                CardType want = GetCardTypeWanted(game);
                yield return GetWantedCard(game, want);
            }
        }

        private IAction GetWantedCard(Game game, CardType type)
        {
            Card card = SelectBeanstalkCard(game);
            if (null != card)
            { 
                return new JackShiftAction()
                {
                    SourceCardPosition = game.GetPositionDescriptorForCard(card),
                    DestinationCardPosition = new StackEndCardPositionDescriptor()
                    {
                        End = StackEnd.Front,
                        Stack = new BeanstalkStackDescriptor()
                    }
                };
            }
            else
            {
                throw new Exception("no selected card");
            }
        }

        private BeanstalkCard SelectBeanstalkCard(Game game)
        {
            int minimumValue = (game.ActiveBeanstalkStack.LastOrDefault()?.Value ?? 0) + 1;
            if (game.ActiveBeanstalkStack.Count == Game.RequiredBeanstalkCards)
            {
                minimumValue = BeanstalkCard.TreasureValue;
            }
            int maximumValue = game.ActiveBeanstalkStack.Count == Game.RequiredBeanstalkCards ? BeanstalkCard.TreasureValue : BeanstalkCard.MaximumValue;
            for (int i = minimumValue; i <= maximumValue; i++)
            {
                foreach (CardStack stack in game.CastleStacks)
                {
                    foreach (BeanstalkCard card in stack.GetEnds().OfType<BeanstalkCard>())
                    {
                        if (card.Value == i)
                        {
                            return card;
                        }
                    }
                }
            }
            return null;
        }

        private CardType GetCardTypeWanted(Game game)
        {
            if (game.ActiveBeanstalkStack.Count == Game.RequiredBeanstalkCards)
            {
                return CardType.Treasure;
            }
            else
            {
                return CardType.Beanstalk;
            }
        }
    }
}
