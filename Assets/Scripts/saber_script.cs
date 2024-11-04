using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SaberScript : MonoBehaviour
{
    private GameObject _saber, _hiltSaber1, _hiltSaber2;
    private Vector3 _offset;
    private readonly Vector3 _downScale = Vector3.zero, _targetScale = Vector3.one;
    private float _zCoord;
    private Coroutine _scaleCoroutine;
    private bool _saberActive = false; // Assign your audio clips in the Inspector
    private AudioSource[] _audioSources;
    public AudioClip[] audioClips;
    public float scaleSpeed = 1f;

    void Start()
    {
        _saber = GameObject.Find("Light Saber/saber");
        _hiltSaber1 = GameObject.Find("Light Saber/hilt_saber1");
        _hiltSaber2 = GameObject.Find("Light Saber/hilt_saber2");

        if (_saber != null)
            InitGameObject(_saber);
        if (_hiltSaber1 != null)
            InitGameObject(_hiltSaber1);
        if (_hiltSaber2 != null)
            InitGameObject(_hiltSaber2);

        _audioSources = new AudioSource[audioClips.Length];
        for (int i = 0; i < audioClips.Length; i++)
        {
            _audioSources[i] = gameObject.AddComponent<AudioSource>();
            _audioSources[i].clip = audioClips[i];
        }
    }

    private void PlaySound(int index)
    {
        foreach (var audioSource in _audioSources)
            audioSource.Stop();

        _audioSources[index].Play();
    }

    private void InitGameObject(GameObject source)
    {
        source.transform.localScale = _downScale;
        var rb = source.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        source.SetActive(false);
    }

    void Activate()
    {
        _saber.SetActive(true);
        _hiltSaber1.SetActive(true);
        _hiltSaber2.SetActive(true);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_saberActive)
            {
                PlaySound(2);
                if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = StartCoroutine(AnimateScale(_saber, _downScale));
                _scaleCoroutine = StartCoroutine(AnimateScale(_hiltSaber1, _downScale));
                _scaleCoroutine = StartCoroutine(AnimateScale(_hiltSaber2, _downScale));
                
            }
            else
            {
                Activate();
                PlaySound(1);
                if (_scaleCoroutine != null) StopCoroutine(_scaleCoroutine);
                _scaleCoroutine = StartCoroutine(AnimateScale(_saber, _targetScale));
                _scaleCoroutine = StartCoroutine(AnimateScale(_hiltSaber1, _targetScale));
                _scaleCoroutine = StartCoroutine(AnimateScale(_hiltSaber2, _targetScale));
                StartCoroutine(WaitAndPlay());
            }

            _saberActive = !_saberActive;
        }
    }

    IEnumerator AnimateScale(GameObject source, Vector3 target)
    {
        Vector3 startScale = source.transform.localScale;
        float progress = 0f;

        while (progress < 1f)
        {
            source.transform.localScale = Vector3.Lerp(startScale, target, progress);
            progress += Time.deltaTime * 1f;
            yield return null;
        }

        source.transform.localScale = target;
        if (target == Vector3.zero)
        {
            source.SetActive(false);
        }
    }

    IEnumerator WaitAndPlay()
    {
        while (_audioSources[1].isPlaying)
        {
            yield return null; // Wait for the next frame
        }

        foreach (var audioSource in _audioSources)
            audioSource.Stop();
        _audioSources[0].loop = true;
        _audioSources[0].Play();
    }
}