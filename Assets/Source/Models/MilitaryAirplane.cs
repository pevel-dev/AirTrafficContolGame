using System.Linq;
using Source.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Source.Models
{
    public class MilitaryAirplane : Airplane, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private void Update()
        {
            if (!_healthBar.Status())
            {
                if (!_downLocalScale)
                    _gameController.AirplaneKilled();
                _downLocalScale = true;
            }

            if (!_downLocalScale && !(_path[0].transform.position.x is >= 0 and <= 1920 && _path[0].transform.position.y is >= 0 and <= 1080))
            {
                _gameController.AddPoints(AirplaneTypes.Soldier);
                _downLocalScale = true;
            }

            UpdatePosition();
            _linesPath.UpdatePosition(Path().ToList());
            var mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheelScroll != 0)
            {
                var newSpeed = speed + mouseWheelScroll * mouseMult;
                if (newSpeed is > 0 and < 100)
                    speed = newSpeed;
            }

            UpdateDelta();
            UpdateLocalScale();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Airplane>() is not null &&
                (other.gameObject.transform.position - transform.position).magnitude < radius)
            {
                _downLocalScale = true;
                airplanePrefab.GetComponent<Animator>().Play("Plane_explosing");
                _gameController.AirplaneKilled();
            }

            if (other.gameObject.CompareTag("airport") && _path[^1].GetComponent<PathPoint>().OnCollisionInRunwayZone &&
                (_path[^1].transform.position - transform.position).magnitude > minimalLandingLength)
            {
                _gameController.EndGame();
            }

            if (other.gameObject.CompareTag("money"))
            {
                Destroy(other.gameObject);
                _gameController.CollectedMoney();
            }
        }

        public new void OnBeginDrag(PointerEventData eventData)
        {
        }

        public new void OnDrag(PointerEventData eventData)
        {
        }

        public new void OnEndDrag(PointerEventData eventData)
        {
            _gameController.EndGame();
        }
    }
}