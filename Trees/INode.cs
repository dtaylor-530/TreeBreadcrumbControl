using System;
using System.Collections;
using System.Collections.Generic;
using Utility.Observables;

namespace Trees
{

    public interface INode
    {
        object Content { get; }

        INode Parent { get; set; }

        IObservable Children { get; }

        IObservable Leaves { get; }

        IObservable Branches { get; }

        IEnumerable Ancestors { get; }
    }
}
