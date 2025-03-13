using UnityEngine;

namespace Bastien {
    public class TestPlayerController : MonoBehaviour {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _rotSpeed;
        [SerializeField] private float _jumpForce;

        [SerializeField] private Camera _playerCamera;

        private Vector3 movement;
        private Vector3 rotation;
        private bool _isGrounded;
        
        private Rigidbody _playerRB;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            Cursor.lockState = CursorLockMode.Locked;
            _playerRB = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update() {
            // Translate
            Vector3 move = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            move *= Time.deltaTime * _moveSpeed;
            transform.Translate(move);

            // Rotate
            Vector3 rotate = new(0, Input.GetAxis("Mouse X"), 0);
            rotate *= Time.deltaTime * _rotSpeed;
            transform.transform.Rotate(rotate);
        
            Vector3 rotateCamera = new(-Input.GetAxis("Mouse Y"), 0, 0);
            rotateCamera *= Time.deltaTime * _rotSpeed;
            _playerCamera.transform.Rotate(rotateCamera);
        }

        private void OnCollisionStay(Collision other) {
            if (other.gameObject.CompareTag("Floor"))
                _isGrounded = true;
        }

        private void OnCollisionExit(Collision other) {
            if (other.gameObject.CompareTag("Floor"))
                _isGrounded = false;
        }
    }
}
