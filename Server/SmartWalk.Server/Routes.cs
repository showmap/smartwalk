using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace SmartWalk.Server
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {                
                new RouteDescriptor {   
                    Priority = 1,
                    Route = new Route("api",
                        new RouteValueDictionary {
                        {"area", "SmartWalk.Server"},
                        {"controller", "Api"},
                        {"action", "Query"},
                    },
                    new RouteValueDictionary (),
                    new RouteValueDictionary {
                        {"area", "SmartWalk.Server"}
                    },
                    new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Priority = 1,
                    Route = new Route(
                        "events",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "List"}
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
                    Priority = 2,
                    Route = new Route(
                        "event/{eventId}",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Event"},
                            {"action", "Edit"},
                            {"eventId", "{eventId}"}
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
                            {"action", "List"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },  
                new RouteDescriptor {
                    Name = "CreateHost",
                    Priority = 1,
                    Route = new Route(
                        "organizers/create",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Host"},
                            {"action", "Edit"},
                            {"entityId", "0"}
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
                            {"action", "List"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },  
                new RouteDescriptor {
                    Name = "CreateVenue",
                    Priority = 1,
                    Route = new Route(
                        "venues/create",
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"},
                            {"controller", "Venue"},
                            {"action", "Edit"},
                            {"entityId", "0"}
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
                            {"action", "Edit"},
                            {"entityId", "{entityId}"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "SmartWalk.Server"}
                        },
                        new MvcRouteHandler())
                },   
            };
        }
    }
}