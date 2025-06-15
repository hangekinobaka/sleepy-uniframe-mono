using System.Diagnostics;
using Sleepy;
using UnityEngine;

public class UnicoreTestCubeDev : MonoBehaviour
{
    private void Awake()
    {
        // Hide the cube if we are in the prod env
        HideMe();

#if SLEEPY_DEV_ENV
        UnityEngine.Debug.Log("This is dev env so you can see:");
#endif
        Dev.Log("This sleepy log");

    }

    [Conditional(SleepyConsts.PROD_ENV)]
    private void HideMe()
    {
        gameObject.SetActive(false);
    }
}
