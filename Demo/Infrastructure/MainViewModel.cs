using Demo.Infrastructure;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using TreeBreadcrumbControl;
using WPF.Commands;

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

    public class Object2Property : Property<INode>
    {
        public Object2Property()
        {
        }
    }




    public class MainViewModel : IObserver
    {
        readonly List<Property> properties = new();
        IDisposable disposable = null;

        readonly ObjectProperty objectProperty;
        readonly Object2Property object2Property;
        readonly Property<Exception> exceptionProperty;
        readonly Property<Collection> children;

        public MainViewModel()
        {
            objectProperty = new(new RelayCommand<Node>(SetCurrentNode)) { GridRow = 0 };
            object2Property = new() { GridColumn = 1, GridRowSpan = 5 };
            exceptionProperty = new() { GridRow = 2 };
            children = new(new()) { GridRow = 4 };
            properties.Add(objectProperty);
            properties.Add(object2Property);
            properties.Add(exceptionProperty);
            properties.Add(children);

            OpenDirectoryCommand = new RelayCommand<DirectoryInfo>(async info =>
            {
                var node = (Node)objectProperty.GetValue().Children.OfType<INode>().First(item => item.Content == info);
                SetCurrentNode(node);
            });

#pragma warning disable 4014
            InitializeCurrentNode();
#pragma warning restore 4014
        }

        public IEnumerable Properties => properties;

        public ICommand OpenDirectoryCommand { get; }

        private void InitializeCurrentNode()
        {

            SetCurrentNode(new TypeNode());
            // SetCurrentNodeAsync(new DirectoryNode(@"C:\"));
            //  SetCurrentNode(new ViewModelNode(typeof(MainViewModel)));
        }

        private void SetCurrentNode(Node node)
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
                children.GetValue().Clear();
                exceptionProperty.SetValue(null);
                objectProperty.SetValue(node);
                object2Property.SetValue(node);
                disposable?.Dispose();
            }
        }

        public void OnNext(object change)
        {
            if (change is Change { Type: ChangeType.Insert, Value: Node value })
            {
                children.GetValue().Add(value.Content);
            }
        }


        public void OnCompleted()
        {
            disposable.Dispose();
            foreach (var property in objectProperty.GetValue().Properties)
            {
                children.GetValue().Add(property);
            }
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }
    }
}
