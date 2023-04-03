using Abstractions;
using System;

namespace Demo.Infrastructure
{
    public class Customer2 : IGuid
    {

        public Customer2() : base()
        {
            //addresses = new ObservableCollection<Address> { new Address { Line1 = "2018 156th Avenue NE", City = "Bellevue", State = "WA", ZipCode = 98007, Country = "USA" } };
        }

        public Guid Guid => Guid.Parse("a5b0fb5d-9b75-4719-bf5a-9017f0c632a1");


        public int IntegerValue { get; set; }

        //[PropertyGridOptions(EditorDataTemplateResourceKey = "AddressListEditor", SortOrder = 10)]
        //[DisplayName("Addresses (custom editor)")]
        //[Category("Collections")]
        //public ObservableCollection<Address> Addresses
        //{
        //    get => addresses;
        //}
    }
}
