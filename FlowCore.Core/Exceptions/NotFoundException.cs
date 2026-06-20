namespace FlowCore.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() { }

        public NotFoundException(string message) : base(message) { }

        public NotFoundException(string entityName, object key)
            : base($"'{entityName}' entity with key ({key}) was not found.") { }
    }
}
