using Source.Controllers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Source.Models
{
    public class MilitaryAirplane : Airplane, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Airplane>() is not null &&
                (other.gameObject.transform.position - transform.position).magnitude < 40)
            {
                _downLocalScale = true;
                airplanePrefab.GetComponent<Animator>().Play("Plane_explosing");
                _gameController.AirplaneKilled();
            }

            if (other.gameObject.CompareTag("airport") && _path[^1].GetComponent<PathPoint>().OnCollisionInRunwayZone &&
                (_path[^1].transform.position - transform.position).magnitude > minimalLandingLength)
            {
                // TODO: Анимация взрыва
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
            // TODO: Анимация взрыва
            _gameController.EndGame();
        }
        private void OnMouseOver()
        {
            var mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheelScroll != 0)
            {
            }
        }
    }
}