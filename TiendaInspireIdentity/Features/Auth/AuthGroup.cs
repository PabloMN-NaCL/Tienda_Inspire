//using Asp.Versioning;

//namespace TiendaInspireIdentity.Features.Auth
//{
//    public static class AuthGroup
//    {
//        //Esto es un metodo de extension 
//        public static RouteGroupBuilder MapAuthGroup(this IEndpointRouteBuilder routes)
//        {
//            var versionSet = routes.NewApiVersionSet()
//                .HasApiVersion(new Asp.Versioning.ApiVersion(1, 0))
//                .ReportApiVersions()
//                .Build();


//            var group = routes.MapGroup("/api/v{version:apiVersion}/auth");

//            group.WithApiVersionSet(versionSet);
//            //Nombre creado por nosotros
//            group.WithTags("Auth");

//            return group;
//        }

//    }
//}
