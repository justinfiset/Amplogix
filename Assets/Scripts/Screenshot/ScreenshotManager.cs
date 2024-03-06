using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour
{
    public SaveScreenShotPopup saveScreenShotPopup;

  
    public void FindCamera()
    {
        saveScreenShotPopup.Show();
    }
}
