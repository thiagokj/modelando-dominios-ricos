using Flunt.Extensions.Br.Validations;
using Flunt.Validations;
using PaymentsContext.Domain.Enums;
using PaymentsContext.Shared.ValueObjects;

namespace PaymentsContext.Domain.ValueObjects;

public class Document : ValueObject
{
    public Document(string number, EDocumentType type)
    {
        Number = number;
        Type = type;

        AddNotifications(new Contract<Document>()
            .Requires()
            .IsCpfOrCnpj(Number, "Document.Number", "CPF ou CNPJ inválido"));
    }
    public string Number { get; private set; }
    public EDocumentType Type { get; private set; }
}