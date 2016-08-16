using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
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
}
