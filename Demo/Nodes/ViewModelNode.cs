using Demo.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override IEnumerable Properties => Array.Empty<Type>();

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
    }
}
