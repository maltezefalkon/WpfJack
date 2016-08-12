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
            if (index == -1)
            {
                return null;
            }
            else
            {
                return this[index];
            }
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
            int ret = -1;
            switch (end)
            {
                case StackEnd.Back:
                    ret = BackIndex + offset;
                    if (ret >= Count)
                    {
                        throw new IndexOutOfRangeException("Back index offset too large");
                    }
                    break;
                case StackEnd.Front:
                    ret = FrontIndex - offset;
                    if (ret < 0)
                    {
                        throw new IndexOutOfRangeException("Front index offset too large");
                    }
                    break;
                default:
                    throw new Exception();
            }
            return ret;
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

        public IEnumerable<T> GetEnds(int count = 1)
        {
            if (count <= 0)
            {
                yield break;
            }
            else if (Count == 1)
            {
                yield return FrontCard;
            }
            else if (Count == 2)
            {
                yield return FrontCard;
                yield return BackCard;
            }
            else if (count * 2 >= Count)
            {
                foreach (T card in this)
                {
                    yield return card;
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    yield return this[i];
                    yield return this[FrontIndex - i];
                }
            }
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
