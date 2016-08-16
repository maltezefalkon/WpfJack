using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class BeanstalkCard : BuildableCard
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
}
