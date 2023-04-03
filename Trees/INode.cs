using System;
using System.Collections;
using System.Collections.Generic;
using Utility.Observables;

namespace TreeBreadcrumbControl
{

    public interface INode
    {
        object Content { get; }

        INode Parent { get; }

        IObservable Children { get; }

        IObservable Properties { get; }

        IEnumerable Ancestors { get; }
    }
}
