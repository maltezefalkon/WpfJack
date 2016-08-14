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
        public abstract CardSubType SubType { get; }
        public abstract int Value { get; protected set; }
        public override string ToString()
        {
            return $"{CardType} {Abbreviation}";
        }

        public virtual string Code
        {
            get
            {
                if (CardType == CardType.Beanstalk)
                {
                    return Value.ToString();
                }
                else if (CardType == CardType.Giant)
                {
                    return SubType.ToString().Substring(1, 1);
                }
                else if (CardType == CardType.Treasure)
                {
                    switch (SubType)
                    {
                        case CardSubType.Gold: return "*";
                        case CardSubType.Goose: return "&";
                        case CardSubType.Harp: return "#";
                        default: return "?";
                    }
                }
                return "?";
            }
        }
    }

    public class BeanstalkCard : ValuedCard
    {
        protected static CardSubType[] SubTypeMap = new[]
        {
            CardSubType.Unknown,
            CardSubType.Beanstalk1,
            CardSubType.Beanstalk2,
            CardSubType.Beanstalk3,
            CardSubType.Beanstalk4,
            CardSubType.Beanstalk5,
            CardSubType.Beanstalk6,
            CardSubType.Beanstalk7,
            CardSubType.Beanstalk8,
            CardSubType.Beanstalk9
        };

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

        public override CardSubType SubType
        {
            get
            {
                return SubTypeMap[Value];
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

        public override CardSubType SubType
        {
            get
            {
                switch (GiantCardType)
                {
                    case GiantCardType.Unknown: return CardSubType.Unknown;
                    case GiantCardType.Fee: return CardSubType.Fee;
                    case GiantCardType.Fie: return CardSubType.Fie;
                    case GiantCardType.Fo: return CardSubType.Foe;
                    case GiantCardType.Fum: return CardSubType.Fum;
                    default: throw new ArgumentException();
                }
            }
        }

        public override int Value
        {
            get
            {
                switch (GiantCardType)
                {
                    case GiantCardType.Unknown: throw new Exception();
                    case GiantCardType.Fee: return -1;
                    case GiantCardType.Fie: return -2;
                    case GiantCardType.Fo: return -3;
                    case GiantCardType.Fum: return -4;
                    default: throw new ArgumentException();
                }
            }
            protected set
            {

            }
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

        public override CardSubType SubType
        {
            get
            {
                switch (TreasureCardType)
                {
                    case TreasureCardType.Unknown: return CardSubType.Unknown;
                    case TreasureCardType.Harp: return CardSubType.Harp;
                    case TreasureCardType.Gold: return CardSubType.Gold;
                    case TreasureCardType.Goose: return CardSubType.Goose;
                    default: throw new ArgumentException();
                }
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

        public override int Value
        {
            get;
            protected set;
        }
    }
}
