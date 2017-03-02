using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using TestSmartProject.Model;

namespace TestSmartProject.ViewModel
{
    public class ViewModelMainPage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        ListViewMainPage lvm;

        public ModelMainPage User { get; private set; }

        public ViewModelMainPage()
        {
            User = new ModelMainPage();
        }

        public ListViewMainPage ListViewModel
        {
            get { return lvm; }
            set
            {
                lvm = value;
                OnPropertyChanged("ListViewModel");
            }
        }
        public string Name
        {
            get { return User.Name; }
            set
            {
                User.Name = value;
                OnPropertyChanged("Name");
            }
        }
        public string IpAdress
        {
            get { return User.IpAdress; }
            set
            {
                User.IpAdress = value;
                OnPropertyChanged("IpAdress");
            }
        }
        public string Group
        {
            get { return User.Group; }
            set
            {
                User.Group = value;
                OnPropertyChanged("Group");
            }
        }

 
        public bool IsValid
        {
            get
            {
                return ((!string.IsNullOrEmpty(Name.Trim())) ||
                    (!string.IsNullOrEmpty(Group.Trim())) || 
                    (!string.IsNullOrEmpty(Name.Trim())));
            }
        }
        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}

