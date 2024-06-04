namespace MyFx.Messaging.Core
{
    /// <summary>
    /// An enum that describes how a message should be dealt with by the subscriber after a the handler has tried to process it.
    /// </summary>
    public enum MessageDispositionKind
    {
        Acknowledge,  // The message was processed successfully and can be removed from the backend.
        Requeue,      // The message was not handled successfully because of a temporary condition in the handler.  Put the message back on its configured channel.
        DeadLetter    // The message will never be handled successfully due to either a fault in the message itself or an unrecoverable condition in the handler's services.
    }
}