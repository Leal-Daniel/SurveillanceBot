# Table of Contents
- [Table of Contents](#table-of-contents)
- [Surveillance Bot](#surveillance-bot)
  - [Installation and Deployment](#installation-and-deployment)
    - [Windows](#windows)
  - [High-Level Overview](#high-level-overview)

# Surveillance Bot
**Project Goal:** Using *only* C#, create a publicly accessible live web server to control the movement of a streaming robotic device that has pet/person recognition and movement detection capabilities.
- Installation: [Build/](/Build/)
- Implementation: [Source/](/Source/)
- Documentation: [Documentation/SoftwareDesign.md](/Documentation/SoftwareDesign.md)
  - Investigations: [Documentations/Investigations/](/Documentation/Investigations/)

![alt text](/Documentation//Investigations//Data/surveillance.gif)

## Installation and Deployment
### Windows
1. Install [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.300-windows-x64-installer) and run the deployment script via PowerShell:
   
   ```powershell
   git clone https://github.com/Leal-Daniel/SurveillanceBot.git $env:USERPROFILE\Downloads\repo
   cd $env:USERPROFILE
   dotnet build $env:USERPROFILE\Downloads\repo\Source\SurveillanceWebServer -v q
   dotnet watch run --project $env:USERPROFILE\Downloads\repo\Source\SurveillanceWebServer

   ```

   **NOTE:** This should open a website with URL `http://localhost:1225` when completed.
2. Setup online web server stream by port forwarding `1225` and modifying the IP address in [line 18 of index.cshtml](/Source/SurveillanceWebServer/Views/Home/Index.cshtml).

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
