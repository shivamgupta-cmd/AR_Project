using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Complete Ionic Bond (NaCl) sequence controller designed for Timeline Signals / UnityEvents.
/// The Editor builder auto-finds the objects inside Iconic_Bond.fbx.
/// </summary>
public class IonicBondModuleController : MonoBehaviour
{
    [Header("FBX Parts - auto assigned by builder")]
    public GameObject sodiumAtom;
    public GameObject chlorineAtom;
    public GameObject sodiumCation;
    public GameObject chlorideAnion;
    public GameObject crystalLattice;
    public Transform sodiumElectron;

    [Header("Transfer Target")]
    [Tooltip("Put this close to the outer shell of chlorine where the transferred electron should land.")]
    public Transform electronTarget;

    [Header("Timing")]
    public float electronHighlightTime = 0.75f;
    public float electronTransferTime = 1.25f;
    public float ionFormDelay = 0.25f;
    public float attractionTime = 1.1f;
    public float latticeBuildTime = 2.2f;

    [Header("Electron Flight")]
    public float arcHeight = 0.18f;
    public AnimationCurve transferEase = AnimationCurve.EaseInOut(0,0,1,1);
    public TrailRenderer electronTrail;

    [Header("Ions Attraction")]
    public Transform cationAttractionTarget;
    public Transform anionAttractionTarget;
    public LineRenderer attractionLine;

    [Header("Charge Icons")]
    public GameObject positiveChargeIcon;
    public GameObject negativeChargeIcon;

    [Header("FX")]
    public GameObject electronHighlightGlow;
    public ParticleSystem transferImpactFX;
    public ParticleSystem ionFormationFX;
    public ParticleSystem latticeCompleteFX;

    [Header("Playback")]
    public bool playOnStart = false;

    Transform electronOriginalParent;
    Vector3 electronOriginalLocalPosition;
    Quaternion electronOriginalLocalRotation;
    Vector3 cationStartPos;
    Quaternion cationStartRot;
    Vector3 anionStartPos;
    Quaternion anionStartRot;
    Coroutine running;
    List<GameObject> latticePieces = new List<GameObject>();

    void Awake()
    {
        CacheInitialState();
        if (playOnStart) PlayFullSequence();
        else ResetModule();
    }

    void CacheInitialState()
    {
        if (sodiumElectron)
        {
            electronOriginalParent = sodiumElectron.parent;
            electronOriginalLocalPosition = sodiumElectron.localPosition;
            electronOriginalLocalRotation = sodiumElectron.localRotation;
        }
        if (sodiumCation)
        {
            cationStartPos = sodiumCation.transform.localPosition;
            cationStartRot = sodiumCation.transform.localRotation;
        }
        if (chlorideAnion)
        {
            anionStartPos = chlorideAnion.transform.localPosition;
            anionStartRot = chlorideAnion.transform.localRotation;
        }
        CacheLatticePieces();
    }

    void CacheLatticePieces()
    {
        latticePieces.Clear();
        if (!crystalLattice) return;
        foreach (Transform t in crystalLattice.GetComponentsInChildren<Transform>(true))
        {
            if (t == crystalLattice.transform) continue;
            Renderer r = t.GetComponent<Renderer>();
            if (r) latticePieces.Add(t.gameObject);
        }
    }

