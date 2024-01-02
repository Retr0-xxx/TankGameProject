using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
 *  script for health bar
 *  health bar is done on the client side
 *  to use a health bar, you need to set the HP and the camera transform
 *
 */
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
    }

  
    void Update()
    {
        slider.value = HP;
        transform.LookAt(CamTransform);
    }
}
