using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class StackEndCardPositionDescriptor : ICardPositionDescriptor<Card>
    {
        public IStackDescriptor<Card> Stack
        {
            get;
            set;
        }

        public int Offset
        {
            get;
            set;
        }

        public StackEnd End
        {
            get;
            set;
        }

        public string Description { get; set; }

        public Card PeekCard(Game game)
        {
            return Stack.GetStack(game).GetEnd(End, Offset);
        }

        public Card PluckCard(Game game, int offset)
        {
            return Stack.GetStack(game).Pop(End, Offset + offset);
        }

        public bool IsValid(Game game)
        {
            return Stack.IsValid(game) && Stack.GetStack(game).Count > 0;
        }

        public void PutCard(Game game, Card card)
        {
            Stack.GetStack(game).Push(card, End, Offset);
        }

        public int GetCardIndex(Game game)
        {
            return Stack.GetStack(game).GetIndexForStackEnd(End, Offset);
        }

        public int? GetCastleStackIndex(Game game, Card card)
        {
            return (Stack.GetStack(game) as CastleStack)?.Index;
        }

        public override string ToString()
        {
            return $"{End}" + (Offset != 0 ? $"-{Offset}" : String.Empty) + $" of {Stack}" + (Description != null ? $" <{Description}>" : null);
        }

        public IEnumerable<StackEndCardPositionDescriptor> GetPositionsInFront(Game game)
        {
            ICardStack<Card> stack = Stack.GetStack(game);
            for (int i = GetCardIndex(game) + 1; i <= stack.FrontIndex; i++)
            {
                yield return new StackEndCardPositionDescriptor()
                {
                    Offset = i,
                    End = StackEnd.Back,
                    Description = $"Positon {i} in front of {this}"
                };
            }
        }
    }
}
