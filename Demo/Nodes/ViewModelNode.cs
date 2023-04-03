﻿using Demo.Infrastructure;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Observables;

namespace Demo.Templates.Infrastructure
{
    public class ViewModelNode : Node
    {
        private Type type;
        bool flag;

        public ViewModelNode(Type type)
        {
            this.type = type;
        }

        public override object Content => Activator.CreateInstance(type);

        public override IObservable Properties => new Collection();

        public override async Task<object?> GetChildren()
        {
            flag = true;
            object? xx = await Task.Run(() => Resolver.Instance.Children(type));
            return xx;
        }

        public override string ToString()
        {
            return type.Name;
        }

        public override Node ToNode(object value)
        {
            return new ViewModelNode(value as Type);
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }

        public override Task<object> GetProperties()
        {
            return Task.FromResult(new object());
        }

    }
}
