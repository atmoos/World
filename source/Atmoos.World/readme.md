# Atmoos.World

[![nuget package](https://img.shields.io/nuget/v/Atmoos.World.svg?logo=nuget)](https://www.nuget.org/packages/Atmoos.World)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/atmoos/World/blob/main/LICENSE)

Contains basic abstractions for access to the world through file systems and time.

These are:

- [IFileSystem](https://github.com/atmoos/World/blob/main/source/Atmoos.World/IFileSystem.cs)
  - which is composed of narrow "sub-interfaces"
  - enabling a fine grained permission like access to a file system.
- [ITime](https://github.com/atmoos/World/blob/main/source/Atmoos.World/ITime.cs)
  - The most basic access to time, or a clock imaginable.

For examples, please refer to the [tests](https://github.com/atmoos/World/tree/main/source/Atmoos.World.Test) and to [Examples.cs](https://github.com/atmoos/World/blob/main/source/Atmoos.World.Test/Examples.cs).
