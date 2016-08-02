using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public interface ICardPositionDescriptor<out T> where T : Card
    {
        IStackDescriptor<T> Stack { get; }
        T PeekCard(Game game);
        T PluckCard(Game game);
        void PutCard(Game game, Card card);
        bool IsValid(Game game);
        int GetCardIndex(Game game);
    }
}
