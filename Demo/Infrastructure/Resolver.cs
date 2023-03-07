using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Trees;

namespace Demo.Infrastructure
{
    public class Resolver
    {
        public Tree<Type> tree = new Tree<Type>(typeof(TopViewModel));

        public Resolver()
        {
            Register<TopViewModel, BreadcrumbsViewModel>();
            Register<BreadcrumbsViewModel, RootViewModel>();
            Register<BreadcrumbsViewModel, DescendantsViewModel>();
        }

        //public object Resolve(Type type)
        //{
        //    return Activator.CreateInstance(type);
        //}

        public IEnumerable Children<T>()
        {
            return Children(typeof(T));
        }

        public IEnumerable Children(Type type)
        {
            if (tree.Match(type) is var branch)
                return branch.Items.Select(a => a.Data).ToArray();
            return Array.Empty<Type>();
        }

        public void Register<TParent, TChild>()
        {
            tree[typeof(TParent)].Add(typeof(TChild));
        }

        public void Register(Type parent, Type child)
        {
            tree[parent].Add(child);
        }

        public static Resolver Instance { get; } = new Resolver();
    }
    public class TopViewModel
    {

    }

    public class BreadcrumbsViewModel
    {

    }
    public class RootViewModel
    {

    }
    public class DescendantsViewModel
    {

    }
}
