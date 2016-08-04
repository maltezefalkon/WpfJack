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
            for (int i = 0; i < 3; i++)
            {
                yield return SelectAction(game, GetPossibleActions(game));
            }
        }

        protected IEnumerable<Tuple<IAction, int>> GetPossibleActions(Game game)
        {
            Tuple<IAction, int> buildActionTuple = GenerateBuildAction(game);
            yield return buildActionTuple;
            Tuple<IAction, int> shiftActionTuple = GenerateShiftAction(game);
            yield return shiftActionTuple;
        }

        private Tuple<IAction, int> GenerateBuildAction(Game game)
        {
            StackEndCardPositionDescriptor buildCardDescriptor = SelectBeanstalkCardForBuilding(game);
            if (null == buildCardDescriptor)
            {
                return null;
            }
            else
            {
                return new Tuple<IAction, int>(
                    new JackShiftAction()
                    {
                        SourceCardPosition = buildCardDescriptor,
                        DestinationCardPosition = new StackEndCardPositionDescriptor()
                        {
                            End = StackEnd.Front,
                            Stack = new ActiveBeanstalkStackDescriptor()
                        }
                    }, 50);
            }
        }

        private Tuple<IAction, int> GenerateShiftAction(Game game)
        {
            StackEndCardPositionDescriptor buildCardDescriptor = SelectBeanstalkCardForBuilding(game);
            if (null != buildCardDescriptor)
            {
                MinMax<int> minMax = GetActiveBeanstalkMinMax(game);
                BeanstalkCard c = buildCardDescriptor.PeekCard(game) as BeanstalkCard;
                if (null != c && c.Value == minMax.Minimum)
                {
                    return null;
                }
            }
            StackEndCardPositionDescriptor digCard = SelectBeanstalkCardForDigging(game);
            if (null == digCard)
            {
                return null;
            }
            else
            {
                StackEndCardPositionDescriptor dest = new StackEndCardPositionDescriptor();
                if (digCard.End == StackEnd.Back)
                {
                    dest.End = StackEnd.Front;
                    dest.Offset = 0;
                    dest.Stack = digCard.Stack;
                }
                else
                {
                    StackEndCardPositionDescriptor giant = FindFrontmostGiantCard(game);
                    dest = new StackEndCardPositionDescriptor()
                    {
                        End = StackEnd.Front,
                        Offset = 0,
                        Stack = giant.Stack
                    };
                }
                return new Tuple<IAction, int>(
                    new JackShiftAction()
                    {
                        SourceCardPosition = digCard,
                        DestinationCardPosition = dest
                    }, 30);
            }
        }

        private StackEndCardPositionDescriptor FindFrontmostGiantCard(Game game)
        {
            return FindFirstBeanstalkCard(game, x => x.CardType == CardType.Giant && game.CastleStacks[game.FindCastleStackIndexForCard(x)].IndexOf(x) > 0)?.Item1;
        }

        private Tuple<StackEndCardPositionDescriptor, int> FindFirstBeanstalkCard(Game game, Func<Card, bool> evaluationFunction, int maxDistance = 5)
        {
            Tuple<StackEndCardPositionDescriptor, int> ret = null;
            MinMax<int> minMax = GetActiveBeanstalkMinMax(game);
            for (int i = 0; i <= maxDistance; i++)
            {
                for (int j = 0; j < game.CastleStacks.Length; j++)
                {
                    if (i > game.CastleStacks[j].Count - 1)
                    {
                        continue;
                    }
                    foreach (StackEnd end in new[] { StackEnd.Front, StackEnd.Back })
                    {
                        StackEndCardPositionDescriptor desc = new StackEndCardPositionDescriptor()
                        {
                            End = end,
                            Offset = i,
                            Stack = new CastleStackDescriptor(j)
                        };
                        Card c = desc.PeekCard(game);
                        if (evaluationFunction(c))
                        {
                            ret = new Tuple<StackEndCardPositionDescriptor, int>(desc, i);
                            break;
                        }
                    }
                    if (null != ret)
                    {
                        break;
                    }
                }
                if (null != ret)
                {
                    break;
                }
            }
            return ret;
        }

        private StackEndCardPositionDescriptor SelectBeanstalkCardForBuilding(Game game)
        {
            StackEndCardPositionDescriptor ret = null;
            MinMax <int> minMax = GetActiveBeanstalkMinMax(game);
            for (int i = minMax.Minimum; i <= minMax.Maximum; i++)
            {
                Tuple<StackEndCardPositionDescriptor, int> tuple = FindFirstBeanstalkCard(game, c => EvaluateBeanstalkCard(c, b => b.Value == i), 0);
                ret = tuple?.Item1;
                if (null != ret)
                {
                    break;
                }
            }
            return ret;
        }

        private bool EvaluateBeanstalkCard(Card c, Func<BeanstalkCard, bool> evaluationFunction)
        {
            BeanstalkCard b = c as BeanstalkCard;
            if (null != b)
            {
                return evaluationFunction(b);
            }
            else
            {
                return false;
            }
        }

        private StackEndCardPositionDescriptor SelectBeanstalkCardForDigging(Game game)
        {
            MinMax<int> minMax = GetActiveBeanstalkMinMax(game);
            Tuple<StackEndCardPositionDescriptor, int> tuple = FindFirstBeanstalkCard(game, c => EvaluateBeanstalkCard(c, b => b.Value == minMax.Minimum));
            if (null != tuple && tuple.Item2 == 1)
            { 
                StackEndCardPositionDescriptor desc = new StackEndCardPositionDescriptor()
                {
                    End = tuple.Item1.End,
                    Offset = 0,
                    Stack = new CastleStackDescriptor(game.FindCastleStackIndexForCard(tuple.Item1.PeekCard(game)))
                };
                return desc;
            }
            return null;
        }

        private MinMax<int> GetActiveBeanstalkMinMax(Game game)
        {
            int minimumValue = (game.ActiveBeanstalkStack.LastOrDefault()?.Value ?? 0) + 1;
            if (game.ActiveBeanstalkStack.Count == Game.RequiredBeanstalkCards)
            {
                minimumValue = BeanstalkCard.TreasureValue;
            }
            int maximumValue = game.ActiveBeanstalkStack.Count == Game.RequiredBeanstalkCards ? TreasureCard.TreasureValue : BeanstalkCard.MaximumValue;
            return new MinMax<int>() { Minimum = minimumValue, Maximum = maximumValue };
        }
    }

    public struct MinMax<T> where T : struct
    {
        public T Minimum;
        public T Maximum;
    }
}
