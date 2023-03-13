using ContactsApp.Data;

namespace FullIntegrationTests;

public static class TestData
{
    public static readonly object LockObject = new ();

    public static IEnumerable<Contact> Contacts =>
        new List<Contact>
        {
            new Contact()
            {
                FirstName = "Homer",
                LastName = "Simpson",
                Phone = "1234567890",
                Street = "Evergreen Terrace",
                City = "Springfield",
                State = "IS",
                ZipCode = "742"
            }
        };
}