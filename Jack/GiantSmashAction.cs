﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class GiantSmashAction : CardShiftAction
    {
        public GiantSmashAction()
        {
            DestinationCardPosition = new StackEndCardPositionDescriptor()
            {
                End = StackEnd.Front,
                Stack = new DiscardPileStackDescriptor()
            };
        }

        public override int NumberOfCardsAffected => 1;
    }
}
