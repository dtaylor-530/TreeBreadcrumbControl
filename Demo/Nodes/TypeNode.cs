using Demo.Templates.Infrastructure;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Demo.Infrastructure
{
    public enum NodeType
    {
        ViewModel,
        Directory
    }


    public class TypeNode : Node
    {
        bool flag;

        public TypeNode()
        {

        }

        public override string Content => nameof(NodeType);

        public override IEnumerable Properties
        {
            get
            {
                yield break;
            }
        }

        public override async Task<object?> GetChildren()
        {
            flag = true;
            return await Task.Run<object?>(() =>
            {

                return Enum.GetValues(typeof(NodeType));
            });
        }


        public override string ToString()
        {
            return Content;
        }

        public override Node ToNode(object value)
        {
            if (value is NodeType nodeType)
                return nodeType switch
                {
                    NodeType.ViewModel => new ViewModelNode(typeof(TopViewModel)),
                    NodeType.Directory => new DirectoryNode(@"C:\"),
                    _ => throw new Exception("r 4333"),
                };
            throw new Exception("2r 11 4333");
        }

        public override Task<bool> HasMoreChildren()
        {
            return Task.FromResult(flag == false);
        }
    }
}
