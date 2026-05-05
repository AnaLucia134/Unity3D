using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class ControlArma : MonoBehaviour
{
	[SerializeField] private Arma arma;

	private void Awake()
	{
		GetComponent<PlayerInput>().actions["Disparar"].performed += ctx => arma.ProcesarEntrada(ctx.action.triggered);
	}

	public void Start()
	{
		Debug.Log("Control Arma");
	}
}
