using System.Collections;
using Trees;
using Utility.Observables;

namespace Models
{
    public abstract class Node : INode
    {
        private bool _isRefreshing;
        protected Collection _children = new();
        protected Collection _properties = new();
        public virtual Task<object?> GetChildren() => throw new NotImplementedException();
        public virtual Task<object?> GetProperties() => throw new NotImplementedException();

        public abstract Task<bool> HasMoreChildren();
        public abstract INode ToNode(object value);

        public abstract object Content { get; }

        public INode Parent { get; set; }

        public virtual IEnumerable Ancestors
        {
            get
            {
                return GetAncestors();
            }

        }

        public virtual IObservable Children
        {
            get
            {
                _ = RefreshAsync();
                return _children;
            }
        }      
        
        public virtual IObservable Branches
        {
            get
            {
                _ = RefreshAsync();
                return _children;
            }
        }

        public virtual IObservable Leaves
        {
            get
            {
                _ = RefreshAsync();
                return _properties;
            }
        }


        private IEnumerable GetAncestors()
        {
            INode parent = this;
            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }


        protected virtual async Task<bool> RefreshAsync()
        {
            if (_isRefreshing)
                return false;

            if (await HasMoreChildren() == false)
                return false;

            _isRefreshing = true;

            try
            {
                {
                    var output = await GetChildren();
                    if (output is IEnumerable enumerable)
                        SetChildrenCache(ToNodes(enumerable).ToList());
                }
                //{
                //    var output = await GetProperties();
                //    if (output is IEnumerable enumerable)
                //        SetPropertiesCache(ToNodes(enumerable).ToList());
                //}
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _isRefreshing = false;
            }

            IEnumerable<INode> ToNodes(IEnumerable collection)
            {
                foreach (var item in collection)
                {
                    var node = ToNode(item);
                    node.Parent = this;
                    yield return node;
                }
            }

            void SetChildrenCache(List<INode> childrenCache)
            {
                _children.Clear();
                _children.AddRange(childrenCache);
                _children.Complete();
            }


            void SetPropertiesCache(List<INode> list)
            {
                _properties.Clear();
                _properties.AddRange(list);
                _properties.Complete();
            }
        }
    }

}
