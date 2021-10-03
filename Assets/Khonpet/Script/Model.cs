using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model
{






    public class OpenSea 
    {
        public string id;
        public string token_id;
        public int num_sales;
        public string background_color;
        public string image_url;
        public string image_preview_url;
        public string image_thumbnail_url;
        public string image_original_url;
        public string animation_url;
        public string animation_original_url;
        public string name;
        public string description;
        public string external_link;

        public UserData owner;
        public UserData creator;
        public class UserData
        {
            public Account user;
            public class Account
            {
                public string username;
            }
            public string profile_img_url;
            public string address;
            public string config;

        }
    }














}
