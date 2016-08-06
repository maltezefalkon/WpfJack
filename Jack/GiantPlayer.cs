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
            return new GiantPlayerTurn(this);
        }

        public override string ToString()
        {
            return "Giant";
        }

    }

    public class GiantPlayerTurn : PlayerTurn
    {
        public GiantPlayerTurn(GiantPlayer player)
            : base(player)
        { }

        public override IEnumerable<IAction> GetActions(Game game)
        {
            ICardPositionDescriptor<Card> position = GetCardPositionToDiscard(game);
            yield return new GiantSmashAction()
            {
                SourceCardPosition = GetCardPositionToDiscard(game),
            };
        }

        private ICardPositionDescriptor<Card> GetCardPositionToDiscard(Game game)
        {
            int maxValue = game.CardsInPlay.OfType<BeanstalkCard>().Max(x => x.Value);
            IEnumerable<BeanstalkCard> highestValues = game.CardsInPlay.OfType<BeanstalkCard>().Where(x => x.Value == maxValue);
            BeanstalkCard toDiscard = highestValues.First();
            return game.GetPositionDescriptorForCard(toDiscard);
        }
    }
}
