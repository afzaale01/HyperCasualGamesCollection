using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lewis.Social.Face
{
    /// <summary>
    /// Scriptable Object that stores the info for when we are sharing to facebook
    /// </summary>
    [CreateAssetMenu(fileName = "ShareInfo", menuName = "ScriptableObjects/Facebook/Share Info")]
    public class FacebookShareInfo : ScriptableObject
    {
        //Links
        [Header("Links")]
        [SerializeField] private string appURL;
        [SerializeField] private string imageLink;

        //Post Descriptions
        [Header("Descriptions")]
        public string linkTitle;
        public string linkCaption;
        public string linkDescription;


        //Properties to convert links in string to URI's
        public Uri AppURL
        {
            get { return new Uri(appURL); }
        }

        public Uri ImageURL
        {
            get { return new Uri(imageLink); }
        }
    }
}

