﻿using ELib.Models;
using Newtonsoft.Json;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ELib.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Donate : ContentPage
    {
        //Αρχικοποιηση του κλειδιου και του endpoint για την επικοινωνια με το API
        private static readonly string key = "04e9282d44eb4078bec62c55a515a449";
        private static readonly string endpoint = "https://visiontextextractor.cognitiveservices.azure.com/";
        private LogIn user;
        public Donate(LogIn user)
        {
            this.user = user;
            InitializeComponent();
        }
       
        //Διμιουργια των λειτουργιων για το κουμπι της επιλογης φωτογραγιας απο τον χρηστη
        async private void PickButton(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
                });
                if (file == null) return;
                imgSelected.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
                MakeRequest(file.Path);
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
        }
        //Διμιουργια των λειτουργιων για το κουμπι της λήψης φωτογραγιας απο τον χρηστη
        async private void TakePhotoButton(object sender, EventArgs e)
        {
            var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions()
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            if (photo != null)
            {


                imgSelected.Source = ImageSource.FromStream(() =>
                {

                    var camerasStream = photo.GetStream();
                    return camerasStream;
                }
                );
            }
            else
            {

            }

            MakeRequest(photo.Path);
        }


        //Διμιουργια συναρτισης για την επικοινωνιας με το API 
        public async void MakeRequest(string text)
        {
            var errors = new List<string>();
            string extractedResult = "";
            ImageInfoViewModel responeData = new ImageInfoViewModel();

            try
            {
                HttpClient client = new HttpClient();

                // Header ετοιματος.  
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

                // Παραμετροι ετοιματος.  
                string requestParameters = "language=unk&detectOrientation=true";

                // Διμιουργια του URI για την επικοινωνια με το API  
                string uri = endpoint + "vision/v2.0/ocr?" + requestParameters;

                HttpResponseMessage response;

                // Μετατροπη της εικονας σε bytes  
                byte[] byteData = GetImageAsByteArray(text);

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(uri, content);


                }
                // Ληψη της απαντησης και μετατροπη σε string
                string result = await response.Content.ReadAsStringAsync();
                // Ελεγχος για την ορθοτητα της απαντησης
                if (response.IsSuccessStatusCode)
                {
                    responeData = JsonConvert.DeserializeObject<ImageInfoViewModel>(result, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include,
                        Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg)
                        {
                            errors.Add(earg.ErrorContext.Member.ToString());
                            earg.ErrorContext.Handled = true;
                        }
                    }
                    );

                    var linesCount = responeData.regions[0].lines.Count;
                    for (int i = 0; i < linesCount; i++)
                    {
                        var wordsCount = responeData.regions[0].lines[i].words.Count;
                        for (int j = 0; j < wordsCount; j++)
                        {
                            //Δημιουργια ενος string για την εμφανιση των αποτελεσματων  
                            extractedResult += responeData.regions[0].lines[i].words[j].text + " ";
                        }

                        extractedResult += Environment.NewLine;
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
            //Εμφανιση των αποτελεσματων στο Label
            if(extractedResult.Contains("ISBN") || extractedResult.Contains("ISBN:"))
            {
                string separator = "ISBN ";
                int separatorIndex = extractedResult.IndexOf(separator);
                string result = extractedResult.Substring(separatorIndex + 24);

                ISBNText.Text = result;
            }
            else
            {
                List<string> arrayfromEntry = new List<string>();
                arrayfromEntry = extractedResult.Split(new char[] { ' ' }).ToList();
                int i;
                for (i = 0; i < arrayfromEntry.Count(); i++)
                {
                    arrayfromEntry[i] = arrayfromEntry[i];
                }
                WriterText.Text = arrayfromEntry[0] + " " + arrayfromEntry[1];
                //string ok = "";
                
                TitleText.Text = extractedResult.Replace(arrayfromEntry[0] + " " + arrayfromEntry[1] + " ", String.Empty);

                //TitleText.Text = ok;
            }
            //TitleText.Text = extractedResult;
            //showText.Text = extractedResult;
            // showText.TextColor = Color.Red;
        }

        //Διμιουργια συναρτισης για την μετατροπη της φωτογραφιας σε bytes
        public byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }


        async private void UploadBtn(object sender, EventArgs e)
        {
            var book = new AddBook();

            book.mail = user.mail;
            book.isbn = ISBNText.Text;
            book.title = TitleText.Text;
            book.writer = WriterText.Text;
            book.type = Type.Text;


            
                try
                {
                    MyWebRequest request = new MyWebRequest();

                    await request.OnAdd(book, "addbook");
                    if (request.Get_Confirmation().Contains("OK"))
                    {
                        await ShowMessage("Succesfully added", "Alert", "Ok", async () =>
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
