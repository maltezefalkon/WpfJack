﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public interface IStackDescriptor<out T> where T : Card
    {
        CardStack<T> GetStack(Game game);
        bool IsValid(Game game);
    }

    public class CastleStackDescriptor : IStackDescriptor<Card>
    {
        public CastleStackDescriptor(int stackIndex)
        {
            CastleStackIndex = stackIndex;
        }

        public int CastleStackIndex
        {
            get;
            private set;
        }

        public CardStack<Card> GetStack(Game game)
        {
            return game.CastleStacks[CastleStackIndex];
        }

        public bool IsValid(Game game)
        {
            return game.CastleStacks.Length > CastleStackIndex;
        }
    }

    public class DiscardPileStackDescriptor : IStackDescriptor<Card>
    {
        public CardStack<Card> GetStack(Game game)
        {
            return game.DiscardPile;
        }

        public bool IsValid(Game game)
        {
            return game.DiscardPile != null;
        }
    }

    public class BeanstalkStackDescriptor : IStackDescriptor<BeanstalkCard>
    {
        public CardStack<BeanstalkCard> GetStack(Game game)
        {
            return game.ActiveBeanstalkStack;
        }

        public bool IsValid(Game game)
        {
            throw new NotImplementedException();
        }
    }

}
