using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Templates;

namespace Utility.WPF.Templates
{
    public class TemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var template = base.SelectTemplate(item, container);
            TemplatesViewModel.Instance.Collection.Add(new TemplateViewModel { Object = item, Template = template });
            return template;
        }

        public static TemplateSelector Instance { get; } = new TemplateSelector();
    }
}
