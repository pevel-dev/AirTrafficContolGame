using System;
using System.Collections.Generic;
using System.Linq;
using Source.Controllers;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;

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

        public float speed;
        public Vector3 delta;

        private VisualizeAirplane _linesPath;
        private HealthBar _healthBar;

        private Vector3 _startSetPathPosition;
        public bool inDragDrop = false;

        public Vector3 Position => transform.position;

        public IEnumerable<Vector3> Path()
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
            // Первая точка  - самолет
            transform.position = path[0];
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
            UpdatePosition();
            _linesPath.UpdatePosition(Path().ToList());
            UpdateDelta();
        }

        private void InitializeHealthBar()
        {
            _healthBar = Instantiate(prefabHealthBar, Position, Quaternion.identity).GetComponent<HealthBar>();
            _healthBar.transform.SetParent(transform);
            _healthBar.Initialize(15);
        }

        private void UpdatePosition()
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


        private void NextPathPoint()
        {
            Destroy(_path[0]);
            _path.RemoveAt(0);

            if (_path.Count == 0)
            {
                var position = transform.position + 5000 * delta;
                var pointObject = Instantiate(prefabPoint, position, Quaternion.identity);
                pointObject.transform.SetParent(parentPrefabPoints.transform);
                _path.Add(pointObject);
            }
        }

        private void UpdateDelta()
        {
            delta = (_path[0].transform.position - transform.position).normalized * speed * Time.deltaTime;
        }

        public bool Alive()
            => transform.position.x >= 0 && transform.position.x <= 1920 && transform.position.y >= 0 &&
               transform.position.y <= 1080 && _healthBar.Status();

        public void OnDestroy()
        {
            Destroy(_healthBar.gameObject);
            Destroy(_linesPath.LineRenderer);
            foreach (var obj in _path)
            {
                Destroy(obj);
            }
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            airplane.GetComponent<Animator>().Play("Plane_explosing");
            Debug.Log("Коллизия началась");
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            // Debug.Log((other.transform.position - transform.position).magnitude); - расстояние
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            Destroy(gameObject);
            Debug.Log("Коллизия закончилась");
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            _startSetPathPosition = transform.position;
            inDragDrop = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // transform.position =
            //     Camera.allCameras[0]
            //         .ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000));
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.position = _startSetPathPosition;

            _path[0].transform.position = Camera.allCameras[0]
                .ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000));
            inDragDrop = false;
        }
    }
}