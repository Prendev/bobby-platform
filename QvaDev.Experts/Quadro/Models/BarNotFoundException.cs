using System;

namespace QvaDev.Experts.Quadro.Models
{
    public class BarNotFoundException : Exception
    {
        public ExpertSetWrapper Exp { get; }
        public DateTime TimeInBar { get; }
        public string Symbol { get; }

        public BarNotFoundException(ExpertSetWrapper exp, string symbol, DateTime timeInBar)
            : base($"{exp.E.Description}: BarNotFoundException({symbol}, {timeInBar})")
        {
            Symbol = symbol;
            TimeInBar = timeInBar;
            Exp = exp;
        }
    }
}
