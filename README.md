# Access to the World

[![main status](https://github.com/atmoos/World/actions/workflows/dotnet.yml/badge.svg)](https://github.com/atmoos/World/actions/workflows/dotnet.yml)
[![nuget package](https://img.shields.io/nuget/v/Atmoos.World.svg?logo=nuget)](https://www.nuget.org/packages/Atmoos.World)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/atmoos/World/blob/main/LICENSE)

An abstraction over all worldly goodness, like IO and time.

## [Atmoos.World](./source/Atmoos.World)

Contains basic abstractions for access to the world through file systems and time.

### The [FileSystem](./source/Atmoos.World/IFileSystem.cs)

Firstly, the file system interface is composed of many more narrow interfaces. This gives designers the ability to specify what "permissions" a component that's dependent on a file system may have.

Secondly, any operation on the file system itself is rooted on some already known directory, similar to always specifying absolute paths. This removes the ambiguity of accessing paths that don't exist and confusion relative to what path some other path may be. Examples are given further below and in the corresponding [readme](./source/Atmoos.World/readme.md) of that project.

### [Time](./source/Atmoos.World/ITime.cs)

The most basic access to time, or a clock imaginable. It provides a point in time `Now` as a [`DateTime`](https://learn.microsoft.com/en-gb/dotnet/api/system.datetime) instance in UTC and a means to measure elapsed time via [`TimeSpan`](https://learn.microsoft.com/en-gb/dotnet/api/system.timespan).

## [Atmoos.World.IO](./source/Atmoos.World.IO)

This implementation targets the actual file system, i.e. operations will be persisted to actual directories and files. The implementation is agnostic toward the underlying file system and operating system.

## [Atmoos.World.InMemory](./source/Atmoos.World.InMemory)

As the name implies, all operations are done purely in memory. When a process ends, noting is persisted. Use cases may vary, but one that seems obvious is using this implementation for unit tests.

Provides an in memory file system and time that's detached from the actual wall clock.

## [Atmoos.World.Time](./source/Atmoos.World.Time)

Perhaps the most fundamental force in this world: *Time*. This implementation targets the actual clock.

## [Examples](./source/Atmoos.World.Test/Examples.cs)

For a full set of examples, please refer to [Examples](./source/Atmoos.World.Test/Examples.cs).

For the following set of examples it doesn't matter on what OS they'd be executed. For illustrative purposes equivalent paths are given for windows and unix-like operating systems. For windows, please assume the current volume is `C:`.

```csharp
using Atmoos.World.IO.FileSystem;

// Unix: "/path/to/directory"
// Windows: "C:\path\to\directory"
Path absPath = Path.Abs(Current.Root, "path", "to", "directory");

IDirectory parent = /* say at /this/path */;
// Unix: "/this/path/is/somewhere/else"
// Windows: "C:\this\path\is\somewhere\else"
Path relPathA = Path.Abs(parent, "is", "somewhere", "else");

// Unix equivalent : "./relative/to/current"
// Windows equivalent : ".\relative\to\current"
Path relPathB = Path.Rel<Current>("relative", "to", "current");

// Unix equivalent : "../../../relative/to/distant/antecedent"
// Windows equivalent : "..\..\..\relative\to\distant\antecedent"
Path relPathC = Path.Rel<Current>(3, "relative", "to", "distant", "antecedent");

// Unix: "/path/to/directory/readme.md"
// Windows: "C:\path\to\directory\readme.md"
FilePath filePath = absPath +  new FileName("readme", Extension: "md");

// Given a file path or path, create files and directories.
IFile file = Current.Create(filePath);

IDirectory directory = Current.Create(relPathA);

// Actual IO happens via the standard stream infrastructure.
using StreamWriter writer = file.AppendText();
writer.WriteLine("Hello World!");
```
