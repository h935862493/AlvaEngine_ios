using UnityEngine;

[System.Serializable]
public class AlvaEvent : Object
{
    [SerializeField]
    public GameObject game;
    public string[] Get()
    {
        string[] strs = new string[2] { "1","2"};
        if (game!= null)
        {
            MonoBehaviour[] monoBehaviours = game.GetComponents<MonoBehaviour>();
            strs = new string[monoBehaviours.Length];
            for (int i = 0; i < monoBehaviours.Length; i++)
            {
                strs[i] = monoBehaviours[i].GetType().ToString();
            }
        }
        return strs;
    }
    public string GetDefaultElement()
    {
        return defaultElement;
    }
    public void OnValueSelected(string content)
    {
        defaultElement = content;
    }
    private string defaultElement = "1";
}