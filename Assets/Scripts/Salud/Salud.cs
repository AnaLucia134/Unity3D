using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Salud : MonoBehaviour
{
    [SerializeField] private float saludMax = 3f;
    [SerializeField] private bool destruirAlMorir = true;
    [SerializeField] private float tiempoEnDestruirse = 0f;
    [SerializeField] private float tiempoAntesDeRegresar = 1.5f;
    [SerializeField] private UnityEvent<float> alPerderSalud;
    [SerializeField] private UnityEvent alMorir;

    private float saludActual;
    private Animator animator;
    private bool estaMuerto = false;

    public event Action alActualizarSalud;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        saludActual = saludMax;
    }

    private void Start()
    {
        alActualizarSalud?.Invoke();
    }

    public bool EstaMuerto()
    {
        return estaMuerto;
    }

    public float ObtenerFraccion()
    {
        return saludActual / saludMax;
    }

    public float ObtenerSalud()
    {
        return saludActual;
    }

    public void AjustarSalud(float salud)
    {
        saludActual = Mathf.Clamp(salud, 0, saludMax);
        alActualizarSalud?.Invoke();

        if (saludActual <= 0 && !estaMuerto)
        {
            Morir();
        }
    }

    public void CurarCompletamente()
    {
        if (estaMuerto) return;

        saludActual = saludMax;
        alActualizarSalud?.Invoke();
    }

    public void PerderSalud(float saludPerdida)
    {
        if (estaMuerto) return;

        saludActual = Mathf.Max(saludActual - saludPerdida, 0);
        alPerderSalud?.Invoke(saludPerdida);
        alActualizarSalud?.Invoke();

        if (saludActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (estaMuerto) return;

        estaMuerto = true;
        alMorir?.Invoke();

        Debug.Log("Murió: " + gameObject.name);

        if (CompareTag("Player"))
        {
            Debug.Log("El jugador murió, regresando al menú...");
            StartCoroutine(RegresarAlMenu());
            return;
        }

        if (destruirAlMorir)
        {
            Destroy(gameObject, tiempoEnDestruirse);
        }
    }

    private IEnumerator RegresarAlMenu()
    {
        yield return new WaitForSeconds(tiempoAntesDeRegresar);
        SceneManager.LoadScene(0);
    }
}