using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UiRollover : MonoBehaviour
{
    public Text transitionTxt;
    public float animationTime = 1.5f; 

    public float desiredNr = 0, initialNr = 0, currentNr = 0;

    public void setNr(float value)
    {
        initialNr = currentNr;
        desiredNr = value;
    } 
    public void addToNr(float value) 
    {
        initialNr = currentNr;
        desiredNr += value;
    }
    public void UpdateNr()
    {
        if(currentNr !=desiredNr)
        {
            if(initialNr < desiredNr)
            {
                currentNr += (animationTime * Time.deltaTime) * (desiredNr - initialNr);
            }
        }
        else
        {
            currentNr -= (animationTime * Time.deltaTime) * (initialNr - desiredNr);
            if(currentNr <= desiredNr)
               currentNr = desiredNr;  
        }
             
        transitionTxt.text = currentNr.ToString("0");
        
    }
}
