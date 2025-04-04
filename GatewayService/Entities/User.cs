﻿
namespace GatewayService.Entities
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
    public class UserLogin
    {
        public required string Name { get; set; }
        public required string Pass { get; set; }
    }

    public class UserCreateModel
    {
        public required string Password { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
    }

    public class JWTAndUser
    {
        public required string Token { get; set; }
        public required UserDTO User { get; set; }
    }

    public class UserUpdateModel
    {
        public int Id { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
