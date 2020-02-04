using UnityEngine;

//Script that makes the images stare at u
public class LookAtUser : MonoBehaviour
{
    private GameObject _player;

    // Use this for initialization
    private void Start()
    {
        _player = GameObject.Find("Participant");

    }

    // Update is called once per frame
    private void Update()
    {
        var origin = transform.position - _player.transform.position;
        origin = origin.normalized;

        var a = Mathf.Rad2Deg * Mathf.Atan2(origin.x, origin.z); //<- Atan2 is the BEST!

        transform.rotation = Quaternion.Euler(0, a, 0);
    }
}
