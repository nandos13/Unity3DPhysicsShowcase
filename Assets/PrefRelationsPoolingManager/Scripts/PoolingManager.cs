using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* AUTHOR:
 * Jake Perry
 * 
 * DESCRIPTION:
 * Static class to manage object pooling.
 */

namespace JakePerry
{
    public static class PoolingManager
    {
        private class internalPool
        {
            private GameObject _obj;

            private float _disableTimer = 0.0f;
            private uint _startSize = 0;

            private List<GameObject> _pool = new List<GameObject>();

            // Constructor
            public internalPool(GameObject prefab, uint startSize = 0, float disableTime = 0.0f)
            {
                if (prefab == null)
                    Debug.LogError("Tried to create a new object pool with a null reference. The pool will not be created!");
                else
                {
                    // Set variables
                    _obj = prefab;
                    _disableTimer = disableTime;
                    _startSize = startSize;

                    // Initialize the pool
                    if (_startSize > 0)
                    {
                        for (int i = 0; i < _startSize; i++)
                        {
                            addNewObject();
                        }
                    }
                }
            }

            private GameObject addNewObject()
            {
                GameObject go = GameObject.Instantiate(_obj);
                go.SetPrefabParent(_obj);
                _pool.Add(go);

                return go;
            }

            public GameObject GetNext()
            {
                GameObject go = null;

                // Iterate through all objects in the pool and find the next inactive one
                for (int i = 0; i < _pool.Count; i++)
                {
                    if (_pool[i].activeSelf == false)
                    {
                        go = _pool[i];
                        break;
                    }
                }

                // If no objects are available, add one to the pool
                if (!go)
                    go = addNewObject();

                // Set the object active and start the disable timer
                go.SetActive(true);
                if (_disableTimer > 0)
                {
                    MonoBehaviour instance = CoroutineManager.GetInstance();
                    if (instance)
                        instance.StartCoroutine(autoDisable(go));
                }

                return go;
            }

            private IEnumerator autoDisable(GameObject go)
            {
                yield return new WaitForSeconds(_disableTimer);
                go.SetActive(false);
            }

            public bool objectEquals(GameObject go)
            {
                return (JakePerry.PrefInstanceManager.AreSamePrefab(go, _obj));
            }
        }

        private static List<internalPool> _pools = new List<internalPool>();

        /// <summary>
        /// Get the next available pooled copy of the specified object. If no pool exists for the specified object, one 
        /// will be created. Returns null if the specified prefab is null. Please use the CreateNewPool method first 
        /// if you wish to set a custom start size or make use of the automatic-disable timer functionality.
        /// </summary>
        public static GameObject GetNextAvailable(GameObject prefab)
        {
            if (!prefab)        // Error check: verify the parameter object is not null
                return null;

            // Check if a pool already exists for the specified object
            internalPool pool = existingPool(prefab);
            if (pool == null)
                pool = internalCreatePool(prefab);
            
            return pool.GetNext();
        }

        /// <summary>
        /// Creates a new pool to store copies of a specified object if no pool already exists for the object. Returns 
        /// true if a new pool was created, false if a pool already exists
        /// </summary>
        /// <param name="prefab">The object to pool</param>
        /// <param name="startSize">The number of objects to be instantiated immediately upon creation</param>
        /// <param name="disableTimer">Time in seconds for the object to stay live after being enabled. 0 = no automatic disable</param>
        public static bool CreateNewPool(GameObject prefab, uint startSize = 0, float disableTimer = 0.0f)
        {
            // Check if a pool already exists for the specified object
            if ((existingPool(prefab)) == null)
            {
                internalCreatePool(prefab, startSize, disableTimer);
                return true;
            }
            return false;
        }

        private static internalPool internalCreatePool(GameObject prefab, uint startSize = 0, float disableTimer = 0.0f)
        {
            internalPool p = new internalPool(prefab, startSize, disableTimer);
            _pools.Add(p);
            return p;
        }

        private static internalPool existingPool(GameObject go)
        {
            // Finds and returns the pool containing the specified object, if one already exists
            foreach (internalPool p in _pools)
            {
                // Check if the pool is storing the specified object
                if (p.objectEquals(go))
                    return p;
            }

            // No existing pools found
            return null;
        }
    }
}
