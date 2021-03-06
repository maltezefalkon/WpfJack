﻿using System;
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

        public override IEnumerable<IStrategy> GetStrategies(Game game)
        {
            throw new NotImplementedException();
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

        protected IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game)
        {
            Game.ActiveBeanstalkStackStats stats = game.GetActiveBeanstalkMinMax();
            IEnumerable<StackEndCardPositionDescriptor> buildableCards = GetBuildableCards(game)
                .Select(x => game.GetPositionDescriptorForCard(x));
            if (!buildableCards.Any())
            {
                throw new Exception("No buildable card found");
            }
            var scores = buildableCards.Select(x => new { Descriptor = x, Score = 50 / (x.Offset + 1 + ((BuildableCard)x.PeekCard(game)).Value) - stats.Minimum });
            StackEndCardPositionDescriptor targetBuildCard = scores.First(x => x.Score == scores.Max(y => y.Score)).Descriptor;
            if (targetBuildCard.Offset == 0)
            {
                yield return new Tuple<IAction, decimal>(
                    new JackShiftAction()
                    {
                        SourceCardPosition = targetBuildCard,
                        DestinationCardPosition = new StackEndCardPositionDescriptor()
                        {
                            End = StackEnd.Front,
                            Stack = new ActiveBeanstalkStackDescriptor(),
                            Description = "Jack Building"
                        }
                    }, 1m);
            }
            else
            {
                StackEndCardPositionDescriptor dest = new StackEndCardPositionDescriptor();
                if (targetBuildCard.End == StackEnd.Back)
                {
                    dest.End = StackEnd.Front;
                    dest.Offset = 0;
                    dest.Stack = targetBuildCard.Stack;
                    dest.Description = "Back to front";
                }
                else
                {
                    StackEndCardPositionDescriptor giant = FindFrontmostGiantCard(game);
                    if (giant.End == StackEnd.Front && giant.Offset == 0 && giant.Stack.GetStack(game) != targetBuildCard.Stack.GetStack(game))
                    {
                        dest = new StackEndCardPositionDescriptor()
                        {
                            End = StackEnd.Front,
                            Offset = 0,
                            Stack = giant.Stack,
                            Description = "Trying to cover a giant card"
                        };
                    }
                    else
                    {
                        Random r = new Random();
                        int destStack = r.Next(0, game.CastleStacks.Length - 2);
                        if (destStack >= giant.Stack.GetStack(game).Index)
                        {
                            destStack++;
                        }
                        dest = new StackEndCardPositionDescriptor()
                        {
                            End = StackEnd.Front,
                            Offset = 0,
                            Stack = new CastleStackDescriptor(destStack),
                            Description = "Picked a random destination"
                        };
                    }
                }
                if (dest != null)
                {
                    yield return new Tuple<IAction, decimal>(
                        new JackShiftAction()
                        {
                            SourceCardPosition = new StackEndCardPositionDescriptor()
                            {
                                End = targetBuildCard.End,
                                Offset = 0,
                                Stack = targetBuildCard.Stack,
                                Description = $"Jack unburying {targetBuildCard}"
                            },
                            DestinationCardPosition = dest
                        }, 0.30m);
                }
            }
            //Tuple<IAction, decimal> buildActionTuple = GenerateBuildAction(game);
            //yield return buildActionTuple;
            //Tuple<IAction, decimal> shiftActionTuple = GenerateShiftAction(game);
            //yield return shiftActionTuple;
        }

        private IEnumerable<BuildableCard> GetBuildableCards(Game game)
        {
            if (game.ActiveBeanstalkStack.Count == game.RequiredBeanstalkCards)
            {
                return game.CardsInPlay.OfType<TreasureCard>().Where(x => !game.ClaimedTreasureCards.Any(y => y.TreasureCardType == x.TreasureCardType));
            }
            else
            {
                Game.ActiveBeanstalkStackStats stats = game.GetActiveBeanstalkMinMax();
                return game.CardsInPlay.OfType<BeanstalkCard>().Where(x => x.Value >= stats.Minimum && x.Value <= stats.Maximum);
            }
        }


        private StackEndCardPositionDescriptor FindFrontmostGiantCard(Game game)
        {
            return FindFirstBeanstalkCard(game, x => x.CardType == CardType.Giant && game.CastleStacks[game.FindCastleStackIndexForCard(x)].IndexOf(x) > 0)?.Item1;
        }

        private Tuple<StackEndCardPositionDescriptor, int> FindFirstBeanstalkCard(Game game, Func<Card, bool> evaluationFunction, int maxDistance = 5)
        {
            Tuple<StackEndCardPositionDescriptor, int> ret = null;
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

    }
}
