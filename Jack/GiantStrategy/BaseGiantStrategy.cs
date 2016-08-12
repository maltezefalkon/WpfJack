using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.GiantStrategy
{
    public abstract class BaseGiantStrategy : IStrategy
    {
        public event EventHandler<LogEventArgs> Log;

        public abstract decimal GetStrength(Game game);

        public abstract IEnumerable<Tuple<IAction, decimal>> GetPossibleActions(Game game);

        public int RoundTurns(decimal turns)
        {
            return (int)Math.Truncate(turns + 0.5m);
        }

        public virtual decimal GetNumberOfTurnsToUnburyCard(Game game, Card c)
        {
            return GetNumberOfTurnsToUnburyCard(game.GetPositionDescriptorForCard(c, StackEnd.Front));
        }

        public virtual decimal GetNumberOfTurnsToUnburyCard(StackEndCardPositionDescriptor stackEndCardPositionDescriptor)
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

        public virtual IEnumerable<Tuple<IAction, decimal>> GetUnburyActionTuples(Game game, StackEndCardPositionDescriptor position)
        {
            if (position.Stack.GetStack(game).Count - position.GetCardIndex(game) >= 4 && position.Stack.GetStack(game).Count >= 4)
            {
                yield return new Tuple<IAction, decimal>(new GiantStompAction()
                {
                    SourceCardPosition = new StackEndCardPositionDescriptor()
                    {
                        End = StackEnd.Front,
                        Offset = 3,
                        Stack = position.Stack,
                        Description = $"(Source) Giant unburying {position.PeekCard(game)} @ {position}"
                    },
                    DestinationCardPosition = new StackEndCardPositionDescriptor()
                    {
                        End = StackEnd.Front,
                        Offset = 0,
                        Stack = new CastleStackDescriptor(game.CastleStacks.First(x => !Object.ReferenceEquals(position.Stack.GetStack(game), x) && x.FrontCard?.CardType != CardType.Giant).Index),
                        Description = $"(Destination) Giant unburying {position.PeekCard(game)} @ {position} random destination w/o giant card"
                    }
                }, 1m);
            }
            else
            {
                IEnumerable<Card> cardsInFront = position.GetPositionsInFront(game).Select(x => x.PeekCard(game));
                StackEndCardPositionDescriptor descDupes = SelectDiscardDupes(game, cardsInFront);
                if (null != descDupes)
                {
                    yield return new Tuple<IAction, decimal>(new GiantSmashAction()
                    {
                        SourceCardPosition = descDupes
                    }, 0.6m);
                }
                StackEndCardPositionDescriptor descHighest = SelectDiscardHighest(game, cardsInFront);
                if (null != descHighest)
                {
                    yield return new Tuple<IAction, decimal>(new GiantSmashAction()
                    {
                        SourceCardPosition = descHighest
                    }, 0.4m);
                }
            }
        }

        public StackEndCardPositionDescriptor SelectDiscardHighest(Game game, IEnumerable<Card> cards)
        {
            if (cards.OfType<BeanstalkCard>().Any())
            {
                int maxValue = cards.OfType<BeanstalkCard>().Max(x => x.Value);
                return game.GetPositionDescriptorForCard(game.CardsInPlay.OfType<BeanstalkCard>().First(x => x.Value == maxValue), StackEnd.Front, $"Card to discard highest First");
            }
            else
            {
                return null;
            }
        }

        public StackEndCardPositionDescriptor SelectDiscardDupes(Game game, IEnumerable<Card> cards)
        {
            var counts = game.CardsPlayed.OfType<BeanstalkCard>().GroupBy(x => x.Value).Select(x => new { Value = x.Key, Count = x.Count() });
            var pool = counts.Where(x => x.Count >= 2).OrderByDescending(x => x.Value * x.Count * x.Count);
            foreach (var p in pool)
            {
                Card c = cards.FirstOrDefault(x => x.Value == p.Value);
                if (null != c)
                {
                    return game.GetPositionDescriptorForCard(c, StackEnd.Front, "Card to discard (dupes)");
                }
            }
            return null;
        }

        public IEnumerable<ICardPositionDescriptor<Card>> GetJackPotentialTargetCards(Game game)
        {
            return game.CastleStacks.Where(x => x.Any()).SelectMany(x => x.GetEnds()).Select(x => game.GetPositionDescriptorForCard(x, null, "Potential Jack Target"));
        }

        protected virtual void OnLog(LogEventArgs args)
        {
            EventHandler<LogEventArgs> handler = Log;
            if (null != handler)
            {
                handler(this, args);
            }
        }
    }
}
