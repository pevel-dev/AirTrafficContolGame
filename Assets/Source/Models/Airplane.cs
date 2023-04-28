using System;
using System.Collections.Generic;
using System.Linq;
using Source.Controllers;
using Unity.VisualScripting;
using UnityEngine;

namespace Source.Models
{
    /// <summary>
    /// Модель самолета
    /// </summary>
    public class Airplane : MonoBehaviour
    {
        private const double Eps = 10;

        public GameObject prefabPoint;
        public GameObject parentPrefabPoints;
        public float speed;
        public int status;
        public List<GameObject> _path = new();
        public Vector3 delta;
        private VisualizeAirplane linesPath;

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
            linesPath = new VisualizeAirplane(lineRenderer);
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
            linesPath.UpdatePosition(Path().ToList());
            UpdateDelta();


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

        public bool OnScreen()
            => transform.position.x >= 0 && transform.position.x <= 1920 && transform.position.y >= 0 &&
               transform.position.y <= 1080;

        public void OnDestroy()
        {
            Destroy(linesPath.LineRenderer);
            foreach (var obj in _path)
            {
                Destroy(obj);
            }
        }
    }
}   