using System.Diagnostics;
using Sleepy;
using UnityEngine;

public class TestCubeProd : MonoBehaviour
{
    private void Awake()
    {
        // Hide the cube if we are in the dev env
        HideMe();
    }

    [Conditional(SleepyConsts.DEV_ENV)]
    private void HideMe()
    {
        gameObject.SetActive(false);
    }
}
