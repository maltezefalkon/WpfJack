using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack
{
    public abstract class PlayerTurn
    {
        public PlayerTurn(Player actingPlayer)
        {
            ActingPlayer = actingPlayer;
        }

        public abstract IEnumerable<IAction> GetActions(Game game);

        protected virtual IAction SelectAction(Game game, IEnumerable<Tuple<IAction, int>> possibleActions)
        {
            IEnumerable<Tuple<IAction, int>> possibilities = possibleActions.Where(x => x != null && x.Item2 > 0);
            int total = possibilities.Sum(x => x.Item2);
            Random r = new Random();
            int choice = r.Next(total) + 1;
            foreach (Tuple<IAction, int> tuple in possibilities)
            {
                if (tuple.Item2 >= choice)
                {
                    return tuple.Item1;
                }
                else
                {
                    choice -= tuple.Item2;
                }
            }
            throw new Exception("Failed to select action");
        }

        public Player ActingPlayer
        {
            get;
            private set;
        }
    }
}
