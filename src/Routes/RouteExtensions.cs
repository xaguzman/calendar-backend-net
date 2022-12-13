namespace CalendarBackend.Routes;

static class RouteExtensions
{
    public static void MapApi(this WebApplication app){
        var auth = app.MapGroup("/api/auth");
        var authRoutes = new AuthRoutes();
        auth.MapPost("/", authRoutes.Login);
        auth.MapPost("/new", authRoutes.AddNew);
        auth.MapGet("/renew", authRoutes.Renew);

        
        
        var events = app.MapGroup("/api/events").RequireAuthorization("user_logged");

        var eventRoutes = new EventRoutes();
        events.MapGet("/", eventRoutes.GetEvents);
        events.MapPost("/", eventRoutes.CreateEvent);
        events.MapPut("/{id}", eventRoutes.UpdateEvent);
        events.MapDelete("/{id}", eventRoutes.DeleteEvent);

    }
    
}