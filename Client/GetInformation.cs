using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Client
{
    public class GetInformation : DialogFragment
    {
        public class OnGetInfoCompletEventArgs : EventArgs
        {
            public string TxtName { get; set; }
            public int BackgroundColor { get; set; }
        }

        public event EventHandler<OnGetInfoCompletEventArgs> OnGetInfoComplete;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.getinfo, container, false);

            EditText txtName = view.FindViewById<EditText>(Resource.Id.txtName);
            RadioGroup radioGroup = view.FindViewById<RadioGroup>(Resource.Id.radioGroup);
            Button btnOkay = view.FindViewById<Button>(Resource.Id.btnOkay);            

            btnOkay.Click += (o, e) =>
            {
                var args = new OnGetInfoCompletEventArgs { TxtName = txtName.Text.Trim() };

                switch (radioGroup.CheckedRadioButtonId)
                {
                    case Resource.Id.radioRed:
                        args.BackgroundColor = 1;
                        break;

                    case Resource.Id.radioGreen:
                        args.BackgroundColor = 2;
                        break;

                    case Resource.Id.radioBlue:
                        args.BackgroundColor = 3;
                        break;
                }

                OnGetInfoComplete.Invoke(this, args);

                Dismiss();
            };

            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
        }
    }
}