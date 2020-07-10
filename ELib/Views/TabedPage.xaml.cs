using ELib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ELib.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabedPage : TabbedPage
    {
        private LogIn user;
        public TabedPage(LogIn user)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            this.user = user;
            BarBackgroundColor = Color.White;
            BarTextColor = Color.Black;
           // this.Children.Add(new HomePage());
            this.Children.Add(new BookB(user));
            this.Children.Add(new Donate(user));
            this.Children.Add(new BackDonate(user));
            this.Children.Add(new BookBBack(user));
        }
    }
}