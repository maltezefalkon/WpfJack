using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public abstract class CardShiftAction : IAction
    {
        public ICardPositionDescriptor<Card> SourceCardPosition { get; set; }
        public ICardPositionDescriptor<Card> DestinationCardPosition { get; set; }

        public abstract int NumberOfCardsAffected { get; }

        public virtual bool IsValid(Game game)
        {
            return
                SourceCardPosition.IsValid(game)
                &&
                DestinationCardPosition.IsValid(game)
                &&
                SourceCardPosition.Stack.GetStack(game).Count >= NumberOfCardsAffected
                &&
                SourceCardPosition.GetCardIndex(game) < SourceCardPosition.Stack.GetStack(game).Count - NumberOfCardsAffected;
        }

        public virtual void Execute(Game game)
        {
            for (int i = 0; i < NumberOfCardsAffected; i++)
            {
                Card movedCard = SourceCardPosition.PluckCard(game);
                DestinationCardPosition.PutCard(game, movedCard);
            }
            game.CheckWinConditions();
        }

        public override string ToString()
        {
            return $"{Strategy} Shift [{SourceCardPosition}] to [{DestinationCardPosition}]";
        }

        public string Strategy
        {
            get;
            set;
        }
    }
}
