using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class GiantPlayer : Player
    {
        public GiantPlayer()
        {
        }

        public override PlayerTurn GetNextTurn(Game game)
        {
            ICardPositionDescriptor<Card> position = GetCardPositionToDiscard(game);
            return new PlayerTurn(this)
            {
                Actions =
                {
                    new GiantSmashAction()
                    {
                        SourceCardPosition = position
                    }
                }
            };
        }

        private ICardPositionDescriptor<Card> GetCardPositionToDiscard(Game game)
        {
            int maxValue = game.CardsInPlay.OfType<BeanstalkCard>().Max(x => x.Value);
            IEnumerable<BeanstalkCard> highestValues = game.CardsInPlay.OfType<BeanstalkCard>().Where(x => x.Value == maxValue);
            BeanstalkCard toDiscard = highestValues.First();
            int castleStackIndex = game.FindCastleStackIndexForCard(toDiscard);
            CastleStackDescriptor stack = new CastleStackDescriptor(castleStackIndex);
            return new RandomCardPositionDescriptor<BeanstalkCard>()
            {
                Stack = new BeanstalkStackDescriptor(),
                Index = stack.GetStack(game).IndexOf(toDiscard)
            };
        }
    }
}
