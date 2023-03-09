using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentsContext.Domain.Entities;
using PaymentsContext.Domain.Enums;
using PaymentsContext.Domain.Queries;
using PaymentsContext.Domain.ValueObjects;

namespace PaymentsContext.Tests.Queries
{
    [TestClass]
    public class StudentQueriesTests
    {
        private IList<Student> _students = new List<Student>();

        public StudentQueriesTests()
        {
            for (int i = 1; i <= 10; i++)
            {
                _students.Add(new Student(
                    new Name("Aluno", $"Alu{i.ToString()}"),
                    new Document($"11111111111{i.ToString()}", EDocumentType.CPF),
                    new Email(i.ToString() + "email@email.suf")
                ));
            }
        }

        [TestMethod]
        public void NullWhenDocumentNotExists()
        {
            var expression = StudentQueries.GetStudentInfo("12345678911");
            var student = _students
                            .AsQueryable()
                            .Where(expression)
                            .FirstOrDefault();

            Assert.AreEqual(null, student);
        }

        [TestMethod]
        public void ReturnStudentWhenDocumentExists()
        {
            var expression = StudentQueries.GetStudentInfo("111111111111");
            var student = _students
                            .AsQueryable()
                            .Where(expression)
                            .FirstOrDefault();

            Assert.AreNotEqual(null, student);
        }
    }
}