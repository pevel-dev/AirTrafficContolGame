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
            // delta = new Vector3(
            //     Position.x - Mathf.Lerp(Position.x, _path[0].transform.position.x, Time.fixedTime * speed),
            //     Position.y - Mathf.Lerp(Position.y, _path[0].transform.position.y, Time.fixedTime * speed),
            //     Position.z - Mathf.Lerp(Position.z, _path[0].transform.position.z, Time.fixedTime * speed));
        }
    }
}