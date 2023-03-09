
using System.Linq.Expressions;
using PaymentsContext.Domain.Entities;

namespace PaymentsContext.Domain.Queries;
public static class StudentQueries
{
    public static Expression<Func<Student, bool>> GetStudentInfo(string document)
    {
        return x => x.Document.Number == document;
    }
}
