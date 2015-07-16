using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.ServiceLocation
{
    public class LocatorStrategyFactory
    {
        protected IList<LocatorStrategy> Strategies { get; set; }

        public IList<LocatorStrategy> GetStrategies()
        {
            if (Strategies == null)
                Strategies = new List<LocatorStrategy>();
            return Strategies;
        }
    }
}
