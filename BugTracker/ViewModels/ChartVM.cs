using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class ChartVM
    {
        public List<ChartData> Data { get; set; }

        public ChartVM()
        {
            this.Data = new List<ChartData>();
        }
    }

    public class ChartData
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}