using System;
using System.Collections.Generic;
using System.Linq;
using Source.Controllers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Source.Models
{
    /// <summary>
    /// Модель самолета
    /// </summary>
    public class Airplane : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] [Header("Префаб путевой точки")]
        private GameObject prefabPoint;

        [SerializeField] [Header("Ссылка на родительский объект для путевых точек")]
        public GameObject parentPrefabPoints;

        [SerializeField] [Header("Префаб бара с жизнями")]
        private GameObject prefabHealthBar;

        [FormerlySerializedAs("airplane")] [SerializeField] [Header("Префаб самолета")]
        protected GameObject airplanePrefab;
        

        [SerializeField] [Header("Скорость уменьшения самолета при снижении")]
        private float downScaleSpeed;

        [SerializeField] [Header("Скорость самолета")]
        private float speed;

        [FormerlySerializedAs("MinimalLandingLength")] [SerializeField] [Header("Минимальное расстояние для посадки")]
        protected float minimalLandingLength;

        [FormerlySerializedAs("MouseMult")] [SerializeField] [Header("Мультипликатор мыши")]
        private float mouseMult;

        [SerializeField] [Header("Время жизни саомлета в секундах")]
        private float lifeTime;

        [FormerlySerializedAs("Eps")] [SerializeField] [Header("Погрешность для определения достижения путевой точки")]
        private float eps;

        [SerializeField] [Header("Жирность путевой линии")]
        private float widthLine;

        [SerializeField] [Header("Цвет путевой линии")]
        private Color lineColor;


        private PathLine _linesPath;
        private HealthBar _healthBar;
        protected bool _downLocalScale;
        private Vector3 _delta;
        protected readonly List<GameObject> _path = new();
        private Vector3 Position => transform.position;

        private IEnumerable<Vector3> Path()
        {
            yield return transform.position;
            foreach (var pathPoint in _path)
                yield return pathPoint.transform.position;
        }

        public void InitializeAirplane(List<Vector3> path)
        {
            LoadPath(path);
            UpdateDelta();
        }

        private void LoadPath(List<Vector3> path)
        {
            foreach (var pathPoint in path.Skip(1))
            {
                var pathPointObject = Instantiate(prefabPoint, transform);
                pathPointObject.transform.position = pathPoint;
                pathPointObject.transform.SetParent(parentPrefabPoints.transform);
                _path.Add(pathPointObject);
            }
        }

        #region unityEvents

        private void Awake()
        {
            InitializeHealthBar();
            var lineRenderer = this.AddComponent<LineRenderer>();
            _linesPath = new PathLine(lineRenderer, lineColor, widthLine);
        }


        private void Update()
        {
            if (!IsAlive())
            {
                if (!_downLocalScale)
                    GameController.AirplaneKilled();
                _downLocalScale = true;
            }

            UpdatePosition();
            _linesPath.UpdatePosition(Path().ToList());
            UpdateDelta();
            UpdateLocalScale();
        }

        private void OnDestroy()
        {
            Destroy(_healthBar.gameObject);
            Destroy(_linesPath.LineRenderer);
            foreach (var obj in _path)
                Destroy(obj);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _path[0].transform.position = Camera.allCameras[0]
                .ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000));
        }


        private void OnMouseOver()
        {
            var mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheelScroll != 0)
            {
                speed += mouseWheelScroll * mouseMult;
                UpdateDelta();
            }
        }

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
                GameController.AddPoints(AirplaneTypes.Basic);
                GameController.AirplaneKilled();
            }

            if (other.gameObject.CompareTag("money"))
            {
                Destroy(other.gameObject);
                GameController.CollectedMoney();
            }
        }

        #endregion

        private bool IsAlive()
            => _healthBar.Status() && transform.position.x is >= 0 and <= 1920 &&
               transform.position.y is >= 0 and <= 1080;

        private void UpdateLocalScale()
        {
            if (_downLocalScale)
                transform.localScale -= new Vector3(downScaleSpeed, downScaleSpeed, downScaleSpeed);

            if (transform.localScale.x < 0.1f)
                Destroy(gameObject);
        }

        private void InitializeHealthBar()
        {
            _healthBar = Instantiate(prefabHealthBar, Position, Quaternion.identity).GetComponent<HealthBar>();
            _healthBar.transform.SetParent(transform);
            _healthBar.Initialize(lifeTime);
        }

        private void UpdatePosition()
        {
            if (_path.Count > 0)
            {
                transform.position += _delta;
                RotateAirplaneToPathPoint();

                if (Vector2.Distance(_path[0].transform.position, Position) < eps)
                    NextPathPoint();
            }
        }

        private void RotateAirplaneToPathPoint()
        {
            var difference = transform.position - _path[0].transform.position;
            difference.Normalize();

            var rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90);
        }

        private void NextPathPoint()
        {
            Destroy(_path[0]);
            _path.RemoveAt(0);

            if (_path.Count == 0)
            {
                var position = transform.position + 100 * _delta;
                var pointObject = Instantiate(prefabPoint, position, Quaternion.identity);
                pointObject.transform.SetParent(parentPrefabPoints.transform);
                _path.Add(pointObject);
            }
        }

        private void UpdateDelta()
        {
            if (_path.Count > 0)
                _delta = (_path[0].transform.position - transform.position).normalized * (speed * Time.deltaTime);
        }
    }
}