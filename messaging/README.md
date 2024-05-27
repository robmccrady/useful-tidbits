# Messaging
This topic refers specifically to patterns in software architecture that allow processes to communicate with each other in a loosely coupled manner.  There are tradeoffs you must keep in mind when introducing message-based communications to your system, and this style of communication is not always applicable or beneficial to your system's health.

### $\color{Orange}{Metaphor Alert}$
If you've ever seen a restaurant where the server writes the order on a pad, then sticks that order into a rail in the window to the kitchen, it's a very similar idea.  Rather than the server having to wait near the kitchen window for your food, and then bringing it back to you, they can go take orders from other tables while your food is being prepared, and then go fetch it once the cook starts screaming about it.

## Main Concept  
In complex workflows, we'll often encounter cases where some part of our process needs to do something that isn't really essential to the work we're doing.  When this happens, and we want to delegate that "side effect" work to another component in our system, we can use messaging between our components to accomplish this.  

_Microservice architectures use messaging quite a lot._

<details>

---
<summary>Example:  Order Tracking System</summary>  

We are maintaining a system for a custom cabinetry company that helps them keep track of jobs as they move through the process of Consultation, Design, Construction, and finally Delivery and Installation.  

Many of their customers call to check on their order fairly frequently, and it's impacting the people in the shop's ability to actually get these orders fulfilled.

They already have custom software used to track the order and the designs, materials, and build progress and the people in the design and construction shops have been using it successfully for some time now.

We've built a new component to plug into their software that can build and send a simple email from a template that is sent to the customer when their order moves from phase to phase of the project, but we don't want the success or failure of sending that email message to interfere with the other parts of the Project Tracking code.

*This feels like a job for a Queued Call!*

We'll install some form of messaging broker next to their system, and add code to the main program that writes a specially formatted message to this broker.

We'll also wrap our email sending code in a discrete service, then add some code that can "listen" to the message broker we installed for that SendProjectUpdate command.

---  
</details>

## Tradeoffs
This all sounds great!  Why doesn't everybody do this???  In return for the improved flexibility in the solution, you're going to pay a few costs.

| Benefits | Drawbacks |
| - | - |
| Lower coupling between components and subsystems | Greatly increased complexity in design, testing, deployment, and debugging |
| Higher throughput for your system | Higher latency when executing workloads that use messaging |
| Allows for more flexible, extensible systems | Requires MUCH closer scrutiny at the architectural level, to ensure adherence to patterns and design standards, and to keep the overal system in a manageable state. |

## Guidelines and Constraints  

When using messaging, it's helpful to think about your system's messages as being one of two kinds:  **Commands** and **Events**.  When using messages, your component will have one or both of the following roles:  **Publisher** and/or **Subscriber**.  Publishers send messages out into the broader system context, and Subscribers receive and process the various messages that they care about.

There are some helpful rules and constraints to use when you're dealing with each kind of message.  

### General Considerations
 - **_Think this stuff through_**, and make diagrams **before diving into code**.  Future You, and your collaborators will thank you.
 - Keep the ratio of Publisher:Subscriber for each message at  1:0-N.  Each message kind uses this ratio or its opposite. 
 - When a component sends a message as part of its workflow, the publishing component **_MUST NOT CARE_** whether the message is received or handled.  If the sub-process produces an effect that impacts the success or failure of your primary use-case, don't use messaging.  
 - Shared Contracts are convenient, but treacherous.  It's far better to publish the message requirements and schema as documentation, and then implement the specific Message Models for your commands and events in their respective publishers and subscribers.  
  - Consider versioning your message schemas, right from the first time you add the class to your project.  This is another favor to Future You and your teammates.

### Commands  
> _"Hey Shiela, I'm working through that order, and I just finished the design.  When you get a chance, let the customer know for me, please?"_

A component issues a command when it needs to start a separate process that can run in parallel to its main work.  (In our example above, that "separate process" is sending an email to the customer when their order progresses through the fulfillment process.)  

**Pub/Sub Ratio:**  0-N:1
> **_Any command type message in your system should be received and processed by one and only one handler._**

This does not mean you can't run multiple instances of the service that will process a given command, but you really only want ONE service to receive and take action on these messages.

### Events
> _"HAY EVERYBODY!!!!  I BROKE THE TABLE SAW AND CAN'T WORK ON THESE CABINETS!"_

An event is a message that can be subscribed to by any number of components.  Think of it as a broadcast announcement, and your system's components can "tune in" to the specific channel you're broadcasting over.  The component that raises the event doesn't care who's interested in it, or what they'll do when the announcement is made, but they know that when certain things happen, they need to make some noise.  

