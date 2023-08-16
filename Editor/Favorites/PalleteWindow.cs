using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PaletteWindow : EditorWindow
{
    private int recentCount = 5;
    private int favoritesCount = 5;
    private Object[] recentObjects;
    private static List<Object> favoriteObjects = new List<Object>();
    static string[] saveProfileList = new string[1];
  
    
    
    
    [MenuItem("Window/Palette")]
    public static void ShowWindow()
    {
        GetWindow<PaletteWindow>("Palette");
        LoadSaveFileOrCreate();
    }




    private void OnEnable() //
    {
        LoadSaveFileOrCreate();
    }


    
    static int selected = 0;
    private static FavoriteData_SO favoritesData;
    private bool showPopup;
    private string currentFavoriteProfileName;
    private string currentProfileNew_Name; //in case the file name changed
    Vector2 scrollPosition = Vector2.zero;
  
    
    private void OnGUI()
    {


        if (GUILayout.Button("+ New "))
        {
            currentProfileNew_Name = "newprofile";
            int i = 1;
            if (favoritesData.GetProfileList() != null)
                while (favoritesData.GetProfileList().Contains(currentProfileNew_Name))
                {
                    i++;
                }
            currentProfileNew_Name += i.ToString();
            currentFavoriteProfileName = "";
            favoriteObjects = new List<Object>();
        }
        // if (showPopup)
        // {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ProfileName:");
        currentProfileNew_Name = EditorGUILayout.TextField(currentProfileNew_Name);
        Debug.Log("current" + currentFavoriteProfileName + " new : " + currentProfileNew_Name);

        if (currentFavoriteProfileName != currentProfileNew_Name)//
        {
            if (GUILayout.Button("save"))
            {
                SaveFavoriteObjects();
            }
        }


        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();


        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK"))
        {

            showPopup = false;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        // }



        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Favorites:");

        favoritesCount = EditorGUILayout.IntSlider("Number of Favorite Objects:", favoritesCount, 0, 10);



        selected = EditorGUILayout.Popup("Select an option:", selected, saveProfileList);

        if (GUILayout.Button("LOAD"))
        {
            favoriteObjects = favoritesData.LoadProfile(saveProfileList[selected]);
        }


        if (favoriteObjects != null)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < favoriteObjects.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();


                //favoriteObjects[i] = EditorGUILayout.ObjectField(favoriteObjects[i], typeof(Object), true);

                if (favoriteObjects[i] != null)
                {
                    if (!AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(favoriteObjects[i])))
                        GUILayout.Box(EditorGUIUtility.ObjectContent(null, favoriteObjects[i].GetType()).image, GUILayout.Width(20), GUILayout.Height(20));
                    else
                    {
                        Texture2D folderIcon = EditorGUIUtility.FindTexture("Folder Icon");
                        GUILayout.Box(folderIcon, GUILayout.Width(20), GUILayout.Height(20));
                    }

                }

                GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.normal.textColor = UnityEngine.Color.red;

                if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(favoriteObjects[i])))
                {
                    EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(favoriteObjects[i]));

                    Debug.Log("This is a folder: " + AssetDatabase.GetAssetPath(favoriteObjects[i]));
                }
                else
                {

                    EditorGUILayout.LabelField(favoriteObjects[i].name, labelStyle);
                }
                Debug.Log(favoriteObjects[i].GetType());
                if (GUILayout.Button(">>"))
                {
                    EditorGUIUtility.PingObject(favoriteObjects[i]);

                }


                if (GUILayout.Button("Remove"))
                {
                    favoriteObjects[i] = null;
                    showPopup = true;
                }



                EditorGUILayout.EndHorizontal();
            }


            EditorGUILayout.EndScrollView();
        }

        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
        {
          
            if (DragAndDrop.objectReferences.Length > 0)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

               
                if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {

                        favoriteObjects.Add(draggedObject);
                    }

                    DragAndDrop.activeControlID = 0;
                }
            }
        }
    }


    private void OnInspectorUpdate()
    {

    }


    private void OnValidate()
    {
        Debug.Log("sdsdsdsdsdsd");
    }

    private void OnDestroy()
    {
        // Save recent and favorite objects to EditorPrefs
        // SaveRecentObjects();
        // SaveFavoriteObjects();
    }



    
    private void LoadRecentObjects()
    {
        recentObjects = new Object[recentCount];

        for (int i = 0; i < recentCount; i++)
        {
            int instanceID = EditorPrefs.GetInt($"RecentObject{i}", 0);
            recentObjects[i] = EditorUtility.InstanceIDToObject(instanceID);
        }
    }

    private void SaveRecentObjects()
    {
        if (recentObjects != null)
        {
            for (int i = 0; i < recentObjects.Length; i++)
            {
                int instanceID = recentObjects[i] != null ? recentObjects[i].GetInstanceID() : 0;
                EditorPrefs.SetInt($"RecentObject{i}", instanceID);
            }
        }


    }

    private static void LoadSaveFileOrCreate()
    {
        favoriteObjects = new List<Object>();
        LoadSaveFile();

        if (favoritesData.GetProfileList() != null)
        {
            saveProfileList = new string[favoritesData.GetProfileList().Count];
            saveProfileList = favoritesData.GetProfileList().ToArray();
            if (saveProfileList.Length != 0)
                favoriteObjects = favoritesData.LoadProfile(saveProfileList[saveProfileList.Length - 1]);

            string profileToLoad = saveProfileList[saveProfileList.Length - 1];
            Debug.Log(profileToLoad);
            favoriteObjects = favoritesData.LoadProfile(profileToLoad);

            selected = favoritesData.GetProfileList().Count - 1;

        }

        // for (int i = 0; i < favoritesCount; i++)
        // {
        //     int instanceID = EditorPrefs.GetInt($"FavoriteObject{i}", 0);
        //     favoriteObjects.Add(EditorUtility.InstanceIDToObject(instanceID));
        // }
    }

    private void SaveFavoriteObjects()
    {
        if (favoriteObjects != null)
        {
            if (currentFavoriteProfileName != currentProfileNew_Name && !string.IsNullOrEmpty(currentFavoriteProfileName))
            {
                favoritesData.UpdateProfile(currentFavoriteProfileName, currentProfileNew_Name, favoriteObjects);
                currentFavoriteProfileName = currentProfileNew_Name;
            }
            else
            {
                favoritesData.SaveProfile(currentProfileNew_Name, favoriteObjects);
                currentFavoriteProfileName = currentProfileNew_Name;
            }
            //

        }
        else
        {
            Debug.LogError("Noting to save");
        }

        EditorUtility.SetDirty(favoritesData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        saveProfileList = new string[favoritesData.GetProfileList().Count];
        saveProfileList = favoritesData.GetProfileList().ToArray();

    }

    [MenuItem("GameObject/Add to Favorites >>", true)]
    private static bool ValidateAddToFavorites()
    {
        if (favoriteObjects.Contains(Selection.activeGameObject)) return false;

        return Selection.activeGameObject != null;
    }
    [MenuItem("GameObject/Remove from Favorites <<", true)]
    private static bool ValidateRemoveFromFavorites()
    {
        if (Selection.activeGameObject == null) return false;
        foreach (var item in favoriteObjects)
        {
            if (item == Selection.activeGameObject as GameObject) return true;
        }

        return false;
    }
    [MenuItem("GameObject/Remove from Favorites <<")]
    private static void RemoveFromFavorites()
    {
        Debug.Log("falds");
        favoriteObjects.Remove(Selection.activeGameObject);
    }
    [MenuItem("GameObject/Add to Favorites >>")]
    private static void AddToFavorites()
    {
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            PaletteWindow paletteWindow = GetWindow<PaletteWindow>("Palette");
            if (favoriteObjects != null)
            {
              
                bool alreadyAdded = false;
                for (int i = 0; i < favoriteObjects.Count; i++)
                {
                    GameObject favoriteObject = favoriteObjects[i] as GameObject;
                    if (favoriteObject == selectedObject)
                    {
                        favoriteObjects[i] = null;
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                {
                   
                    int emptySlotIndex = -1;
                    for (int i = 0; i < favoriteObjects.Count; i++)
                    {
                        if (favoriteObjects[i] == null)
                        {
                            emptySlotIndex = i;
                            break;
                        }
                    }

                    if (emptySlotIndex == -1)
                        emptySlotIndex = 0;


                    favoriteObjects.Add(selectedObject);

                }
            }
        }
    }




    private static void LoadSaveFile()//
    {

     
        string[] guids = AssetDatabase.FindAssets("FavoritesDataSavefile");
        if (guids.Length > 1) Debug.LogError("multiple save containers found");
        if (guids.Length > 0)
        {
            Debug.Log("loading save asset");

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            favoritesData = AssetDatabase.LoadAssetAtPath<FavoriteData_SO>(path) as FavoriteData_SO;
        }
        else
        {
            Debug.Log("Creating new save asset");
            favoritesData = ScriptableObject.CreateInstance<FavoriteData_SO>();
            AssetDatabase.CreateAsset(favoritesData, "Assets/Editor/Favorites/FavoritesDataSavefile.asset");
            AssetDatabase.SaveAssets();//
        }

        saveProfileList = new string[1];
    }
}




