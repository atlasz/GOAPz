using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitArrayTest : MonoBehaviour {

    private BitArray m_bits = new BitArray(5);
	// Use this for initialization
	void Start () {
        m_bits.Set(3, true);
        Debug.Log(m_bits.Get(6));
    }

}
