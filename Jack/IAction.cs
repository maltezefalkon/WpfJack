using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public interface IAction
    {
        bool IsValid(Game game);
        void Execute(Game game);
        string ToString(Game game);
        bool IsExecuted { get; }
    }
}
