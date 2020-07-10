using ELib.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace ELib.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Register : ContentPage
    {
        private string status;
        public Register()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void Register_OnClicked(object sender, EventArgs e)
        {
            var user = new LogIn();

            user.mail = Mail.Text;
            user.password = Password.Text;
            user.name = Name.Text;
            user.surrname = Surrname.Text;
            user.phone = Phone.Text;


            if (Password.Text == Password2.Text)
            {
                try
                {
                    MyWebRequest request = new MyWebRequest();

                    await request.OnAdd(user, "register");
                    if (request.Get_Confirmation().Contains("OK"))
                    {
                        await ShowMessage("Succesfully register", "Alert", "Ok", async () =>
                        {
                            this.Navigation.PushAsync(new TestLog(), true);
                        });

                    }
                }
                catch (Exception exception)
                {

                    await ShowMessage("This e-mail is already in use!", "Alert", "Ok", async () =>
                    {
                        this.Navigation.PushAsync(new Register(), true);
                    });


                }


            }
            else
            {
                await ShowMessage("Not matched passwords", "Alert", "Ok", async () =>
                {
                    this.Navigation.PushAsync(new Register(), true);
                });
            }


        }

        public async Task ShowMessage(string message,
            string title,
            string buttonText,
            Action afterHideCallback)
        {
            await DisplayAlert(
                title,
                message,
                buttonText);

            afterHideCallback?.Invoke();
        }
    }
}
