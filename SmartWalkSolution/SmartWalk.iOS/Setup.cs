using System;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using SmartWalk.Core;
using SmartWalk.Core.Services;
using SmartWalk.iOS.Services;

namespace SmartWalk.iOS
{
	public class Setup : MvxTouchSetup
	{
		public Setup (MvxApplicationDelegate appDelegate, IMvxTouchViewPresenter presenter)
			: base(appDelegate, presenter)
		{
		}

		protected override IMvxApplication CreateApp ()
		{
            Mvx.RegisterType<IOrganizationService, OrganizationService>();

			return new SmartWalkApplication();
		}
	}
}