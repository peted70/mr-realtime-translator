using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSCript : MonoBehaviour
{
    List<Transform> _children = new List<Transform>();

	// Use this for initialization
	void Start ()
    {
	    for (int i=0;i<gameObject.transform.childCount;i++)
        {
            _children.Add(gameObject.transform.GetChild(i));
        }
	}

    private void OnEnable()
    {
        InvokeRepeating("BlinkChildren", 0.0f, 0.4f);    
    }

    void SetChildren(bool on)
    {
        foreach (var child in _children)
        {
            child.gameObject.GetComponent<Renderer>().enabled = on;
        }
    }

    public IEnumerator BlinkChildren()
    {
        SetChildren(false);
        yield return new WaitForSeconds(1.0f);
        SetChildren(true);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
}
