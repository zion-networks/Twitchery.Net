namespace TwitcheryNet.Commands;

public class CommandDispatcher
{
    public ICommand this[string cmdName]
    {
        get => GetCommand(cmdName);
        set => TryRegisterCommand(value);
    }
    
    public ICommand GetCommand(string cmdName)
    {
        throw new NotImplementedException();
    }

    public void TryRegisterCommand(ICommand value)
    {
        throw new NotImplementedException();
    }
}