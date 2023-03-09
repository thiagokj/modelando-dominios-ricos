using Flunt.Validations;
using PaymentsContext.Shared.ValueObjects;

namespace PaymentsContext.Domain.ValueObjects;

public class Email : ValueObject
{
    public Email(string address)
    {
        Address = address;

        AddNotifications(new Contract<Email>()
            .Requires()
            .IsEmail(Address, "Email.Address", "E-mail inválido")
        );
    }

    public string Address { get; private set; }
}