using System.Collections;
using System.Collections.Generic;
namespace SmartWalk.Shared.DataContracts.Api
{
    public class Response
    {
        public ICollection<object[]> Results { get; set; }
    }
}