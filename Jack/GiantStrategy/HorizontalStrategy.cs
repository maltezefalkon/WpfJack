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
            IEnumerable<IAction> winActions = GetWinActions(game);
            if (winActions.Any())
            {
                return winActions.Select(x => new Tuple<IAction, decimal>(x, 10m));
            }
            IDictionary<GiantCardType, StackEndCardPositionDescriptor> frontmostPositions = GetFrontmostPositions(game);
            IEnumerable<IGrouping<int, KeyValuePair<GiantCardType, StackEndCardPositionDescriptor>>> groupedByStackIndex = frontmostPositions.GroupBy(x => x.Value.GetCastleStackIndex(game).Value);
            int stackCount = groupedByStackIndex.Count();
            if (stackCount == 3)
            {
                IGrouping<int, KeyValuePair<GiantCardType, StackEndCardPositionDescriptor>> group = groupedByStackIndex.Where(x => x.Count() > 1).First();
                KeyValuePair<GiantCardType, StackEndCardPositionDescriptor> buried = group.OrderBy(x => x.Value.GetCardIndex(game)).First();
                Card altCard = game.CardsInPlay.Single(x => x.SubType == buried.Value.PeekCard(game).SubType && x != buried.Value.PeekCard(game));
                StackEndCardPositionDescriptor altPosition = game.GetPositionDescriptorForCard(altCard);
                if (!groupedByStackIndex.Select(x => x.Key).Contains(altPosition.GetCastleStackIndex(game).Value))
                {
                    return GetUnburyActionTuples(game, altPosition);
                }
            }
            int max = frontmostPositions.Max(x => x.Value.GetCardIndex(game));
            StackEndCardPositionDescriptor targetPosition = frontmostPositions.First(x => x.Value.GetCardIndex(game) == max).Value;
            return GetUnburyActionTuples(game, targetPosition);
        }

        private IEnumerable<IAction> GetWinActions(Game game)
        {
            IEnumerable<Tuple<IAction, IAction>> actions = GeneratePossibleSnatchActions(game);
            foreach (Tuple<IAction, IAction> tuple in actions)
            {
                if (TestActionForWin(game, tuple.Item1))
                {
                    yield return tuple.Item1;
                    throw new Exception();
                }
                if (TestActionForWin(game, tuple.Item1, tuple.Item2))
                {
                    yield return tuple.Item1;
                    yield return tuple.Item2;
                }
            }
        }

        public decimal ScoreAction(Game game, params IAction[] actions)
        {
            decimal ret = 0m;
            foreach (IAction action in actions)
            {
                action.ExecuteCore(game);
            }
            bool win = (null != game.GetWinCondition());
            if (win)
            {
                ret = 100m;
            }
            else
            {
                ret = 1 - (GetFrontmostPositions(game).Average(x => (decimal)x.Value.Offset) * 0.2m);
            }
            foreach (IAction action in actions)
            {
                action.UndoCore(game);
            }
            return ret;
        }

        private bool TestActionForWin(Game game, params IAction[] actions)
        {
            foreach (IAction action in actions)
            {
                action.ExecuteCore(game);
            }
            bool ret = (null != game.GetWinCondition());
            foreach (IAction action in actions)
            {
                action.UndoCore(game);
            }
            return ret;
        }

        private IEnumerable<Tuple<IAction, IAction>> GeneratePossibleSnatchActions(Game game)
        {
            foreach (IAction firstShift in GeneratePossibleShifts(game))
            {
                foreach (IAction secondShift in GeneratePossibleShifts(game))
                {
                    yield return new Tuple<IAction, IAction>(firstShift, secondShift);
                }
            }
        }

        private IEnumerable<IAction> GeneratePossibleShifts(Game game)
        {
            foreach (StackEndCardPositionDescriptor sourcePos in GeneratePositions(game, StackEnd.Front))
            {
                foreach (StackEndCardPositionDescriptor destPos in GeneratePositions(game, StackEnd.Front).Where(x => x.GetCastleStackIndex(game) != sourcePos.GetCastleStackIndex(game)))
                {
                    yield return new GiantSnatchAction()
                    {
                        SourceCardPosition = sourcePos,
                        DestinationCardPosition = destPos
                    };
                }
            }
        }

        private IEnumerable<StackEndCardPositionDescriptor> GeneratePositions(Game game, StackEnd end, int v = 1)
        {
            for (int i = 0; i < v; i++)
            {
                for (int j = 0; j < game.CastleStacks.Length; j++)
                {
                    if (game.CastleStacks[j].Count > i)
                    {
                        yield return new StackEndCardPositionDescriptor()
                        {
                            End = end,
                            Offset = i,
                            Stack = new CastleStackDescriptor(j)
                        };
                    }
                }
            }
        }

        protected virtual IDictionary<GiantCardType, StackEndCardPositionDescriptor> GetFrontmostPositions(Game game)
        {
            IEnumerable<StackEndCardPositionDescriptor> giantCardPositons = game.CardsInPlay.OfType<GiantCard>().Select(x => game.GetPositionDescriptorForCard(x, StackEnd.Front));
            return giantCardPositons.GroupBy(x => ((GiantCard)x.PeekCard(game)).GiantCardType).Select(x => new { GiantCardType = x.Key, FrontmostPosition = x.OrderBy(y => y.Offset).First() }).ToDictionary(x => x.GiantCardType, y => y.FrontmostPosition);
        }
    }
}

