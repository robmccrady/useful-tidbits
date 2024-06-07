# Serialization tidbits
It seems like every time I spin up a new solution, I have to rewrite my Json Serialization code for handling payloads.  It's getting old.

Before I dig into all the how, I want to explain WHAT I'm doing here that might be a little different from what you're used to, and WHY I've fallen into these habits.  

## First off:  WHAT.
We're going to define a small handful of base classes that can be used in our systems that will handle the gnarlier parts of serialization and deserialization.  This will allow us to use them in a simple but opinionated communication framework that we can use.

## Next:  WHY?
In a simple system, passing class instances between components directly is a perfectly fine way of doing things, especially if all of your components are loaded and executing within the same process space, or in .Net terms, AppDomain.

If your system design is more complex though, with multiple services handling different aspects of your use cases, you start running into some real head-scratcher questions.  

- "My web backend runs as a managed identity.  How do I pass the actual user's identity through to the File Access service so that we can enforce our RBAC rules from within that service?"
- "My use case is hopping from service to service to service, and something's going wrong somewhere in the middle, and I'm having a heck of a time figuring out which inputs are causing the problem."

Yes, you can use simple DTO classes to pass between the different system services, loading the metadata into Request and Response headers, but I'm usually focused on larger things than implementation details like that, especially when they're not right in front of my face.  Sometimes, my brain feels more egg-like than wrinkly like a raisin.

The type of metadata that needs to be included when passing control to a system service doesn't change all that much, but it does vary from one system to the next.  The PAYLOAD of your service calls will change drastically from one Use Case to another.

Because the NATURE of calling from service to service is relatively static within a small handful of patterns, we can define a handful of classes that will help us make these calls, and put some base classes in our back pockets that will allow us to standardize the way we exchange data within our distributed systems.

