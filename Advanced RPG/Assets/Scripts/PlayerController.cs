using System.Collections;
using System.Collections.Generic;
using UnityEngine.PostProcessing;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{

    Camera cam;
    PlayerMotor motor;

    [Header("Refrences")]
    [SerializeField]
    GameObject prefab;
    [SerializeField] LayerMask movementMask;
    [SerializeField] RawImage HealthBar;
    [SerializeField] Text HealthRatio;
    [SerializeField] Text deathText;
    [SerializeField] PostProcessingProfile CC;

    [Header("Settings")]
    [SerializeField]
    float MaxHealth = 1000;

    private byte alpha = 1;
    private float CurrentHealth;
    private float slowMo = 1f;
    private bool isDead = false;
    // Use this for initialization
    void Start()
    {
        deathText.enabled = false;
        CurrentHealth = MaxHealth;
        cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            deathText.GetComponent<Animator>().enabled = true;
            var gradientSettings = CC.colorGrading.settings;
            gradientSettings.basic.saturation -= Time.deltaTime / 4;
            if (gradientSettings.basic.saturation > 0) { CC.colorGrading.settings = gradientSettings; }
            slowMo -= Time.deltaTime / 4;
            if (slowMo < 0.1f) { return; }
            Time.timeScale = slowMo;
            alpha++;
            alpha /= 2;
            return;

        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, movementMask))
            {
                Instantiate(prefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                motor.MoveToPoint(hit.point);
                motor.isFocusig = false;
            }
        }
    }

    public void ChangeHealth(int Value)
    {
        Value = (Value / 2);
        if (Value > 0)
        {
            float newValue = CurrentHealth += Value;
            if (newValue > MaxHealth)
            {
                CurrentHealth = MaxHealth;
                UpdateBar();
            }
            else
            {
                CurrentHealth += Value;
                UpdateBar();
            }
        }
        else
        {
            float newValue = CurrentHealth += Value;
            if (newValue <= 0)
            {
                OnDeath();
                CurrentHealth = 0;
                UpdateBar();
            }
            else
            {
                CurrentHealth += Value;
                UpdateBar();
            }
        }

    }

    void UpdateBar()
    {
        float percentage = CurrentHealth / MaxHealth;
        HealthBar.rectTransform.localScale = new Vector3(percentage, 1, 1);
        HealthRatio.text = (percentage * 100).ToString("0") + '%';
    }

    void OnDeath()
    {
        deathText.enabled = true;
        isDead = true;
        Time.fixedDeltaTime = 0.02f * 0.1f;
        motor.isDead = true;
    }

    void OnDisable()
    {
        var colorSettings = CC.colorGrading.settings;
        colorSettings.basic.saturation = 1;
        CC.colorGrading.settings = colorSettings;
    }
}
