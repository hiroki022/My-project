using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoystickPlayer {
    public class JoystickPlayerExample : MonoBehaviour
    {
        public float speed;
        public FloatingJoystick floatingJoystick;
        public Rigidbody rb;

        public void FixedUpdate()
        {
            Vector3 direction = Vector3.forward * floatingJoystick.Vertical + Vector3.right * floatingJoystick.Horizontal;
            rb.AddForce(direction * speed * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
