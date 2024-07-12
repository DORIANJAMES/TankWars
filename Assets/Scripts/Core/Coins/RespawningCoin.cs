using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class RespawningCoin : Coin
    {
    
        public event Action<RespawningCoin> OnCollected;
        
        private Vector3 _previousPosition;
        
        [SerializeField] private float turnSpeed;
        private void Update()
        {
            if (_previousPosition != transform.position)
            {
                Show(true);
            }

            _previousPosition = transform.position;
            SpinForever();
        }

        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (alreadyCollected)
            {
                return 0;
            }

            alreadyCollected = true;
            OnCollected?.Invoke(this);
            return coinValue;
        }

        public void Reset()
        {
            alreadyCollected = false;
        }

        public void SpinForever()
        {
            
            transform.Rotate(0, 2 * turnSpeed, 0);
        }
    }
