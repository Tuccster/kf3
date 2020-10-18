using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour, IWeapon
{
    [Header("Settings")]
    public float fireRate;
    public AnimationCurve damageFalloff;
    public float maxDamageDistance;
    public float maxDamage;
    public float ironSightSpeed;
    public float ironSightFOV;
    public AnimationCurve accuracyFalloff;
    public float accuracyFalloffDuration;
    public float accuracyRecoveryMultiplier;
    [Range(0, 1)]
    public float accuracyFalloffPercent;
    public float accuracyDistance;
    public float accuracyDistanceADS;
    public float accuracyDistanceFirstShot;
    public float reloadSpeed;
    public float recoilStrength;
    public int recoilFrames;

    [Header("Debug")]
    public bool infiniteAmmo;
    public bool disableRecoil;
    public bool drawShootPoint;
    public bool drawBulletPath;
    public bool drawBulletImpact;
    public bool printBulletInfo;
    public float bulletPathLife;
    public int maxBulletImpacts;

    [Header("Resources")]
    public GameObject modelParent;
    public Transform barrelEnd;
    public Transform posDefault;
    public Transform posIronSight;
    public Transform posCasingEject;
    public Camera pCamera;
    public CMF.CameraController cameraController;

    protected int currentAmmo = 0;
    protected bool fireDelayComplete = true;
    private WaitForSeconds fireRateDelay;
    private float ironSightTimeElapsed = 0.0f;
    private float ironSightCompletion;
    private float defaultCameraFOV;
    private IEnumerator reloadCoroutine;
    private IEnumerator recoilCoroutine;
    private List<Vector3> bulletImpactPositions = new List<Vector3>();
    private float sprayTime = 0.0f;
    private AudioSource audioSource;
    
    [Space(10)]
    public GameObject particlesMuzzleFlash;
    public ParticleSystem particlesBulletCasing;

    [Space(10)]
    public Animator animator;
    public AnimationClip animFire;
    public AnimationClip animInspect;

    [Space(10)]
    public float audioFirePitchVariance;
    public AudioClip audioFire;

    private void Awake()
    {
        // 1/2 REMOVE ASAP
        Cursor.lockState = CursorLockMode.Locked;

        fireRateDelay = new WaitForSeconds(fireRate);
        defaultCameraFOV = pCamera.fieldOfView;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Key inputs should eventually use a static key reference

        // ADS : Clean up implementation
        if (Input.GetKey(KeyCode.Mouse1))
            ironSightTimeElapsed = Mathf.Clamp(ironSightTimeElapsed + Time.deltaTime, 0, ironSightSpeed);
        else
            ironSightTimeElapsed = Mathf.Clamp(ironSightTimeElapsed - Time.deltaTime, 0, ironSightSpeed);
        
        ironSightCompletion = ironSightTimeElapsed / ironSightSpeed;
        Vector3 modelPosDelta = modelParent.transform.position;
        modelParent.transform.position = Vector3.Lerp(posDefault.position, posIronSight.position, ironSightCompletion);
        modelPosDelta = modelPosDelta - modelParent.transform.position;
        barrelEnd.transform.position -= modelPosDelta;
        pCamera.fieldOfView = defaultCameraFOV - (ironSightCompletion * (defaultCameraFOV - ironSightFOV));
        // TODO: adjust sensetivity when aiming down sight

        // Fire
        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (fireDelayComplete)
            {
                float calculatedAccDist = accuracyDistance + (ironSightCompletion * (accuracyDistanceADS - accuracyDistance));
                calculatedAccDist *= 1 - ((1 - accuracyFalloff.Evaluate(sprayTime / accuracyFalloffDuration)) * accuracyFalloffPercent);
                Vector3 targetPos = barrelEnd.transform.position + (barrelEnd.forward * calculatedAccDist + Random.insideUnitSphere);

                RaycastHit hit;
                if (Physics.Raycast(barrelEnd.position, targetPos - barrelEnd.position, out hit, maxDamageDistance))
                {
                    float calcDamage = -damageFalloff.Evaluate(hit.distance / maxDamageDistance) * maxDamage;
                    if (hit.transform.GetComponent<IDamageable>() != null)
                        hit.transform.GetComponent<IDamageable>().HealthDelta(calcDamage);

                    if (drawBulletPath)  Debug.DrawLine(barrelEnd.position, hit.point, Color.red, bulletPathLife);
                    if (printBulletInfo) Debug.Log($"hit.distance={hit.distance} | calcDamage={calcDamage} | calculatedAccDist={calculatedAccDist}");
                }

                if (!disableRecoil)
                {
                    recoilCoroutine = RecoilCoroutine(pCamera.transform.position + pCamera.transform.forward + Vector3.up);
                    StartCoroutine(recoilCoroutine);
                }

                //GameObject.Instantiate(particlesMuzzleFlash, barrelEnd.position, Quaternion.Euler(barrelEnd.forward), barrelEnd);

                if (bulletImpactPositions.Count > maxBulletImpacts)
                    bulletImpactPositions.RemoveAt(0);
                bulletImpactPositions.Add(hit.point);

                audioSource.pitch = Random.Range(1 - audioFirePitchVariance, 1 + audioFirePitchVariance);
                audioSource.PlayOneShot(audioFire);

                GameObject.Instantiate(particlesBulletCasing, posCasingEject.position, Quaternion.identity, posCasingEject);

                StartCoroutine(UseTimer());
            }

            sprayTime = Mathf.Clamp(sprayTime + Time.deltaTime, 0, accuracyFalloffDuration);
        }
        else
        {
            sprayTime = Mathf.Clamp(sprayTime - (Time.deltaTime * accuracyRecoveryMultiplier), 0, accuracyFalloffDuration);
        }

        // Inspect
        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.Play(animInspect.name);
        }

        // 2/2 REMOVE ASAP
        if (Input.GetKeyDown(KeyCode.Escape))
            Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator UseTimer()
    {
        fireDelayComplete = false;
        yield return fireRateDelay;
        fireDelayComplete = true;
    }

    private IEnumerator RecoilCoroutine(Vector3 targetPosition)
    {
        animator.Play(animFire.name);
        for (int i = 0; i < recoilFrames; i++)
        {
            yield return new WaitForFixedUpdate();
            cameraController.RotateTowardPosition(targetPosition, recoilStrength / recoilFrames);
        }
        recoilCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        if (drawShootPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(barrelEnd.transform.position, 0.025f);
        }
        if (drawBulletImpact)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < bulletImpactPositions.Count; i++)
                Gizmos.DrawSphere(bulletImpactPositions[i], 0.05f);
        }
    }
}
