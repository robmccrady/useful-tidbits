using System.Threading.Tasks;

namespace MyFx.Messaging.Core
{
    /// <summary>
    /// IConnection is an interface that represents a connection to a messaging backend.
    /// 
    /// TConnection corresponds to the Type that is used to connect to the specific messaging backend.
    /// TConfiguration corresponds to the Type that is used to configure the specific messaging backend.
    /// Both of these concrete types must be defined for each implemented messaging backend.
    /// 
    /// TConnection will likely be a built-in type such as RabbitMQConnection or AzureEventHubConnection.
    /// TConfiguration may need to be implemented directly to hold the necessary configuration elements required by the specific messaging backend.
    /// </summary>
    public interface IConnectionBuilder<TConnection, TConfiguration> where TConfiguration : BrokerConfigurationBase
    {
        /// <summary>
        /// Builds a connection to a messaging backend using the supplied Configuration object.
        /// </summary>
        Task<TConnection> BuildConnectionAsync(TConfiguration configuration);
    }
}