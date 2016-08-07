using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class CardStack<T> : List<T>, ICardStack<T> where T : Card
    {
        public CardStack()
            : this(Guid.NewGuid().ToString())
        {
        }

        public CardStack(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        public T GetEnd(StackEnd end, int offset)
        {
            int index = GetIndexForStackEnd(end, offset);
            return this[index];
        }

        public T FrontCard => FrontIndex == -1 ? null : this[FrontIndex];

        public T BackCard => BackIndex == -1 ? null : this[BackIndex];

        public void Push(Card card, StackEnd end, int offset = 0)
        {
            if (end == StackEnd.Front)
            {
                Add((T)card);
            }
            else
            {
                Insert(GetIndexForStackEnd(end, offset), card);
            }
        }

        public int FrontIndex => Count - 1;

        public int BackIndex => 0;

        public int Index
        {
            get;
            set;
        }

        public int GetIndexForStackEnd(StackEnd end, int offset = 0)
        {
            switch (end)
            {
                case StackEnd.Back:
                    return BackIndex + offset;
                case StackEnd.Front:
                    return FrontIndex - offset;
                default:
                    throw new Exception();
            }
        }

        public T Pop(StackEnd end, int offset = 0)
        {
            int idx = GetIndexForStackEnd(end, offset);
            T ret = this[idx];
            RemoveAt(idx);
            return ret;
        }

        public void Shuffle()
        {
            Random r = new Random();
            const int shuffles = 100000;
            for (int i = 0; i < shuffles; i++)
            {
                int x = (int)(r.NextDouble() * Count);
                int y = (int)(r.NextDouble() * Count);
                Swap(x, y);
            }
        }

        public void Swap(int x, int y)
        {
            T c = this[x];
            this[x] = this[y];
            this[y] = c;
        }

        public IEnumerable<T> GetEnds()
        {
            yield return FrontCard;
            yield return BackCard;
        }

        public void Insert(int index, Card card)
        {
            base.Insert(index, (T)card);
        }

        public int IndexOf(Card card)
        {
            return base.IndexOf((T)card);
        }

        public void Remove(Card card)
        {
            base.Remove((T)card);
        }
    }
}
