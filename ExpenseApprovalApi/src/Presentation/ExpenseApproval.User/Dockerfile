FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/ExpenseApproval.Domain/ExpenseApproval.Domain.csproj ExpenseApproval.Domain/
COPY src/ExpenseApproval.Application/ExpenseApproval.Application.csproj ExpenseApproval.Application/
COPY src/ExpenseApproval.Infrastructure/ExpenseApproval.Infrastructure.csproj ExpenseApproval.Infrastructure/
COPY src/Presentation/ExpenseApproval.User/ExpenseApproval.User.csproj Presentation/ExpenseApproval.User/
RUN dotnet restore Presentation/ExpenseApproval.User/ExpenseApproval.User.csproj

COPY src/ExpenseApproval.Domain/ ExpenseApproval.Domain/
COPY src/ExpenseApproval.Application/ ExpenseApproval.Application/
COPY src/ExpenseApproval.Infrastructure/ ExpenseApproval.Infrastructure/
COPY src/Presentation/ExpenseApproval.User/ Presentation/ExpenseApproval.User/
RUN dotnet publish Presentation/ExpenseApproval.User/ExpenseApproval.User.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ExpenseApproval.User.dll"]
