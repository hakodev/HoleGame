using UnityEngine;

public class CoffeeCupCollider : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<CoffeeCup>() && this.enabled)
        {
            StartCoroutine(other.GetComponent<CoffeeCup>().FillCoffee());
            this.enabled = false;
        }
    }
}
