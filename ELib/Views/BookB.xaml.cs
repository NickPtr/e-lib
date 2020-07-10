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
        
        public BookB()
        {
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
                WebRequest.Create("http://elibrarysamos.azurewebsites.net/api/book/getallbooks") as HttpWebRequest;
            var apiresp = "";

            using (var response = apireq.GetResponse() as HttpWebResponse)
            {
                var reader = new StreamReader(response.GetResponseStream());
                apiresp = reader.ReadToEnd();
            }

            posdef = JsonConvert.DeserializeObject<List<PrintBook>>(apiresp);


        }
        private void Book(object sender, EventArgs e)
        {
            PrintBook d = (PrintBook)listView.SelectedItem;

        }

    }
}