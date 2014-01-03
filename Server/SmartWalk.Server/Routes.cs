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
                    Priority = 19,
                    Route = new Route(
                        "TestFb",
                        new RouteValueDictionary {
                            {"area", "Store.GsmCounters"},
                            {"controller", "Test"},
                            {"action", "TestFb"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Store.GsmCounters"}
                        },
                        new MvcRouteHandler())
                },
            };
        }
    }
}