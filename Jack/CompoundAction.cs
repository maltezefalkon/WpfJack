using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public class CompoundAction : IAction
    {
        public CompoundAction(params IAction[] actions)
        {
            Components = new List<IAction>(actions);
        }

        public List<IAction> Components
        {
            get;
            private set;
        }

        public void Execute(Game game)
        {
            foreach (IAction action in Components)
            {
                action.Execute(game);
            }
        }

        public bool IsValid(Game game)
        {
            foreach (IAction action in Components)
            {
                if (!action.IsValid(game))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
