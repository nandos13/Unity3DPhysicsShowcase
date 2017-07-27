using UnityEngine;
using System.Collections;

namespace JakePerry
{
    public class SetPrefabRelation : MonoBehaviour
    {

        public GameObject original;

        void Awake()
        {
            if (original && original != this.gameObject)
            {
                // Add the relationship
                JakePerry.PrefInstanceManager.AddRelationship(this.gameObject, original);

                // Remove this component
                Destroy(this);
            }
        }
    }
}
