using Flunt.Validations;
using PaymentsContext.Shared.ValueObjects;

namespace PaymentsContext.Domain.ValueObjects;

public class Address : ValueObject
{
    public Address(
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string country,
        string zipCode)
    {
        Street = street;
        Number = number;
        Neighborhood = neighborhood;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;

        AddNotifications(new Contract<Address>()
            .Requires()
            .IsGreaterOrEqualsThan(Street, 3, "A rua deve conter pelo menos 3 caracteres")
            .IsGreaterOrEqualsThan(Number, 1, "A número deve conter pelo menos 1 caracter")
            .IsGreaterOrEqualsThan(Neighborhood, 3, "O bairro deve conter pelo menos 3 caracteres")
            .IsGreaterOrEqualsThan(City, 3, "A cidade deve conter pelo menos 3 caracteres")
            .IsGreaterOrEqualsThan(State, 2, "O estado deve conter pelo menos 2 caracteres")
            .IsGreaterOrEqualsThan(Country, 2, "O país deve conter pelo menos 2 caracteres")
            .IsGreaterOrEqualsThan(ZipCode, 8, "CEP inválido")
        );

    }

    public string Street { get; private set; }
    public string Number { get; private set; }
    public string Neighborhood { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string ZipCode { get; private set; }
}
