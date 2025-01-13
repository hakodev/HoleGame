using UnityEngine;

public class CoffeeCupCollider : MonoBehaviour
{
    CoffeeCup coffeeCup;
    private void OnTriggerStay(Collider other)
    {
        if(coffeeCup?.gameObject != other.gameObject)
        {
            if (other.gameObject.GetComponent<CoffeeCup>())
            {
                coffeeCup = other.gameObject.GetComponent<CoffeeCup>();
            }
        }

        if (!coffeeCup.coffeeFilled && this.enabled)
        {
            StartCoroutine(other.GetComponent<CoffeeCup>().FillCoffee());
            this.enabled = false;
        }
    }
}
