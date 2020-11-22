﻿using System;
using UnityEngine;

namespace SandboxMoba.Shaders
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class AppearingShaderController : MonoBehaviour
    {
        private const string ORIGIN_POINT_KEY = "_AppearingOriginPoint";
        private const string ORIGIN_ROTATION_KEY = "_AppearingRotationQuaternion";
        private const string MAX_BORDER_KEY = "_ClipBorderMax";
        private const string MIN_BORDER_KEY = "_ClipBorderMin";
        private const string MIN_BORDER_RANGE_KEY = "_MinMeshY";
        private const string MAX_BORDER_RANGE_KEY = "_MaxMeshY";

        [SerializeField] private bool _syncOriginPoint;
        [SerializeField] private bool _syncOriginRotation;
        [SerializeField] private Renderer[] _renderersToControl;

        private Material[] _materialsToControl;
        private Vector3 _previouPos;
        private Quaternion _previousRotation;

        private bool _minBorderAnimationLooped;
        private float _minBorderAnimationTotalTime;
        private Func<float, float> _minBorderAnimation;
        private float _minBorderAnimationTime;
        private bool _maxBorderAnimationLooped;
        private float _maxBorderAnimationTotalTime;
        private Func<float, float> _maxBorderAnimation;
        private float _maxBorderAnimationTime;

        private float _minBorderRange;
        private float _maxBorderRange;

        public bool IsMaterialsCached => _materialsToControl != null && _materialsToControl.Length > 0;

        public float BorderRage
        {
            get => _maxBorderRange - _minBorderRange;
        }

        public float MinBorderCoordinate
        {
            get
            {
                if (!IsMaterialsCached)
                    return 0f;
                return _minBorderRange + (_maxBorderRange - _minBorderRange) * MinBorder;
            }
        }

        public float MaxBorderCoordinate
        {
            get
            {
                if (!IsMaterialsCached)
                    return 0f;
                return _minBorderRange + (_maxBorderRange - _minBorderRange) * MaxBorder;
            }
        }

        /// <summary>
        /// Setting resets max border animation
        /// </summary>
        public float MaxBorder
        {
            set
            {
                _maxBorderAnimation = null;
                setMaxBorderInternal(value);
            }
            get
            {
                if (_materialsToControl == null || _materialsToControl.Length == 0)
                    return 0f;
                return _materialsToControl[0].GetFloat(MAX_BORDER_KEY);
            }
        }

        /// <summary>
        /// Setting resets min border animation
        /// </summary>
        public float MinBorder
        {
            set
            {
                _minBorderAnimation = null;
                setMinBorderInternal(value);
            }
            get
            {
                if (_materialsToControl == null || _materialsToControl.Length == 0)
                    return 0f;
                return _materialsToControl[0].GetFloat(MIN_BORDER_KEY);
            }
        }

        private void setMaxBorderInternal(float value)
        {
            value = Mathf.Clamp(value, 0, 1f);
            foreach (Material mat in _materialsToControl)
            {
                mat.SetFloat(MAX_BORDER_KEY, value);
            }
        }

        private void setMinBorderInternal(float value)
        {
            value = Mathf.Clamp(value, 0, 1f);
            foreach (Material mat in _materialsToControl)
            {
                mat.SetFloat(MIN_BORDER_KEY, value);
            }
        }

#if UNITY_EDITOR
        public bool SyncInEditor { get; set; }
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
                return;
#endif
            CacheNewMaterials();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isEditor && !SyncInEditor)
                return;
#endif
            if (!_syncOriginPoint && !_syncOriginRotation)
                return;
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            foreach (Material mat in _materialsToControl)
            {

                if (_syncOriginPoint && _previouPos != position)
                {
                    mat.SetVector(ORIGIN_POINT_KEY, position);
                }
                if (_syncOriginRotation && _previousRotation != rotation)
                {
                    mat.SetVector(ORIGIN_ROTATION_KEY, new Vector4(rotation.x, rotation.y, rotation.z, rotation.w));
                }
            }

            _previousRotation = rotation;
            _previouPos = position;

            procceedAnimations();
        }

        private void procceedAnimations()
        {
            if (_minBorderAnimation != null)
            {
                _minBorderAnimationTime += Time.deltaTime;
                if (_minBorderAnimationTime > _minBorderAnimationTotalTime)
                {
                    if (_minBorderAnimationLooped)
                    {
                        _minBorderAnimationTime -= _minBorderAnimationTotalTime;
                        setMinBorderInternal(_minBorderAnimation(_minBorderAnimationTime));
                    }
                    else
                    {
                        setMinBorderInternal(_minBorderAnimation(_minBorderAnimationTotalTime));
                        _minBorderAnimation = null;
                    }
                }
                else
                {
                    setMinBorderInternal(_minBorderAnimation(_minBorderAnimationTime));
                }
            }

            if (_maxBorderAnimation != null)
            {
                _maxBorderAnimationTime += Time.deltaTime;
                if (_maxBorderAnimationTime > _maxBorderAnimationTotalTime)
                {
                    if (_maxBorderAnimationLooped)
                    {
                        _maxBorderAnimationTime -= _maxBorderAnimationTotalTime;
                        setMinBorderInternal(_maxBorderAnimation(_maxBorderAnimationTime));
                    }
                    else
                    {
                        setMinBorderInternal(_maxBorderAnimation(_maxBorderAnimationTotalTime));
                        _maxBorderAnimation = null;
                    }
                }
                else
                {
                    setMinBorderInternal(_maxBorderAnimation(_maxBorderAnimationTime));
                }
            }
        }

        public void CacheNewMaterials()
        {
            _materialsToControl = new Material[_renderersToControl.Length];
            for (int i = 0; i < _renderersToControl.Length; i++)
            {
                _materialsToControl[i] = new Material(_renderersToControl[i].sharedMaterial);
                _renderersToControl[i].sharedMaterial = _materialsToControl[i];
            }

            if (_materialsToControl.Length > 0)
            {
                _minBorderRange = _materialsToControl[0].GetFloat(MIN_BORDER_RANGE_KEY);
                _maxBorderRange = _materialsToControl[0].GetFloat(MAX_BORDER_RANGE_KEY);
            }

        }

        /// <summary>
        /// Starts changing min border acoording <paramref name="minBorderByTime"/>
        /// </summary>
        /// <param name="minBorderByTime"></param>
        /// <param name="animationTime"></param>
        /// <param name="loop"></param>
        public void SetMinAnimation(Func<float, float> minBorderByTime, float animationTime, bool loop = false)
        {
            _minBorderAnimationTime = 0f;
            _minBorderAnimation = minBorderByTime;
            _minBorderAnimationTotalTime = animationTime;
            _minBorderAnimationLooped = loop;
        }

        /// <summary>
        /// Starts changing max border acoording <paramref name="maxBorderByTime"/>
        /// </summary>
        /// <param name="maxBorderByTime"></param>
        /// <param name="animationTime"></param>
        /// <param name="loop"></param>
        public void SetMaxAnimation(Func<float, float> maxBorderByTime, float animationTime, bool loop = false)
        {
            _maxBorderAnimationTime = 0f;
            _maxBorderAnimation = maxBorderByTime;
            _maxBorderAnimationTotalTime = animationTime;
            _maxBorderAnimationLooped = loop;
        }
    }
}
