using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Source.Models
{
    /// <summary>
    /// Путевая точка 
    /// </summary>
    public class PathPoint : MonoBehaviour
    {
        public bool OnCollisionInRunwayZone { get; private set; }

        private void OnMouseDrag()
        {
            transform.position =
                Camera.allCameras[0]
                    .ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("RunwayLandingZone"))
                OnCollisionInRunwayZone = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("RunwayLandingZone"))
                OnCollisionInRunwayZone = false;
        }
    }
}