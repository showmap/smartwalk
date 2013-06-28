using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core.Model;
using SmartWalk.Core.Services;

namespace SmartWalk.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel, IHomeViewModel
	{
		private readonly ISmartWalkDataService _organizationService;

		private IEnumerable<Organization> _organizations;

		public HomeViewModel(ISmartWalkDataService organizationService)
		{
			_organizationService = organizationService;
		}

		public IEnumerable<Organization> Organizations 
		{
			get
			{
				return _organizations;
			}
			set
			{
				_organizations = value;
				RaisePropertyChanged(() => Organizations);
			}
		}

		public override void Start()
		{
			UpdateOrganizations();
			base.Start();
		}

		private void UpdateOrganizations()
		{
			_organizationService.GetOrgs((orgs, ex) => 
          		{
					if (ex == null) 
					{
						Organizations = orgs;
					}
					else 
					{
						// TODO: handling
					}
				});
		}
	}
}