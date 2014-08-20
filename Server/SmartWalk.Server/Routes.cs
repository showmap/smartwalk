using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using SmartWalk.Server.Utils;
using SmartWalk.Shared;

namespace SmartWalk.Server {

    [UsedImplicitly]
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes()) {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "api",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Api"},
                            {"action", "Query"},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "Events",
                    Priority = 1,
                    Route = new Route(
                        "events",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "List"},
                            {"display", DisplayType.All},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "CreateEvent",
                    Priority = 1,
                    Route = new Route(
                        "events/create",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "Create"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "MyEvents",
                    Priority = 1,
                    Route = new Route(
                        "events/my",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "List"},
                            {"display", DisplayType.My},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 2,
                    Route = new Route(
                        "event/{eventId}",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "View"},
                            {"eventId", "{eventId}"},
                            {"day", null}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "event/{eventId}/day/{day}",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "View"},
                            {"eventId", "{eventId}"},
                            {"day", "{day}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 2,
                    Route = new Route(
                        "event/{eventId}/edit",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "Edit"},
                            {"entityId", "{eventId}"},
                            {"day", null}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 2,
                    Route = new Route(
                        "event/{eventId}/day/{day}/edit",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "Edit"},
                            {"entityId", "{eventId}"},
                            {"day", "{day}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "organizers",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Host"},
                            {"action", "List"},
                            {"display", DisplayType.All}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 2,
                    Route = new Route(
                        "organizers/create",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Host"},
                            {"action", "Create"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "MyHosts",
                    Priority = 1,
                    Route = new Route(
                        "organizers/my",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Host"},
                            {"action", "List"},
                            {"display", DisplayType.My},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 2,
                    Route = new Route(
                        "organizer/{entityId}",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Host"},
                            {"action", "View"},
                            {"entityId", "{entityId}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "organizer/{entityId}/edit",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Host"},
                            {"action", "Edit"},
                            {"entityId", "{entityId}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "venues",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Venue"},
                            {"action", "List"},
                            {"display", DisplayType.All}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 2,
                    Route = new Route(
                        "venues/create",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Venue"},
                            {"action", "Create"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Name = "MyVenues",
                    Priority = 1,
                    Route = new Route(
                        "venues/my",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Venue"},
                            {"action", "List"},
                            {"display", DisplayType.My},
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 2,
                    Route = new Route(
                        "venue/{entityId}",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Venue"},
                            {"action", "View"},
                            {"entityId", "{entityId}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "venue/{entityId}/edit",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Venue"},
                            {"action", "Edit"},
                            {"entityId", "{entityId}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },

                // U S E R S   A C C O U N T

                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "users/account/sign-in",
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"},
                            {"controller", "Account"},
                            {"action", "LogOn"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "users/account/sign-off",
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"},
                            {"controller", "Account"},
                            {"action", "LogOff"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "users/account/register",
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"},
                            {"controller", "Account"},
                            {"action", "Register"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "users/account/request-lost-password",
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"},
                            {"controller", "Account"},
                            {"action", "RequestLostPassword"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "users/account/change-password",
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"},
                            {"controller", "Account"},
                            {"action", "ChangePassword"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Orchard.Users"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "users/account/edit-profile",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Account"},
                            {"action", "EditProfile"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },
                // A walkaround for previous xml data of version 1.0
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "data/{*path}",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Redirect"},
                            {"action", "Index"},
                            {"host", "http://smartwalkstorage.blob.core.windows.net/xmldata/"},
                            {"path", "{path}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
    }
}