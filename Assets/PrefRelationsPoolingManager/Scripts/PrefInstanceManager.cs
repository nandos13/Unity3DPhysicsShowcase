using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* AUTHOR:
 * Jake Perry
 * 
 * DESCRIPTION:
 * Static class for managing relationships between instantiated objects and their original copies.
 */

namespace JakePerry
{
    public static class PrefInstanceManager
    {
        private static Dictionary<GameObject, GameObject> _relationships = new Dictionary<GameObject, GameObject>();

        /// <summary>
        /// Adds a new relationship to the list.
        /// </summary>
        /// <param name="newObj">The newly instantiated object.</param>
        /// <param name="originalObj">The object instantiated from</param>
        public static void AddRelationship(GameObject newObj, GameObject originalObj)
        {
            _relationships[newObj] = originalObj;
        }

        /// <summary>
        /// Removes the relationship between child and the object it was instantiated from.
        /// </summary>
        public static void RemoveRelationship(GameObject child)
        {
            _relationships.Remove(child);
        }

        /// <summary>
        /// Returns a reference to the GameObject the paramater object was instantiated from.
        /// </summary>
        /// <returns>If the relationship exists, returns the object the child was instantiated from. Else, returns null.</returns>
        public static GameObject GetInstantiatedParent(GameObject child)
        {
            GameObject val = null;
            _relationships.TryGetValue(child, out val);

            return val;
        }

        /// <summary>
        /// Recursively loops through instantiated-parent heirarchy and returns a reference to the top-level parent.
        /// </summary>
        /// <returns>If any relationships exist, returns the top-level object the child was instantiated from. Else, returns null.</returns>
        public static GameObject GetRootParent(GameObject child)
        {
            GameObject temp = GetInstantiatedParent(child);

            if (temp)
            {
                // Get root object
                GameObject parent = temp;
                while (true)
                {
                    temp = GetInstantiatedParent(temp);
                    if (temp)
                        parent = temp;
                    else
                        break;
                }

                return parent;
            }
            else
                return null;  // child is unique object
        }

        public static bool AreSamePrefab(GameObject obj1, GameObject obj2)
        {
            if (!obj1 || !obj2)     // Error check: ensure both parameters have a value that is not null
                return false;

            // Find top-level original objects for each parameter object
            GameObject o1Root = GetRootParent(obj1);
            if (o1Root == null)
                o1Root = obj1;

            GameObject o2Root = GetRootParent(obj2);
            if (o2Root == null)
                o2Root = obj2;

            return (o1Root == o2Root);
        }
    }
}
