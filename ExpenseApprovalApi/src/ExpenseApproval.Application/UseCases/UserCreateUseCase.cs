using ExpenseApproval.Application.DTOs;
using ExpenseApproval.Application.Interfaces;
using ExpenseApproval.Domain.Entities;
using ExpenseApproval.Domain.Interfaces;

namespace ExpenseApproval.Application.UseCases;

public class UserCreateUseCase : IUserCreateUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserCreateUseCase(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    public async Task<UserDto> ExecuteAsync(CreateUserDto dto)
    {
        var role = await _roleRepository.GetByIdAsync(dto.RoleId)
            ?? throw new KeyNotFoundException($"Role {dto.RoleId} not found.");

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            Auth0Id = dto.Auth0Id,
            Email = dto.Email,
            FullName = dto.FullName,
            RoleId = dto.RoleId
        };

        var created = await _userRepository.AddAsync(user);
        created.Role = role;
        return UserMapper.MapToDto(created);
    }
}
