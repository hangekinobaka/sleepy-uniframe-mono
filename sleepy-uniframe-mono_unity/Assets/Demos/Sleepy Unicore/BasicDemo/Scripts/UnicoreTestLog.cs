using UnityEngine;

namespace Sleepy.Demo
{
    public class UnicoreTestLog : MonoBehaviour
    {
        void Start()
        {

#if SLEEPY_DEV_ENV
            UnityEngine.Debug.Log("This is dev env so you can see:");
#elif SLEEPY_PROD_ENV
            UnityEngine.Debug.Log("This is prod env so you cannot see the sleepy log");
#endif
            Dev.Log("This sleepy log");

        }

    }
}
