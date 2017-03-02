using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSmartProject.ViewModel;

using Xamarin.Forms;

namespace TestSmartProject
{
	public partial class RegistrationListPage 
	{
        public RegistrationListPage()
		{
			InitializeComponent ();
            BindingContext = new ListViewMainPage() { Navigation = this.Navigation };
		}
	}
}
