using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Utility.WPF.Templates
{
    public class TemplateViewModel
    {
        public object Object { get; set; }
        public DataTemplate Template { get; set; }
    }


    public class TemplatesViewModel
    {
        private ObservableCollection<TemplateViewModel> collection = new();

        public ObservableCollection<TemplateViewModel> Collection => collection;

        public static TemplatesViewModel Instance { get; } = new TemplatesViewModel();
    }
}
