using TMPro;
using UnityEngine;

public class UIContoller : MonoBehaviour
{
   
    public TMP_Text ammoText;


    public void UpdateAmmoText(int newAmmoAmount)
    {
        ammoText.text =  newAmmoAmount.ToString();
    }
    public void UpdateAmmoText(int newAmmoAmount, int remainingAmmo)
    {
        ammoText.text =  newAmmoAmount.ToString() + " / " + remainingAmmo.ToString();
    }

}
