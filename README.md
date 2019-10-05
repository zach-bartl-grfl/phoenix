# phoenix

## Components
1. `phx_mongo`: Database
2. `phx_api`: Phoenix Api
3. `phx_sync`: Phoenix Third Party Sync Microservices

## Areas of Interest
1. Business logic: `phoenix.requests.(Customers/Orders).*(Command/Query).cs`
2. Traceability API event handlers: `phoenix.requests.(Customers/Orders).Sync*ToLeviathan.cs`
3. Traceability API error handling / retry logic: `phoenix.core.Http.BaseRestClient.cs`
4. Sync "Microservice": `phoenix.sync.LeviathanSync.cs`
5. Retry "Microservice": `phoenix.sync.LeviathanRetry.cs`

## Run
1. `> docker-compose build && docker-compose up`

## TODO
* Continue building out parity support for entire Leviathan API
* Mock out a second traceability provider to ensure loose coupling of Phoenix to any specific traceability API.
* Setup integration test suite ala https://medium.com/@zbartl/authentication-and-asp-net-core-integration-testing-using-testserver-15d47b03045a
* Add Swagger support
* Build React SPA on top of Phoenix.
* Utilize SignalR to notify React SPA of sync issues with any traceability API.
