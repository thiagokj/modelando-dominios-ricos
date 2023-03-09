using PaymentsContext.Domain.Entities;
using PaymentsContext.Domain.Repositories;

namespace PaymentsContext.Tests.Mocks;

public class FakeStudentRepository : IStudentRepository
{
    public void CreateSubscription(Student student)
    {

    }

    public bool DocumentExists(string document)
    {
        if (document == "99999999999")
            return true;

        return false;
    }

    public bool EmailExists(string email)
    {
        if (email == "hello@domain.sufix")
            return true;

        return false;
    }
}
