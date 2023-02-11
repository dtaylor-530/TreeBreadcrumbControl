using System.Collections;
using System.Collections.Generic;

namespace TreeBreadcrumbControl
{
    public interface INode
    {
        object Content { get; }

        object Parent { get; }

        IEnumerable Children { get; }

        IEnumerable Ancestors { get; }
    }
}