    public void ResetModule()
    {
        StopRunning();

        SetActive(sodiumAtom, true);
        SetActive(chlorineAtom, true);
        SetActive(sodiumCation, false);
        SetActive(chlorideAnion, false);
        SetActive(crystalLattice, false);
        SetActive(positiveChargeIcon, false);
        SetActive(negativeChargeIcon, false);
        SetActive(electronHighlightGlow, false);

        if (attractionLine) attractionLine.enabled = false;
        StopParticle(transferImpactFX);
        StopParticle(ionFormationFX);
        StopParticle(latticeCompleteFX);

        if (sodiumElectron)
        {
            sodiumElectron.SetParent(electronOriginalParent, false);
            sodiumElectron.localPosition = electronOriginalLocalPosition;
            sodiumElectron.localRotation = electronOriginalLocalRotation;
            sodiumElectron.gameObject.SetActive(true);
        }
        if (electronTrail)
        {
            electronTrail.Clear();
            electronTrail.emitting = false;
        }
        if (sodiumCation)
        {
            sodiumCation.transform.localPosition = cationStartPos;
            sodiumCation.transform.localRotation = cationStartRot;
        }
        if (chlorideAnion)
        {
            chlorideAnion.transform.localPosition = anionStartPos;
            chlorideAnion.transform.localRotation = anionStartRot;
        }
    }

    public void ShowAtoms()
    {
        SetActive(sodiumAtom, true);
        SetActive(chlorineAtom, true);
        SetActive(sodiumCation, false);
        SetActive(chlorideAnion, false);
        SetActive(crystalLattice, false);
    }

