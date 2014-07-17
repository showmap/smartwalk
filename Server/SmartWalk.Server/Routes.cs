﻿using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;
using SmartWalk.Server.ViewModels;
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
                            {"Parameters.Display", DisplayType.All.ToString()},
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
                            {"action", "Edit"},
                            {"eventId", "0"}
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
                            {"Parameters.Display", DisplayType.My.ToString()},
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
                            {"eventId", "{eventId}"}
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
                            {"entityId", "{eventId}"}
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
                            {"Parameters.Display", DisplayType.All.ToString()}
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
                            {"Parameters.Display", DisplayType.My.ToString()},
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
                            {"Parameters.Display", DisplayType.All.ToString()}
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
                            {"Parameters.Display", DisplayType.My.ToString()},
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
                }
            };
        }
    }
}