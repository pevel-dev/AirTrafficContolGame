using System.Collections.Generic;
using System.Linq;
using Source.Controllers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Source.Models
{
    /// <summary>
    /// Модель самолета
    /// </summary>
    public class Airplane : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const double Eps = 10;

        public GameObject prefabPoint;
        public GameObject parentPrefabPoints;
        public GameObject prefabHealthBar;
        public GameObject airplane;
        public List<GameObject> _path = new();
        public float downScaleSpeed = 0.002f;
        public float speed;
        public Vector3 delta;
        public float MinimalLandingLength = 300;

        private VisualizeAirplane _linesPath;
        private HealthBar _healthBar;

        private Vector3 _startSetPathPosition;

        private bool _downLocalScale = false;

        private Vector3 Position => transform.position;

        private IEnumerable<Vector3> Path()
        {
            yield return transform.position;
            foreach (var pathPoint in _path)
            {
                yield return pathPoint.transform.position;
            }
        }

        public void Awake()
        {
            var lineRenderer = this.AddComponent<LineRenderer>();
            _linesPath = new VisualizeAirplane(lineRenderer);
            InitializeHealthBar();
        }

        public void LoadPath(List<Vector3> path)
        {
            foreach (var pathPoint in path.Skip(1))
            {
                var pathPointObject = Instantiate(prefabPoint, transform);
                pathPointObject.transform.position = pathPoint;
                pathPointObject.transform.SetParent(parentPrefabPoints.transform);
                _path.Add(pathPointObject);
            }

            UpdateDelta();
        }

        public void Update()
        {
            if (!_healthBar.Status())
                _downLocalScale = true;
            UpdatePosition();
            _linesPath.UpdatePosition(Path().ToList());
            UpdateDelta();
            UpdateLocalScale();
        }

        private void UpdateLocalScale()
        {
            if (_downLocalScale)
            {
                transform.localScale -= new Vector3(downScaleSpeed, downScaleSpeed, downScaleSpeed);
            }

            if (transform.localScale.x < 0.1f)
            {
                Destroy(gameObject);
            }
        }

        private void InitializeHealthBar()
        {
            _healthBar = Instantiate(prefabHealthBar, Position, Quaternion.identity).GetComponent<HealthBar>();
            _healthBar.transform.SetParent(transform);
            _healthBar.Initialize(15);
        }

        private void UpdatePosition()
        {
            if (_path.Count > 0)
            {
                transform.position += delta;
                var difference = transform.position - _path[0].transform.position;
                difference.Normalize();

                var rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);

                if (Vector2.Distance(_path[0].transform.position, Position) < Eps)
                {
                    NextPathPoint();
                }
            }
        }

        private void NextPathPoint()
        {
            Destroy(_path[0]);
            _path.RemoveAt(0);

            if (_path.Count == 0)
            {
                var position = transform.position + 100 * delta;
                var pointObject = Instantiate(prefabPoint, position, Quaternion.identity);
                pointObject.transform.SetParent(parentPrefabPoints.transform);
                _path.Add(pointObject);
            }
        }

        private void UpdateDelta()
        {
            if (_path.Count > 0)
                delta = (_path[0].transform.position - transform.position).normalized * speed * Time.deltaTime;
        }

        public void OnDestroy()
        {
            AirplanesController.AirplaneCount--;
            Destroy(_healthBar.gameObject);
            Destroy(_linesPath.LineRenderer);
            foreach (var obj in _path)
            {
                Destroy(obj);
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Airplane>() is not null &&
                (other.gameObject.transform.position - transform.position).magnitude < 40)
            {
                _downLocalScale = true;
                airplane.GetComponent<Animator>().Play("Plane_explosing");
            }

            if (other.gameObject.CompareTag("airport") && _path[^1].GetComponent<PathPoint>().OnCollisionInRunwayZone && (_path[^1].transform.position - transform.position).magnitude > MinimalLandingLength)
            {
                _downLocalScale = true;
            }
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            // if (other.gameObject.GetComponent<Airplane>() is not null &&
            //     (other.gameObject.transform.position - transform.position).magnitude < 40)
            //     Destroy(gameObject);
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _startSetPathPosition = transform.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = _startSetPathPosition;

            _path[0].transform.position = Camera.allCameras[0]
                .ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000));
        }

        public void OnDrag(PointerEventData eventData)
        {
            var difference = transform.position - Camera.allCameras[0]
                .ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000));
            difference.Normalize();

            var rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);
        }
    }
}