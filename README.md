# useful-tidbits
There are a lot of bits of code that I find I have to write way too often and I'm sick of it.  I'm gonna start dropping these bits in here so I have easy access to them as needed.  Be aware than most of this content will be in C#.  If I dive into any other languages regularly, I'll be sure to call those out as I add them.

I'll be organizing the repository along topics or particular technologies, with each topic holding a root-level folder, which will be further organized into sub-topics.

> **This repository DOES contain some C# code that I use often.  That said, trying to Clone or Fork this repo and turn it into a cohesive project or solution is going to be a pain.  Instead, I'd recommend you find the source files that you need and just copy/paste the interfaces and classes into your own projects, then modify them as necessary.**

## Topics
### [Object Model](/serialization/README.md)
We'll talk about the concept of getting our application data from one component to the others, especially when transferring that data across process boundaries.  

(When our system is composed of a number of distributed and self-hosted components, we need some way of passing data and important metadata between these components.)
### [Messaging](/messaging/README.md)  
In this topic, I explore message based architecture, along with discussing tradeoffs.  The interfaces and stub implementations that I'm sick of typing in all the time will be in the /src subfolder.