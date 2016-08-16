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

}
