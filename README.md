# phoenix

## Components
1. `phx_mongo`: Database
2. `phx_api`: Phoenix Api
3. `phx_sync`: Phoenix Third Party Sync Microservices

## Run
1. `> docker-compose build && docker-compose up`

## TODO
* Continue building out parity support for entire Leviathan API
* Mock out a second traceability provider to ensure loose coupling of Phoenix to any specific traceability API.
* Setup integration test suite ala https://medium.com/@zbartl/authentication-and-asp-net-core-integration-testing-using-testserver-15d47b03045a
* Build React SPA on top of Phoenix.
* Utilize SignalR to notify React SPA of sync issues with any traceability API.
