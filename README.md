# Table of Contents
- [Table of Contents](#table-of-contents)
- [Surveillance Bot](#surveillance-bot)
  - [Installation and Deployment](#installation-and-deployment)
    - [Windows](#windows)
  - [High-Level Overview](#high-level-overview)

# Surveillance Bot
**Project Goal:** Using *only* C#, create a publicly accessible live server to control the movement of a streaming robotic device that has pet/person recognition and movement detection capabilities.
- Installation: [Build/](/Build/)
- Implementation: [Source/](/Source/)
- Documentation: [Documentation/SoftwareDesign.md](/Documentation/SoftwareDesign.md)
  - Investigations: [Documentations/Investigations/](/Documentation/Investigations/)
  
## Installation and Deployment
### Windows
1. Install [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.300-windows-x64-installer) and run the deployment script via PowerShell:
   
   ```powershell
   git clone https://github.com/Leal-Daniel/SurveillanceBot.git $env:USERPROFILE\Downloads\repo
   cd $env:USERPROFILE
   .\Downloads\repo\Build\WindowsDeployment.ps1
   ```
3. Setup online video monitoring using tunneling:
   1. The previous script should have opened a website with URL `http://localhost:[PORT_NUMBER]`.
   2. Activate the tunnel. This will output the URL your server is now running on:
  
      ```powershell
      lt --port [PORT_NUMBER]
      ```
   3. Get the tunnel password to access the running server:
   
      ```powershell
      curl https://loca.lt/mytunnelpassword
      ```
   4. Paste the password into the box prompted by your server URL and you should now have access to it from anywhere!

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
