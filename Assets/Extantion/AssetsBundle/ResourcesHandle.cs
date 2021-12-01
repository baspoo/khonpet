using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class ResourcesHandle
{
    public enum LoadType
    {
        Editor, CloudFile
    }
    public enum FileType
    {
        img, png, jpg, prefab, txt, json, mp3, mp4
    }

    static LoadType GetLoadType
    {
        get
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                return GameObject.FindObjectOfType<Setting>().bundle.LoadType;
            }
            else
            {
                return LoadType.Editor;
            }
#endif
            return LoadType.CloudFile;
        }
    }


    public static void Init( string petID , System.Action<bool> done)
    {
        if (GetLoadType == LoadType.Editor)
        {
            done?.Invoke(true);
        }
        if (GetLoadType == LoadType.CloudFile)
        {
            string pathSeparator = "/";
            var url = $"{Application.streamingAssetsPath}{pathSeparator}AssetBundle{pathSeparator}Pets{pathSeparator}{petID}";
            var assetsbundle = AssetsBundleHandle.Init();
            assetsbundle.OnStartDownloading(petID,url, done);
        }
    }











    public static Object Load(string bundle, string path, FileType filetype = FileType.img, System.Action<Object> async = null) => Load(GetLoadType, bundle, path, filetype, async);
    public static Object LoadEditor(string bundle, string path, FileType filetype = FileType.img, System.Action<Object> async = null) => Load(LoadType.Editor, bundle, path, filetype, async);
    static Object Load(LoadType loadtype, string bundle, string path, FileType filetype = FileType.img, System.Action<Object> async = null)
    {

        // Debug.Log($"Load : {loadtype} / {bundle} / {path} / {filetype} ");

        if (loadtype == LoadType.Editor)
        {
#if UNITY_EDITOR
            Object Obj = null;
            if (filetype == FileType.img)
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.png}") }";
                Obj = AssetDatabase.LoadMainAssetAtPath(finalpath);
                if (Obj == null)
                {
                    finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.jpg}") }";
                    Obj = AssetDatabase.LoadMainAssetAtPath(finalpath);
                }
                if (Obj == null) Debug.LogError($"NotFound : {finalpath}");
            }
            else
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{filetype}") }";
                Obj = AssetDatabase.LoadMainAssetAtPath(finalpath);
                if (Obj == null) Debug.LogError($"NotFound : {finalpath}");
            }

            if (async != null)
                async.Invoke(Obj);
            else
                return Obj;
#endif
        }

        if (!Application.isPlaying)
            return null;

        if (loadtype == LoadType.CloudFile)
        {

            Object Obj = null;
            if (filetype == FileType.img)
            {
                var finalpath_png = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.png}") }";
                var finalpath_jpg = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{FileType.jpg}") }";
                if (async != null)
                {
                    AssetsBundleHandle.instance.OnLoadAssetAsync(bundle, finalpath_png, (r) =>
                    {
                        if (r == null)
                            AssetsBundleHandle.instance.OnLoadAssetAsync(bundle, finalpath_jpg, async);
                        else
                            async?.Invoke(r);
                    });
                }
                else
                {
                    Obj = AssetsBundleHandle.instance.OnLoadAsset(bundle, finalpath_png);
                    if (Obj == null)
                        Obj = AssetsBundleHandle.instance.OnLoadAsset(bundle, finalpath_jpg);
                }
            }
            else
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}{ (path.Contains(".") ? "" : $".{filetype}") }";

                if (async != null) AssetsBundleHandle.instance.OnLoadAssetAsync(bundle, finalpath, async);
                else Obj = AssetsBundleHandle.instance.OnLoadAsset(bundle, finalpath);
            }
            return Obj;
        }

        return null;
    }





    public static Object[] LoadAll(string bundle, string path) => LoadAll(GetLoadType, bundle, path);
    public static Object[] LoadAllEditor(string bundle, string path) => LoadAll(LoadType.Editor, bundle, path);
    static Object[] LoadAll(LoadType loadtype, string bundle, string path)
    {
        if (loadtype == LoadType.Editor)
        {
#if UNITY_EDITOR
            List<Object> let = new List<Object>();
            var finalpath = $"{AssetsBundlePath.FullPathInput}/{bundle}";
            if (!string.IsNullOrEmpty(path)) finalpath += "/" + path;

            var dir = new DirectoryInfo(finalpath);
            foreach (var file in dir.GetFiles())
            {
                if (!file.Name.Contains(".meta"))
                {
                    var source = Load(bundle, $"{path}/{file.Name}");
                    if (source != null)
                    {
                        let.Add(source);
                    }
                }
            }
            Debug.Log(finalpath + " // " + let.Count.ToString());
            return let.ToArray();
#endif
        }


        if (!Application.isPlaying)
            return null;

        if (loadtype == LoadType.CloudFile)
        {
            List<Object> let = new List<Object>();
            var assetsbandle = AssetsBundleHandle.instance.GetAssetBundle(bundle);
            if (assetsbandle != null)
            {
                var finalpath = $"{AssetsBundlePath.pathInput}/{bundle}/{path}";
                List<string> files = new List<string>();
                foreach (var str in assetsbandle.GetAllAssetNames())
                {
                    if (str.ToLower().Contains(finalpath.ToLower()))
                    {
                        var file = AssetsBundleHandle.instance.OnLoadAsset(bundle, str);
                        let.Add(file);
                    }
                }
            }
            return let.ToArray();
        }


        return null;
    }


}
