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

        public bool IsExecuted
        {
            get;
            private set;
        }

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
            game.BeginAction(this);
            for (int i = 0; i < NumberOfCardsAffected; i++)
            {
                Card movedCard = SourceCardPosition.PluckCard(game, NumberOfCardsAffected);
                DestinationCardPosition.PutCard(game, movedCard);
            }
            game.CompleteAction(this);
            IsExecuted = true;
            game.CheckWinConditions();
        }

        public override string ToString()
        {
            return $"Shift {NumberOfCardsAffected} [{SourceCardPosition}] to [{DestinationCardPosition}]";
        }

        public virtual string ToString(Game game = null)
        {
            return $"Shift {NumberOfCardsAffected} [{SourceCardPosition} ({SourceCardPosition.PeekCard(game)})] to [{DestinationCardPosition}]";
        }
    }
}
