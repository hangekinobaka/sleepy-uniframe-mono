using System.Diagnostics;
using Sleepy;
using UnityEngine;

public class UnicoreTestCubeProd : MonoBehaviour
{
    private void Awake()
    {
        // Hide the cube if we are in the dev env
        HideMe();

#if SLEEPY_PROD_ENV
        UnityEngine.Debug.Log("This is prod env so you cannot see the sleepy log");
#endif
    }

    [Conditional(SleepyConsts.DEV_ENV)]
    private void HideMe()
    {
        gameObject.SetActive(false);
    }
}
