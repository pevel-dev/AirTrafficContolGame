using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Source.Models
{
    public class Airplane
    {
        private const double Eps = 10;

        public int id;
        public int textureId;
        public int fuel;
        public float speed;

        public Vector3 nextPoint;
        public List<Vector3> path; // TODO: Переделать - List не оптимальная структура для этого
        public Vector3 currentPosition;
        public Vector3 delta;


        public Airplane(int id, int textureId, int fuel, float speed, IEnumerable<Vector3> path)
        {
            this.id = id;
            this.textureId = textureId;
            this.fuel = fuel;
            this.speed = speed;
            this.path = new List<Vector3>(path);
            currentPosition = this.path[0];
            nextPoint = this.path[1];
            this.path.RemoveAt(0);
            this.path.RemoveAt(0);
            UpdateDelta();
        }

        public void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            currentPosition += -delta;

            if (Vector2.Distance(nextPoint, currentPosition) < Eps)
            {
                NextPathPoint();
            }
        }

        private void NextPathPoint()
        {
            if (path.Count == 0)
            {
                path.Add(RotateVectorOn2(currentPosition, 0.074533f));
            }

            nextPoint = path[0];
            UpdateDelta();
            path.RemoveAt(0);
        }

        private void UpdateDelta()
        {
            delta = new Vector3(
                currentPosition.x - Mathf.Lerp(currentPosition.x, nextPoint.x, Time.deltaTime * speed),
                currentPosition.y - Mathf.Lerp(currentPosition.y, nextPoint.y, Time.deltaTime * speed),
                currentPosition.z - Mathf.Lerp(currentPosition.z, nextPoint.z, Time.deltaTime * speed));
        }

        private static Vector3 RotateVectorOn2(Vector3 vector, float angle)
        {
            //TODO: ВЫНЕСТИ
            var newX = vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle);
            var newY = vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle);
            vector.x = newX;
            vector.y = newY;
            return vector;
        }
    }
}