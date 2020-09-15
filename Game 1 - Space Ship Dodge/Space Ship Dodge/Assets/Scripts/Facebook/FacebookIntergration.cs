using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Facebook.Unity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lewis.Social.Face
{
    /// <summary>
    /// Class for social interaction with Facebook
    /// </summary>
    public static class FacebookIntergration
    {

        //Enum for login event results
        public enum LoginEventResult
        {
            LoginFailed,
            LoginSuccess,
            Logout,
            
            LoginResultCount
        }

        //Events for facebook intergration
        public delegate void FacebookEvent(LoginEventResult result);
        public static event FacebookEvent FBLoginEvent;

        //Info for currently logged in user
        public class FBUserInfo
        {
            public FBUserInfo (string name, long id)
            {
                userName = name;
                userID = id;
            }

            //User Info, read only so it can only we written to
            //by the constructor
            public readonly long userID;
            public readonly string userName;
        }
        //Property for logged in user info
        public static FBUserInfo CurrentUser { get; private set; }

        //Property for if we are currently logged in to facebook
        public static bool IsUserLoggedIn
        {
            get { return CurrentUser != null; }
        }

        //List of permissions that we want the user to agree to
        private static readonly List<string> FBPermissions = new List<string>(){ "public_profile", "email", "user_friends" };

        /// <summary>
        /// Initalise the facebook intergration
        /// </summary>
        /// <returns></returns>
        public static bool InitFacebook()
        {
            //If facebook is not initalised, initalise it
            if (!FB.IsInitialized)
            {
                FB.Init();

                //Check that facebook has now been initalised, if it has not throw a warning.
                if (!FB.IsInitialized)
                {
                    Debug.LogWarning("Could not initalise facebook");
                    return false;
                }
            }

            //Activate our App
            FB.ActivateApp();
            return true;
        }

        #region Login / Logout

        /// <summary>
        /// Login in to Facebook account
        /// </summary>
        public static void FacebookLogin()
        {
            //Don't allow login with uninitalised facebook
            if (!FB.IsInitialized)
            {
                Debug.LogError("Could not login to facebook as it has not been initalised");
                FBLoginEvent?.Invoke(LoginEventResult.LoginFailed);
                return;
            }

            if (!IsUserLoggedIn)
            {
                //Login to facebook with permissions
                FB.LogInWithReadPermissions(FBPermissions, LoginCallback);
            }
        }

        /// <summary>
        /// Callback function for login from facebook
        /// </summary>
        /// <param name="result">Result of the login</param>
        private static void LoginCallback(ILoginResult result)
        {
            //If login is cancelled or there was an error then don't save info
            if (result.Cancelled || result.Error != null)
            {
                CurrentUser = null;
                Debug.LogWarning("Did not login to facebook");

                FBLoginEvent?.Invoke(LoginEventResult.LoginFailed);

                //Log out incase there is a cached user logged in
                FacebookLogout();
                return;
            }

            //Get the info about the logged in user, that is name and FB user ID
            FB.API("/me", HttpMethod.GET, UserInfoCallback);


        }

        /// <summary>
        /// Gets (from Facebook API) and sets (in to CurrentUser) info about the user
        /// (i.e Name and FB User ID) from a Facebook API call
        /// </summary>
        /// <param name="callbackResult">Result of a FB API Call for ("/me")</param>
        private static void UserInfoCallback(IGraphResult callbackResult)
        {
            //Create a dictonary of the result that we got back from the API Call
            Dictionary<string, object> callbackResultsDictionary =
                (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(callbackResult.RawResult);

            //Check that we have got the right call back (i.e it contains a key for name and user ID)
            if (!callbackResultsDictionary.ContainsKey("name") || !callbackResultsDictionary.ContainsKey("id"))
            {
                Debug.LogWarning("UserInfoCallback was called with the wrong call back info");

                //Trigger Event for unsuccessful login and log out the current user
                FBLoginEvent?.Invoke(LoginEventResult.LoginFailed);
                FacebookLogout();
                return;
            }

            //We get the name and userID back from our API call. (Name: "", userID: "")
            //we need to put this in our user info class
            CurrentUser = new FBUserInfo((string) callbackResultsDictionary["name"],
                Convert.ToInt64(callbackResultsDictionary["id"]));

            //Trigger event for successful logon
            FBLoginEvent?.Invoke(LoginEventResult.LoginSuccess);
        }

        /// <summary>
        /// Log out of Facebook account
        /// </summary>
        public static void FacebookLogout()
        {
            //Don't allow logout with uninitalised facebook
            if (!FB.IsInitialized || !IsUserLoggedIn)
            {
                return;
            }

            //Null out logged in user info 
            CurrentUser = null;

            //Logout of facebook and call event
            FB.LogOut();
            FBLoginEvent?.Invoke(LoginEventResult.Logout);
            
        }
        #endregion

        /// <summary>
        /// Share info to facebook
        /// </summary>
        /// <param name="info">Info to share</param>
        public static void FacebookShare(FacebookShareInfo info)
        {
            FB.ShareLink(info.AppURL, info.linkTitle, info.linkDescription, info.ImageURL);
        }
    }
}