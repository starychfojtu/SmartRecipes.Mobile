﻿using SmartRecipes.Mobile.Models;
using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public class SignInViewModel
    {
        private readonly INavigation navigation;

        private readonly Authenticator authenticator;

        public SignInViewModel(Authenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public void SignIn()
        {
            authenticator.Authenticate(
                new SignInCredentials(Email, Password),
                () => { },
                () => { });
        }

        public void NavigateToSignUp()
        {
            Application.Current.MainPage = DIContainer.Instance.Resolve<SignUpPage>();
        }
    }
}