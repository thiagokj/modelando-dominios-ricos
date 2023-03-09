using PaymentsContext.Domain.Services;

namespace PaymentsContext.Tests.Mocks;

public class FakeEmailService : IEmailService
{
    public void Send(string to, string email, string subject, string body)
    {

    }
}
