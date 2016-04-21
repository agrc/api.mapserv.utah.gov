namespace Soe.Common.GDB.Connect
{
    public interface IConnectionInformation
    {
        string DatabaseName { get; }
        string InstanceName { get; }
        string Name { get; }
        string Password { get; }
        string Version { get; }
    }
}