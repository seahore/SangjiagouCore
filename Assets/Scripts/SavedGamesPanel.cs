using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SangjiagouCore;

public class SavedGamesPanel : MonoBehaviour
{
    public GameObject SavedGameSelection;

    FileInfo[] _savedGames;

    void Start()
    {
        _savedGames = new DirectoryInfo(Game.SavesPath).GetFiles();
        var l = transform.Find("Scroll View/Viewport/Saved Games List");
        var unitHeight = SavedGameSelection.GetComponent<RectTransform>().rect.height;
        l.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ((unitHeight + 8) * _savedGames.Length) + 8);
        float t = -8;
        foreach (var i in _savedGames) {
            var o = Instantiate(SavedGameSelection, l.transform);
            o.GetComponent<Toggle>().group = l.GetComponent<ToggleGroup>();
            o.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn) {
                if (isOn) transform.Find("Filename Input").GetComponent<InputField>().text = i.Name;
            });
            o.GetComponent<RectTransform>().offsetMin = new Vector2(8, t - unitHeight);
            o.GetComponent<RectTransform>().offsetMax = new Vector2(-8, t);
            o.transform.Find("Filename").GetComponent<Text>().text = i.Name;
            o.transform.Find("Date Time").GetComponent<Text>().text = i.LastWriteTime.ToString("G");
            t -= unitHeight + 4;
        }
    }

    public void OnCloseButtonClick()
    {
        Destroy(gameObject);
    }

    public void OnSaveButtonClick()
    {

    }
}
