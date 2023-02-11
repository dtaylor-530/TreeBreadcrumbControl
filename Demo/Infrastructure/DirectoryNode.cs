using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Infrastructure
{
    public class DirectoryNode : Node
    {
        public DirectoryNode(DirectoryInfo content) : base(content)
        {
        }

        public DirectoryNode(string content) : base(new DirectoryInfo(content))
        {
        }

        public override async Task<object?> GetChildren(object value)
        {
            return await Task.Run<object?>(() =>
             {
                 try
                 {
                     if (Content is DirectoryInfo directoryInfo)
                     {
                         return Directory.GetDirectories(directoryInfo.FullName)
                             .Select(item => new DirectoryInfo(item))
                             .ToArray();
                     }
                 }
                 catch (Exception e)
                 {
                     return e;
                 }

                 return new Exception("Content is not DirectoryInfo");
             });
        }

        public override string StringFormat(object value)
        {
            return (Content as DirectoryInfo).Name.Replace("\\", "").Replace("/", "");
        }

        public override Node ToNode(object value)
        {
            return new DirectoryNode(value as DirectoryInfo);
        }
    }
}
