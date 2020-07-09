using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    
    [SerializeField] PlayerController player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(player.HandContacts.ContainsKey(collision))
        {
            player.HandContacts[collision]++;
        }
        else
        {
            player.HandContacts.Add(collision, 1);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(player.HandContacts[collision] <= 1)
        {
            player.HandContacts.Remove(collision);
        }
        else
        {
            player.HandContacts[collision]--;
        }
    }
}
