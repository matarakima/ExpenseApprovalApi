using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Application.UseCases;
using ExpenseApproval.Domain.Interfaces;
using ExpenseApproval.Infrastructure.Data;
using ExpenseApproval.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseApproval.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString, sql =>
                    sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IExpenseRequestRepository, ExpenseRequestRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            // Expense Request UseCases
            services.AddScoped<IExpenseRequestGetAllUseCase, ExpenseRequestGetAllUseCase>();
            services.AddScoped<IExpenseRequestGetByIdUseCase, ExpenseRequestGetByIdUseCase>();
            services.AddScoped<IExpenseRequestCreateUseCase, ExpenseRequestCreateUseCase>();
            services.AddScoped<IExpenseRequestUpdateUseCase, ExpenseRequestUpdateUseCase>();
            services.AddScoped<IExpenseRequestApproveUseCase, ExpenseRequestApproveUseCase>();
            services.AddScoped<IExpenseRequestRejectUseCase, ExpenseRequestRejectUseCase>();
            services.AddScoped<IExpenseRequestFilterUseCase, ExpenseRequestFilterUseCase>();
            services.AddScoped<IExpenseRequestGetMetricsUseCase, ExpenseRequestGetMetricsUseCase>();

            // Role UseCases
            services.AddScoped<IRoleGetAllUseCase, RoleGetAllUseCase>();
            services.AddScoped<IRoleGetByIdUseCase, RoleGetByIdUseCase>();
            services.AddScoped<IRoleCreateUseCase, RoleCreateUseCase>();
            services.AddScoped<IRoleAddClaimUseCase, RoleAddClaimUseCase>();
            services.AddScoped<IRoleRemoveClaimUseCase, RoleRemoveClaimUseCase>();

            // User UseCases
            services.AddScoped<IUserGetAllUseCase, UserGetAllUseCase>();
            services.AddScoped<IUserGetByIdUseCase, UserGetByIdUseCase>();
            services.AddScoped<IUserGetByAuth0IdUseCase, UserGetByAuth0IdUseCase>();
            services.AddScoped<IUserCreateUseCase, UserCreateUseCase>();

            // FluentValidation
            services.AddValidatorsFromAssemblyContaining<Application.Validators.CreateExpenseRequestValidator>();

            return services;
        }
    }
}
