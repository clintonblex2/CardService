﻿namespace CardService.Application.Common.Interfaces
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
    }
}
