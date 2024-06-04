using System;

namespace MyFx.Messaging.Core
{
    ///<summary>
    /// Base class for all messages.  This class is not intended to be extended outside of its assembly.
    /// All custom subclasses must inherit from either EventBase or CommandBase.
    /// </summary> 
    public abstract class MessageBase
    {
        /// <summary>
        /// Required for serialization/deserialization.
        /// </summary>
        public MessageBase():this(null)
        {}

        /// <summary>
        /// Sets the base properties common to all message types.
        /// </summary>
        /// <param name="messageTypeName"></param>
        /// <param name="payloadJson"></param>
        /// <param name="workloadCorrelationId"></param>
        internal MessageBase(Guid? workloadCorrelationId)
        {
            MessageId = Guid.NewGuid();
            CreatedAtUtcTicks = DateTime.UtcNow.Ticks;
            PayloadJson = string.Empty;
            WorkloadCorrelationId = workloadCorrelationId;
        }

        /// <summary>
        /// The unique identifier of the message.
        /// </summary>
        public Guid MessageId { get; init; }

        /// <summary>
        /// The unique identifier of the workload that this message belongs to.
        /// </summary>
        public Guid? WorkloadCorrelationId { get; init; }

        /// <summary>
        /// The UTC time in ticks at which the message was created.
        /// </summary>
        public long CreatedAtUtcTicks { get; init; }
        
        /// <summary>
        /// The JSON payload of the message.  This will be deserialized by the message handler into a compatible type.
        /// </summary>
        public string PayloadJson { get; protected set; }
    }
}