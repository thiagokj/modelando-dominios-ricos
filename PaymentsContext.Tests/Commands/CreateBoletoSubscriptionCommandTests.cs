using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentsContext.Domain.Commands;

namespace PaymentsContext.Tests;

[TestClass]
public class CreateBoletoSubscriptionCommandTests
{
    [TestMethod]
    public void ErrorWhenFirstNameIsInvalid()
    {
        var command = new CreateBoletoSubscriptionCommand();
        command.FirstName = "";

        command.Validate();
        Assert.AreEqual(false, command.IsValid);
    }
}