using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContract
{
    public class StrategyDataContract
    {
        public double CurrentCash;
        public List<SecPosition> SecPositions;

        public StrategyDataContract()
        {
            SecPositions = new List<SecPosition>();
        }
    }
}
