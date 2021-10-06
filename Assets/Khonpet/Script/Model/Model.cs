using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model
{

    [System.Serializable]
    public class OpenSeaOwner
    {
        public List<OpenSeaAsset> assets;
    }



    [System.Serializable]
    public class OpenSeaAsset
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
        public string permalink;
        

        public UserData owner;
        public UserData creator;
        [System.Serializable]
        public class UserData
        {
            public Account user;
            [System.Serializable]
            public class Account
            {
                public string username;
            }
            public string profile_img_url;
            public string address;
            public string config;
        }


    
        public AssetContract asset_contract;
        [System.Serializable]
        public class AssetContract
        {
            public string address;
            public string asset_contract_type;
            public string created_date;
            public string name;
            public string nft_version;
            public string schema_name;
            public string symbol;
            public string description;
            public string external_link;
            public string image_url;
            public string payout_address;
        }


        public Collection collection;
        [System.Serializable]
        public class Collection
        {
            public Dictionary<string, object> stats;
            public string banner_image_url;
            public string created_date;
            public string description;
            public string external_url;
            public string image_url;
            public string name;
            public string payout_address;
            public string twitter_username;
            public string instagram_username;
            public string wiki_url;
        }

    }











}
