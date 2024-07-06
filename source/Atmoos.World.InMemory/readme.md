# Atmoos.World.InMemory

[![nuget package](https://img.shields.io/nuget/v/Atmoos.World.InMemory.svg?logo=nuget)](https://www.nuget.org/packages/Atmoos.World.InMemory)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/atmoos/World/blob/main/LICENSE)

Instead of providing access to the world, this keeps the world hidden from sight by keeping everything in memory.

The main use case for this library appears to be for unit test purposes.

In other words, as soon as the process ends, so does the representation of the world.

The current implementations are:

- An in memory [UnixFileSystem](https://github.com/atmoos/World/blob/main/source/Atmoos.World.InMemory/IO/UnixFileSystem.cs).
  - Mimics a unix file system.
  - Time must be injected.
- A user controllable "[clock](https://github.com/atmoos/World/blob/main/source/Atmoos.World.InMemory/Time.cs)".
  - The time can be set arbitrarily without affecting system time.
