using UnityEngine;

namespace Sleepy.Demo
{
    public class InterfaceImplemented : MonoBehaviour, ITestInterface1, ITestInterface2
    {
        public void DemoFunction1()
        {
            Dev.Log("Yeah! Success!");
        }

        public void DemoFunction2()
        {
            Dev.Log("Yeah! Success!! Again!");
        }
    }
}