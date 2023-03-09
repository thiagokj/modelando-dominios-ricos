using Flunt.Validations;
using PaymentsContext.Shared.Entities;

namespace PaymentsContext.Domain.Entities;

public class Subscription : Entity
{
    private IList<Payment> _payments;
    private object value;

    public Subscription(DateTime? expireDate)
    {
        CreateDate = DateTime.Now;
        LastUpdateDate = DateTime.Now;
        ExpireDate = expireDate;
        Active = true;
        _payments = new List<Payment>();
    }

    public DateTime CreateDate { get; private set; }
    public DateTime LastUpdateDate { get; private set; }
    public DateTime? ExpireDate { get; private set; }
    public bool Active { get; private set; }
    public IReadOnlyCollection<Payment> Payments { get { return _payments.ToArray(); } }

    public void AddPayment(Payment payment)
    {
        AddNotifications(new Contract<Payment>()
            .Requires()
            .IsLowerThan(payment.PaidDate, DateTime.Now, "Subscription.Payments",
            "A data do pagamento deve ser futura")
        );

        _payments.Add(payment);
    }

    public void IsActive(bool status)
    {
        Active = status;
        LastUpdateDate = DateTime.Now;
    }
}