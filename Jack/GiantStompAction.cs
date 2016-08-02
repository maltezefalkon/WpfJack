using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class GiantStompAction : CardShiftAction
    {
        public override int NumberOfCardsAffected
        {
            get
            {
                return 4;
            }
        }
    }
}
