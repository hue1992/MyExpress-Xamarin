using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Xamarin.Forms;

namespace MyExpress
{
    public partial class MainPage : ContentPage
    {
        private string source_url = "";
        private string source_url_en = "http://myexpress.info/";
        private string source_url_mm = "http://mm.myexpress.info/";

        public bool IsBackButtonVisible { get; set; }
        private string farlingUrl;
        private string loadingUrl;

        public MainPage()
        {
            InitializeComponent();


            //set Defalts url
            if (Settings.DefaultUrl == string.Empty)
            {
                Settings.DefaultUrl = source_url_en;
                source_url = source_url_en;
            }
            else
            {
                source_url = Settings.DefaultUrl;
            }

            //set Defalts farling url and loading url
            farlingUrl = source_url;
            loadingUrl = source_url;
            //show Loading progress
            showLoading(true);
            if (!CheckInternetConnection())
            {
                showNoConnection();
            }
            else
            {
                Console.WriteLine(".....Connection...");
                myWebview.Source = source_url;
            }


        }

        private void showNoConnection()
        {
            showLoading(false);
            LoadErrorMessage("No Connection");
            ButtonOption.IsEnabled = false;
        }



        private static ImageSource getImageSource()
        {
            return ImageSource.FromFile("imageName.png");
        }

        void webOnNavigating(object sender, WebNavigatingEventArgs e)
        {
            if (myWebview.CanGoBack) ButtonBlack.IsVisible = true;
            string url_load = e.Url.ToString();
            showLoading(true);
            if (url_load.Equals(source_url_en) || url_load.Equals(source_url_mm)) ButtonBlack.IsVisible = false;
            if (!url_load.StartsWith(source_url))
            {
                Debug.WriteLine("On Navigating,Not Load url:" + url_load);
                showLoading(false);
                e.Cancel = true;
            }
        }

        void webOnEndNavigating(object sender, WebNavigatedEventArgs e)
        {
            //Debug.WriteLine("End Navigating,Result:" + e.Result + ",URL:" + e.Url.ToString());
            string result = e.Result.ToString();
            string url = e.Url.ToString();
            if (result.Contains("Failure"))
            {
                if (url.StartsWith(source_url))
                {
                    Debug.WriteLine("End Navigating,Result:Failure Starts with URL:" + url);

                    //Load error html message               
                    LoadErrorMessage("An error occurred");
                    farlingUrl = url;
                    showLoading(false);
                    ButtonBlack.IsVisible = false;
                }
                else
                {
                    showLoading(false);
                    Debug.WriteLine("End Navigating,Result:Failure Non Starts with URL:" + url);
                }


            }
            else
            {
                Debug.WriteLine("End Navigating,Result:" + result + ",URL:" + url);
                loadingUrl = url;
                if (url.Equals(source_url_en) || url.Equals(source_url_mm)) ButtonBlack.IsVisible = false;
                else
                {
                    ButtonBlack.IsVisible = true;
                }
                ButtonReload.IsVisible = false;
                ButtonOption.IsEnabled = true;
                showLoading(false);
            }


        }

        private void LoadErrorMessage(string message)
        {
            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html =
                    @"<html>
                        <body>
                            <center>
                                <h1>Error</h1>" +
                                    "<p>" + message + "</p>" +
                            "</center>" +
                        "</body>" +
                      "</html>";
            myWebview.Source = htmlSource;
            ButtonReload.IsVisible = true;
            ButtonBlack.IsVisible = false;

        }

        void Back_Clicked(object sender, System.EventArgs e)
        {
            showLoading(true);
            bool cangoback = myWebview.CanGoBack;
            Debug.WriteLine("Back_Clicked,Can go back:" + cangoback);
            if (cangoback)
            {
                if ((loadingUrl.Equals(source_url_en) || loadingUrl.Equals(source_url_mm)))
                {
                    ButtonBlack.IsVisible = false;
                }
                myWebview.GoBack();
            }
            else ButtonBlack.IsVisible = false;
        }

        private void showLoading(bool show)
        {
            actIndicator2.IsVisible = show;
            actIndicator2.IsRunning = show;
            myWebview.IsVisible = !show;
            //if (myWebview.CanGoBack) ButtonBlack.IsVisible = true;

        }

        private void btnRefresh_Clicked(object sender, EventArgs e)
        {
            showLoading(true);
            if (!CheckInternetConnection())
            {
                showNoConnection();
            }
            else
            {
                Console.WriteLine(".....Connection...");
                //set farling to source webview 
                myWebview.Source = farlingUrl;
                ButtonReload.IsVisible = false;
                ButtonBlack.IsVisible = false;
            }



        }




        async void showOpition_Clicked(object sender, EventArgs e)
        {
            if (actIndicator2.IsRunning == false)
            {
                var action = await DisplayActionSheet("", "Cancel", null, "English", "Myanmar");
                Debug.WriteLine("Action: " + action);
                if (action.Equals("English"))
                {
                    showLoading(true);
                    source_url = source_url_en;
                    myWebview.Source = source_url;
                    //save selection
                    Settings.DefaultUrl = source_url;
                }
                else if (action.Equals("Myanmar"))
                {
                    showLoading(true);
                    source_url = source_url_mm;
                    myWebview.Source = source_url;
                    //save selection
                    Settings.DefaultUrl = source_url;
                }

            }


        }

        public bool CheckInternetConnection()
        {
            string CheckUrl = "http://www.google.com";

            try
            {
                HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);

                iNetRequest.Timeout = 5000;

                WebResponse iNetResponse = iNetRequest.GetResponse();

                //Console.WriteLine("...connection established..." + iNetRequest.ToString());
                iNetResponse.Close();

                return true;

            }
            catch (WebException ex)
            {

                //Console.WriteLine(".....no connection..." + ex.ToString());

                return false;
            }
        }

        async void myWebview_Focused(object sender, FocusEventArgs e)
        {
            if (CheckInternetConnection())
            {
                var answer = await DisplayAlert("Your phone is Offline?", "You can turn off Airplane Mode or turn on Wifi", "Settings", "OK");
                if (answer.Equals("Settings"))
                {

                }
            }
        }
    }

}
