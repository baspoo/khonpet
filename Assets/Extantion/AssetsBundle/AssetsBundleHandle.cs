
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetsBundleHandle : MonoBehaviour
{
    static AssetsBundleHandle m_instance;
    public static AssetsBundleHandle instance => m_instance;
    public static AssetsBundleHandle Init()
    {
        if (m_instance == null)
        {
            m_instance = ((GameObject)Instantiate(assetsbundleHandle.gameObject)).GetComponent<AssetsBundleHandle>();
            DontDestroyOnLoad(m_instance.gameObject);
        }
        return m_instance;
    }
    public static AssetsBundleHandle assetsbundleHandle => ((GameObject)Resources.Load("AssetsBundleHandle")).GetComponent<AssetsBundleHandle>();













    #if UNITY_EDITOR
    public UnityEditor.BuildTarget BuildTarget;
    public List<UnityEditor.BuildTarget> BuildTargets;
    #endif


    public void OnStartDownloading( PetData pet  , System.Action<bool> done )
    {
        StartCoroutine(StartDownloading(pet, done)); ;
    }
    IEnumerator StartDownloading(PetData pet  , System.Action<bool> callback) 
    {
        string pathSeparator = "/";
        var url = $"{Application.streamingAssetsPath}{pathSeparator}AssetBundle{pathSeparator}Pets{pathSeparator}{pet.ID}";

        var done = true;
        yield return new WaitForEndOfFrame();
        yield return StartCoroutine(DownloadAssets(pet.ID, url , pet.vBundle, (r) => {
            if (!r)
                done = false;
        }));
        Logger.Log("StartDownloading Done : " + done);
        callback?.Invoke(done);
    }






    Dictionary<string, AssetBundle> dictstore = new Dictionary<string, AssetBundle>();
    WWW current;
    public float progress => current == null ? 0.0f : current.progress;
    [HideInInspector]
    public string bundleName;
    //public void OnDownloadAssets(string bundle , string url  , System.Action<bool> callback) => StartCoroutine(DownloadAssets(bundle,url, callback));
    IEnumerator DownloadAssets(string bundle, string url , int version , System.Action<bool> callback)
    {
        bundleName = bundle;
        Logger.Log($"DownloadAssets : {url}");
        yield return new WaitForEndOfFrame();
        string path = url;
        if (!dictstore.ContainsKey(path))
        {

            bool iscache =  Caching.IsVersionCached(path, version);
            if (!iscache) 
            {
                Caching.ClearCache();
            }

            WWW www = WWW.LoadFromCacheOrDownload(path, version);
            current = www;
             yield return www;
            if (www.error == null)
            {
                AssetBundle assetbundle = www.assetBundle;
                dictstore.Add( bundle , assetbundle);

                var materials = assetbundle.LoadAllAssets(typeof(Material));
                foreach (Material m in materials)
                {
                    var shaderName = m.shader.name;
                    var newShader = Shader.Find(shaderName);
                    if (newShader != null)
                    {
                        m.shader = newShader;
                    }
                    else
                    {
                        Debug.LogWarning("unable to refresh shader: " + shaderName + " in material " + m.name);
                    }
                }

                Logger.Log($"DownloadAssets done is > {  bundle }");
                callback?.Invoke(true);
            }
            else 
            {
                Debug.LogError($"{path} : {www.error}");
                callback?.Invoke(false);
                yield break;
            }
        }
        else
        {
            callback?.Invoke(true);
        }
    }
    public void OnClear() 
    {
        Caching.ClearCache();
    }
    public void OnClear(string bandle)
    {
        Caching.ClearAllCachedVersions(bandle);
    }

    public Object OnLoadAsset(string bundle , string filePath) 
    {
        //Debug.Log($"{bundle} {filePath} {dictstore.ContainsKey(bundle)}");

        if (dictstore.ContainsKey(bundle))
        {
           
            var assetsbundle = dictstore[bundle];
            var result = assetsbundle.LoadAsset(filePath);
            return result;
        }
        else
        {
            return null;
        }
    }
    public void OnLoadAssetAsync(string bundle, string filePath, System.Action<Object> callback) => StartCoroutine(LoadAssetAsync(bundle, filePath, callback));
    IEnumerator LoadAssetAsync(string bundle, string filePath, System.Action<Object> callback)
    {
        yield return new WaitForEndOfFrame();
        if (dictstore.ContainsKey(bundle))
        {
            var assetsbundle = dictstore[bundle];
            //Debug.Log(filePath);
            var loadAsset = assetsbundle.LoadAssetAsync(filePath);
            yield return loadAsset;
            var result = loadAsset.asset;
            //Debug.Log(result != null);
            callback?.Invoke(result);
        }
        else 
        {
            Debug.LogError($"not bundle : {bundle}");
            callback?.Invoke(null);
        }
    }
    IEnumerator LoadAllAssetAsync(string bundle, string filePath, System.Action<Object[]> callback)
    {
        yield return new WaitForEndOfFrame();
        if (dictstore.ContainsKey(bundle))
        {
            var assetsbundle = dictstore[bundle];
            //var fullpath = $"Assets/AssetBundles/{filePath}";
            Debug.Log(filePath);
            var loadAsset = assetsbundle.LoadAssetAsync(filePath);
            yield return loadAsset;
            var result = loadAsset.allAssets;
            callback?.Invoke(result);
        }
        else
        {
            callback?.Invoke(null);
        }
    }



    public AssetBundle GetAssetBundle(string bundle) 
    {
        if (dictstore.ContainsKey(bundle)) 
        {
            var assetsbandle = dictstore[bundle];
            return assetsbandle;
        }
        return null;
    }


}





