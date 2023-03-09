using PaymentsContext.Shared.Commands;

namespace PaymentsContext.Shared.Handlers
{
    public interface IHandler<T> where T : ICommand
    {
        ICommandResult Handle(T command);
    }
}