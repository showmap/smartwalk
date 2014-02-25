﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using SmartWalk.Shared.DataContracts.Api;

namespace SmartWalk.Server.Services
{
    public interface IQueryService : IDependency {
        Response ExecuteQuery(Request request);
    }
}