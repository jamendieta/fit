# GenAI Notes

## Prompt Used
Generate a .NET 10 REST API for a task management system using Clean Architecture style with Core, Application, Infrastructure, and Presentation projects. Use SQLite without Entity Framework, Dapper, or MediatR. Include CRUD endpoints for tasks (title, description, status, due_date, userId), register/login endpoints, JWT authentication, and public/private endpoints. Add business validations, seed demo data, and xUnit tests for business logic, data layer, and API endpoints.

## Representative Output Sample (edited)
```csharp
[HttpPost("login")]
[AllowAnonymous]
public async Task<IActionResult> Login([FromBody] LoginRequest request)
{
    var user = await _userService.ValidateUserAsync(request.Username, request.Password);
    if (user == null)
        return Unauthorized("Invalid credentials");

    var token = _tokenService.CreateToken(user);
    return Ok(new
    {
        token,
        user = new { user.Id, user.Username, user.Email }
    });
}
```

## How the AI Suggestions Were Validated
- Compiled all projects with `dotnet build` until clean.
- Executed full test suite with `dotnet test`.
- Verified frontend build using `npm run build`.
- Checked endpoint security by adding integration tests for anonymous and authenticated routes.

## Corrections and Improvements Applied
- Fixed naming conflicts between entity `Task` and `System.Threading.Tasks.Task`.
- Replaced plain-text password handling with SHA-256 hashing.
- Added JWT token generation and auth middleware wiring.
- Restricted task CRUD access to authenticated user's own tasks.
- Added seed consistency to keep demo credentials stable.

## Edge Cases, Authentication, and Validations
- Reject empty task titles and past due dates during create.
- Reject duplicate usernames during registration.
- Return `Unauthorized` for invalid credentials.
- Return `Forbid` when accessing another user's tasks.
- Include public and authorized endpoints to validate auth behavior.

## Security Notes for Production
- Move JWT secret to secure configuration (environment secrets/key vault).
- Replace SHA-256 with salted password hashing (`PBKDF2`, `bcrypt`, or `Argon2`).
- Add refresh tokens and token revocation strategy.
- Add rate limiting and stricter input validation.
