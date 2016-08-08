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
            IEnumerable<ICardPositionDescriptor<Card>> potentialTargets = GetJackPotentialTargetCards(game);
            IEnumerable<BeanstalkCard> valuedCards = potentialTargets.Select(x => x.PeekCard(game)).OfType<BeanstalkCard>();
            Game.ActiveBeanstalkStackStats stats = game.GetActiveBeanstalkMinMax();
            for (int i = stats.Maximum; i >= stats.Minimum; i--)
            {
                ValuedCard toDiscard = valuedCards.FirstOrDefault(x => x.Value == i);
                if (null != toDiscard)
                {
                    return game.GetPositionDescriptorForCard(toDiscard);
                }
            }
            int max = game.CardsInPlay.OfType<BeanstalkCard>().Max(x => x.Value);
            return game.GetPositionDescriptorForCard(game.CardsInPlay.OfType<BeanstalkCard>().First(x => x.Value == max));
        }

        private IEnumerable<ICardPositionDescriptor<Card>> GetJackPotentialTargetCards(Game game)
        {
            return game.CastleStacks.Where(x => x.Any()).SelectMany(x => x.GetEnds()).Select(x => game.GetPositionDescriptorForCard(x));
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
    }
}
