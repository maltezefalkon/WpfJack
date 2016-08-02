using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class RandomCardPositionDescriptor<T> : ICardPositionDescriptor<T> where T : Card
    {
        public IStackDescriptor<T> Stack
        {
            get;
            set;
        }

        public int Index
        {
            get;
            set;
        }

        public T PeekCard(Game game)
        {
            return Stack.GetStack(game)[Index];
        }

        public T PluckCard(Game game)
        {
            T ret = PeekCard(game);
            Stack.GetStack(game).Remove(ret);
            return ret;
        }

        public int GetCardIndex(Game game)
        {
            return Index;
        }

        public bool IsValid(Game game)
        {
            return Stack.GetStack(game).Count > Index;
        }

        public void PutCard(Game game, Card card)
        {
            Stack.GetStack(game).Insert(Index, card);
        }
    }
}
