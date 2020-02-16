using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemCollisionHandler : MonoBehaviour {

	private ItemRotater _itemRotater;

    private Collider _collider;

	// Use this for initialization
	void Start ()
    {
        _itemRotater = GetComponentInChildren<ItemRotater> ();
        _collider = GetComponent<Collider>();
    }
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag(Tags.player))
		{
            if(_itemRotater != null)
                _itemRotater.StopRotation();
            _collider.enabled = false;
		}
	}
}
