using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyFx.Messaging.Core
{

    /// <summary>
    /// Base class for all command messages.  This class is intended to be extended outside of this assembly.
    /// A Command message is sent when a a condition is met while executing a workload that requires a secondary process be triggered.
    /// The primary workload must have no requirement that the secondary process succeed or even complete.
    ///
    /// The details of a Command should be specified by the subsystem that will handle the command.
    /// (Subscriber Owns the Contract)
    /// </summary>
    public abstract class CommandBase:MessageBase
    {
        
        protected CommandBase(string commandName, Guid? workloadCorrelationId) : base(workloadCorrelationId)
        {
            CommandName = commandName;
        }

        /// <summary>
        /// Identifies the System-defined Command represented by this instance.
        /// 
        /// Concrete Child classes should assign this value upon instantiation.
        /// It is recommended that these values be defined in a System-Specific framework as constants which are 
        /// shared available to all projects within a particular solution.
        /// </summary>
        public string CommandName {get; init;}

       
    }

    /// <summary>
    /// Extends the CommandBase class to allow for generic payload types.
    /// The Generic T parameter refers to the type of Payload and should be defined by the system that uses the Command.
    /// </summary>
    public abstract class CommandBase<TPayload>:CommandBase where TPayload: new()
    {

        public CommandBase(string commandName, TPayload? payload, Guid? workloadCorrelationId) : base(commandName, workloadCorrelationId)
        {
            PayloadTypeName = typeof(TPayload).Name;
            Payload = payload;
        }

        public string PayloadTypeName { get; init; }

        public TPayload? Payload { get; init; }

        /// <summary>
        /// Deserializes the provided Json string into a instance of TPayload.
        /// If the jsonString is empty, whitespace, or cannot be deserialized into the requested
        /// payload type, we'll return the default value of TPayload.
        /// 
        /// It is the responsibility of the consumer of this method to check and appropriately handle
        /// rehydration failures.
        /// </summary>
        public static TPayload? Rehydrate(string json)
        {
            TPayload? payload = default(TPayload);

            if(string.IsNullOrWhiteSpace(json) == false)
            {
                try
                {
                    payload = JsonSerializer.Deserialize<TPayload>(json);
                }
                catch
                {
                    // nom nom nom
                    // We don't care if the json deserialization fails since 
                    // our desired failure mode is to return the default of the Payload's type.
                    // If we try rehydration and get nothing back, it's up to 
                    //the consumer of the Message to handle that.
                }
            }

            return payload;
        }
    }
}