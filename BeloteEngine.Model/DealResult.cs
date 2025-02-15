using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeloteEngine.Model
{
    public class DealResult
    {
        public Dictionary<int, int> TeamPoints { get; private set; }
        public DealResult(Dictionary<int, int> teamPoints)
        {
            TeamPoints = teamPoints;
        }
    }
}
