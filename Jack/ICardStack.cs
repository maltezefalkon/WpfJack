using System.Collections.Generic;

namespace Jack
{
    public interface ICardStack<out T> where T : Card
    {
        T BackCard { get; }
        int BackIndex { get; }
        T FrontCard { get; }
        int FrontIndex { get; }
        string Name { get; }
        int Count { get; }
        T this[int index] { get; }
        T GetEnd(StackEnd end);
        IEnumerable<T> GetEnds();
        int GetIndexForStackEnd(StackEnd end);
        T Pop(StackEnd end);
        void Push(StackEnd end, Card card);
        void Shuffle();
        void Swap(int x, int y);
        void Insert(int index, Card card);
        int IndexOf(Card card);
        void RemoveAt(int index);
        void Remove(Card card);
    }
}