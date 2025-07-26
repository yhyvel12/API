var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<User> users = new();

// 🟢 Create
app.MapPost("/users", (User user) =>
{
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

// 🟡 Update
app.MapPut("/users/{username}", (string username, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.Username == username);
    if (user == null) return Results.NotFound();

    user.Username = updatedUser.Username;
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

public class User
{
    required public string Username { get; set; }
    public int UserAge { get; set; }
}