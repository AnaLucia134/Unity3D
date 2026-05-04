using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoME : MaquinaEstados
{
    [SerializeField] private GameObject objetivo;
    [SerializeField] private float rangoDeteccion = 5f;
    [SerializeField] private float rangoAtaque = 1.5f;
    [SerializeField] private float velVoltearAVer = 5f;
    [SerializeField] private float tiempoAgresion = 5f;
    [SerializeField] private float danioAtaque = 1f;

    public float RangoDeteccion { get { return rangoDeteccion; } }
    public float RangoAtaque { get { return rangoAtaque; } }
    public float VelVoltearAVer { get { return velVoltearAVer; } }
    public float TiempoAgresion { get { return tiempoAgresion; } }
    public float DanioAtaque { get { return danioAtaque; } }

    public NavMeshAgent NavMeshAgent { get; private set; }
    public Transform TransformObjetivo { get; private set; }
    public float DistanciaAlObjetivo { get; private set; } = Mathf.Infinity;

    [NonSerialized] public float ContTiempoAgresion;

    private void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();

        if (objetivo != null)
        {
            TransformObjetivo = objetivo.transform;
        }

        var estados = new Dictionary<Type, EstadoBase>()
        {
            { typeof(EstadoReposo), new EstadoReposo(this) },
            { typeof(EstadoPerseguir), new EstadoPerseguir(this) },
            { typeof(EstadoAtacar), new EstadoAtacar(this) },
        };

        DefinirEstados(estados);
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (TransformObjetivo != null)
        {
            DistanciaAlObjetivo = Vector3.Distance(TransformObjetivo.position, transform.position);
        }
    }

    protected override Type ObtenerEstadoInicial()
    {
        return typeof(EstadoReposo);
    }

    public bool RevisarDistancia()
    {
        return DistanciaAlObjetivo <= RangoDeteccion;
    }

    public bool RevisarRangoAtaque()
    {
        return DistanciaAlObjetivo <= RangoAtaque;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}