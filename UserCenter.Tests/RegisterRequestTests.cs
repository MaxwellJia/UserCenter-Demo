﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoFixture;
using UserCenter.Core.DTOs.Auth;
using Xunit;

public class RegisterUserDtoTests
{
    private bool ValidateModel(object model, out List<ValidationResult> results)
    {
        var context = new ValidationContext(model);
        results = new List<ValidationResult>();
        return Validator.TryValidateObject(model, context, results, true);
    }

    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public void AutoGeneratedValidRequest_ShouldPassValidation()
    {
        var request = _fixture.Build<RegisterUserDto>()
            .With(x => x.Username, "User_123")
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "Secure@123")
            .Create();

        var isValid = ValidateModel(request, out var results);
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(3)]    // too short
    [InlineData(40)]   // too long
    public void AutoGeneratedUsernameLengthViolation_ShouldFailValidation(int length)
    {
        string username = new string('a', length);
        var request = _fixture.Build<RegisterUserDto>()
            .With(x => x.Username, username)
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "Secure@123")
            .Create();

        var isValid = ValidateModel(request, out var results);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterUserDto.Username)));
    }

    [Theory]
    [InlineData("user@name")]
    [InlineData("user name")]
    [InlineData("user#name")]
    [InlineData("name=evil")]
    public void AutoGeneratedUsernameSpecialChars_ShouldFailValidation(string username)
    {
        var request = _fixture.Build<RegisterUserDto>()
            .With(x => x.Username, username)
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, "Secure@123")
            .Create();

        var isValid = ValidateModel(request, out var results);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterUserDto.Username)));
    }

    [Theory]
    [InlineData("nodigitPASS!")]
    [InlineData("nosymbol123A")]
    [InlineData("nolowercase@123")]
    [InlineData("NOUPPERCASE@123")]
    [InlineData("Short1!")]     // only 7 chars
    [InlineData("12345678")]    // no special chars
    [InlineData("!@#$%^&*()")]  // no letters or digits
    [InlineData("LongPassword123!LongPassword123!LongPassword123!LongPassword123!LongPassword123!LongPassword123!LongPassword123!")]
    public void AutoGeneratedWeakPassword_ShouldFailValidation(string password)
    {
        var request = _fixture.Build<RegisterUserDto>()
            .With(x => x.Username, "validuser")
            .With(x => x.Email, "user@example.com")
            .With(x => x.Password, password)
            .Create();

        var isValid = ValidateModel(request, out var results);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterUserDto.Password)));
    }

    [Theory]
    [InlineData("user@com")]       // invalid TLD
    [InlineData("user@.com")]      // invalid domain
    [InlineData("user@domain.")]   // trailing dot
    [InlineData("user.com")]       // missing @
    public void AutoGeneratedInvalidEmail_ShouldFailValidation(string email)
    {
        var request = _fixture.Build<RegisterUserDto>()
            .With(x => x.Username, "validuser")
            .With(x => x.Password, "Secure@123")
            .With(x => x.Email, email)
            .Create();

        var isValid = ValidateModel(request, out var results);
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterUserDto.Email)));
    }

    [Fact]
    public void AutoGeneratedEmptyFields_ShouldFailValidation()
    {
        var request = new RegisterUserDto
        {
            Username = null,
            Password = null,
            Email = null
        };

        var isValid = ValidateModel(request, out var results);
        Assert.False(isValid);
        Assert.Equal(3, results.Count);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterUserDto.Username)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterUserDto.Password)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterUserDto.Email)));
    }
}
