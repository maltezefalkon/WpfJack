using System;
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

    public class BeanstalkCard : ValuedCard
    {
        public static int MinimumValue = 1;
        public static int MaximumValue = 9;
        public static int TreasureValue = MaximumValue + 1;

        public BeanstalkCard(int value)
            : base(value)
        {
        }

        public override CardType CardType
        {
            get
            {
                return CardType.Beanstalk;
            }
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

    public class TreasureCard : ValuedCard
    {
        public TreasureCard(TreasureCardType type)
            : base(BeanstalkCard.MaximumValue + 1)
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

    public abstract class ValuedCard : Card
    {
        protected ValuedCard(int value)
        {
            Value = value;
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
}
