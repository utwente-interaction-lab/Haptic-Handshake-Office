using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dexmo.Unity.Interaction
{
    public class ButtonMover : MonoBehaviour
    {
        public Transform BtnStartPos
        { get { return _btnStartPos; } }
        public Transform BtnEndPos
        { get { return _btnEndPos; } }

        [SerializeField]
        private Transform _btnStartPos;
        [SerializeField]
        private Transform _btnEndPos;

        private BoxCollider _meshChecker;
        private Vector3 _movingVector;

        public float CurPositionNormalized;
        private int _handLayer;

        void Start()
        {
            _handLayer = LayerMask.NameToLayer("Hand");
            _meshChecker = GetComponent<BoxCollider>();
        }

        public void FollowTriggeringBody()
        {
            CurPositionNormalized = 0;
            _movingVector = _btnEndPos.position - _btnStartPos.position;
            BinarySearchNormalized(20);
        }

        private void BinarySearchNormalized(int maxIter = 1000)
        {
            int iter = 0;
            float lower = 0;
            float upper = 1;
            float curPositionNormalized=0;
            _MoveToPosition(curPositionNormalized);
            while (iter < maxIter)
            {
                if (CheckOverlappingWithTriggeringBody())
                {
                    lower = curPositionNormalized;
                    curPositionNormalized = (lower + upper) / 2;
                }
                else
                {
                    upper = curPositionNormalized;
                    curPositionNormalized = (lower + upper) / 2;
                }

                _MoveToPosition(curPositionNormalized);
                iter++;
            }
        }

        public bool CheckOverlappingWithTriggeringBody()
        {
            Collider[] CollidersInMeshChecker = CheckOverlapColliders(_meshChecker);
            var handInChecker = false;
            if (CollidersInMeshChecker != null)
            {
                foreach (var item in CollidersInMeshChecker)
                {
                    if (item.gameObject.layer == _handLayer&& !item.gameObject.name.Contains("Constrain"))
                    {
                        handInChecker = true;
                    }
                }
            }
            return handInChecker;
        }

        private void _MoveToPosition(float positionNormalized)
        {
            CurPositionNormalized = positionNormalized;
            positionNormalized = Mathf.Clamp01(positionNormalized);
            transform.position = _btnStartPos.position + positionNormalized * _movingVector;
        }

        public void MoveToStartPos()
        {
            transform.position = _btnStartPos.position;
        }

        public void MoveToEndPos()
        {
            transform.position = _btnEndPos.position;
        }

        private Collider[] CheckOverlapColliders(BoxCollider _boxCollider)
        {
            Transform _attachedTransform = _boxCollider.transform;
            Vector3 _center = _attachedTransform.TransformPoint(_boxCollider.center);
            Vector3 _extents = Vector3.Scale(_boxCollider.size * 0.5f, _attachedTransform.lossyScale).Abs();
            return Physics.OverlapBox(_center, _extents, _attachedTransform.rotation);
        }
    }
}

