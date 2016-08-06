using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class CastleStack : CardStack<Card>
    {
        public CastleStack(int index)
            : base($"Castle stack {index}")
        {
            Index = index;
        }
    }
}
