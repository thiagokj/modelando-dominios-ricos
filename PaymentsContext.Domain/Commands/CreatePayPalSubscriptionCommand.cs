using Flunt.Notifications;
using Flunt.Validations;
using PaymentsContext.Domain.Enums;
using PaymentsContext.Shared.Commands;

namespace PaymentsContext.Domain.Commands;

public class CreatePayPalSubscriptionCommand : Notifiable<Notification>, ICommand
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }

    public string TransactionCode { get; set; }

    public string PaymentNumber { get; set; }
    public DateTime PaidDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPaid { get; set; }
    public string Payer { get; set; }
    public string PayerDocument { get; set; }
    public EDocumentType PayerDocumentType { get; set; }
    public string PayerEmail { get; set; }

    public string Street { get; set; }
    public string AddressNumber { get; set; }
    public string Neighborhood { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
    public string ZipCode { get; set; }

    public void Validate()
    {
        AddNotifications(new Contract<CreatePayPalSubscriptionCommand>()
            .Requires()
            .IsGreaterOrEqualsThan(FirstName, 3,
            "CreatePayPalSubscriptionCommand.FirstName",
            "O nome deve conter ao menos 3 caracteres")
            .IsGreaterOrEqualsThan(LastName, 3,
            "CreatePayPalSubscriptionCommand.FirstName",
            "O sobrenome deve conter ao menos 3 caracteres")
        );
    }
}

