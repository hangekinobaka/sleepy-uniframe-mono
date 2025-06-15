using System.Diagnostics;
using UnityEngine;

namespace Sleepy.Demo
{
    public class UnicoreTestCubeProd : MonoBehaviour
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
}