using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class GiantPlayer : Player
    {
        private static bool FlipFlop = false;

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

        public override IEnumerable<IStrategy> GetStrategies(Game game)
        {
            yield return new GiantStrategy.DiscardStrategy();
            //yield return new GiantStrategy.HorizontalStrategy();
        }

        public int GetTurnsToAchieveHorizontalVictory(Game game)
        {
            IEnumerable<StackEndCardPositionDescriptor> giantCardPositons = game.CardsInPlay.OfType<GiantCard>().Select(x => game.GetPositionDescriptorForCard(x, StackEnd.Front));
            Dictionary<GiantCardType, StackEndCardPositionDescriptor> frontmostPositions = giantCardPositons.GroupBy(x => ((GiantCard)x.PeekCard(game)).GiantCardType).Select(x => new { GiantCardType = x.Key, FrontmostPosition = x.OrderBy(y => y.Offset).First() }).ToDictionary(x => x.GiantCardType, y => y.FrontmostPosition);
            if (frontmostPositions.Values.Select(x => x.Stack.GetStack(game).Index).Distinct().Count() < frontmostPositions.Count)
            {   // horizontal win is blocked, return -1
                return -1;
            }
            else
            {
                return RoundTurns(frontmostPositions.Sum(x => GetNumberOfTurnsToUnburyCard(x.Value)));
            }
        }

        public int? GetTurnsToAchieveVerticalVictory(Game game)
        {
            Card target = GetTargetCardVerticalVictory(game);
            if (null != target)
            {
                return RoundTurns(GetNumberOfTurnsToUnburyCard(game, target));
            }
            else
            {
                return null;
            }
        }

        public Card GetTargetCardVerticalVictory(Game game)
        {
            foreach (CastleStack castleStack in game.CastleStacks)
            {
                for (int i = 0; i <= castleStack.Count - 3; i++)
                {
                    IEnumerable<GiantCard> group = castleStack.Skip(i).Take(3).OfType<GiantCard>();
                    IEnumerable<GiantCardType> distinctTypes = group.Select(x => x.GiantCardType).Distinct();
                    if (group.Count() == 3 && distinctTypes.Count() == 3)
                    {
                        List<GiantCardType> types = new List<GiantCardType>(new[] { GiantCardType.Fee, GiantCardType.Fie, GiantCardType.Fo, GiantCardType.Fum });
                        types.RemoveAll(x => distinctTypes.Contains(x));
                        GiantCardType lastType = types.Single();
                        IEnumerable<StackEndCardPositionDescriptor> giantCardPositons = game.CardsInPlay.OfType<GiantCard>().Select(x => game.GetPositionDescriptorForCard(x, StackEnd.Front));
                        Dictionary<GiantCardType, StackEndCardPositionDescriptor> frontmostPositions = giantCardPositons.GroupBy(x => ((GiantCard)x.PeekCard(game)).GiantCardType).Select(x => new { GiantCardType = x.Key, FrontmostPosition = x.OrderBy(y => y.Offset).First() }).ToDictionary(x => x.GiantCardType, y => y.FrontmostPosition);
                        return frontmostPositions[lastType].PeekCard(game);
                    }
                }
            }
            return null;
        }

        public int RoundTurns(decimal turns)
        {
            return (int)Math.Truncate(turns + 0.5m);
        }

        public decimal GetNumberOfTurnsToUnburyCard(Game game, Card c)
        {
            return GetNumberOfTurnsToUnburyCard(game.GetPositionDescriptorForCard(c, StackEnd.Front));
        }

        private decimal GetNumberOfTurnsToUnburyCard(StackEndCardPositionDescriptor stackEndCardPositionDescriptor)
        {
            decimal ret = 0m;
            if (stackEndCardPositionDescriptor.End != StackEnd.Front)
            {
                throw new ArgumentException();
            }
            int remaining = stackEndCardPositionDescriptor.Offset;
            while (remaining > 0)
            {
                if (remaining >= 4)
                {
                    remaining -= 4;
                    ret++;
                }
                else if (remaining >= 2)
                {
                    remaining -= 2;
                    ret++;
                }
                else
                {
                    remaining -= 1;
                    ret += 0.5m;
                }
            }
            return ret;
        }
    }

    public class GiantPlayerTurn : PlayerTurn
    {

        public GiantPlayerTurn(GiantPlayer player)
            : base(player)
        { }

        public new GiantPlayer ActingPlayer
        {
            get
            {
                return base.ActingPlayer as GiantPlayer;
            }
        }

        public override IEnumerable<IAction> GetActions(Game game)
        {

            yield return SelectAction(game, GetPossibleActions(game));
        }

        protected virtual IEnumerable<IStrategy> GetStrategies(Game game)
        {
            return ActingPlayer.GetStrategies(game);
        }

        public virtual IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game)
        {
            List<Tuple<IAction, decimal>> ret = new List<Tuple<IAction, decimal>>();
            foreach (IStrategy strat in GetStrategies(game))
            {
                decimal strength = strat.GetStrength(game);
                foreach (Tuple<IAction, decimal> possibleAction in strat.GetPossibleActions(game))
                {
                    ret.Add(new Tuple<IAction, decimal>(possibleAction.Item1, possibleAction.Item2 * strength));
                }
            }
            return ret;
        }
    }
}
