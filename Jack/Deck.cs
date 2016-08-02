﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class Deck : CardStack
    {
        public Deck()
        {
            CreateBeanstalkCards();
            CreateGiantCards();
            CreateTreasureCards();
        }

        private void CreateBeanstalkCards()
        {
            const int copies = 4;
            for (int i = BeanstalkCard.MinimumValue; i <= BeanstalkCard.MaximumValue; i++)
            {
                for (int j = 0; j < copies; j++)
                {
                    Push(StackEnd.Front, new BeanstalkCard(i));
                }
            }
        }

        private void CreateGiantCards()
        {
            const int copies = 2;
            foreach (GiantCardType gct in Enum.GetValues(typeof(GiantCardType)))
            {
                if (gct != GiantCardType.Unknown)
                {
                    for (int j = 0; j < copies; j++)
                    {
                        Push(StackEnd.Front, new GiantCard(gct));
                    }
                }
            }
        }

        private void CreateTreasureCards()
        {
            const int copies = 2;
            foreach (TreasureCardType tct in Enum.GetValues(typeof(TreasureCardType)))
            {
                if (tct != TreasureCardType.Unknown)
                {
                    for (int j = 0; j < copies; j++)
                    {
                        Push(StackEnd.Front, new TreasureCard(tct));
                    }
                }
            }
        }
    }
}