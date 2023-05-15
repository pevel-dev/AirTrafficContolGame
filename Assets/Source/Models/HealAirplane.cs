using Source.Controllers;
using UnityEngine;

namespace Source.Models
{
    public class HealAirplane : Airplane
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Airplane>() is not null &&
                (other.gameObject.transform.position - transform.position).magnitude < 40)
            {
                _downLocalScale = true;
                airplanePrefab.GetComponent<Animator>().Play("Plane_explosing");
                GameController.AirplaneKilled();
            }

            if (other.gameObject.CompareTag("airport") && _path[^1].GetComponent<PathPoint>().OnCollisionInRunwayZone &&
                (_path[^1].transform.position - transform.position).magnitude > minimalLandingLength)
            {
                _downLocalScale = true;
                GameController.AddPoints(AirplaneTypes.Gold);
                GameController.AddHeals();
            }
            if (other.gameObject.CompareTag("money"))
            {
                Destroy(other.gameObject);
                GameController.CollectedMoney();
            }
        }
    }
}