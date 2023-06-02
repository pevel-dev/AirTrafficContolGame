using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Source.Controllers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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

        [SerializeField] [Header("Радиус")] 
        protected int radius;
        
        protected GameController _gameController;
        protected PathLine _linesPath;
        protected HealthBar _healthBar;
        protected bool _downLocalScale;
        private Vector3 _delta;
        protected readonly List<GameObject> _path = new();
        private Vector3 Position => transform.position;
        private static float Speed { get; set; }

        protected IEnumerable<Vector3> Path()
        {
            yield return transform.position;
            foreach (var pathPoint in _path)
                yield return pathPoint.transform.position;
        }

        public void InitializeAirplane(Vector3 end, GameController gameController)
        {
            LoadPath(end);
            UpdateDelta();
            _gameController = gameController;
        }

        private void LoadPath(Vector3 end)
        {
            var pathPointObject = Instantiate(prefabPoint, parentPrefabPoints.transform); 
            pathPointObject.transform.position = end; 
            _path.Add(pathPointObject);
        }

        #region unityEvents

        private void Awake()
        {
            if (Speed == 0)
                Speed = speed;
            else
                speed = Speed;
            InitializeHealthBar();
            var lineRenderer = this.AddComponent<LineRenderer>();
            _linesPath = new PathLine(lineRenderer, lineColor, widthLine);
        }

        private void Start()
        {
            //airplanePrefab.GetComponent<Animator>().Play("Light");
        }


        private void Update()
        {
            if (!IsAlive())
            {
                if (!_downLocalScale)
                    _gameController.AirplaneKilled();
                _downLocalScale = true;
            }
            UpdatePosition();
            _linesPath.UpdatePosition(Path().ToList());
            CheckMouseScroll();
            UpdateDelta();
            UpdateLocalScale();
        }

        protected void CheckMouseScroll()
        {
            var mouseWheelScroll = Input.GetAxis("Mouse ScrollWheel");
            if (mouseWheelScroll == 0) return;
            var newSpeed = speed + mouseWheelScroll * mouseMult;
            if (newSpeed is <= 0 or >= 100) return;
            speed = newSpeed;
            Speed = newSpeed;
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
            var newPos = Camera.allCameras[0]
                .ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1000));
            if (newPos.x is >= 0 and <= 1920 &&
                newPos.y is >= 0 and <= 1080)
            {
                _path[0].transform.position = newPos;
            }
        }
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_downLocalScale)
            {
                if (other.gameObject.GetComponent<Airplane>() is not null &&
                    (other.gameObject.transform.position - transform.position).magnitude < radius)
                {

                    _downLocalScale = true;
                    airplanePrefab.GetComponent<Animator>().Play("Plane_explosing");
                    _gameController.AirplaneKilled();
                }

                if (other.gameObject.CompareTag("airport") &&
                    _path[^1].GetComponent<PathPoint>().OnCollisionInRunwayZone &&
                    (_path[^1].transform.position - transform.position).magnitude > minimalLandingLength)
                {
                    _downLocalScale = true;
                    _gameController.AddPoints(AirplaneTypes.Basic);
                    _gameController.AirplaneDown();
                }

                if (other.gameObject.CompareTag("money"))
                {
                    Destroy(other.gameObject);
                    _gameController.CollectedMoney();
                }
            }
        }

        #endregion

        private bool IsAlive()
            => _healthBar.Status() && _path[0].transform.position.x is >= 0 and <= 1920 &&
               _path[0].transform.position.y is >= 0 and <= 1080;

        protected void UpdateLocalScale()
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

        protected void UpdatePosition()
        {
            if (_path.Count > 0)
            {
                transform.position += transform.up * speed;
                //transform.position += new Vector3((float)Math.Cos(transform.rotation.z * Math.PI / 180), (float)Math.Sin(transform.rotation.z * Math.PI / 180), 0);
                RotateAirplaneToPathPoint();

                if (Vector2.Distance(_path[0].transform.position, Position) < eps)
                    NextPathPoint();
            }
        }

        private void RotateAirplaneToPathPoint()
        {
            var difference = transform.position - _path[0].transform.position;
            difference.Normalize();
            
            var rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + 90;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, rotZ), speed * 2 * Time.deltaTime);

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

        protected void UpdateDelta()
        {
            if (_path.Count > 0)
                _delta = (_path[0].transform.position - transform.position).normalized * (speed * Time.deltaTime);
        }
    }
}