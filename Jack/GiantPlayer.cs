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

        public StackEndCardPositionDescriptor GetGiantCardToUnbury(Game game)
        {
            Dictionary<GiantCardType, StackEndCardPositionDescriptor> minOffsets = new Dictionary<GiantCardType, StackEndCardPositionDescriptor>();
            foreach (GiantCard g in game.CardsInPlay.OfType<GiantCard>())
            {
                StackEndCardPositionDescriptor pos = game.GetPositionDescriptorForCard(g, StackEnd.Front, $"Minimum offset for {g.GiantCardType}");
                if (!minOffsets.ContainsKey(g.GiantCardType) || pos.Offset < minOffsets[g.GiantCardType].Offset)
                {
                    minOffsets[g.GiantCardType] = pos;
                }
            }
            int max = 0;    
            StackEndCardPositionDescriptor ret = null;
            foreach (KeyValuePair<GiantCardType, StackEndCardPositionDescriptor> pair in minOffsets)
            {
                if (pair.Value.Offset > max)
                {
                    max = pair.Value.Offset;
                    ret = pair.Value;
                }
            }
            return ret;
        }

        public virtual ICardPositionDescriptor<Card> GetCardToDiscardDupes(Game game)
        {
            var counts = game.CardsPlayed.GroupBy(x => x.Value).Select(x => new { Value = x.Key, Count = x.Count() });
            var pool = counts.Where(x => x.Count > 1 && x.Count < 4).OrderByDescending(x => x.Value);
            if (pool.Any())
            {
                int valToDiscard = FlipFlop ? pool.First().Value : pool.Last().Value;
                IEnumerable<Card> cardsIMightDiscard = game.CardsInPlay.Where(x => x.Value == valToDiscard);
                Card cardToDiscard = FlipFlop ? cardsIMightDiscard.First() : cardsIMightDiscard.Last();
                return game.GetPositionDescriptorForCard(cardToDiscard, StackEnd.Front, "Card to discard dupes");
            }
            else
            {
                return null;
            }
        }

        public virtual ICardPositionDescriptor<Card> GetCardToDiscardHighest(Game game)
        {
            int maxValue = game.CardsInPlay.OfType<BeanstalkCard>().Max(x => x.Value);
            FlipFlop = !FlipFlop;
            if (FlipFlop)
            {
                return game.GetPositionDescriptorForCard(game.CardsInPlay.OfType<BeanstalkCard>().First(x => x.Value == maxValue), StackEnd.Front, $"Card to discard highest First");
            }
            else
            {
                return game.GetPositionDescriptorForCard(game.CardsInPlay.OfType<BeanstalkCard>().Last(x => x.Value == maxValue), StackEnd.Front, $"Card to discard highest Last");
            }
        }

        public virtual ICardPositionDescriptor<Card> GetCardToDiscardDisrupt(Game game)
        {
            IEnumerable<ICardPositionDescriptor<Card>> potentialTargets = GetJackPotentialTargetCards(game);
            IEnumerable<BeanstalkCard> valuedCards = potentialTargets.Select(x => x.PeekCard(game)).OfType<BeanstalkCard>();
            Game.ActiveBeanstalkStackStats stats = game.GetActiveBeanstalkMinMax();
            for (int i = stats.Maximum; i >= stats.Minimum; i--)
            {
                ValuedCard toDiscard = valuedCards.FirstOrDefault(x => x.Value == i);
                if (null != toDiscard)
                {
                    return game.GetPositionDescriptorForCard(toDiscard, StackEnd.Front, "Card to discard disrupt");
                }
            }
            int max = game.CardsInPlay.OfType<BeanstalkCard>().Max(x => x.Value);
            return game.GetPositionDescriptorForCard(game.CardsInPlay.OfType<BeanstalkCard>().First(x => x.Value == max));
        }

        private IEnumerable<ICardPositionDescriptor<Card>> GetJackPotentialTargetCards(Game game)
        {
            return game.CastleStacks.Where(x => x.Any()).SelectMany(x => x.GetEnds()).Select(x => game.GetPositionDescriptorForCard(x, null, "Potential Jack Target"));
            //for (int i = 0; i < game.CastleStacks.Length; i++)
            //{
            //    if (game.CastleStacks[i].Count > 6)
            //    {
            //        for (int j = 0; j < 3; j++)
            //        {
            //            yield return new StackEndCardPositionDescriptor()
            //            {
            //                End = StackEnd.Front,
            //                Offset = j,
            //                Stack = new CastleStackDescriptor(i)
            //            };
            //            yield return new StackEndCardPositionDescriptor()
            //            {
            //                End = StackEnd.Back,
            //                Offset = j,
            //                Stack = new CastleStackDescriptor(i)
            //            };
            //        }
            //    }
            //    else
            //    {
            //        foreach (Card c in game.CastleStacks[i])
            //        {
            //            yield return game.GetPositionDescriptorForCard(c);
            //        }
            //    }
            //}
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

        public virtual IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game)
        {
            StackEndCardPositionDescriptor unburyPos = ActingPlayer.GetGiantCardToUnbury(game);
            if (unburyPos.Offset >= 4 && unburyPos.Stack.GetStack(game).Count > 4)
            {
                yield return new Tuple<IAction, decimal>(new GiantStompAction()
                {
                    SourceCardPosition = new StackEndCardPositionDescriptor()
                    {
                        End = StackEnd.Front,
                        Offset = 3,
                        Stack = unburyPos.Stack,
                        Description = $"(Source) Giant unburying {unburyPos.PeekCard(game)} @ {unburyPos}"
                    },
                    DestinationCardPosition = new StackEndCardPositionDescriptor()
                    {
                        End = StackEnd.Front,
                        Offset = 0,
                        Stack = new CastleStackDescriptor(game.CastleStacks.Where(x => !Object.ReferenceEquals(unburyPos.Stack.GetStack(game), x) && x.FrontCard?.CardType != CardType.Giant).First().Index),
                        Description = $"(Destination) Giant unburying {unburyPos.PeekCard(game)} @ {unburyPos} random destination w/o giant card"
                    }
                }, 1m);
            }
            else
            {
                ICardPositionDescriptor<Card> discardPositionHighest = ActingPlayer.GetCardToDiscardHighest(game);
                discardPositionHighest.Description = "Discard highest";
                if (discardPositionHighest != null)
                {
                    yield return new Tuple<IAction, decimal>(new GiantSmashAction()
                    {
                        SourceCardPosition = discardPositionHighest
                    }, 0.4m);
                }
                ICardPositionDescriptor<Card> discardPositionDupes = ActingPlayer.GetCardToDiscardDupes(game);
                if (discardPositionDupes != null)
                {
                    yield return new Tuple<IAction, decimal>(new GiantSmashAction()
                    {
                        SourceCardPosition = discardPositionDupes
                    }, 0.4m);
                }
                ICardPositionDescriptor<Card> discardPositionDisrupt = ActingPlayer.GetCardToDiscardDisrupt(game);
                if (discardPositionDisrupt != null)
                {
                    yield return new Tuple<IAction, decimal>(new GiantSmashAction()
                    {
                        SourceCardPosition = discardPositionDisrupt
                    }, 0.2m);
                }
            }
        }
    }
}
