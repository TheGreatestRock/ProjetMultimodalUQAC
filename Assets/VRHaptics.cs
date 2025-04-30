using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class VRHaptics : MonoBehaviour
{
    public HapticImpulsePlayer leftHaptic;
    public HapticImpulsePlayer rightHaptic;
    public XROrigin xrOrigin;

    public int vibrationMode = 1;
    public float randomMinDuration = 0.1f;
    public float randomMaxDuration = 0.5f;
    public float randomPause = 0.5f;
    public float moveSpeed = 2.0f;

    private Coroutine _vibrationRoutine;

    public void SetHapticFeedback()
    {
        if (UserSessionManager.Instance != null)
        {
            string vibrationType = UserSessionManager.Instance.VibrationType;
            switch (vibrationType)
            {
                case "Aucun retour":
                    SetVibrationMode(1);
                    break;
                case "Vibration aléatoire":
                    SetVibrationMode(2);
                    break;
                case "Vibration rythmique":
                    SetVibrationMode(3);
                    break;
                case "Vibration dépendante de la vitesse":
                    SetVibrationMode(4);
                    break;
                default:
                    SetVibrationMode(1);
                    break;
            }

            Debug.Log("Vibration type: " + vibrationType);
        }
    }

    public void SetVibrationMode(int mode)
    {
        vibrationMode = mode;

        if (_vibrationRoutine != null)
        {
            StopCoroutine(_vibrationRoutine);
            _vibrationRoutine = null;
        }

        switch (vibrationMode)
        {
            case 1:
                // No haptic feedback
                break;
            case 2:
                _vibrationRoutine = StartCoroutine(RandomVibration());
                break;
            case 3:
                _vibrationRoutine = StartCoroutine(FixedRhythmicVibration());
                break;
            case 4:
                _vibrationRoutine = StartCoroutine(SpeedBasedVibration());
                break;
        }
    }

    void SendHapticFeedback(HapticImpulsePlayer haptic, float amplitude, float duration)
    {
        if (haptic)
        {
            haptic.SendHapticImpulse(amplitude, duration);
        }
    }

    IEnumerator RandomVibration()
    {
        while (true)
        {
            float duration = Random.Range(randomMinDuration, randomMaxDuration);
            SendHapticFeedback(leftHaptic, 0.5f, duration);
            SendHapticFeedback(rightHaptic, 0.5f, duration);
            yield return new WaitForSeconds(duration);
        }
    }

    IEnumerator FixedRhythmicVibration()
    {
        while (true)
        {
            SendHapticFeedback(leftHaptic, 0.7f, 0.5f);
            SendHapticFeedback(rightHaptic, 0.7f, 0.5f);
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator SpeedBasedVibration()
    {
        if (!xrOrigin)
        {
            Debug.LogWarning("XR Origin is not assigned.");
            yield break;
        }

        Vector3 lastPosition = xrOrigin.transform.position;

        while (true)
        {
            Vector3 currentPosition = xrOrigin.transform.position;
            float speed = (currentPosition - lastPosition).magnitude / 0.1f;
            lastPosition = currentPosition;

            float intensity = Mathf.Clamp01(speed);

            SendHapticFeedback(leftHaptic, intensity, 0.1f);
            SendHapticFeedback(rightHaptic, intensity, 0.1f);

            yield return new WaitForSeconds(0.1f);
        }
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetHapticFeedback();
    }
}