public class AssetsBundleDownload : MonoBehaviour 
{


}



public class AssetsBundlePath : MonoBehaviour
{
    public  static string pathOutput => "AssetsBundle";
    public static string pathInput => "Assets/AssetsResources/Editor/AssetsBundle";

    public static string FullPathInput
    {
        get
        {
            var path = $"{Application.dataPath}/{pathInput.Replace("Assets/", "")}";
            return path;
        }
    }
    public static string FullPathOutput
    {
        get
        {
            var path = $"{Application.dataPath}/{pathOutput}";
            path = path.Replace("Assets/", "");
            return path;
        }
    }
    #if UNITY_EDITOR
    public static string FullPathOutputByType(UnityEditor.BuildTarget BuildTarget)
    {
      return $"{FullPathOutput}/{BuildTarget}";
    }
    #endif
}
#if UNITY_EDITOR
public class EditorAssetsBundleHandle : EditorWindow
{
    static int selected = 0;
    Vector2 ScrollView;
    [MenuItem("Assets/EditorAssetsBundleHandle")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EditorAssetsBundleHandle), true, "EditorAssetsBundleHandle");
    }
    void OnGUI()
    {


        selected = GUILayout.Toolbar(selected, new string[] { "AssetsBundle", "Caching" });


        //isOpenCache = EditorGUILayout.Toggle("isOpenCache", isOpenCache);
        if (selected == 1) 
        { 
            EditorGUILayout.TextField("ready : " + Caching.ready.ToString());
            EditorGUILayout.TextField("spaceAvailable : " + Caching.spaceAvailable.ToString());
            EditorGUILayout.TextField("spaceFree : " + Caching.spaceFree.ToString());
            EditorGUILayout.TextField("spaceOccupied : " + Caching.spaceOccupied.ToString());
            EditorGUILayout.TextField("spaceUsed : " + Caching.spaceUsed.ToString());
            EditorGUILayout.TextField("maximumAvailableDiskSpace : " + Caching.maximumAvailableDiskSpace.ToString());
            EditorGUILayout.TextField("CacheCount : " + Caching.cacheCount.ToString());
            for (int i = 0; i < Caching.cacheCount; i++)
            {
                var cache = Caching.GetCacheAt(i);
                EditorGUILayout.TextField($"cache [{i}] : {cache.path}");
            }
            if (GUILayout.Button("ClearCache"))
            {
                Caching.ClearCache();
            }
            return;
        }



        var names = GetAllBundlePath();
        ScrollView = EditorGUILayout.BeginScrollView(ScrollView);
        IsFullBuild = EditorGUILayout.Toggle("IsFullBuild", IsFullBuild);
        AssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("AssetBundleOptions", AssetBundleOptions);
        AssetsBundleHandle.assetsbundleHandle.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("BuildTarget", BuildTarget);
        if (GUILayout.Button("Output Directory"))
        {
            if (!Directory.Exists(AssetsBundlePath.pathOutput))
            {
                Directory.CreateDirectory(AssetsBundlePath.pathOutput);
            }
            Application.OpenURL(AssetsBundlePath.pathOutput);
        }



        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Delete All"))
        {
            CleanFile(new string[0], BuildTarget);
        }
        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("CleanFile"))
        {
            CleanFile(names, BuildTarget);
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();



        EditorGUILayout.Space(40);
        if (names != null) {

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("\nBuild All\n"))
            {
                CallBuild(string.Empty);
            }
            EditorGUILayout.EndHorizontal();

            foreach (var name in names) 
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.TextField(name,GUILayout.Width(80));
                if (GUILayout.Button($"Update BundleName"))
                {
                    UpdateBundlesName(name);
                }
                if (GUILayout.Button($"Build"))
                {
                    CallBuild(name);
                }
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button($"Remove", GUILayout.Width(80)))
                {
                    AssetDatabase.RemoveAssetBundleName(name, true);
                }
                GUI.backgroundColor = Color.white;

                EditorGUILayout.EndHorizontal();
            }


        }

        EditorGUILayout.Space(20);

        EditorGUILayout.EndScrollView();
    }



    void CallBuild(string bundle) 
    {
        if (BuildTarget == BuildTarget.NoTarget)
        {
            foreach (var build in AssetsBundleHandle.assetsbundleHandle.BuildTargets)
                BuildAllAssetBundles(bundle, build);
        }
        else 
        {
            BuildAllAssetBundles(bundle, BuildTarget);
        }
    }



    static bool IsFullBuild;
    static BuildAssetBundleOptions AssetBundleOptions = BuildAssetBundleOptions.None;
    static BuildTarget BuildTarget => AssetsBundleHandle.assetsbundleHandle.BuildTarget;


    static void BuildAllAssetBundles(string bundle ,BuildTarget target)
    {
        bool IsCanBuild = true;
        switch (target)
        {
            case BuildTarget.iOS: if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS, target)) IsCanBuild = false;break;
            case BuildTarget.Android: if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, target)) IsCanBuild = false; break;
            case BuildTarget.StandaloneOSX: if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, target)) IsCanBuild = false; break;
            case BuildTarget.StandaloneWindows: if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, target)) IsCanBuild = false; break;
            case BuildTarget.WebGL: if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, target)) IsCanBuild = false; break;
        }
        if (!IsCanBuild) 
        {
            Debug.LogError($"This Editor Not IsBuildTargetSupported : {target} .");
            return;
        }








        string assetBundleDirectory = AssetsBundlePath.pathOutput + "/" + target.ToString();
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        if (string.IsNullOrEmpty(bundle))
        {
            foreach (var name in AssetDatabase.GetAllAssetBundleNames())
            {
                UpdateBundlesName(name);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, AssetBundleOptions, target);
        }
        else
        {
            UpdateBundlesName(bundle);
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = bundle;
            buildMap[0].assetNames = GetAllPath(bundle);
            Debug.Log($" '{bundle}' Done - Assets Amount : " + buildMap[0].assetNames.Length);
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, AssetBundleOptions, target);

            //var paths = GetAllPath(bundle);
            //Debug.Log(paths.Length);


            //var objects =  GetAllObject(bundle);
            //Debug.Log(objects.Length);


            //BuildPipeline.BuildAssetBundleExplicitAssetNames(objects, paths , assetBundleDirectory, AssetBundleOptions, target);


        }

        if (!IsFullBuild) 
        {
            CleanFile(AssetDatabase.GetAllAssetBundleNames(), target);
        }

    }
    static void UpdateBundlesName(string bundle)
    {
        foreach (var assetPath in GetAllPath(bundle))
        {
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant(bundle, "");
        }
    }
    static string[] GetAllPath( string bundle ) 
    {
        var guids = AssetDatabase.FindAssets("*", new[] { $"{AssetsBundlePath.pathInput}/{bundle}" });
        List<string> let = new List<string>();
        foreach (var guid in guids )
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if(!let.Contains(assetPath))
                let.Add(assetPath);
        }
        return let.ToArray();
    }
    static Object[] GetAllObject(string bundle)
    {
        List<Object> let = new List<Object>();
        foreach (var file in GetAllPath(bundle))
        {
            if (!file.Contains(".meta"))
            {
                Debug.Log(file);
                var source = AssetDatabase.LoadMainAssetAtPath(file);
                if (source != null)
                {
                    let.Add(source);
                }
            }
        }
        return let.ToArray();
    }

    static void CleanFile(string[] ignore , BuildTarget build )
    {
        var path = AssetsBundlePath.FullPathOutputByType(build);
        if (Directory.Exists(path))
        {
            var dir = new DirectoryInfo(path);
            foreach (var file in dir.GetFiles())
            {
                if (!ignore.Contains(file.Name))
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
    static string[] GetAllBundlePath( )
    {
        var path = AssetsBundlePath.FullPathInput;
        List<string> paths = new List<string>();
        foreach (var file in Directory.GetDirectories(path))
        {
            paths.Add(file.Replace(path+"\\",""));
        }
        return paths.ToArray();
    }
}
#endif