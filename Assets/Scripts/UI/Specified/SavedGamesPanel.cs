using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SangjiagouCore;
using static SangjiagouCore.Utilities.ChineseNumerals;

public class SavedGamesPanel : MonoBehaviour
{
    public GameObject MaskPrefab;
    public GameObject SavedGameSelection;
    public GameObject SelectSchoolPanelPrefab;

    UIHandler handler;

    FileInfo[] _savedGames;

    void Start()
    {
        handler = GameObject.FindWithTag("UIHandler").GetComponent<UIHandler>();

        var mask = Instantiate(MaskPrefab, transform.parent);
        transform.SetParent(mask.transform);

        var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam.GetComponent<Animator>().SetTrigger("Unfocus");

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

    public void OnCloseButtonClick()
    {
        if (GameObject.Find("Upper UI Canvas").transform.childCount <= 1) {
            var mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            mainCam.GetComponent<Animator>().SetTrigger("Focus");
        }
        GetComponent<Animator>().SetTrigger("Close");
    }

    /// <summary>
    /// 这个函数会在关闭动画的最后调用
    /// </summary>
    public void OnCloseAnimationFinished()
    {
        Destroy(transform.parent.gameObject);
    }

    public void OnSaveButtonClick()
    {
        try {
            string filename = transform.Find("Filename Input").GetComponent<InputField>().text.Trim();
            if (filename != "") {
                Game.SaveGame(transform.Find("Filename Input").GetComponent<InputField>().text.Trim(), false);
                OnCloseButtonClick();
            } else {

            }
        } catch (IOException) {
            handler.ShowWarningBox($"存档文件正在被占用");
        }
    }

    public void OnLoadButtonClick(bool switchScene)
    {
        string filename = transform.Find("Filename Input").GetComponent<InputField>().text.Trim();
        if (filename == "") return;
        try {
            Game.LoadGame(filename, false, false);
        } catch(FileNotFoundException) {
            handler.ShowWarningBox($"\'{filename}\'无效。");
        } catch (IOException) {
            handler.ShowWarningBox($"存档文件正在被占用");
        }

        if (switchScene) {
            handler.SetLoadingMask("GameScene", () => {
                void f(Scene scene, LoadSceneMode mode) {
                    OnLoaded();
                    SceneManager.sceneLoaded -= f;
                };
                SceneManager.sceneLoaded += f;
            });
        } else {
            OnLoaded();
            GetComponent<Animator>().SetTrigger("Close");   // 后面还有一个选择学派的面板要显示，所以不解除模糊
        }
    }

    public void OnLoaded()
    {
        GameObject.FindGameObjectWithTag("Tilemap").GetComponent<MapRenderer>().RefreshMap();

        GameObject.Find("Player Panel").transform.Find("Next Turn Button/Text").GetComponent<Text>().text = Game.CurrentEntities.DateToString();

        Instantiate(SelectSchoolPanelPrefab, GameObject.FindGameObjectWithTag("UIHandler").GetComponent<UIHandler>().UpperUICanvas.transform);
    }
}
