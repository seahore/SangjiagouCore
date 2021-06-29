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
        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<ScreenBlurFX>().BlurSizeChangeTo(0.2f);

        // 检查存档目录下的所有存档，一一在SavedGamesPanel中生成一个项
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

    void OnDestroy()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount > 1)
            return;

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<ScreenBlurFX>().BlurSizeChangeTo(0);
        var mask = GameObject.FindGameObjectWithTag("UpperUIMask");
        if (!(mask is null)) Destroy(mask);
    }

    public void OnCloseButtonClick()
    {
        Destroy(gameObject);
    }

    public void OnSaveButtonClick()
    {
        try {
            Game.SaveGame(transform.Find("Filename Input").GetComponent<InputField>().text.Trim(), false);
        } catch (IOException) {
            GameObject.Find("UI Handler").GetComponent<UIHandler>().ShowWarningBox($"存档文件正在被占用");
        }
        OnCloseButtonClick();
    }

    public void OnLoadButtonClick()
    {
        string filename = transform.Find("Filename Input").GetComponent<InputField>().text.Trim();
        if (filename == "") return;
        try {
            Game.LoadGame(filename, false);
        } catch(FileNotFoundException) {
            GameObject.Find("UI Handler").GetComponent<UIHandler>().ShowWarningBox($"\'{filename}\'无效。");
        } catch (IOException) {
            GameObject.Find("UI Handler").GetComponent<UIHandler>().ShowWarningBox($"存档文件正在被占用");
        }

    }
}
