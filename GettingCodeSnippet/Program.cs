using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// 🧩 Middleware: Simple Logging
app.Use(async (context, next) =>
{
    Console.WriteLine($"[{DateTime.Now}] {context.Request.Method} {context.Request.Path}");
    await next();
});

List<User> users = new();

// ✅ Validation helper - indi tuple qaytarır
static (bool IsValid, List<ValidationResult> Errors) ValidateUser(User user)
{
    var context = new ValidationContext(user);
    var results = new List<ValidationResult>();
    bool isValid = Validator.TryValidateObject(user, context, results, true);
    return (isValid, results);
}

// 🟢 Create
app.MapPost("/users", (User user) =>
{
    var (isValid, errors) = ValidateUser(user);
    if (!isValid)
        return Results.BadRequest(errors);

    // Username təkrarlanmaması üçün yoxla
    if (users.Any(u => u.Username == user.Username))
        return Results.Conflict($"User with username '{user.Username}' already exists.");

    users.Add(user);
    return Results.Created($"/users/{user.Username}", user);
});

// 🔵 Read All
app.MapGet("/users", () =>
{
    return Results.Ok(users);
});

// 🔵 Read One
app.MapGet("/users/{username}", (string username) =>
{
    var user = users.FirstOrDefault(u => u.Username == username);
    return user != null ? Results.Ok(user) : Results.NotFound();
});

// 🟡 Update - Username dəyişdirilmir, yalnız UserAge yenilənir
app.MapPut("/users/{username}", (string username, User updatedUser) =>
{
    var (isValid, errors) = ValidateUser(updatedUser);
    if (!isValid)
        return Results.BadRequest(errors);

    var user = users.FirstOrDefault(u => u.Username == username);
    if (user == null) return Results.NotFound();

    if (updatedUser.Username != username)
        return Results.BadRequest("Username cannot be changed.");

    user.UserAge = updatedUser.UserAge;
    return Results.Ok(user);
});

// 🔴 Delete
app.MapDelete("/users/{username}", (string username) =>
{
    var user = users.FirstOrDefault(u => u.Username == username);
    if (user == null) return Results.NotFound();

    users.Remove(user);
    return Results.Ok(user);
});

app.Run();

#nullable enable
// ✅ User Model with Validation and nullable annotations
public class User
{
    [Required]
    [MinLength(3)]
    public string Username { get; set; } = null!;

    [Range(0, 120)]
    public int UserAge { get; set; }
}
