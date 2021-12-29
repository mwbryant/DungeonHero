using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpewer : MonoBehaviour
{
    public GameObject CoinPrefab;
    public int CoinsToSpew;
    public float SpewForce = 10;
    public float SpewRate = .2f;
    public bool KillParent;

    IEnumerator CoinSpew()
    {
        for (int i = 0; i < CoinsToSpew; i++)
        {
            GameObject new_coin = Instantiate(CoinPrefab, transform.position, Quaternion.identity);
            if (new_coin.GetComponent<Rigidbody2D>() != null)
            {
                Vector2 direction = SpewForce * Random.insideUnitCircle;
                Rigidbody2D body = new_coin.GetComponent<Rigidbody2D>();
                body.AddForce(direction, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(SpewRate);
        }
        if(KillParent) {
            Destroy(gameObject);
        } else {
            Destroy(GetComponent<TreasureChest>());
            Destroy(this);
        }
    }

    public void Spew()
    {
        StartCoroutine(CoinSpew());
    }
}
