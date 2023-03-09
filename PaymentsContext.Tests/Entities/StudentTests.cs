using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentsContext.Domain.Entities;
using PaymentsContext.Domain.Enums;
using PaymentsContext.Domain.ValueObjects;

namespace PaymentsContext.Tests;

[TestClass]
public class StudentTests
{
    private readonly Name _name;
    private readonly Document _document;
    private readonly Email _email;
    private readonly Address _address;
    private readonly Student _student;

    public StudentTests()
    {
        _name = new Name("Bruce", "Teste");
        _document = new Document("82498967346", EDocumentType.CPF);
        _email = new Email("batman@dc.com");
        _address = new Address(
            "Rua alameda dos heróis",
            "234",
            "Vila Monte bom",
            "Rio Largo",
            "Rio de Janeiro",
            "Brasil",
            "12345678"
        );
        _student = new Student(_name, _document, _email);
    }

    [TestMethod]
    public void ErrorWhenHadActiveSubscription()
    {
        var subscription = new Subscription(null);
        var payment = new PayPalPayment(
            "12345678",
            DateTime.Now,
            DateTime.Now.AddDays(5),
            10,
            10,
            "Wayne Corp.",
            _document,
            _address,
            _email
        );

        subscription.AddPayment(payment);
        _student.AddSubscription(subscription);
        _student.AddSubscription(subscription);

        Assert.IsFalse(_student.IsValid);
    }

    [TestMethod]
    public void ErrorWhenHadSubscriptionHasNoPayment()
    {
        var subscription = new Subscription(null);
        _student.AddSubscription(subscription);
        Assert.IsFalse(_student.IsValid);
    }

    [TestMethod]
    public void SuccessWhenHadNoActiveSubscription()
    {
        var subscription = new Subscription(null);
        var payment = new PayPalPayment(
            "12345678",
            DateTime.Now,
            DateTime.Now.AddDays(5),
            10,
            10,
            "Wayne Corp.",
            _document,
            _address,
            _email
        );
        subscription.AddPayment(payment);
        _student.AddSubscription(subscription);

        Assert.IsTrue(_student.IsValid);
    }
}