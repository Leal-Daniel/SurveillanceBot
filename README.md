# Table of Contents
- [Table of Contents](#table-of-contents)
- [Pet Surveillance Bot](#pet-surveillance-bot)
  - [High-Level Overview](#high-level-overview)

# Pet Surveillance Bot
**Project Goal:** Using *only* C#, create a publicly accessible live server to control the movement of a streaming robotic device that has pet recognition and movement detection capabilities. 
- Design: [Design/SoftwareDesign.md](/Design/SoftwareDesign.md)
- Implementation: [Source/]()

## High-Level Overview
```mermaid
graph TD
  EP[Entry Point]
  PVH[Possess Valid HW]
  DS[Deploy SW]
  ACC[Activate Camera Content\nand apply Object Detection]
  AL[Activate Logger]
  SCC[Stream Camera Content]
  ALS[Activate Live Server\nand Joystick]
  DLS[Display Stream\nand Joystick Content]
  HR[HTTP Request to Server]
  RH[Request Handling]
  A{Authentication}
  AWS[Access Website]
  AcLS[Access Live Stream\nand Joystick]
  LL[Log Logins]

  EP ===> PVH
  PVH --> DS

  subgraph Controller
    DS --> AL
    DS --> ACC
    DS --> ALS

    subgraph Logger
      AL --> LL
    end

    subgraph ASP.NET Server
      ALS --> DLS
    end

    subgraph Camera Content
      ACC -->  SCC
      SCC --> DLS
    end
    
  end
  
  HR --> RH
  RH --> A
  
  subgraph Client-Server Interaction
    A -->|Success| AWS
    AWS --> AcLS
    AcLS --> DLS
    AWS --> LL
  end
```
