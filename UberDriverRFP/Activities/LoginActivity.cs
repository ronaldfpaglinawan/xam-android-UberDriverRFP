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
using UberDriverRFP.EventListeners;
using UberDriverRFP.Helpers;

namespace UberDriverRFP.Activities
{
    [Activity(Label = "LoginActivity", Theme = "@style/UberTheme", MainLauncher = false)]
    public class LoginActivity : AppCompatActivity
    {
        Button loginButton;
        TextInputLayout textInputEmail;
        TextInputLayout textInputPassword;
        CoordinatorLayout rootView;
        TextView clickToSignUp;
        FirebaseDatabase database;
        FirebaseAuth mAuth;
        FirebaseUser currentUser;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);
            InitializeFirebase();
            ConnectViews();
        }

        void InitializeFirebase()
        {
            mAuth = AppDataHelper.GetFirebaseAuth();
            currentUser = AppDataHelper.GetCurrentUser();
            database = AppDataHelper.GetDatabase();
        }

        void ConnectViews()
        {
            textInputEmail = (TextInputLayout)FindViewById(Resource.Id.emailText);
            textInputPassword = (TextInputLayout)FindViewById(Resource.Id.passwordText);
            rootView = (CoordinatorLayout)FindViewById(Resource.Id.rootView);
            loginButton = (Button)FindViewById(Resource.Id.loginButton);
            clickToSignUp = (TextView)FindViewById(Resource.Id.clickToRegister);

            loginButton.Click += LoginButton_Click;
            clickToSignUp.Click += ClickToSignUp_Click;
        }

        private void ClickToSignUp_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrationActivity));
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string email, password;
            email = textInputEmail.EditText.Text;
            password = textInputPassword.EditText.Text;

            TaskCompletionListener taskCompletionListener = new TaskCompletionListener();
            taskCompletionListener.Success += TaskCompletionListener_Success;
            taskCompletionListener.Failure += TaskCompletionListener_Failure;

            mAuth.SignInWithEmailAndPassword(email, password)
                .AddOnSuccessListener(this, taskCompletionListener)
                .AddOnFailureListener(this, taskCompletionListener);
        }

        private void TaskCompletionListener_Failure(object sender, EventArgs e)
        {
            Snackbar.Make(rootView, "Login failed", Snackbar.LengthShort).Show();
        }

        private void TaskCompletionListener_Success(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
        }
    }
}