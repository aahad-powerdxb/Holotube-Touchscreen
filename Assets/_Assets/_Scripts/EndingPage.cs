using UnityEngine;
using System;

public class EndingPage : MonoBehaviour
{
    public event Action OnTimerFinished;
    private float _timer = 0;
    private bool _isActive = false;
    private const float DURATION = 15f;

    public void Show()
    {
        gameObject.SetActive(true);
        _isActive = true;
        _timer = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _isActive = false;
    }

    private void Update()
    {
        if (_isActive)
        {
            _timer += Time.deltaTime;
            if (_timer >= DURATION)
            {
                _isActive = false;
                OnTimerFinished?.Invoke();
            }
        }
    }
}