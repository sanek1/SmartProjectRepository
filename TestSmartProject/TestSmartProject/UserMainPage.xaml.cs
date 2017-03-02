using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSmartProject.ViewModel;
using Xamarin.Forms;


namespace TestSmartProject
{
	public partial class UserMainPage 
	{
        public ViewModelMainPage ViewModel { get; private set; }

        public UserMainPage(ViewModelMainPage vm)
		{
            InitializeComponent();

            ViewModel = vm;
            this.BindingContext = ViewModel;
		}
	}
}
