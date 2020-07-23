using UnityEngine;
using System.Collections;

public class HiResScreenShots : MonoBehaviour
{
    public Camera camera, _arCamera;
    public RenderTexture default2DCamera;
    public int resWidth = 1000;
    public int resHeight = 1000;

    private bool takeHiResShot = false;
    private bool takeHiResShotMain = false;

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    public void TakeHiResShotMain()
    {
        takeHiResShotMain = true;
    }

    void LateUpdate()
    {
        if (takeHiResShot)
        {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;


            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            screenShot.Apply();

            NativeGallery.SaveImageToGallery(screenShot, "GardeningResult", "Image.png");
            // Save the screenshot to Gallery/Photos

            // To avoid memory leaks
            RenderTexture.active = null; // JC: added to avoid errors
            camera.targetTexture = default2DCamera;
            Destroy(screenShot);
            takeHiResShot = false;
            AndroidNativeFunctions.ShowToast("Photo Saved to Gallery");
        }

        if (takeHiResShotMain)
        {
            RenderTexture rtMain = new RenderTexture(1080, 1920, 24);
            _arCamera.targetTexture = rtMain;
            _arCamera.Render();
            RenderTexture.active = rtMain;


            Texture2D screenShotMain = new Texture2D(1080, 1920, TextureFormat.RGB24, false);
            screenShotMain.ReadPixels(new Rect(0, 0, 1080, 1920), 0, 0);
            screenShotMain.Apply();

            NativeGallery.SaveImageToGallery(screenShotMain, "Gardening Capture", "Image.png");
            // Save the screenshot to Gallery/Photos

            // To avoid memory leaks
            RenderTexture.active = null; // JC: added to avoid errors
            _arCamera.targetTexture = null;
            Destroy(screenShotMain);
            takeHiResShotMain = false;
            AndroidNativeFunctions.ShowToast("Photo Saved to Gallery");
        }
    }
}