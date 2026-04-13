using Bogus;

namespace DataGenerator.Fakers;

public record CustomerDto(string FirstName, string LastName, string Email, string City, string State);

public class CustomerFaker : Faker<CustomerDto>
{
    public CustomerFaker()
    {
        CustomInstantiator(f => new CustomerDto(
            f.Name.FirstName(),
            f.Name.LastName(),
            f.Internet.Email(),
            f.Address.City(),
            f.Address.StateAbbr()
        ));
    }
}
