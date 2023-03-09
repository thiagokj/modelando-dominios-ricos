using Flunt.Validations;
using PaymentsContext.Shared.ValueObjects;

namespace PaymentsContext.Domain.ValueObjects;

public class Name : ValueObject
{
    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;

        AddNotifications(new Contract<Name>()
            .Requires()
            .IsGreaterOrEqualsThan(FirstName, 3, "Name.FirstName",
             "O nome deve conter 3 caracteres ou mais.")
            .IsGreaterOrEqualsThan(LastName, 3, "Name.LastName",
             "O sobrenome deve conter 3 caracteres ou mais.")
            .IsLowerOrEqualsThan(FirstName, 40, "Name.FirstName",
             "O nome deve conter até 40 caracteres")
        );
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}