using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {

    public GameObject destroyed;

    private string destroyedName;
    private AudioSource Sound_Destroy_Knife;
    // Use this for initialization
    void Start () {
        destroyedName = destroyed.name + "_Target";
        Sound_Destroy_Knife = GameObject.Find("Sound_Destroy_Knife").GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == destroyedName)
        {
            GameObject block = other.gameObject;
            GameObject fragment = Instantiate(destroyed, block.transform.position + new Vector3(0, -4.5f, 0), block.transform.rotation);
            Sound_Destroy_Knife.Play();
            Destroy(block);
            Utility.score += 1;
            Destroy(fragment, 3);
        }

    }
}
