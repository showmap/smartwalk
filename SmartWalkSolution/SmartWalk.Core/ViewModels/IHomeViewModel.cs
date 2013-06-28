using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;

namespace SmartWalk.Core.ViewModels
{
    public interface IHomeViewModel : IMvxViewModel
	{
		IEnumerable<Organization> Organizations { get; }
	}
}