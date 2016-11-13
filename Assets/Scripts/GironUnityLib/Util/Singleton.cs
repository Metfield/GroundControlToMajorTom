/**
* Singleton.cs written by Mathias Bylund, 2016
*/

using UnityEngine;

/// <summary>
/// Singelton template
/// </summary>
/// <typeparam name="T">Template of the time that extends the Singleton behavior</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;

    //Returns the instance of this singleton.
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError("An instance of " + typeof(T) +
                       " is needed in the scene, but there is none.");
                }
            }

            return instance;
        }
    }
}
