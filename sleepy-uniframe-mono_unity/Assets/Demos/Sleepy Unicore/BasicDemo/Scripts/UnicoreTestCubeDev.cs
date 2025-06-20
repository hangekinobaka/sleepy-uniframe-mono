using System.Diagnostics;
using UnityEngine;

namespace Sleepy.Demo
{
    public class UnicoreTestCubeDev : MonoBehaviour
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
}