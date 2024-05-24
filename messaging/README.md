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

When using messaging, it's helpful to think about your system's messages as being one of two kinds:  **Commands** and **Events**.  There are some helpful rules and constraints to use when you're dealing with each kind of message.  

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
