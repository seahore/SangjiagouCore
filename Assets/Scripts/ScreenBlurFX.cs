using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ScreenBlurFX : MonoBehaviour
{
    // 预先定义shader渲染用的pass
    const int BLUR_HOR_PASS = 0;
    const int BLUR_VER_PASS = 1;

    RenderTexture finalBlurRT;
    RenderTexture tempRT;
    public Material BlurMat; // 模糊材质球

    // 外部参数
    [Range(0, 127)]
    public float BlurSize = 1.0f; // 模糊额外散步大小
    [Range(1, 10)]
    public int BlurIteration = 4; // 模糊采样迭代次数
    public float BlurSpread = 1; // 模糊散值
    public int BlurDownSample = 4; // 模糊初始降采样比率
    public bool RenderBlurFX = false; // 是否开始渲染模糊效果

    float sampleTarget;
    public float fadeVelocity = 0.5f;

    void Start()
    {
        sampleTarget = BlurSize;
    }

    void Update()
    {
        if (fadeVelocity != 0.0f) {
            float newVal;
            if (BlurSize < sampleTarget) {
                newVal = BlurSize + (fadeVelocity * Time.deltaTime);
                BlurSize = newVal > sampleTarget ? sampleTarget : newVal;
            } else {
                newVal = BlurSize - (fadeVelocity * Time.deltaTime);
                BlurSize = newVal < sampleTarget ? sampleTarget : newVal;
            }
            RenderBlurFX = BlurSize < 0.001f ? false : true;
        }
    }

    public void BlurSizeChangeTo(float newVal) => sampleTarget = newVal;


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (BlurMat != null && RenderBlurFX) {
            // 首先对输出的结果做一次降采样，也就是降低分辨率，减小RT图的大小
            int width = src.width / BlurDownSample;
            int height = src.height / BlurDownSample;
            // 将当前摄像机画面渲染到被降采样的RT上
            finalBlurRT = RenderTexture.GetTemporary(width, height, 0);
            Graphics.Blit(src, finalBlurRT);

            int curIterNum = 1; // 初始化迭代
            while (curIterNum <= BlurIteration) {
                BlurMat.SetFloat("_BlurSize", (1.0f + curIterNum * BlurSpread) * BlurSize);  // 设置模糊扩散uv偏移
                tempRT = RenderTexture.GetTemporary(width, height, 0);
                // 使用blit的其他重载，针对对应的材质球和pass进行渲染并输出结果
                Graphics.Blit(finalBlurRT, tempRT, BlurMat, BLUR_HOR_PASS);
                Graphics.Blit(tempRT, finalBlurRT, BlurMat, BLUR_VER_PASS);
                RenderTexture.ReleaseTemporary(tempRT);   // 释放临时RT
                curIterNum++;
            }
            Graphics.Blit(finalBlurRT, dest);
            RenderTexture.ReleaseTemporary(finalBlurRT);  // final_blur_rt作用已经完成，可以回收了
        } else {
            Graphics.Blit(src, dest);
        }
    }

}
