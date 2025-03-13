using System;
using UnityEngine;

namespace Bastien {
    
    [RequireComponent(typeof(BoxCollider))]
    
    public class PortalCollider : MonoBehaviour {
        // Pair of events to be called from Portal script
        public event Action<Collider> OnPortalEntered;
        public event Action<Collider> OnPortalExited;
        
        // The following functions are based on Fred Bast's code
        private void OnTriggerEnter(Collider player) {
            OnPortalEntered?.Invoke(player);
        }

        private void OnTriggerExit(Collider player) {
            OnPortalExited?.Invoke(player);
        }
    }
}