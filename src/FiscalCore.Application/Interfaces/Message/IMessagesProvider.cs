namespace FiscalCore.Application.Interfaces.Message;

public interface IMessagesProvider
{
    string GetError(string resourceName);
    string GetMessage(string resourceName);
}
