using Lightbug.CharacterControllerPro.Core;
using UnityEngine;

namespace fps
{
    [RequireComponent(typeof(CharacterActor))]
    [DefaultExecutionOrder(ExecutionOrder.CharacterActorOrder - 1)]
    public class PlayerController : MonoBehaviour
    {
        #region COMPONENTS
        private CharacterActor characterActor;
        private Transform      graphics;
        #endregion

        #region VARIABLES
        [Header("Speed")] [SerializeField] [Range(5, 25)]
        private float moveSpeed = 10.0f;

        [SerializeField] [Range(5, 25)]
        private float verticalRotationSpeed = 10.0f;

        [SerializeField] [Range(5, 25)]
        private float horizontalRotationSpeed = 10.0f;

        [SerializeField]
        private float maxVerticalRotationAngle = 30;
        #endregion

        private void Awake()
        {
            characterActor   = GetComponent<CharacterActor>();
            graphics         = transform.Find("Graphics");
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            UpdatePhysics();
        }

        private void UpdatePhysics()
        {
            // Movement
            Vector3 velocity = GetMovementVelocity();
            characterActor.Velocity = velocity;

            // Rotation
            float yaw = Input.GetAxis("Mouse X") * horizontalRotationSpeed;
            float pitch = -Input.GetAxis("Mouse Y") * verticalRotationSpeed;

            Vector3 eulerAngles = characterActor.Rotation.eulerAngles + new Vector3(0, yaw, 0);
            characterActor.Rotation = Quaternion.Euler(eulerAngles);

            // vertical rotation should only affect visible graphics.
            float endAngle = QuaternionUtility.WrapAngle(graphics.localEulerAngles.x + pitch);
            if (endAngle > -maxVerticalRotationAngle && endAngle < maxVerticalRotationAngle)
            {
                graphics.Rotate(pitch, 0.0f, 0.0f, Space.Self);
            }
        }

        private Vector3 GetMovementVelocity()
        {
            Vector3 velocity = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                velocity += characterActor.Forward;
            }

            if (Input.GetKey(KeyCode.S))
            {
                velocity += -characterActor.Forward;
            }

            if (Input.GetKey(KeyCode.A))
            {
                velocity += -characterActor.Right;
            }

            if (Input.GetKey(KeyCode.D))
            {
                velocity += characterActor.Right;
            }

            return velocity * moveSpeed;
        }

    }

}