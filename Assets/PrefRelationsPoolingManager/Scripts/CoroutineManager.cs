using UnityEngine;
using System.Collections;

/* AUTHOR:
 * Jake Perry
 * 
 * DESCRIPTION:
 * Static class to manage coroutines. Useful for running coroutines from
 * a non-MonoBehaviour-derived class.
 */

namespace JakePerry
{
    public class CoroutineManager : MonoBehaviour
    {

        private static MonoBehaviour _instance;

        void Awake()
        {
            if (!_instance)
            {
                _instance = this;
            }
            else
            {
                // An instance already exists
                DestroyImmediate(this);
            }
        }

        public static MonoBehaviour GetInstance()
        {
            if (_instance == null)
                Debug.LogError("Coroutine Manager has not been initialized. Please make sure an instance of CoroutineManager exists on an empty GameObject in the scene at startup.");
            return _instance;
        }
    }
}
