﻿using ESAPIInfo.Structures;
using LazyOptimizer.UI.ViewModels;
using LazyOptimizerDataService.DBModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace LazyOptimizer.Model
{
    public interface IPlanCachedModel : IPlanBaseModel
    {
        DateTime CreationDate { get; }
        string Description { get; set; }
        long? SelectionFrequency { get; set; }
    }
}
