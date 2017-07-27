using UnityEngine;
using System.Collections;

/* AUTHOR:
 * Jake Perry
 * 
 * DESCRIPTION:
 * Static class for extension methods.
 */

namespace JakePerry
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Records parent as the object's original copy. This method should be called on a newly instantiated object, 
        /// with the parent paramater of the original object, immediately after being created.
        /// </summary>
        /// <param name="parent"></param>
        public static void SetPrefabParent(this GameObject go, GameObject parent)
        {
            JakePerry.PrefInstanceManager.AddRelationship(go, parent);
        }
    }
}
