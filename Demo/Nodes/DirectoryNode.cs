using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TreeBreadcrumbControl;
using Utility.Observables;

namespace Demo.Infrastructure
{
    public class DirectoryNode : Node
    {
        private readonly Lazy<DirectoryInfo> lazyContent;
        private readonly string path;
        private readonly SynchronizationContext context;
        private bool flag;

        public DirectoryNode(string path) : this()
        {
            lazyContent = new Lazy<DirectoryInfo>(() => new(path));
            this.path = path;
            //Task.Run(GetChildren);
            context = SynchronizationContext.Current ?? throw new Exception("er 434434");
        }

        public DirectoryNode(DirectoryInfo info):this()
        {
            lazyContent = new Lazy<DirectoryInfo>(() => info);
            this.path = info.Name;
            context = SynchronizationContext.Current ?? throw new Exception("er 434434");
        }

        Subject<FileSystemInfo> subject = new();

        private DirectoryNode()
        {
            subject.Subscribe(a =>
            {
                if (a is DirectoryInfo directoryInfo)
                    context.Post(a => { _children.Add(ToNode(a)); }, directoryInfo);
                else if (a is FileInfo fileInfo)
                    context.Post(a => { _properties.Add(fileInfo); }, a);
            });
        }

        public override DirectoryInfo Content => lazyContent.Value;

        public override async Task<bool> HasMoreChildren()
        {
            return await Task.FromResult(flag == false);
        }

        public override string ToString()
        {
            return path.EndsWith("\\") ? path.Replace("/", "\\").Remove(path.Length - 1).Split("\\").Last() : path.Replace("/", "\\").Split("\\").Last();
        }

        public override Node ToNode(object value)
        {
            if (value is string str)
                return new DirectoryNode(str) { Parent = this };
            else if (value is DirectoryInfo info)
                return new DirectoryNode(info) { Parent = this };
            throw new Exception("r 3 33");
        }


        public override IObservable Children
        {
            get
            {
      
                _ = RefreshChildrenAsync();
                return _children;
            }
        }
        public override IObservable Properties
        {
            get
            {
                _ = RefreshPropertiesAsync();
                return _properties;
            }
        }

        bool propertyflag;
        bool childrenflag;

        protected Task<bool> RefreshChildrenAsync()
        {
            if (childrenflag == false)
                childrenflag = true;
            else
                return Task.FromResult(false);
            Task.Run(() =>
            {
                try
                {
                    foreach (var directoryInfo in Directory.EnumerateDirectories(Content.FullName).Select(item => new DirectoryInfo(item)))
                    {
                        subject.OnNext(directoryInfo);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    return;
                }
                catch (Exception ex)
                {

                }
            });
            return System.Threading.Tasks.Task.FromResult(true);
        }    
        
        protected Task<bool> RefreshPropertiesAsync()
        {
            if (propertyflag == false)
                propertyflag = true;
            else
                return Task.FromResult(false);

            Task.Run(() =>
            {
                try
                {
                    foreach (var fileInfo in Directory.EnumerateFiles(Content.FullName).Select(item => new FileInfo(item)))
                    {
                        subject.OnNext(fileInfo);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    return;
                }
                catch (Exception ex)
                {

                }
             
            });
            return System.Threading.Tasks.Task.FromResult(true);
        }

    }
}
