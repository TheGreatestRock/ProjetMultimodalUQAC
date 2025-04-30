using System;
using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using Random = UnityEngine.Random;

public class VRHaptics : MonoBehaviour
{
    public HapticImpulsePlayer leftHaptic;   // Haptics du contrôleur gauche
    public HapticImpulsePlayer rightHaptic;  // Haptics du contrôleur droit
    public XROrigin xrOrigin; // XR Origin pour la vitesse du joueur
    public int vibrationMode = 1; // Mode de vibration actif
    public float randomMinDuration = 0.1f; // Durée min vibration aléatoire
    public float randomMaxDuration = 0.5f; // Durée max vibration aléatoire
    public float randomPause = 0.5f; // Pause entre les vibrations aléatoires
    public float moveSpeed = 2.0f; // Vitesse du joueur

    private Coroutine vibrationRoutine;

    
    //set haptics
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
            //log the vibration type
            Debug.Log("Vibration type: " + vibrationType);
        }
    }

    public void SetVibrationMode(int mode)
    {
        vibrationMode = mode;

        if (vibrationRoutine != null)
            StopCoroutine(vibrationRoutine);

        switch (vibrationMode)
        {
            case 1:
                // Aucun retour haptique
                break;
            case 2:
                vibrationRoutine = StartCoroutine(RandomVibration());
                break;
            case 3:
                vibrationRoutine = StartCoroutine(FixedRhythmicVibration());
                break;
            case 4:
                vibrationRoutine = StartCoroutine(SpeedBasedVibration());
                break;
        }
    }

    void SendHapticFeedback(HapticImpulsePlayer haptic, float amplitude, float duration)
    {
        if (haptic)
            haptic.SendHapticImpulse(amplitude, duration);
    }

    IEnumerator RandomVibration()
    {
        while (true)
        {
            float duration = Random.Range(randomMinDuration, randomMaxDuration);
            SendHapticFeedback(leftHaptic, 0.5f, duration);
            SendHapticFeedback(rightHaptic, 0.5f, duration);
            yield return new WaitForSeconds(duration + randomPause);
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
        while (true)
        {
            //moving speed of the player
            float intensity = Mathf.Clamp(xrOrigin.GetComponent<Rigidbody>().velocity.magnitude / moveSpeed, 0f, 1f);
            SendHapticFeedback(leftHaptic, intensity, 0.1f);
            SendHapticFeedback(rightHaptic, intensity, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
