﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    [DebuggerDisplay("{Abbreviation}")]
    public abstract class Card
    {
        public abstract CardType CardType { get; }
        public abstract string Abbreviation { get; }
    }

    public class BeanstalkCard : Card
    {
        public static int MinimumValue = 1;
        public static int MaximumValue = 9;

        public BeanstalkCard(int value)
        {
            Value = value;
        }

        public override CardType CardType
        {
            get
            {
                return CardType.Beanstalk;
            }
        }

        public override string Abbreviation
        {
            get
            {
                return Value.ToString();
            }
        }

        public int Value
        {
            get;
            private set;
        }
    }

    public class GiantCard : Card
    {
        public GiantCard(GiantCardType type)
        {
            GiantCardType = type;
        }

        public override CardType CardType
        {
            get
            {
                return CardType.Giant;
            }
        }

        public override string Abbreviation
        {
            get
            {
                return GiantCardType.ToString();
            }
        }

        public virtual GiantCardType GiantCardType
        {
            get;
            private set;
        }
    }

    public class TreasureCard : Card
    {
        public TreasureCard(TreasureCardType type)
        {
            TreasureCardType = type;
        }

        public virtual TreasureCardType TreasureCardType
        {
            get;
            private set;
        }

        public override string Abbreviation
        {
            get
            {
                return TreasureCardType.ToString();
            }
        }

        public override CardType CardType
        {
            get
            {
                return CardType.Treasure;
            }
        }
    }
}