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
                SourceCardPosition.GetCardIndex(game) < SourceCardPosition.Stack.GetStack(game).Count - NumberOfCardsAffected + 1;
        }

        public virtual void Execute(Game game)
        {
            game.BeginAction(this);
            ExecuteCore(game);
            game.CompleteAction(this);
            IsExecuted = true;
            game.CheckWinConditions();
        }

        public virtual Win Test(Game game)
        {
            ExecuteCore(game);
            Win ret = game.GetWinCondition();
            Undo(game);
            return ret;
        }

        public virtual void ExecuteCore(Game game)
        {
            for (int i = 0; i < NumberOfCardsAffected; i++)
            {
                Card movedCard = SourceCardPosition.PluckCard(game, -i);
                DestinationCardPosition.PutCard(game, movedCard);
            }
        }

        public virtual void UndoCore(Game game)
        {
            for (int i = NumberOfCardsAffected - 1; i >= 0; i--)
            {
                Card movedCard = DestinationCardPosition.PluckCard(game, -i);
                SourceCardPosition.PutCard(game, movedCard);
            }
        }

        public virtual void Undo(Game game)
        {
            game.BeginAction(this);
            ExecuteCore(game);
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
