using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _playerCam;
    [SerializeField] private CinemachineVirtualCamera _playerDeathCam;

    private void Start()
    {
        _playerCam.gameObject.SetActive(true);
        _playerDeathCam.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Death.onDeath += OnDeathZoom;
    }

    private void OnDisable()
    {
        Death.onDeath -= OnDeathZoom;
    }

    private void OnDeathZoom()
    {
        _playerCam.gameObject.SetActive(false);
        _playerDeathCam.gameObject.SetActive(true);
    }

}
