﻿using System;

namespace QvaDev.Common.Integration
{
    public class Position
    {
        public long Id { get; set; }
        public string Symbol { get; set; }
        public Sides Side { get; set; }

        public double Lots { get; set; }
        public long Volume { get; set; }
        public long RealVolume { get; set; }

        public string Comment { get; set; }

        public DateTime OpenTime { get; set; }
        public double OperPrice { get; set; }

        public RetryOrder CloseOrder { get; set; }
    }
}