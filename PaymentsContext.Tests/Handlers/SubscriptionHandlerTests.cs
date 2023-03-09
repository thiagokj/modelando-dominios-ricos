using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentsContext.Domain.Commands;
using PaymentsContext.Domain.Enums;
using PaymentsContext.Domain.Handlers;
using PaymentsContext.Tests.Mocks;

namespace PaymentsContext.Tests.Handlers
{
    [TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ErrorWhenDocumentExists()
        {
            var handler = new SubscriptionHandler(
                new FakeStudentRepository(),
                new FakeEmailService()
            );

            var command = new CreateBoletoSubscriptionCommand();

            command.FirstName = "Thiago";
            command.LastName = "Ronaldo";
            command.Document = "99999999999";
            command.Email = "email@domain.sufixo";
            command.BarCode = "2452342311211";
            command.BoletoNumber = "1231414466";
            command.PaymentNumber = "142453454";
            command.PaidDate = DateTime.Now;
            command.ExpireDate = DateTime.Now.AddMonths(1);
            command.Total = 60;
            command.TotalPaid = 60;
            command.Payer = "WAYNE CORP";
            command.PayerDocument = "134534534354";
            command.PayerDocumentType = EDocumentType.CPF;
            command.PayerEmail = "robin@dc.com";
            command.Street = "Rua dos testes";
            command.AddressNumber = "4444";
            command.Neighborhood = "Alegredidade";
            command.City = "São João dos cabrobró";
            command.State = "Piauí";
            command.Country = "Brasil";
            command.ZipCode = "11111000";

            handler.Handle(command);
            Assert.AreEqual(false, handler.IsValid);
        }
    }
}