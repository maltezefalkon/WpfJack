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
            CardType want = GetCardTypeWanted(game);
            IAction action = GetWantedCard(game, want);
            return new PlayerTurn(this)
            {
                Actions =
                {
                    action
                }
            };
        }

        private IAction GetWantedCard(Game game, CardType type)
        {
            if (type == CardType.Beanstalk)
            {
                Card card = Get(game);
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
            throw new Exception();
        }

        private Card Get(Game game)
        {
            int minimumValue = game.ActiveBeanstalkStack.LastOrDefault()?.Value ?? 1;
            for (int i = minimumValue; i <= BeanstalkCard.MaximumValue; i++)
            {
                foreach (CardStack stack in game.CastleStacks)
                {
                    foreach (Card card in stack.GetEnds())
                    {
                        if (card.CardType == CardType.Beanstalk && ((BeanstalkCard)card).Value == i)
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
            if (game.ActiveBeanstalkStack.Count == 6)
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
