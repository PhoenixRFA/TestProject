namespace TestProject.Models;

public record User(string Id, string Name, string Email, string Password, string? Phone = null);
public record UserViewModel(string Id, string Name, string Email, string? Phone);
public record UserLoginModel(string Email, string Password);