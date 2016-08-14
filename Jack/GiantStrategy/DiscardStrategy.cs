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
            yield return new Tuple<IAction, decimal>(new GiantSmashAction()
            {
                SourceCardPosition = SelectDiscardDisrupt(game)
            }, 0.5m);
            //foreach (IEnumerable<Card> cardsToConsider in GetCardSets(game))
            //{
            //    StackEndCardPositionDescriptor dupesPosition = SelectDiscardDupes(game, cardsToConsider);
            //    if (null != dupesPosition)
            //    {
            //        yield return new Tuple<IAction, decimal>(new GiantSmashAction()
            //        {
            //            SourceCardPosition = dupesPosition
            //        }, 0.5m);
            //    }
            //    //StackEndCardPositionDescriptor highestPosition = SelectDiscardHighest(game, cardsToConsider);
            //    //if (null != highestPosition)
            //    //{
            //    //    yield return new Tuple<IAction, decimal>(new GiantSmashAction()
            //    //    {
            //    //        SourceCardPosition = highestPosition
            //    //    }, 0.5m);
            //    //    break;
            //    //}
            //}
        }

        private IEnumerable<IEnumerable<Card>> GetCardSets(Game game)
        {
            yield return GetJackPotentialTargetCards(game);
            yield return game.CardsInPlay;
        }

        public override decimal GetStrength(Game game)
        {
            return 1m;
        }
    }
}
