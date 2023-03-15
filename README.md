# Modelando domínios ricos

Projeto para estudos e aprendizado dos conceitos de modelagem. [Curso balta.io](https://github.com/balta-io/1975).

## Linguagem Ubíqua

Linguagem ubíqua é garantir que todos sabem do que se trata o assunto, com os jargões. Todos trabalhando na mesma linguagem.

Temos a definição dos conceitos, tratamento e especificação no processo de modelagem antes de iniciar o desenvolvimento de aplicativos.

Os nomes comuns que o cliente usa devem ser incluídos no projeto, sem traduções ou adaptações.
Ex: francesinha, sangria de caixa, boleto.

Codificar em inglês ou português? Deve ser levado em consideração qual o time e seu nível técnico.

Evite o "**portuglês**". Ex: ClienteGetByAno (GetBaiano), UsuarioRepository.

## Domínios ricos vs Domínios anêmicos

**Domínio anêmico** | Classes e entidades possuem apenas propriedades básicas, sem expressividade.
Ex: Todas as regras de negócio rodam no banco de dados e a aplicação só possui a modelagem básica, refletindo o banco.

Problemas:

- Dificuldade para criar e fazer testes, garantindo que está tudo funcionando.
- Linguagens TSQL /PLSQL possuem limitações por serem interpretadas, nao conseguindo pegar erro em tempo de execução.

**Domínio rico** | Toda modelagem é feita na aplicação e o banco de dados é utilizado como repositório.
Trabalhando nesse cenário, é possível separar as regras de negócio, permitindo testes sem utilizar o banco de dados.

Devemos sempre evitar **Over Engineering** (super dimensionamento), que é criar uma solução muito complexa para resolver problemas simples. Por isso é tão importante analisar o que é solicitado e desenvolver uma aplicação para atender a demanda, sem exageros.

## Sub domínios

Nada mais é do que segmentar domínios grandes em partes. Ex: Um ERP é um sistema muito grande.

Crie as partes do sistema ERP como módulos. Ex: Financeiro, Comercial, Fabrica, Faturamento, Qualidade, etc.

## Separando em contextos delimitados

Para o sucesso de projetos, devemos controlar o tamanho da mudança. Ao invés de pensar em um sistema como um todo, foque apenas em uma pequena parte. Ex: criar uma solução para fazer pagamentos.

A ideia é criar APIs para cada pequeno pedaço do sistema e fazer a comunicação por meio dessas APIs.

Ex: A Netflix possui uma API com apenas um método GET para retornar filmes.

Cada pedacinho do aplicativo da Netflix pode estar em uma API diferente, facilitando a gestão de mudanças futuras e manutenção.

## Organizando a solução

Basicamente devemos criar um contexto e separar em 3 pastas: Domain, Shared e Tests.

Ex: PagamentoContexto

- Pagamento.Domínio
- Pagamento.Compartilhado
- Pagamento.Testes

1. Crie uma solution para organizar logicamente: **dotnet new sln**

1. Após criar a solution, crie os projetos nas pastas(domain, shared). Ex: **dotnet new classlib**
1. Na pasta de testes, crie um projeto com **dotnet new mstest**

Agora adicione os novos projetos a solução:

```csharp
dotnet sln add .\PaymentsContext.Domain\PaymentsContext.Domain.csproj
dotnet sln add .\PaymentsContext.Shared\PaymentsContext.Shared.csproj
dotnet sln add .\PaymentsContext.Tests\PaymentsContext.Tests.csproj
```

PaymentsContext.Domain -> Domínio rico a ser modelado.
PaymentsContext.Shared -> Itens que podem ser compartilhado entre múltiplos domínios.
PaymentsContext.Tests -> Testes que vão ser executados.

Por fim, adicione as referencias entre os projetos:

Domain depende do shared.
Shared não depende de nenhum projeto.
Testes depende de Domain e Shared.

```csharp
/*PaymentsContext.Domain*/ dotnet add reference ..\PaymentsContext.Shared\PaymentsContext.Shared.csproj
/*PaymentsContext.Tests*/ dotnet add reference ..\PaymentsContext.Shared\PaymentsContext.Shared.csproj
/*PaymentsContext.Tests*/ dotnet add reference ..\PaymentsContext.Domain\PaymentsContext.Domain.csproj
```

## Definindo as entidades

As entidades são as referências de uma conversa. Tudo que é necessário para ser automatizado sai de uma conversa, como forma de pagamento, cadastro de clientes, pedidos, vouchers. Cada um desses elementos podem ser separados e organizados em entidades.

1. Crie uma pasta para organizar as entidades.

1. Crie as entidades com base no contexto do projeto, pensando em como organizar o processo em código.

1. Defina as propriedades iniciais de cada entidade. Caso seja necessário alterar ou adicionar mais propriedades no futuro, não há problema.

```csharp
// Representação básica de um cadastro de um aluno, com dados para identificação.
namespace PaymentsContext.Domain.Entities
{
    public class Student
    {
        public string FirsName { get; set; }
        public string LastName { get; set; }
        public string Document { get; set; }
        public string Email { get; set; }
        // 1 aluno pode ter múltiplas assinaturas
        public List<Subscription> Subscriptions { get; set; }

    }
}

public abstract class Payment
{
    public string Number { get; set; }
    public DateTime PaidDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPaid { get; set; }
    public string Payer { get; set; }
    public string Document { get; set; }
    public string Address { get; set; }
}

public class BoletoPayment : Payment
{
    public string BarCode { get; set; }
    public string BoletoNumber { get; set; }
}

public class Subscription
{
    public DateTime CreateDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool Active { get; set; }
    public List<Payment> Payments { get; set; }
}
```

## Corrupção no código

Crie a mesma estrutura de entidades no projeto de testes.

O construtor é ideal nas entidades, deixando o código mais expressivo, facilitando a criação de novos objetos.

Quando limitamos instanciar objetos via construtor, temos apenas um único ponto de entrada, facilitando as validações.

Então:

1. Limite os métodos set das propriedades para **private set**, evitando modificações do objeto fora do construtor. Caso necessário alterar alguma propriedade do objeto, crie um método para fazer isso.

```csharp
public void TestMethod1()
{
    var Student = new Student(
        "Thiago",
        "Carvalho",
        "36584512545",
        "teste@meudomain.sufix"
    );

    // Evitamos esse caminho de alteração no código
    student.FirstName = "";

    // Forma melhorada, validando a alteração com um método
    student.ChangeFirstName("João");
}
```

1. Faça o mesmo para listas, limitando o tipo de lista com o escopo **IReadOnlyCollection**.

```csharp
...
// Trecho da entidade
public class Student
{
    private IList<Subscription> _subscriptions;

    public IReadOnlyCollection<Subscription> Subscriptions { get { return _subscriptions.ToArray(); } }

    public void AddSubscription(Subscription subscription)
    {
        // Caso existam outras assinaturas, inativa todas.
        foreach (var sub in Subscriptions)
            sub.Active = false;

        // Adiciona a nova assinatura como principal.
        _subscriptions.Add(subscription);
    }
}

...
// Trecho do método de teste
var subscription = new Subscription();

// Limite o escopo da lista, evitando esse caminho para entrada por fora do construtor.
student.Subscriptions.Add(subscription);

// Forma aprimorada para adicionar uma assinatura a um aluno
student.AddSubscription(subscription);
```

Refatorar o código é algo muito comum durante a modelagem e evolução do projeto. O trecho abaixo foi modificado conforme a limitação de escopo.

```csharp
...
// Trecho de Student
 public void AddSubscription(Subscription subscription)
    {
        // Caso existam outras assinaturas, inativa todas.
        foreach (var sub in Subscriptions)
            sub.IsActive(false);

        // Adiciona a nova assinatura como principal
        _subscriptions.Add(subscription);
    }
...

// Trecho de Subscription
 public IReadOnlyCollection<Payment> Payments { get; private set; }

    public void IsActive(bool status)
    {
        Active = status;
        LastUpdateDate = DateTime.Now;
    }
```

## Primitive Obsession

Criar todas as propriedades com string ou int não é uma boa prática, pois acabamos tendo que fazer validações repetitivas.

```csharp
public string FirstName { get; private set; }
public string LastName { get; private set; }
string document;

if(document.length > 0)
```

Os tipos primitivos são os tipos básicos do Csharp. O ideal é criar seus próprios tipos, que incluem tratamento e validações, deixando o código mais enxuto e reutilizável.

## Value Objects

Os objetos de valor são tipos que compõem uma entidade. Ao utilizar **VO**, paramos de usar tipos primitivos e passamos a usar tipos complexos.

Crie pastas ValueObjects em cada projeto na solution para melhor organização.

```csharp
// Propriedades da entidade Student com tipos primitivos
public string FirstName { get; private set; }
public string LastName { get; private set; }

// Criando um objeto de valor, aprimorando a representação das propriedades
public class Name
{
    public Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
}

// Limpando o código, removendo as propriedades primitivas do tipo string, passando a usar
// os Value Objects, criando uma propriedade complexa do tipo Name.
...
public Name Name
```

## Implementando Validações

Após modelar os VOs, temos a estrutura para fazer as validações.

Devemos evitar lançar exceções, pois em produção, a Thread é interrompida e gera múltiplos logs no sistema operacional.

**Exceptions** | Algo não esperado que ocorra. Ex: Banco de dados fora do ar, API indisponível, falha de conexão.

**Validations** | Algo que você espera que aconteça. Ex: Limite de caracteres no campo, valor maior que zero.

Uma melhor estratégia é trabalhar com **notificações**.

1. Instale o pacote Flunt em todos os projetos da Solution **dotnet add package Flunt**
1. Faça com que as Entidade Base (Shared -> Entity.cs) e Shared -> ValueObject herdem da classe Notifiable.
1. Assim podemos utilizar em todas as entidades com herança no projeto as notificações padronizadas.

```csharp
public Student(Name name, Document document, Email email)
{
  Name = name;
  Document = document;
  Email = email;
  _subscriptions = new List<Subscription>();

  // Caso o nome seja inválido, é adicionada uma notificação.
  if (string.IsNullOrEmpty(name.FirstName))
      AddNotification("Name.FirstName", "Nome inválido");
}

// Exemplo de como ficaria seria feito um teste em StudentTests.cs
...
    [TestMethod]
    public void TestAddSubscription()
    {
        var name = new Name(null, "Teste");
        foreach (var item in name.Notifications)
        {
            Assert.Fail(item.Message);
        }

        Assert.AreEqual("Teste", name.FirstName);
    }
```

## Design by Contracts

O design por contratos é um padrão de projeto, trabalhando com um ponto único de falha.
Ex: Apenas um método de login. Não há outra forma de logar no sistema sem passar pelo método.

Evite diversos testes com if em sequencia e duplicidade de código.

```csharp
// Métodos iguais, só muda o parâmetro de entrada
if (string.IsNullOrEmpty(FirstName))
   AddNotification("Name.FirstName", "Nome inválido");

if (string.IsNullOrEmpty(LastName))
   AddNotification("Name.LastName", "Nome inválido");

// Preferível
AddNotifications(new Contract<Name>()
   .Requires()
   .IsGreaterOrEqualsThan(FirstName, 3, "Name.FirstName",
    "O nome deve conter 3 caracteres ou mais.")
   .IsGreaterOrEqualsThan(LastName, 3, "Name.LastName",
    "O sobrenome deve conter 3 caracteres ou mais.")
   .IsLowerOrEqualsThan(FirstName, 40, "Name.FirstName",
    $"O nome deve conter até 40 caracteres")
);
```

## Testando as Entidades e VOs

Para amadurecer nos casos de testes, procure usar a metodologia **Red, Green, Refactor**

Faça tudo falhar, faça tudo passar e depois refatore.

1. Crie sua classe para simular o teste, fazendo todos os métodos falharem.

```csharp
[TestClass]
public class DocumentTests
{
    [TestMethod]
    public void ShouldReturnErrorWhenCPForCNPJIsInvalid()
    {
        Assert.Fail();
    }

    [TestMethod]
    public void ShouldReturnSuccessWhenCPForCNPJIsValid()
    {
        Assert.Fail();
    }
```

1. Agora crie casos de testes, onde todos devem passar com exito.
   Obs: Para gerar dados de testes, utilizei os links [Gerador CNPJ](https://www.geradorcnpj.com/) e [Gerador CPF](https://www.geradordecpf.org/).

```csharp
...
[TestMethod]
    public void ShouldReturnErrorWhenCNPJIsInvalid()
    {
        // Um documento com apenas 3 números para CNPJ é invalido, retornando erro.
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
        // Um documento com apenas 3 números para CPF é invalido, retornando erro.
        var document = new Document("123", EDocumentType.CNPJ);
        Assert.IsFalse(document.IsValid);
    }

    // Utilizando a anotação abaixo, é possível testar com valores diferentes o método
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
```

## Commands

Utilizando o Padrão CQRS -> Segregação de Responsabilidade de Comandos e Consulta.

Aqui temos a segregação de inputs e queries, separando as operações de leitura e atualização de um armazenamento de dados.

Os Commands são a junção de comandos para criar um objeto. São a entrada de dados para manipular os dados e gerar um retorno de saída.

Crie pastas Commands em cada projeto na solution para melhor organização.

```csharp
public class CreateBoletoSubscriptionCommand : Notifiable<Notification>, ICommand
{
public string FirstName { get; set; }
public string LastName { get; set; }
public string Number { get; set; }
public string Email { get; set; }

public string BarCode { get; set; }
public string BoletoNumber { get; set; }
...
}
```

## Fail Fast Validations

É uma técnica onde colocamos as validações diretamente nos Commands.

A intenção é validar antes de dar sequencia a uma requisição, evitando acesso desnecessário ao banco de dados e outros recursos.

Em cenários comuns sem validações temos:

A requisição consulta APIs -> faz tratamentos -> acessa o banco de dados -> retorna a resposta ao solicitante.

Caso a requisição esteja com erro, retorna somente no fim do processo, consumindo todos os recursos no caminho.

```csharp
public class CreateBoletoSubscriptionCommand : Notifiable<Notification>, ICommand
{
...
   public void Validate()
   {
     AddNotifications(new Contract<CreateBoletoSubscriptionCommand>()
         .Requires()
         .IsGreaterOrEqualsThan(FirstName, 3,
         "CreateBoletoSubscriptionCommand.FirstName",
         "O nome deve conter ao menos 3 caracteres")
         .IsGreaterOrEqualsThan(LastName, 3,
         "CreateBoletoSubscriptionCommand.LastName",
         "O sobrenome deve conter ao menos 3 caracteres")
     );
   }
}
```

## Testando os commands

Crie casos de testes para avaliar os commands.

```csharp
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
    ...
}
```

## Repository Pattern

Esse padrão de design é uma abstração de fonte de dados. Criamos uma representação para ler e gravar os dados posteriormente em um banco.

Pensando em trabalhar com dependências, **sempre dependa da abstração e nunca da implementação**.
Assim tornamos o código mais flexível, testável, com baixo acoplamento e abstraindo a complexidade desnecessária. Isso ajuda a criar um código mais modular e escalável.

Crie pastas Repositories em cada projeto na solution para melhor organização.

```csharp
// Abstração utilizando uma interface para o repositório de aluno.
// Aqui informamos apenas os métodos que serão implementados.
public interface IStudentRepository
{
    // Método que verifica se o aluno possui documentos na fonte de dados
    bool DocumentExists(string document);

    // Método que verifica se o aluno possui email na fonte de dados
    bool EmailExists(string email);

    // Método para criar uma assinatura, passando o objeto aluno.
    void CreateSubscription(Student student);
}
```

## Handlers

Os manipuladores são as ações a serem executadas, seguindo o fluxo da aplicação, unindo todas as partes da aplicação.

Nesse projeto, vamos seguir um exemplo de fluxo para criar uma assinatura na plataforma:

- Validações iniciais com Fail Fast Validation.
- Verificação se o CPF está cadastrado.
- Verificação se o Email está cadastrado.
- Gera os Value Objects.
- Gera as Entidades.
- Relacionamentos das entidades.
  - Adiciona um pagamento.
  - Adiciona uma assinatura da plataforma.
- Agrupa as validações.
- Checagem de notificações.
- Salva os dados no banco de dados.
- Envia um email de boas vindas.
- Retornar informações de cadastrado realizado com sucesso.

No Handler podem ser herdadas as abstrações e implementadas as interfaces para realizar as ações.

```csharp
// No Handler, herdamos as Notificações e implementamos todas as formas de
// Assinaturas via interface IHandler: Boleto, Cartão e PayPal
public class SubscriptionHandler :
    Notifiable<Notification>,
    IHandler<CreateBoletoSubscriptionCommand>,
    IHandler<CreatePayPalSubscriptionCommand>,
    IHandler<CreateCreditCardSubscriptionCommand>,
{
    // As referencias externas ao Handler são declaradas como privadas e injetadas posteriormente
    private readonly IStudentRepository _repository; // Referencia para ler e gravar os dados.
    private readonly IEmailService _emailService; // Referencia de serviço de envio de email.

    public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }
...
}
```

Agora iniciamos o fluxo de processo com cada uma das formas de assinaturas da plataforma

```csharp
...
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

        // Salva as informações no repositório
        _repository.CreateSubscription(student);

        // Envia o Email de boas vindas
        _emailService.Send(student.Name.ToString(), student.Email.Address,
         "Bem vindo a plataforma", "Sua assinatura foi criada");

        // Retornar informações de cadastrado realizado com sucesso.
        return new CommandResult(true, "Assinatura realizada com sucesso");
    }
```

## Testando os Handlers

Teste somente o que for necessário e crítico, não há necessidade de testar tudo o que já foi testado e validado.

Para testar a parte de referências externas como banco de dados e serviços de email, SMS, utilizamos a técnica de criar **Mocks**.

Os **Mocks** são a representação de dados de fictícios para simulação de testes na aplicação.

1. Crie uma pasta no projeto de testes chamada Mocks.
1. Adicione as representações dos dados com o prefixo Fake. Ex: FakeStudentRepository.cs

```csharp
// Como o repositório é uma interface, reaproveitamos o código para testes, pois a implementação é flexível.
public class FakeStudentRepository : IStudentRepository
{
    public void CreateSubscription(Student student)
    {

    }

    // Representando 1 registro no banco de dados com CPF cadastrado como 99999999999.
    public bool DocumentExists(string document)
    {
        if (document == "99999999999")
            return true;

        return false;
    }

    // Representando 1 registro no banco de dados com Email cadastrado como hello@domain.sufix.
    public bool EmailExists(string email)
    {
        if (email == "hello@domain.sufix")
            return true;

        return false;
    }
}
```

Agora crie o HandlerTests para testar o processo:

```csharp
[TestClass]
    public class SubscriptionHandlerTests
    {
        [TestMethod]
        public void ErrorWhenDocumentExists()
        {
            // Criado um objeto do tipo SubscriptionHandler com os dados fictícios
            var handler = new SubscriptionHandler(
                new FakeStudentRepository(),
                new FakeEmailService()
            );

            // Criado um objeto do tipo CreateBoletoSubscriptionCommand, para passagem de parâmetros
            var command = new CreateBoletoSubscriptionCommand();

            command.FirstName = "Thiago";
            command.LastName = "Ronaldo";
            command.Document = "99999999999";
            command.Email = "email@domain.sufixo";
            // Demais propriedades obrigatórias
            ...

            // O handler executa o método Handle da interface ICommandResult.
            // Como o handler é um objeto do tipo SubscriptionHandler, ele irá executar o método com
            // a assinatura do tipo do objeto command, que no caso é o CreateBoletoSubscriptionCommand.
            // Obs: É o mesmo método acima, do tópico ## Handlers
            handler.Handle(command);

            // Deve retornar false, pois o CPF já existe no repositório FakeStudent, impedindo o cadastro
            // de uma nova assinatura acontecer.
            Assert.AreEqual(true, handler.IsValid);
        }
    }
```

## Queries

Aqui criamos as regras para leitura de dados.

As queries serão aplicáveis no caso de consultas simples. Para Stored Procedures, é necessário fazer um DE/PARA.

Crie uma pasta Queries no domain. Agora crie suas consultas usando LINQ do Csharp:

```csharp
public static class StudentQueries
{
    public static Expression<Func<Student, bool>> GetStudentInfo(string document)
    {
        return x => x.Document.Number == document;
    }
}
```

Agora faça os testes simulando a consulta de um documento:

```csharp
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
```
