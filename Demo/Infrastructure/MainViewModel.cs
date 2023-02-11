using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using TreeBreadcrumbControl;
using TreeBreadcrumbControl.Commands;

namespace Demo
{
    public class ObjectProperty : Property<ITreeNode<DirectoryInfo>>
    {
        public ObjectProperty(ICommand setCommand) : base(setCommand)
        {
        }
    }




    public class MainViewModel : BindableBase
    {
        //private ITreeNode<DirectoryInfo> _currentNode;
        private ObservableCollection<FileSystemInfo> _fileSystemInfos = new();
        private Exception _exception;


        public ObjectProperty Object { get; }
        //public ITreeNode<DirectoryInfo> Object
        //{
        //    get => _currentNode;
        //    private set => SetProperty(ref _currentNode, value);
        //}

        public ObservableCollection<FileSystemInfo> FileInfos
        {
            get => _fileSystemInfos;
        }

        //public ICommand SetCurrentNodeCommand { get; }

        public ICommand OpenDirectoryCommand { get; }

        public Exception Exception
        {
            get => _exception;
            set => SetProperty(ref _exception, value);
        }

        public MainViewModel()
        {
            Object = new(new RelayCommand<LazyObservableTreeNode<DirectoryInfo>>(async node => await SetCurrentNodeAsync(node)));

            OpenDirectoryCommand = new RelayCommand<DirectoryInfo>(async info =>
            {
                var node = (LazyObservableTreeNode<DirectoryInfo>)Object.Value.Children.First(item => item.Content == info);
                await SetCurrentNodeAsync(node);
            });

#pragma warning disable 4014
            InitializeCurrentNodeAsync();
#pragma warning restore 4014
        }

        private async Task InitializeCurrentNodeAsync()
        {
            var rootNode = new LazyObservableTreeNode<DirectoryInfo>(new DirectoryInfo(@"C:\"))
            {
                ChildrenProvider = content => Task.Run(() =>
                {
                    try
                    {
                        if (content is DirectoryInfo directoryInfo)
                        {
                            return Directory.GetDirectories(directoryInfo.FullName)
                                .Select(item => new DirectoryInfo(item));
                        }
                    }
                    catch (Exception e)
                    {
                        Exception = e;
                    }

                    return null;
                }),
                StringFormat = content => content.Name.Replace("\\", "").Replace("/", "")
            };


            await SetCurrentNodeAsync(rootNode);
        }

        private async Task SetCurrentNodeAsync(LazyObservableTreeNode<DirectoryInfo> node)
        {
            Exception = null;

            await node.RefreshAsync();
            Object.Value = node;

            _fileSystemInfos.Clear();
            try
            {

                foreach (var child in node.Children)
                {
                    _fileSystemInfos.Add(child.Content);
                }
                foreach (var fileInfo in Directory.GetFiles(node.Content.FullName)
                    .Select(item => new FileInfo(item)))
                {
                    _fileSystemInfos.Add(fileInfo);
                }


            }
            catch (Exception e)
            {
                Exception = e;
                FileInfos.Clear();
            }
        }
    }
}
