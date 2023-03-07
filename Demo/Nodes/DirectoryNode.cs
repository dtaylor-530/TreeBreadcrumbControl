using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Infrastructure
{
    public class DirectoryNode : Node
    {
        private readonly Lazy<DirectoryInfo> lazyContent;
        private readonly string path;
        string[] children;

        public DirectoryNode(string path)
        {
            lazyContent = new Lazy<DirectoryInfo>(() => new(path));
            this.path = path;
            Task.Run(GetChildren);
        }

        public override DirectoryInfo Content => lazyContent.Value;

        public override IEnumerable Properties
        {
            get
            {
                foreach (var fileInfo in Directory.GetFiles(Content.FullName).Select(item => new FileInfo(item)))
                {
                    yield return fileInfo;
                }
            }
        }

        public override async Task<bool> HasMoreChildren()
        {
            if (await getchildren() is string[] children)
                return children.Equals(this.children) != true;
            return false;
        }

        public override async Task<object?> GetChildren()
        {
            if (children == null)
            {
                var result = await getchildren();
                if (result is string[] children)
                    this.children = children;
                return result;
            }
            return children;
        }


        public override string ToString()
        {
            return path.EndsWith("\\")? path.Replace("/", "\\").Remove(path.Length-1).Split("\\").Last() : path.Replace("/", "\\").Split("\\").Last();
        }

        public override Node ToNode(object value)
        {
            return new DirectoryNode(value as string);
        }

        public async Task<object?> getchildren()
        {
            return await Task.Run<object?>(() =>
            {
                try
                {
                    return Directory.GetDirectories(path).ToArray();
                }
                catch (Exception e)
                {
                    return e;
                }
            });
        }

    }
}
