﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    [Serializable]
    class Settings
    {
        public int TypicalBeginningOfPeriod = 1;
        public string PathToAppData = "dataset.data";
    }
}
