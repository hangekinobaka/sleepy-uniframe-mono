using System.Diagnostics;
using Sleepy;
using UnityEngine;

public class TestCubeDev : MonoBehaviour
{
    private void Awake()
    {
        // Hide the cube if we are in the prod env
        HideMe();
    }

    [Conditional(SleepyConsts.PROD_ENV)]
    private void HideMe()
    {
        gameObject.SetActive(false);
    }
}
