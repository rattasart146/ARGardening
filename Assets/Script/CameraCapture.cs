using System.Collections;
using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    private void Update()
    {
		StartCoroutine(TakeScreenshotAndSave());
	}

    private IEnumerator TakeScreenshotAndSave()
	{
		yield return new WaitForEndOfFrame();

		Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		ss.Apply();
		NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png");
		// Save the screenshot to Gallery/Photos

		// To avoid memory leaks
		Destroy(ss);
	}
}