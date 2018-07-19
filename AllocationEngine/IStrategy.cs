using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllocationEngine
{
    public interface IStrategy
    {
        void SetStrategyName(string strategyName);

        Dictionary<string, int> ExecuteStrategy(double CurrentCash, Dictionary<string, int> CurrentPostions, Dictionary<string, double> BuyPrice, Dictionary<string, List<double>> CurrentPrice);
    }
}
