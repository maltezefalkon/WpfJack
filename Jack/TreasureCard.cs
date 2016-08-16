using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class TreasureCard : BuildableCard
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
}
