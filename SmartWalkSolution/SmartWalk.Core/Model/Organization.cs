using System;
using Cirrious.MvvmCross.ViewModels;

namespace SmartWalk.Core.Model
{
    public class Organization : MvxViewModel
	{
		public Organization()
		{
		}

        public string Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

        public string Logo { get; set; }
	}
}