**Pub/Sub Ratio:**  1:0-N  
> **_Specific events should be published by one and only one component of your system._**

## And now, to Code!
We're going to discuss these at a more conceptual level here.  _I'll add C# code that illustrates the concepts to the /src folder._ 

Because every system is different, both in terms of which message exchange backend you choose and in the messages that get exchanged between system components, we're going to deal in abstractions here.  That is, we're going to cover the DESCRIPTIONS of the base required behaviors, and the BARE MINIMUM attributes of the Message types.  The following code-level entities describe the behaviors and attributes that I've encountered every time I've built a message based system.

First, the behavioral side of things:  

---
### IPublisher  
#### Purpose:  _Responsible for pushing a message into a named structure on the messaging backend._

#### Notes to implementers  
Implement this object against a specific Messaging backend such as RabbitMQ, Azure EventGrid, Azure EventHub, and a multitude of other solutions from a large number of vendors.  Components that USE this concept can send messages to subscribers that are connected to the same backend.

#### Methods:
| Method | Purpose |
| - | - |
| [SendCommandAsync](#sendcommand) | Accepts a CommandMessage instance and pushes it onto the route configured on the messaging backend |
| [BroadcastEventAsync](#broadcastevent) | Accepts an EventMessage instance and pushes it onto the route configured on the messaging backend. |

#### SendCommandAsync
##### Definition  
```csharp
Task SendCommandAsync(CommandMessageBase command)
```
##### Remarks
Accepts an instance of a CommandMessageBase sub-type and places it on the backend structure(route) that's been designated for that particular Command type. 

#### BroadcastEventAsync
##### Definition  
```csharp
Task BroadcastEventAsync(EventMessageBase event)
```
Accepts an instance of an EventMessageBase sub-type and places it on the backend structure(route) that has been pre-configured for that particular event.  

---
### ISubscriber  
#### Purpose:  _Responsible for receiving messages from specific routes on the messaging backend and forwarding those messages to registered handlers._  

_When a message is processed, the Subscriber is responsible for the disposition of the message within the backend service._

#### Notes to implementers  
Implement this object against a specific Messaging backend.  Components that USE this concept register methods to receive Command or Event types from the subscriber and take action as these messages arrive.  

It is the responsibility of each method that implements a MessageHandler to respond back to the Subscriber with an instruction for how to dispose of the particular message.

#### Methods:
| Method | Purpose |
| - | - |
| [RegisterHandler](#registerhandler) | Accepts the "name" of a Command or Event and a reference to a delegate method that has been created to process the incoming message. |
| [StartListening](#startlistening) | Tells the Subscriber component to start receiving and forwarding messages on its configured channels. |
| [StopListening](#stoplistening) | Tells the Subscriber component to stop receiving and forwarding messages. |  

#### RegisterHandler  
##### Definition  
```csharp
void RegisterHandler<TMessage>(Func<Task<DispositionKind>, TMessage> handler)
```
This method configures the routing rule for the type of message described by the generic TMessage parameter.  A subscriber will receive a message from the messaging backend in some format and must likely decode and deserialize it into a concrete subclass of either Message type.  The resulting instance is then passed to the provided handler function.  

When the function completes, the subscriber will receive the DispositionKind back, which instructs the subscriber component on how to deal with the message itself.  

_Each Messaging Backend will have its own idiocyncracies around this process.  Consult the documentation for whichever backend you select._

#### StartListening  
##### Definition  
```csharp
void StartListeningAsync(CancellationToken? cancelToken)
```
Accepts an instance of an EventMessageBase sub-type and places it on the backend structure(route) that has been pre-configured for that particular event.  

#### StopListening  
##### Definition  
```csharp
void StopListeningAsync()
```
Accepts an instance of an EventMessageBase sub-type and places it on the backend structure(route) that has been pre-configured for that particular event.  

---
### IConnection  

Next, the Data Contracts:

### DispositionKind
An enum that describes how a message should be dealt with by the subscriber after a the handler has tried to process it.
```csharp
public enum DispositionKind
{
  Acknowledge,  // The message was processed successfully and can be removed from the backend.
  Requeue,      // The message was not handled successfully because of a temporary condition in the handler.  Put the message back on its configured channel.
  DeadLetter    // The message will never be handled successfully due to either a fault in the message itself or an unrecoverable condition in the handler's services.
}
```

### RouteConfigurationBase

### MessageBase

### EventMessageBase  

### CommandMessageBase  