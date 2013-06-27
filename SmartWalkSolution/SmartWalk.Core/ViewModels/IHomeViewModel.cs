using System;
using System.Collections.Generic;

namespace SmartWalk.Core.ViewModels
{
	public interface IHomeViewModel
	{
		IEnumerable<Organization> Organizations { get; }
	}
}