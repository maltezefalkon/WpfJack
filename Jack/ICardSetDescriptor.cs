using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public interface ICardSetDescriptor : IEnumerable<ICardPositionDescriptor<Card>>
    {
        Game Game { get; }
    }
}
