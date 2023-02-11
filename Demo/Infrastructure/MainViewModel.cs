using Demo.Infrastructure;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using TreeBreadcrumbControl;
using TreeBreadcrumbControl.Commands;

namespace Demo
{
    public class ObjectProperty : Property<INode>
    {
        public ObjectProperty(ICommand setCommand)
        {
            SetCommand = setCommand;
        }

        public ICommand? SetCommand { get; protected set; }
    }


    public class MainViewModel : IObserver
    {
        List<Property> properties = new();

        IDisposable disposable = null;

        readonly ObjectProperty objectProperty;
        readonly Property<Exception> exceptionProperty;
        readonly Property<Collection> fileSystemInfosProperty;      


        public MainViewModel()
        {
            objectProperty = new(new RelayCommand<Node>(node => SetCurrentNodeAsync(node))) { GridRow = 0 };
            exceptionProperty = new() { GridRow = 2 };
            fileSystemInfosProperty = new(new()) { GridRow = 4 };

            properties.Add(objectProperty);
            properties.Add(exceptionProperty);
            properties.Add(fileSystemInfosProperty);

            OpenDirectoryCommand = new RelayCommand<DirectoryInfo>(async info =>
            {
                var node = (Node)objectProperty.GetValue().Children.OfType<INode>().First(item => item.Content == info);
                SetCurrentNodeAsync(node);
            });

#pragma warning disable 4014
            InitializeCurrentNodeAsync();
#pragma warning restore 4014
        }

        public IEnumerable Properties => properties;

        public ICommand OpenDirectoryCommand { get; }


        private void InitializeCurrentNodeAsync()
        {
            SetCurrentNodeAsync(new DirectoryNode(@"C:\"));
        }

        private void SetCurrentNodeAsync(Node node)
        {
            try
            {
                Reset();
                disposable = node.Children.Subscribe(this);
            }
            catch (Exception e)
            {
                exceptionProperty.SetValue(e);
                //fileSystemInfosProperty.GetValue().Clear();
            }

            void Reset()
            {
                fileSystemInfosProperty.GetValue().Clear();
                exceptionProperty.SetValue(null);
                objectProperty.SetValue(node);
                disposable?.Dispose();
            }
        }

        public void OnNext(object change)
        {
            if (change is Change { Type: ChangeType.Insert, Value: Node value })
            {
                fileSystemInfosProperty.GetValue().Add(value.Content);
            }
        }


        public void OnCompleted()
        {
            foreach (var fileInfo in Directory.GetFiles((objectProperty.GetValue().Content as FileSystemInfo).FullName)
                    .Select(item => new FileInfo(item)))
            {
                fileSystemInfosProperty.GetValue().Add(fileInfo);
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
