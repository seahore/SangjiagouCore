using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SangjiagouCore;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float Speed = 10;
    public int ScrollTriggerEdgeWidth = 20;
    public int PixelsPerUnit = 100;

    Rect upArea;
    Rect downArea;
    Rect leftArea;
    Rect rightArea;
    Vector3 velocity;

    const float CAMERA_STANDARD_SIZE = 5;

    void Start()
    {
        upArea = transform.position.y + (0.5 * GetComponent<Camera>().pixelHeight / PixelsPerUnit) <= Game.CurrentEntities.MapSize.y
            ? new Rect(0, Screen.height - ScrollTriggerEdgeWidth, Screen.width, ScrollTriggerEdgeWidth)
            : Rect.zero;
        downArea = (transform.position.y * PixelsPerUnit) - (0.5 * GetComponent<Camera>().pixelHeight) >= 0
            ? new Rect(0, 0, Screen.width, ScrollTriggerEdgeWidth)
            : Rect.zero;
        rightArea = transform.position.x + (0.5 * GetComponent<Camera>().pixelHeight / PixelsPerUnit) <= Game.CurrentEntities.MapSize.x
            ? new Rect(Screen.width - ScrollTriggerEdgeWidth, 1f, ScrollTriggerEdgeWidth, Screen.height)
            : Rect.zero;
        leftArea = transform.position.x - (0.5 * GetComponent<Camera>().pixelHeight / PixelsPerUnit) >= 0
            ? new Rect(0, 0, ScrollTriggerEdgeWidth, Screen.height)
            : Rect.zero;
    }

    void Update()
    {
        // 移动
        velocity = new Vector3 {
            y = upArea.Contains(Input.mousePosition) ? Speed : (downArea.Contains(Input.mousePosition) ? -Speed : 0),
            x = rightArea.Contains(Input.mousePosition) ? Speed : (leftArea.Contains(Input.mousePosition) ? -Speed : 0),
            z = 0
        };
        // 因为摄像机的缩放是缩小视口，所以当缩放较近时移动速度可能会太快，故乘一个相机当前size和标准size的比例
        transform.Translate(velocity * Time.deltaTime * (GetComponent<Camera>().orthographicSize / CAMERA_STANDARD_SIZE));

        // 缩放
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            float wheel = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100;
            GetComponent<Camera>().orthographicSize -= wheel;
        }
    }
}
