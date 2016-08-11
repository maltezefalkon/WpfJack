using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.GiantStrategy
{
    public class HorizontalStrategy : BaseGiantStrategy
    {
        public override decimal GetStrength(Game game)
        {
            IDictionary<GiantCardType, StackEndCardPositionDescriptor> frontmostPositions = GetFrontmostPositions(game);
            return (20 - RoundTurns(frontmostPositions.Sum(x => GetNumberOfTurnsToUnburyCard(x.Value))) + 1) / 20m;
        }

        public override IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game)
        {
            IDictionary<GiantCardType, StackEndCardPositionDescriptor> frontmostPositions = GetFrontmostPositions(game);
            int max = frontmostPositions.Max(x => x.Value.Offset);
            StackEndCardPositionDescriptor targetPosition = frontmostPositions.First(x => x.Value.Offset == max).Value;
            return GetUnburyActionTuples(game, targetPosition);
        }

        protected virtual IDictionary<GiantCardType, StackEndCardPositionDescriptor> GetFrontmostPositions(Game game)
        {
            IEnumerable<StackEndCardPositionDescriptor> giantCardPositons = game.CardsInPlay.OfType<GiantCard>().Select(x => game.GetPositionDescriptorForCard(x, StackEnd.Front));
            return giantCardPositons.GroupBy(x => ((GiantCard)x.PeekCard(game)).GiantCardType).Select(x => new { GiantCardType = x.Key, FrontmostPosition = x.OrderBy(y => y.Offset).First() }).ToDictionary(x => x.GiantCardType, y => y.FrontmostPosition);
        }
    }
}
