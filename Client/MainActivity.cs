using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.AspNet.SignalR.Client;
using Android.Graphics;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Client
{
    [Activity(Label = "Client", MainLauncher = true)]
    public class MainActivity : Activity
    {
        public string UserName;
        public int BackgroundColor;
        public readonly String urlString = "http://signalr-samoyl.azurewebsites.net";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            AppCenter.Start("34a69288-2d3b-4909-b542-188878f55b33",
                   typeof(Analytics), typeof(Crashes));
            GetInformation getInfo = new GetInformation();
            getInfo.OnGetInfoComplete += GetInfo_OnGetInfoComplete;
            getInfo.Show(FragmentManager, "GetInfo");
        }

        private async void GetInfo_OnGetInfoComplete(object sender, GetInformation.OnGetInfoCompletEventArgs e)
        {
            UserName = e.TxtName;
            BackgroundColor = e.BackgroundColor;
            var hubConnection = new HubConnection(urlString);
            var chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

            chatHubProxy.On<string, int, string>("UpdateChatMessage", (message, color, user) =>
            {
                RunOnUiThread(() =>
                {
                    TextView txt = new TextView(this);
                    txt.Text = user + ": " + message;
                    txt.SetTextSize(Android.Util.ComplexUnitType.Sp, 20);
                    txt.SetPadding(10, 10, 10, 10);

                    switch (color)
                    {
                        case 1:
                            txt.SetTextColor(Color.Red);
                            break;

                        case 2:
                            txt.SetTextColor(Color.DarkGreen);
                            break;

                        case 3:
                            txt.SetTextColor(Color.Blue);
                            break;

                        default:
                            txt.SetTextColor(Color.Black);
                            break;

                    }

                    txt.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                    {
                        TopMargin = 10,
                        BottomMargin = 10,
                        LeftMargin = 10,
                        RightMargin = 10,
                        Gravity = GravityFlags.Left
                    };

                    FindViewById<LinearLayout>(Resource.Id.llChatMessages)
                            .AddView(txt);
                });
            });

            try
            {
                await hubConnection.Start();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            FindViewById<Button>(Resource.Id.btnSend).Click += async (o, e2) =>
            {
                var message = FindViewById<EditText>(Resource.Id.txtChat).Text;
                FindViewById<EditText>(Resource.Id.txtChat).Text = null;
                await chatHubProxy.Invoke("SendMessage", new object[] { message, BackgroundColor, UserName });
            };

        }
    }
}