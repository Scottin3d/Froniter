using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomName : MonoBehaviour {
    public static RandomName current;
    public TextAsset placesJson;
    public TextAsset namesMale;
    public TextAsset namesFemale;

    // Start is called before the first frame update
    public Places places;
    public Names maleNames;
    public Names femaleNames;

    private void Awake() {
        current = this;
        Debug.Assert(placesJson != null, "Please set places.json in editor!");
        places = JsonUtility.FromJson<Places>(placesJson.text);
        maleNames = JsonUtility.FromJson<Names>(namesMale.text);
        femaleNames = JsonUtility.FromJson<Names>(namesFemale.text);

    }
    public string GetRandomPlaceName() {
         return places.GetRandomPlace((int)Random.Range(0, places.places.Length));
    }

    public string GetRandomMaleName() {
        return maleNames.GetRandomName((int)Random.Range(0, maleNames.names.Length));
    }

    public string GetRandomFemaleName() {
        return femaleNames.GetRandomName((int)Random.Range(0, femaleNames.names.Length));
    }
}

[System.Serializable]
public class Names {
    public Name[] names;
    public string GetRandomName(int index) {
        return names[index].name;
    }
}
[System.Serializable]
public class Name {
    public string name;
}

[System.Serializable]
public class Places {
    public  PlaceName[] places;
    public string GetRandomPlace(int index) {
        return places[index].name;
    }
}
[System.Serializable]
public class PlaceName {
    public string name;
}

