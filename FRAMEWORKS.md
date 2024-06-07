# Frameworks
Let's talk a little bit about Frameworks.  I know a lot of architects really hate the idea of formal framework projects or packages, and their points are somewhat valid.

### A few arguments against "Frameworks":
- **POINT:** They hide low-level implementation details from less seasoned developers, robbing them of the opportunity to learn about some aspect of application development.
- **POINT:** Most Framework projects get polluted with code that changes more frequently than the components that depend on them.  This creates needless code churn throughout the system, when our goal should be to contain that code-level volatility as much as possible.
- **POINT:** Framework projects come with a maintenance cost that must be paid alongside the rest of the application.

I agree with all of those points, but in my opinion, a carefully considered and strictly governed, and _MINIMAL_ application framework can help smooth over a lot of problems that commonly appear while developing large systems.

- **COUNTERPOINT: (Hiding details from the dev team)**  
  Yes, framework code can hide low-level implementation details from my less experienced developers.  I don't want those developers concerning themselves with C# internals, or the nitty-gritty details of whatever Logging Package we're using.  I want to consolidate and standardize those boilerplate details away from them, so that they can focus on implementing business logic instead of plumbing and wiring.  

> Diving into the language or platform internals is an important part of the career path, and opportunities to work at that level of code should be provided. These aspects should not however, be part of a junior developer's day-to-day cognitive load.

- **COUNTERPOINT: (Pollution of Frameworks with Domain Logic)**  
This is absolutely a risk.  The only way to mitigate this "domain drift" is careful code review throughout the development process.  As an Architect or Tech Lead, it's up to us to understand WHY we don't let domain logic leak into our System Framework.

> There are two "levels" of framework that I advocate for.  At the very bottom, the foundation of our system, is a set of classes and interfaces that standardize things like Domain Entity specification, Transmission of these Domain Entities between the inner parts of our System, and low-level matters such as serialization and deserialization.  This Framework should manifest as a nuget package (or even set of packages) your System Projects have as a dependency.  

> The second level of "framework" that I recommend exists as a project in your solution that defines artifacts and utility classes that must be available throughout your system.  This would include any Constants used in exchanging Queued Messages along a MessageBus, Logging methods, and so on.  
> This Domain-Level Framework explicitly excludes DomainEntity definitions, interfaces that define use cases, compute activities, or storage access tasks. 