using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Firebase.Database;
using Java.Util;
using UberDriverRFP.EventListeners;
using UberDriverRFP.Helpers;

namespace UberDriverRFP.Activities
{
    [Activity(Label = "RegistrationActivity", MainLauncher = true, Theme = "@style/UberTheme")]
    public class RegistrationActivity : AppCompatActivity
    {
        TextInputLayout fullNameText;
        TextInputLayout phoneText;
        TextInputLayout emailText;
        TextInputLayout passwordText;
        Button registerButton;
        CoordinatorLayout rootView;
        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentUser;
        TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register);
            SetupFirebase();
            ConnectViews();
        }

        void SetupFirebase()
        {
            database = AppDataHelper.GetDatabase();
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();
        }

        void ConnectViews()
        {
            fullNameText = (TextInputLayout)FindViewById(Resource.Id.fullNameText);
            phoneText = (TextInputLayout)FindViewById(Resource.Id.phoneText);
            emailText = (TextInputLayout)FindViewById(Resource.Id.emailText);
            passwordText = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            registerButton = (Button)FindViewById(Resource.Id.registerButton);

            registerButton.Click += RegisterButton_Click;
        }

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            string fullname, phone, email, password;

            fullname = fullNameText.EditText.Text;
            phone = phoneText.EditText.Text;
            email = emailText.EditText.Text;
            password = passwordText.EditText.Text;

            if (fullname.Length < 3)
            {
                Snackbar.Make(rootView, "Please enter a valid name", Snackbar.LengthShort).Show();
                return;
            }
            else if (phone.Length < 9)
            {
                Snackbar.Make(rootView, "Please enter a valid number", Snackbar.LengthShort).Show();
                return;
            }
            else if (!email.Contains("@"))
            {
                Snackbar.Make(rootView, "Please enter a valid email", Snackbar.LengthShort).Show();
                return;
            }
            else if (password.Length < 8)
            {
                Snackbar.Make(rootView, "Please enter a password up to 8 characters", Snackbar.LengthShort).Show();
                return;
            }

            mAuth.CreateUserWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);

            taskCompletionListener.Success += (o, g) =>
            {
                DatabaseReference newDriverRef = database.GetReference("drivers/" + mAuth.CurrentUser.Uid);
                HashMap map = new HashMap();

                map.Put("fullname", fullname);
                map.Put("phone", phone);
                map.Put("email", email);
                map.Put("created_at", DateTime.Now.ToString());

                newDriverRef.SetValue(map);
                Snackbar.Make(rootView, "Driver was registered successfully", Snackbar.LengthShort).Show();
                StartActivity(typeof(MainActivity));
            };

            taskCompletionListener.Failure += (w, r) =>
            {
                Snackbar.Make(rootView, "Driver could not be registered", Snackbar.LengthShort).Show();
            };
        }
    }
}