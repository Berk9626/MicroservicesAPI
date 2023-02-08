namespace Play.Common.Settings
{
    public class RabbitMQSettings
    {
        public string  Host { get; init; } //nobody should be setting theese properties after they have been deserialized from the configuration

    }
    
}