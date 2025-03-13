using UnityEngine;

namespace Bastien {
    
    public class Portal : MonoBehaviour {
        /// <summary>
        /// Base class for portals. The portal uses a render texture to display the "other side".
        /// The camera is inferred from the children's components.
        /// </summary>
        
        //What information does a portal need ?
        [Header("Essentials")] 
        [SerializeField] private Portal _destination;   //Allows for portal chain creation. ends w/null

        [SerializeField] private PortalCollider _portalCollider;         //Nested portal collider for events
        [SerializeField] private Camera _playerCamera;                   //Self-explanatory. Inferred from the "Main Camera" tag
        [SerializeField] private Camera _portalCamera;                   //Self-explanatory. One per portal
        [SerializeField] private RenderTexture _portalRenderTexture;     //Texture showing the destination portals' side
        [SerializeField] private Renderer _portalRenderer;               //Portal
        [SerializeField] private MeshRenderer _portalMeshRenderer;

        private void Awake() {
            _portalCollider = GetComponentInChildren<PortalCollider>();

            _playerCamera = Camera.main;            
            _portalCollider = GetComponentInChildren<PortalCollider>();
            _portalCamera = GetComponentInChildren<Camera>();
            _portalRenderer = transform.Find("PortalPlane").GetComponent<Renderer>();
            _portalMeshRenderer = transform.Find("PortalPlane").GetComponent<MeshRenderer>();
        }

        private void OnEnable() {
            _portalCollider.OnPortalEntered += PortalEnter;
            _portalCollider.OnPortalExited += PortalExit;
        }

        private void OnDisable() {
            _portalCollider.OnPortalEntered -= PortalEnter;
            _portalCollider.OnPortalExited -= PortalExit;
        }

        private void Start() {
            if (_destination) {
                CreateRenderingEnvironment();
            } else {
                _portalMeshRenderer.enabled = false;
            }
        }

        private void Update() {
            //Allows for portal chain creation. Offsets might look strange when the player is not
            //in the right rooms, but lines up with 1 portal/room
            if (_destination == null) return;
            
            Vector3 playerCamOffset = _playerCamera.transform.position - transform.position;
            _destination._portalCamera.transform.localPosition = Quaternion.AngleAxis(transform.rotation.y, Vector3.forward) * playerCamOffset;
            
            Quaternion playerCamRotation = _playerCamera.transform.rotation;
            _destination._portalCamera.transform.localRotation = playerCamRotation;
        }

        private void CreateRenderingEnvironment() {
            
            //Render texture size varies on resolution. Using RHalf for memory considerations.
            _portalRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
            _portalRenderTexture.Create();
            //AssetDatabase.CreateAsset(_portalRenderTexture, "Assets/Textures/TEMP/" + $"{this.GetInstanceID()}.rendertexture");

            //Set the destination camera to output on the original portal's texture
            _destination._portalCamera.targetTexture = _portalRenderTexture;
            _portalRenderer.material.SetTexture("_MainTex", _portalRenderTexture);
        }

        private void PortalEnter(Collider player) {
             if (_destination == null || player.CompareTag("Player") == false) return;
             
             Vector3 playerEntryOffset = player.transform.position - transform.position;
             Vector3 playerExitOffset = Vector3.Cross(playerEntryOffset, _destination.transform.forward);
             
             player.transform.position = new Vector3(
                 _destination.transform.position.x + playerExitOffset.x,
                 _destination.transform.position.y,
                 _destination.transform.position.z + playerExitOffset.z);

             //Make it so that the player keeps facing the same way despite rotation of the room
             player.transform.rotation = _destination.transform.rotation * player.transform.localRotation;
             
             //player.transform.rotation += _destination.transform.rotation;
             Debug.Log($"WPOS: {player.transform.position} -- WROT: {player.transform.rotation.eulerAngles}\n" +
                       $"LROT: {player.transform.localRotation.eulerAngles}");
        }

        private void PortalExit(Collider player) {
            if (_destination == null || player.CompareTag("Player") == false) return;
            Debug.Log($"WPOS: {player.transform.position} -- WROT: {player.transform.rotation.eulerAngles}\n" +
                      $"LROT: {player.transform.localRotation.eulerAngles}");
        }
    }
}
