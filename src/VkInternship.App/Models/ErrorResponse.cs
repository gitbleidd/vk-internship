﻿namespace VkInternship.App.Models;

public enum ErrorResponseCode
{
    UserNotFound = 0,
    UserNameTaken,
    UnknownUserGroup,
    AdminAlreadyExists,
}

public record ErrorResponse(ErrorResponseCode Code, string Description);