using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public abstract class BuildableCard : Card
    {
        protected BuildableCard(int value)
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
