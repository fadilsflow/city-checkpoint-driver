using UnityEngine;

public class ScreenshotTaker : MonoBehaviour
{
    public string screenshotPath = "/tmp/ss_gameplay.png";

    private void Start()
    {
        ScreenCapture.CaptureScreenshot(screenshotPath);
    }
}
