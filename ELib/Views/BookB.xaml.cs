using ELib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ELib.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookB : ContentPage
    {
        public ObservableCollection<PrintBook> list { get; set; }
        private List<PrintBook> posdef = null;
        private LogIn user;
        public BookB(LogIn user)
        {
            this.user = user;
            InitializeComponent();
            Request();


            if (!posdef.Equals(null))
            {
                list = new ObservableCollection<PrintBook>();
                foreach (var i in posdef)
                {
                    list.Add(i);
                }

                listView.ItemsSource = list;
            }


            
        }


        public void Request()
        {
            var apireq =
                WebRequest.Create("http://elibrarysamos.azurewebsites.net/api/books/getallbooks") as HttpWebRequest;
            var apiresp = "";

            using (var response = apireq.GetResponse() as HttpWebResponse)
            {
                var reader = new StreamReader(response.GetResponseStream());
                apiresp = reader.ReadToEnd();
            }

            posdef = JsonConvert.DeserializeObject<List<PrintBook>>(apiresp);


        }
        async private void Book(object sender, EventArgs e)
        {
            PrintBook d = (PrintBook)listView.SelectedItem;

            var bookL = new LendBook();

            bookL.Book = d;
            bookL.ReturnDate = DateTime.Now;
            bookL.Username = user.name;

            try
            {
                MyWebRequest request = new MyWebRequest();

                await request.OnAdd(bookL, "lendbook");
                if (request.Get_Confirmation().Contains("OK"))
                {
                    await ShowMessage("Succesfully Booked", "Alert", "Ok", async () =>
                    {
                        this.Navigation.PushAsync(new TabedPage(user), true);

                    });

                }
            }
            catch (Exception exception)
            {

                await ShowMessage("Something went wrong!", "Alert", "Ok", async () =>
                {
                    //this.Navigation.PushAsync(new Register(), true);
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