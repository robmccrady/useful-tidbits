namespace MyFx.Messaging.Core
{
    /// <summary>
    /// Base class for all messaging backend configurations.
    ///
    /// Subclasses of this base class must encapsulate the information required to make a connection to a specific messaging backend.
    /// The values carried will include things like the name of the Broker, a URL, credentials or a connection string, as appropriate
    /// and required by the specific Messaging Backend.  These details will vary from product to product.
    ///
    /// It is not intended that this class, nor its child classes be used to configure the solution-specific broker objects that organize
    /// the flow of messages between components.  RabbitMQ's Exchanges and Queues, Azure EventGrid's Topics and Subscriptions, etc. should all
    /// be designed within the context of your solution, and the configuration of the Broker backend's Routing objects should be handled within
    /// your solution itelf.
    /// </summary>
    public abstract class BrokerConfigurationBase
    {
        protected BrokerConfigurationBase(string brokerProduct)
        {
            BrokerProduct = brokerProduct;
        }
        
        /// <summary>
        /// The name of the messaging backend.  This must be set silently by the backend specific implementation.
        /// </summary>
        public string BrokerProduct { get; private set; }

        /// <summary>
        /// Must be overridden by a concrete BrokerConfiguration implementation that is created specifically for a particular messaging backend. 
        /// Produces a concrete instance of T from the provided JSON string.
        /// T must be a subclass of BrokerConfigurationBase.
        /// </summary>
        public abstract T FromJson<T>(string json) where T:BrokerConfigurationBase;
    }
}