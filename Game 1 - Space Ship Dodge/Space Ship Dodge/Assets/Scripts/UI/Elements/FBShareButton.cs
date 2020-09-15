using System.Collections;
using System.Collections.Generic;
using Lewis.Score;
using Lewis.Social.Face;
using UnityEngine;


namespace Lewis.Social.Face
{
    /// <summary>
    /// Class for button that shares the current score to facebook
    /// </summary>
    public class FBShareButton : MonoBehaviour
    {
        [SerializeField] private FacebookShareInfo facebookInfo;

        private void Awake()
        {
            //Hide if user is not logged in to facebook
            if (!FacebookIntergration.IsUserLoggedIn)
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Share the current score to facebook
        /// </summary>
        public void ShareToFacebook()
        {
            //Check we are logged in to facebook 
            if (FacebookIntergration.IsUserLoggedIn)
            {
                FacebookIntergration.FacebookShare(facebookInfo);
            }

        }
    }
}

