using System;
using MonoTouch.UIKit;

namespace SmartWalkiOS
{
	public class HomeController : UITableViewController
	{
		public HomeController () : base(UITableViewStyle.Plain)
		{
			NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Add), true);
			NavigationItem.LeftBarButtonItem.Clicked += (s, e) => 
			{ 
				System.Console.WriteLine("Hello Worlds!");
			};
		}
	}
}