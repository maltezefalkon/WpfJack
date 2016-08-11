using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.GiantStrategy
{
    public class DiscardStrategy : BaseGiantStrategy
    {
        public override IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game)
        {
            IEnumerable<Card> cardsToConsider = game.CardsInPlay;
            yield return new Tuple<IAction, decimal>(new GiantSmashAction()
            {
                SourceCardPosition = SelectDiscardDupes(game, cardsToConsider)
            }, 0.6m);
            yield return new Tuple<IAction, decimal>(new GiantSmashAction()
            {
                SourceCardPosition = SelectDiscardHighest(game, cardsToConsider)
            }, 0.4m);
        }

        public override decimal GetStrength(Game game)
        {
            return 1m;
        }
    }
}
