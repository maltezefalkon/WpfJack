using Jack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.UI;

namespace WpfApplication1
{
    public class ViewModel
    {
        public ViewModel(UIParameterManager mgr, Game game)
        {
            UIParameterManager = mgr;
            Game = game;
        }

        public Game Game
        {
            get;
            private set;
        }

        public UIParameterManager UIParameterManager
        {
            get;
            private set;
        }
    }
}
