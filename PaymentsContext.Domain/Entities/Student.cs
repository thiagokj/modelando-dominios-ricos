using Flunt.Validations;
using PaymentsContext.Domain.ValueObjects;
using PaymentsContext.Shared.Entities;

namespace PaymentsContext.Domain.Entities;

public class Student : Entity
{
    private IList<Subscription> _subscriptions;

    public Student(Name name, Document document, Email email)
    {
        Name = name;
        Document = document;
        Email = email;
        _subscriptions = new List<Subscription>();
        AddNotifications(name, document, email);
    }

    public Name Name { get; private set; }
    public Document Document { get; private set; }
    public Email Email { get; private set; }
    public Address? Address { get; private set; }
    public IReadOnlyCollection<Subscription> Subscriptions { get { return _subscriptions.ToArray(); } }

    public void AddSubscription(Subscription subscription)
    {
        var hasSubscriptionActive = false;
        foreach (var item in _subscriptions)
        {
            if (item.Active)
                hasSubscriptionActive = true;
        }

        AddNotifications(new Contract<Student>()
            .Requires()
            .IsFalse(hasSubscriptionActive,
            "Student.Subscriptions", "Você já possui uma assinatura ativa")
            .IsGreaterThan(subscription.Payments.Count, 0, "Student.Subscriptions.Payments",
             "Essa assinatura não possui pagamentos")
        );

        if (IsValid)
            _subscriptions.Add(subscription);
    }
}
