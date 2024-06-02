# Atmoos World | Design

Three *abstractions*:

- File IO
- Time
- Networking IO

via static interfaces.

This then allows for *concretions* like:

- *Current*
  - Actual access to underling systems
- *In Memory*
  - In memory representation of "The World"
- *Stateless*
  - Stateless representation of "The World"

## Why Static Interfaces?

We can use type injection, which can be realised in a very compact form.

```csharp
public sealed class Service<TWorld> : IService
    where TWorld : IFileSystem, ITime // others...
{
    // make use of the world :-)
}

```

This frees us from constructor injection of potentially many worldly abstractions and also enables us to inject in memory file systems for testing, etc.
