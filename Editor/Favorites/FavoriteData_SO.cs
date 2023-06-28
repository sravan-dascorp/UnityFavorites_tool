using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FavoriteData_SO : ScriptableObject
{

    [SerializeField] List<FavoriteProfile> favoritesSavedProfiles = new  List<FavoriteProfile>();
   
    [SerializeField] List<string> profileNames;
    public void SaveProfile(string profileName, List<Object> list)
    {
        bool hasItem  = favoritesSavedProfiles.Any(x=>x.ProfileName == profileName);
       
        if(!hasItem)
        {
            FavoriteProfile fav = new FavoriteProfile();
            fav.ProfileName = profileName;
            fav.iteemList = list;
            favoritesSavedProfiles.Add(fav);
        }
        else
        {
            var item = favoritesSavedProfiles.First(x=>x.ProfileName == profileName);
            item.iteemList = list;
        }

        profileNames = GetProfileList();
    }

    public void UpdateProfile(string oldName, string newName, List<Object> list)
    {
      
        bool hasItem  = favoritesSavedProfiles.Any(x=>x.ProfileName == oldName);
       
        if (!hasItem)
        {  
            Debug.LogError("cant update cuz no key with Oldname found");//            
        }
        else
        {
            var item = favoritesSavedProfiles.First(x=>x.ProfileName == oldName);
            item.ProfileName = newName;
            item.iteemList = list;
        }

        profileNames = GetProfileList();

    }


    public List<string> GetProfileList()
    {
        List<string> profileNameList = new List<string>();

        if(favoritesSavedProfiles.Count  == 0) return null;
        foreach (var item in favoritesSavedProfiles)
        {
            profileNameList.Add(item.ProfileName);
        }

        return profileNameList;
    }


    public List<Object> LoadProfile(string name)
    {
        foreach (var item in favoritesSavedProfiles)
        {
            if(item.ProfileName == name)
            {
                return item.iteemList;
            }
        }

        return null;
    }


    public void DeleteProfile(string profileName)
    {
        var item = favoritesSavedProfiles.First(x=>x.ProfileName == profileName);

        favoritesSavedProfiles.Remove(item);

    }

}


[System.Serializable]
public class FavoriteProfile
{
    [SerializeField]  public string ProfileName;
    [SerializeField]   public List<Object> iteemList;

}




// [System.Serializable]
// public class SerializableDictionary
// {
//     [SerializeField]
//     public List<string> Keys = new List<string>();

//     [SerializeField]
//     private List<List<Object>> values = new List<List<Object>>();

//     private Dictionary<string, Object> dictionary = new Dictionary<string, Object>();

//     public void Add(string key, List<Object> value)
//     {
//         if (Keys.Contains(key))
//         {
//             Debug.LogError("the key is already in the dict");
//             return;
//         }

//         Keys.Add(key);
//         values.Add(value);

//     }

//     public void Remove(string key)
//     {
//         int i = 0;
//         foreach (var item in Keys)
//         {
//             if (item == key)
//             {
//                 Keys.Remove(key);
//                 values.RemoveAt(i);

//             }

//             i++;
//         }
//     }

//     public bool ContainsKey(string key)
//     {
//         return Keys.Contains(key);
//     }

//     public List<Object> GetVAlue(string key)
//     {
//         if(!Keys.Contains(key))
//         {
//             Debug.Log("Not found");
//         }
//         int i = 0;

//         foreach (var item in Keys)
//         {
//             if(item == key)
//             {
//                 return values[i];
//             }
//             i++;
//         }
//         return null;//
//     }
// }
