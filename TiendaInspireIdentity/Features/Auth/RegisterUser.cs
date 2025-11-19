


//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Identity;

//namespace TiendaInspireIdentity.Features.Auth
//{
//    Minimal APIs  solo se usan para OpenApi, no Swagger
//    public static class RegisterUser
//    {
//        public record Request(string nombre, string email, string password);
//        public record Response(string userId, string nombre, string email);

//        public static IEndpointRouteBuilder MapRegisterUser(this IEndpointRouteBuilder group)
//        {
//            var Authgroup = group.MapAuthGroup();

//            Authgroup.MapPost("/register", HandlerAsync)
//                    .WithName("registerUser")
//                    .AllowAnonymous();

//            return group;

//        }

//        private static async Task<IResult> HandlerAsync(Request request, UserManager<IdentityUser> usermanager)
//        {
//            var user = new IdentityUser
//            {
//                UserName = request.nombre,
//                Email = request.email
//            };

//            var result = await usermanager.CreateAsync(user, request.password);

//            return Results.Ok();


//        }
//    }
//}
