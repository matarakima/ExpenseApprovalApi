FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/ExpenseApproval.Domain/ExpenseApproval.Domain.csproj ExpenseApproval.Domain/
COPY src/ExpenseApproval.Application/ExpenseApproval.Application.csproj ExpenseApproval.Application/
COPY src/ExpenseApproval.Infrastructure/ExpenseApproval.Infrastructure.csproj ExpenseApproval.Infrastructure/
COPY src/Presentation/ExpenseApproval.Api/ExpenseApproval.Api.csproj Presentation/ExpenseApproval.Api/
RUN dotnet restore Presentation/ExpenseApproval.Api/ExpenseApproval.Api.csproj

COPY src/ExpenseApproval.Domain/ ExpenseApproval.Domain/
COPY src/ExpenseApproval.Application/ ExpenseApproval.Application/
COPY src/ExpenseApproval.Infrastructure/ ExpenseApproval.Infrastructure/
COPY src/Presentation/ExpenseApproval.Api/ Presentation/ExpenseApproval.Api/
RUN dotnet publish Presentation/ExpenseApproval.Api/ExpenseApproval.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ExpenseApproval.Api.dll"]
