using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentsContext.Domain.Enums;
using PaymentsContext.Domain.ValueObjects;

namespace PaymentsContext.Tests.ValueObjects;

[TestClass]
public class DocumentTests
{
    [TestMethod]
    public void ShouldReturnErrorWhenCNPJIsInvalid()
    {
        // Um documento com apenas 3 numeros para CNPJ é invalido, retornando erro.
        var document = new Document("123", EDocumentType.CNPJ);
        Assert.IsFalse(document.IsValid);
    }

    [TestMethod]
    public void ShouldReturnSuccessWhenCNPJIsValid()
    {
        // Um documento formatado como CNPJ é valido, retornando sucesso.
        var document = new Document("16677822000141", EDocumentType.CNPJ);
        Assert.IsTrue(document.IsValid);
    }

    [TestMethod]
    public void ShouldReturnErrorWhenCPFIsInvalid()
    {
        // Um documento com apenas 3 numeros para CPF é invalido, retornando erro.
        var document = new Document("123", EDocumentType.CNPJ);
        Assert.IsFalse(document.IsValid);
    }

    [TestMethod]
    [DataTestMethod]
    [DataRow("33330965525")]
    [DataRow("90108183300")]
    [DataRow("95784333798")]
    public void ShouldReturnSuccessWhenCPFIsValid(string cpf)
    {
        // Um documento formatado como CPF é valido, retornando sucesso.
        var document = new Document(cpf, EDocumentType.CNPJ);
        Assert.IsTrue(document.IsValid);
    }
}