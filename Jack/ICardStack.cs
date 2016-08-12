using System.Collections.Generic;

namespace Jack
{
    public interface ICardStack<out T> : IEnumerable<T> where T : Card 
    {
        T BackCard { get; }
        int BackIndex { get; }
        T FrontCard { get; }
        int FrontIndex { get; }
        string Name { get; }
        int Count { get; }
        int Index { get; set; }

        T this[int index] { get; }
        T GetEnd(StackEnd end, int offset);
        IEnumerable<T> GetEnds(int count = 1);
        int GetIndexForStackEnd(StackEnd end, int offset = 0);
        T Pop(StackEnd end, int offset = 0);
        void Push(Card card, StackEnd end, int offset = 0);
        void Shuffle();
        void Swap(int x, int y);
        void Insert(int index, Card card);
        int IndexOf(Card card);
        void RemoveAt(int index);
        void Remove(Card card);
    }
}