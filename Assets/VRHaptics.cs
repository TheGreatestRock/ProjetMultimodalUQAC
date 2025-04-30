using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

/// <summary>
/// Gère les retours haptiques (vibrations) des contrôleurs VR.
/// </summary>
public class VRHaptics : MonoBehaviour
{
    public HapticImpulsePlayer leftHaptic; // Contrôleur haptique gauche
    public HapticImpulsePlayer rightHaptic; // Contrôleur haptique droit
    public XROrigin xrOrigin; // Référence à l'origine XR

    public int vibrationMode = 1; // Mode de vibration actuel
    public float randomMinDuration = 0.1f; // Durée minimale pour les vibrations aléatoires
    public float randomMaxDuration = 0.5f; // Durée maximale pour les vibrations aléatoires
    public float randomPause = 0.5f; // Pause entre les vibrations aléatoires
    public float moveSpeed = 2.0f; // Vitesse de déplacement pour les vibrations basées sur la vitesse

    private Coroutine _vibrationRoutine; // Routine en cours pour les vibrations

    /// <summary>
    /// Configure le retour haptique en fonction du type de vibration défini.
    /// </summary>
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

            Debug.Log("Type de vibration : " + vibrationType);
        }
    }

    /// <summary>
    /// Définit le mode de vibration et démarre la routine correspondante.
    /// </summary>
    /// <param name="mode">Mode de vibration (1 = aucun, 2 = aléatoire, etc.).</param>
    public void SetVibrationMode(int mode)
    {
        vibrationMode = mode;

        // Arrête toute routine en cours
        if (_vibrationRoutine != null)
        {
            StopCoroutine(_vibrationRoutine);
            _vibrationRoutine = null;
        }

        // Démarre la routine appropriée
        switch (vibrationMode)
        {
            case 1:
                // Aucun retour haptique
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

    /// <summary>
    /// Envoie un retour haptique à un contrôleur.
    /// </summary>
    /// <param name="haptic">Contrôleur haptique.</param>
    /// <param name="amplitude">Intensité de la vibration.</param>
    /// <param name="duration">Durée de la vibration.</param>
    void SendHapticFeedback(HapticImpulsePlayer haptic, float amplitude, float duration)
    {
        if (haptic)
        {
            haptic.SendHapticImpulse(amplitude, duration);
        }
    }

    /// <summary>
    /// Routine pour des vibrations aléatoires.
    /// </summary>
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

    /// <summary>
    /// Routine pour des vibrations rythmiques fixes.
    /// </summary>
    IEnumerator FixedRhythmicVibration()
    {
        while (true)
        {
            SendHapticFeedback(leftHaptic, 0.7f, 0.5f);
            SendHapticFeedback(rightHaptic, 0.7f, 0.5f);
            yield return new WaitForSeconds(1.0f);
        }
    }

    /// <summary>
    /// Routine pour des vibrations basées sur la vitesse de déplacement.
    /// </summary>
    IEnumerator SpeedBasedVibration()
    {
        if (!xrOrigin)
        {
            Debug.LogWarning("XR Origin non assigné.");
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

    /// <summary>
    /// Réinitialise le retour haptique lors du chargement d'une nouvelle scène.
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetHapticFeedback();
    }
}