// using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using CalendarBackend.Models;
using CalendarBackend.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace CalendarBackend.Routes;

// /api/auth
internal class AuthRoutes
{


    private IResult ValidationErrorsReponse(ValidationResult results)
    {
        var dic = results.ToDictionary();

        // return Results.ValidationProblem(new {
        //     [{status = ""}]
        // });

        return Results.Json(
            new
            {
                errors = dic,
                status = 400
            },
            statusCode: 400
        );
    }

    public async Task<IResult> Login(UsersService usersService, Login credentials, JwtManager jwtManager)
    {
        var (email, password) = credentials;

        var user = await usersService.GetAsync(x => x.Email == email);
        
        if (user == null)
        {
            return Results.BadRequest(
                new
                {
                    ok = false,
                    message = "Oops! Usuario y contraseña no coinciden"
                }
            );
        }

        bool validPassword = usersService.PasswordMatches(user, password);

        if (!validPassword)
        {
            return Results.BadRequest(
                new {
                    ok = false,
                    message = "Oops! Usuario y contraseña no coinciden"
                }
            );
        }

        // JWT Generation
        var jwt = jwtManager.GenerateUserJwtToken(user);


        return Results.Ok(
            new
            {
                ok = true,
                uid = user.Id,
                name = user.Name,
                token = jwt
            }
        );
    }

    public async Task<IResult> AddNew(UsersService usersService, IValidator<UserModel> validator, UserModel postedUser)
    {
        var validationResult = await validator.ValidateAsync(postedUser);
        // await validator.ValidateAndThrowAsync(user);

        if (!validationResult.IsValid)
        {
            return ValidationErrorsReponse(validationResult);
        }

        var existingUser = await usersService.GetAsync(x => x.Email == postedUser.Email);

        if (existingUser != null)
        {

            return Results.BadRequest(
                new
                {
                    ok = false,
                    message = "Un usuario ya existe con ese correo"
                }
            );
        }

        await usersService.CreateAsync(postedUser);


        // var (Name, Email, Password) = user;
        // return Results.Created<UserModel>()
        return Results.Json(
            new
            {
                ok = true,
                uid = postedUser.Id,
                name = postedUser.Name
            },
            statusCode: 201
        );
    }

    [Authorize("user_logged")]
    public async Task<IResult> Renew(HttpContext context, UsersService usersService, JwtManager jwtManager)
    {
        var claims = context.User.Claims;
        var uid = claims.First(x => x.Type == ClaimTypes.NameIdentifier)!.Value;

        var user = await usersService.GetAsync( uid );
        var jwt = jwtManager.GenerateUserJwtToken(user!);
        
        return Results.Json(
            new
            {
                ok = true,
                uid= uid,
                name= user!.Name,
                token = jwt
            }
        );
    }
}