using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSmartProject.ViewModel;

using Xamarin.Forms;

namespace TestSmartProject
{
	public partial class ViewKosmopark 
	{
        private ListViewMainPage Page;

		public ViewKosmopark (ListViewMainPage page)
		{
			InitializeComponent ();
            Page = page;
            BindingContext = page;
		}
	}
}
