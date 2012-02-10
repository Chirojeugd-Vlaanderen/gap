using System.Data.Services;
using System.Linq;
using Chiro.Poc.OData.DataServicesJSONP;

namespace Chiro.Poc.OData.DataServicesJSONP
{
    #region Sample data source
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class ContactsData
    {
        public IQueryable<Person> People
        {
            get
            {
                return new Person[] {
                    new Person { ID = 1, Name = "p1", Email = "p1@p1.com" },
                    new Person { ID = 2, Name = "p2", Email = "p2@p2.com" },
                    new Person { ID = 3, Name = "p3", Email = "p3@p3.com" },
                    new Person { ID = 4, Name = "p4", Email = "p4@p4.com" }
                }.AsQueryable();
            }
        }
    }
    #endregion

    [JSONPSupportBehavior]
    public class SampleService : DataService<ContactsData>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(IDataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
        }
    }
}
