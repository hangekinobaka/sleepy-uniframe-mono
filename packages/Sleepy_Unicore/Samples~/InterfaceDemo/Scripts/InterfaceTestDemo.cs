using UnityEngine;

namespace Sleepy.Demo
{
    public class InterfaceTestDemo : MonoBehaviour
    {
        [SerializeField]
        SleepyInterface<ITestInterface1> _testInterface1 = new SleepyInterface<ITestInterface1>();

        [SerializeField]
        SleepyInterface<ITestInterface2> _testInterface2 = new SleepyInterface<ITestInterface2>();

        private void Awake()
        {
            if (_testInterface1 == null)
            {
                Dev.Error("No implementation for ITestInterface1");
            }
            else
            {
                _testInterface1.Instance.DemoFunction1();
            }

            if (_testInterface2 == null)
            {
                Dev.Error("No implementation for ITestInterface2");
            }
            else
            {
                _testInterface2.Instance.DemoFunction2();
            }
        }
    }
}