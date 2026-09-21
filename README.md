# RoomBookingSystem

An app to book a room for a negotiation (meeting).

## Overview

RoomBookingSystem lets users reserve meeting/negotiation rooms and manage bookings through a web API (and, in progress, a web frontend).

## Tech stack

- **Backend:** ASP.NET Core 10 Web API (C#)
- **Frontend:** in progress (see `feature/frontend` branch)
- **Database:** Entity Framework Core (see `feature/db` branch)

## Project structure

```
RoomBookingApp/
  RoomBookingApp.slnx          # Solution file
  RoomBookingApp/               # ASP.NET Core Web API project
```

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the API

```bash
cd RoomBookingApp
dotnet restore
dotnet run --project RoomBookingApp
```

The API will be available at the URL printed in the console (see `RoomBookingApp/Properties/launchSettings.json`). In development, OpenAPI docs are available via `/openapi`.

## Branches

- `main` — stable
- `develop` — active development
- `feature/db` — database/EF Core setup
- `feature/backend` — backend API work
- `feature/frontend` — frontend work

## Status

Early-stage project. The API currently exposes only the default scaffolded endpoint; room booking features are under active development.
