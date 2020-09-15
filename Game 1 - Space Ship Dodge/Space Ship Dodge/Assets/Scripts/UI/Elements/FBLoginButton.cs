using System;
using System.Collections;
using System.Collections.Generic;
using Lewis.Social.Face;
using Newtonsoft.Json.Bson;
using TMPro;
using UnityEngine;

namespace Lewis.Social.Face
{
    /// <summary>
    /// Class for Facebook login button
    /// Used for both login and log out
    /// </summary>
    public class FBLoginButton : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI buttonText;

        //String to display when there is an un resolved error
        private const string errorText = "Facebook Error";

        //Strings to display for login/logout text display
        private const string loginText = "Login to Facebook";
        private const string logoutText = "Logout of Facebook\nLogged in as ";

        /// <summary>
        /// Performs a login or logout, depenent on our currrent status
        /// Called by link in edtior UI
        /// </summary>
        public void DoLoginLogout()
        {
            //Get our current statis
            bool loggedIn = FacebookIntergration.IsUserLoggedIn;

            //Logout if logged in, login if logged out
            if (loggedIn)
            {
                FacebookIntergration.FacebookLogout();
            }
            else
            {
                FacebookIntergration.FacebookLogin();
            }


        }

        /// <summary>
        /// Updates the login text to either ask the user to login or logout
        /// </summary>
        private void UpdateLoginText(FacebookIntergration.LoginEventResult loginResult)
        {
            //Change the text based on our login result
            string newText;

            switch (loginResult)
            {
                //If we have failed to log in or logged out display login text
                case FacebookIntergration.LoginEventResult.Logout:
                //Fall through
                case FacebookIntergration.LoginEventResult.LoginFailed:
                    newText = loginText;
                    break;
                case FacebookIntergration.LoginEventResult.LoginSuccess:
                    //Display logout text and who we are logged in as
                    newText = logoutText + FacebookIntergration.CurrentUser.userName;
                    break;
                case FacebookIntergration.LoginEventResult.LoginResultCount:
                //Fall through to default
                default:
                    newText = errorText;
                    break;
            }

            //Apply string to UI text
            buttonText.text = newText;
        }

        #region Event Subs/UnSubs

        private void OnEnable()
        {
            FacebookIntergration.FBLoginEvent += UpdateLoginText;
        }

        private void OnDisable()
        {
            FacebookIntergration.FBLoginEvent -= UpdateLoginText;
        }

        #endregion
    }
}