    public void HighlightElectron()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(HighlightElectronRoutine());
    }

    IEnumerator HighlightElectronRoutine()
    {
        SetActive(electronHighlightGlow, true);
        float t = 0;
        while (t < electronHighlightTime)
        {
            t += Time.deltaTime;
            if (electronHighlightGlow && sodiumElectron)
            {
                electronHighlightGlow.transform.position = sodiumElectron.position;
                float pulse = 1f + Mathf.Sin(t * 10f) * 0.12f;
                electronHighlightGlow.transform.localScale = new Vector3(
    pulse * 0.09f,
    pulse * 0.09f,
    pulse * 0.09f
);
            }
            yield return null;
        }
        running = null;
    }

    public void TransferElectron()
    {
        if (!sodiumElectron || !electronTarget) return;
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(TransferElectronRoutine());
    }

    IEnumerator TransferElectronRoutine()
    {
        SetActive(electronHighlightGlow, false);
        Vector3 start = sodiumElectron.position;
        Quaternion startRot = sodiumElectron.rotation;
        sodiumElectron.SetParent(transform, true);
        if (electronTrail)
        {
            electronTrail.Clear();
            electronTrail.emitting = true;
        }

        float t = 0f;
        while (t < electronTransferTime)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / electronTransferTime);
            float e = transferEase.Evaluate(n);
            Vector3 pos = Vector3.Lerp(start, electronTarget.position, e);
            pos += Vector3.up * Mathf.Sin(e * Mathf.PI) * arcHeight;
            sodiumElectron.position = pos;
            sodiumElectron.Rotate(Vector3.up, 360f * Time.deltaTime, Space.World);
            yield return null;
        }
        sodiumElectron.position = electronTarget.position;
        if (electronTrail) electronTrail.emitting = false;
        if (transferImpactFX)
        {
            transferImpactFX.transform.position = electronTarget.position;
            transferImpactFX.Play();
        }
        running = null;
    }

    public void FormIons()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(FormIonsRoutine());
    }

    IEnumerator FormIonsRoutine()
    {
        yield return new WaitForSeconds(ionFormDelay);
        SetActive(sodiumAtom, false);
        SetActive(chlorineAtom, false);
        if (sodiumElectron) sodiumElectron.gameObject.SetActive(false);
        SetActive(sodiumCation, true);
        SetActive(chlorideAnion, true);
        SetActive(positiveChargeIcon, true);
        SetActive(negativeChargeIcon, true);
        if (ionFormationFX) ionFormationFX.Play();
        running = null;
    }

    public void StartAttraction()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(AttractionRoutine());
    }

    IEnumerator AttractionRoutine()
    {
        if (!sodiumCation || !chlorideAnion) yield break;
        Vector3 ca = sodiumCation.transform.position;
        Vector3 an = chlorideAnion.transform.position;
        Vector3 caTarget = cationAttractionTarget ? cationAttractionTarget.position : Vector3.Lerp(ca, an, 0.42f);
        Vector3 anTarget = anionAttractionTarget ? anionAttractionTarget.position : Vector3.Lerp(an, ca, 0.42f);

        if (attractionLine) attractionLine.enabled = true;

        float t = 0f;
        while (t < attractionTime)
        {
            t += Time.deltaTime;
            float n = transferEase.Evaluate(Mathf.Clamp01(t / attractionTime));
            sodiumCation.transform.position = Vector3.Lerp(ca, caTarget, n);
            chlorideAnion.transform.position = Vector3.Lerp(an, anTarget, n);
            UpdateAttractionLine();
            yield return null;
        }
        UpdateAttractionLine();
        running = null;
    }

    void UpdateAttractionLine()
    {
        if (!attractionLine || !sodiumCation || !chlorideAnion) return;
        attractionLine.positionCount = 2;
        attractionLine.SetPosition(0, sodiumCation.transform.position);
        attractionLine.SetPosition(1, chlorideAnion.transform.position);
    }

    public void BuildCrystalLattice()
    {
        if (running != null) StopCoroutine(running);
        running = StartCoroutine(BuildLatticeRoutine());
    }

    IEnumerator BuildLatticeRoutine()
    {
        SetActive(sodiumCation, false);
        SetActive(chlorideAnion, false);
        SetActive(positiveChargeIcon, false);
        SetActive(negativeChargeIcon, false);
        if (attractionLine) attractionLine.enabled = false;
        SetActive(crystalLattice, true);
        if (latticePieces.Count == 0) CacheLatticePieces();

        foreach (var p in latticePieces) if (p) p.SetActive(false);
        float delay = latticePieces.Count > 0 ? latticeBuildTime / latticePieces.Count : 0f;
        foreach (var p in latticePieces)
        {
            if (!p) continue;
            p.SetActive(true);
            p.transform.localScale = Vector3.zero;
            float d = Mathf.Min(0.12f, Mathf.Max(0.025f, delay));
            float t = 0;
            while (t < d)
            {
                t += Time.deltaTime;
                float n = Mathf.Clamp01(t / d);
                p.transform.localScale = Vector3.one * Mathf.SmoothStep(0,1,n);
                yield return null;
            }
            p.transform.localScale = Vector3.one;
        }
        if (latticeCompleteFX) latticeCompleteFX.Play();
        running = null;
    }

    public void ShowFinalLattice()
    {
        StopRunning();
        SetActive(sodiumAtom, false);
        SetActive(chlorineAtom, false);
        SetActive(sodiumCation, false);
        SetActive(chlorideAnion, false);
        SetActive(positiveChargeIcon, false);
        SetActive(negativeChargeIcon, false);
        if (attractionLine) attractionLine.enabled = false;
        SetActive(crystalLattice, true);
        foreach (var p in latticePieces)
        {
            if (!p) continue;
            p.SetActive(true);
            p.transform.localScale = Vector3.one;
        }
    }

    public void PlayFullSequence()
    {
        StopRunning();
        running = StartCoroutine(FullSequenceRoutine());
    }

    IEnumerator FullSequenceRoutine()
    {
        ResetModule();
        yield return new WaitForSeconds(0.35f);
        ShowAtoms();
        yield return new WaitForSeconds(1f);
        yield return HighlightElectronRoutine();
        yield return TransferElectronRoutine();
        yield return FormIonsRoutine();
        yield return new WaitForSeconds(0.5f);
        yield return AttractionRoutine();
        yield return new WaitForSeconds(0.5f);
        yield return BuildLatticeRoutine();
        running = null;
    }

    void StopRunning()
    {
        if (running != null)
        {
            StopCoroutine(running);
            running = null;
        }
    }

    static void SetActive(GameObject go, bool value) { if (go) go.SetActive(value); }
    static void StopParticle(ParticleSystem ps) { if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); }
}
