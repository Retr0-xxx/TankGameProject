using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Slider slider;
    private float HP;
    private Transform CamTransform;
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
    }

    public void setHP(int HPin) 
    {
        HP =  HPin;
    }

    public void setCam(Transform CamTransfrom) 
    {
        this.CamTransform = CamTransfrom;
        Debug.Log(CamTransform);
    }

  
    void Update()
    {
        slider.value = HP;
        transform.LookAt(CamTransform);
    }
}
