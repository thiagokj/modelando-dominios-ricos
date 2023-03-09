using Flunt.Notifications;
using PaymentsContext.Domain.Commands;
using PaymentsContext.Domain.Entities;
using PaymentsContext.Domain.Enums;
using PaymentsContext.Domain.Repositories;
using PaymentsContext.Domain.Services;
using PaymentsContext.Domain.ValueObjects;
using PaymentsContext.Shared.Commands;
using PaymentsContext.Shared.Handlers;

namespace PaymentsContext.Domain.Handlers;

// No Handler, herdamos as notificações e implementamos todas as formas de Assinaturas via interface IHandler:
// Boleto, Cartão e PayPal
public class SubscriptionHandler :
    Notifiable<Notification>,
    IHandler<CreateBoletoSubscriptionCommand>,
    IHandler<CreatePayPalSubscriptionCommand>
{
    // As referencias externas ao Handler são declaradas como privadas e injetadas posteriormente
    private readonly IStudentRepository _repository;
    private readonly IEmailService _emailService;

    public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }

    public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
    {
        // Fluxo de processo

        // Fail Fast Validation | Otimiza o processo, retornando antes de executar todo o fluxo em caso de erro.
        command.Validate();
        if (!command.IsValid)
        {
            AddNotifications(command);
            return new CommandResult(false, "Não foi possível realizar a assinatura");
        }

        // Verifica se o CPF está cadastrado.
        if (_repository.DocumentExists(command.Document))
        {
            AddNotification("Document", "CPF já está cadastrado");
        }

        // Verifica se o Email está cadastrado
        if (_repository.EmailExists(command.Email))
        {
            AddNotification("Email", "E-mail já está cadastrado");
        }

        // Gera os Value Objects
        var name = new Name(command.FirstName, command.LastName);
        var document = new Document(command.Document, EDocumentType.CPF);
        var email = new Email(command.Email);
        var address = new Address(
            command.Street,
            command.AddressNumber,
            command.Neighborhood,
            command.City,
            command.State,
            command.Country,
            command.ZipCode
        );

        // Gera as Entidades
        var student = new Student(name, document, email);
        var subscription = new Subscription(DateTime.Now.AddMonths(1)); // Assinatura mensal
        var payment = new BoletoPayment(
            command.BarCode,
            command.BoletoNumber,
            command.PaidDate,
            command.ExpireDate,
            command.Total,
            command.TotalPaid,
            command.Payer,
            new Document(command.PayerDocument, command.PayerDocumentType),
            address,
            email
        );

        // Relacionamentos
        subscription.AddPayment(payment);
        student.AddSubscription(subscription);

        // Agrupa as validações
        AddNotifications(name, document, email, address, student, subscription, payment);

        // Checagem de notificações
        if (!IsValid)
        {
            return new CommandResult(false, "Não foi possível realizar a assinatura");
        }

        // Salva as informações no repositorio
        _repository.CreateSubscription(student);

        // Envia o Email de boas vindas
        _emailService.Send(student.Name.ToString(), student.Email.Address,
         "bem vindo a plataforma", "Sua assinatura foi criada");

        return new CommandResult(true, "Assinatura realizada com sucesso");
    }

    public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
    {
        // Fluxo de processo

        // Fail Fast Validation | Otimiza o processo, retornando antes de executar todo o fluxo em caso de erro.
        command.Validate();
        if (!command.IsValid)
        {
            AddNotifications(command);
            return new CommandResult(false, "Não foi possível realizar a assinatura");
        }

        // Verifica se o CPF está cadastrado.
        if (_repository.DocumentExists(command.Document))
        {
            AddNotification("Document", "CPF já está cadastrado");
        }

        // Verifica se o Email está cadastrado
        if (_repository.EmailExists(command.Email))
        {
            AddNotification("Email", "E-mail já está cadastrado");
        }

        // Gera os Value Objects
        var name = new Name(command.FirstName, command.LastName);
        var document = new Document(command.Document, EDocumentType.CPF);
        var email = new Email(command.Email);
        var address = new Address(
            command.Street,
            command.AddressNumber,
            command.Neighborhood,
            command.City,
            command.State,
            command.Country,
            command.ZipCode
        );

        // Gera as Entidades
        var student = new Student(name, document, email);
        var subscription = new Subscription(DateTime.Now.AddMonths(1)); // Assinatura mensal
        var payment = new PayPalPayment(
            command.TransactionCode,
            command.PaidDate,
            command.ExpireDate,
            command.Total,
            command.TotalPaid,
            command.Payer,
            new Document(command.PayerDocument, command.PayerDocumentType),
            address,
            email
        );

        // Relacionamentos
        subscription.AddPayment(payment);
        student.AddSubscription(subscription);

        // Agrupa as validações
        AddNotifications(name, document, email, address, student, subscription, payment);

        // Checagem de notificações
        if (!IsValid)
        {
            return new CommandResult(false, "Não foi possível realizar a assinatura");
        }

        // Salva as informações no repositorio
        _repository.CreateSubscription(student);

        // Envia o Email de boas vindas
        _emailService.Send(student.Name.ToString(), student.Email.Address,
         "bem vindo a plataforma", "Sua assinatura foi criada");

        return new CommandResult(true, "Assinatura realizada com sucesso");
    }
}