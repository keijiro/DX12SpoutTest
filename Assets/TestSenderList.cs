using UnityEngine;
using Klak.Spout;

sealed class TestSenderList : MonoBehaviour
{
    void Start()
    {
        foreach (var name in SpoutManager.GetSourceNames())
            Debug.Log(name);
    }
}
