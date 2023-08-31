# CQRS & Event Sourcing

Implement CQRS and Event Sourcing.

## Get Started 🚀

Deploy required infrastructure

```shell
...\> docker compose up -d
```

Run Command API at ...\CQRSES.Command.Api>

```shell
dotnet run --launch-profile=https
```

Run Query API at ...\CQRSES.Query.Api>

```shell
dotnet run --launch-profile=https
```

Test Command API

```shell
https://localhost:7090/swagger
```

Test Query API

```shell
https://localhost:7091/swagger
```

Remove deployed infrastructure

```shell
...\> docker compose down --volume
```