using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class ControlArma : MonoBehaviour
{
	[SerializeField] private Arma arma;
 
	public void OnDisparar(InputAction.CallbackContext value)
	{
    	arma.ProcesarEntrada(value.action.triggered);
	}

	public void Start()
	{
		Debug.Log("Control Arma");
	}

}
