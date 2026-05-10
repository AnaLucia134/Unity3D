using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public enum TipoArma
{
    Estandar,
    Shotgun,
    Sniper,
    Rafaga
}

/*
 * Arma
 * ----
 * Sistema de armas parametrizable usando raycasts.
 *
 * La idea principal del proyecto es que este mismo script permita crear
 * diferentes armas desde el Inspector, sin crear una clase separada para
 * cada una. El comportamiento cambia con el enum TipoArma y con los
 * parametros serializados.
 *
 * Ejemplos de configuracion:
 * - Shotgun: varios proyectiles por disparo, alcance corto y dispersion alta.
 * - Sniper: un proyectil, alcance largo, mucho dano, cooldown alto y dispersion 0.
 * - Rafaga: varios proyectiles separados por una pausa usando corrutina.
 *
 * Flujo:
 * 1. ControlArma recibe la entrada del PlayerInput.
 * 2. ControlArma llama a ProcesarEntrada().
 * 3. El arma valida cooldown y tipo.
 * 4. Se ejecutan uno o varios Physics.Raycast.
 * 5. Si el impacto tiene Salud, se aplica dano con PerderSalud().
 * 6. Se dibuja un TrailRenderer o una linea de debug para visualizar el disparo.
 */
public class Arma : MonoBehaviour
{
    [Header("Configuracion del arma")]
    // Define que comportamiento especial usara el arma actual.
    [SerializeField] private TipoArma tipoArma = TipoArma.Estandar;

    // Cantidad de vida que pierde el objetivo por cada raycast que impacta.
    [FormerlySerializedAs("ataque")]
    [SerializeField, Min(0f)] private float danoPorProyectil = 5f;

    // Cooldown general antes de permitir otro disparo completo.
    [FormerlySerializedAs("tiempoEntreDisparo")]
    [SerializeField, Min(0.01f)] private float tiempoEntreDisparos = 0.5f;

    // Distancia maxima del raycast. Para sniper conviene un valor alto.
    [FormerlySerializedAs("rango")]
    [SerializeField, Min(1f)] private float alcance = 100f;

    // Variacion aleatoria en grados. 0 significa precision perfecta.
    [FormerlySerializedAs("dispersion")]
    [SerializeField, Range(0f, 45f)] private float dispersionGrados = 0f;

    // Capas que el raycast puede detectar. Sirve para ignorar UI u objetos no impactables.
    [FormerlySerializedAs("layerMask")]
    [SerializeField] private LayerMask capasImpactables = ~0;

    [Header("Shotgun")]
    // Cantidad de raycasts simultaneos cuando TipoArma es Shotgun.
    [FormerlySerializedAs("balasPorDisparo")]
    [SerializeField, Min(1)] private int proyectilesPorDisparo = 6;

    [Header("Rafaga")]
    // Cantidad de raycasts consecutivos cuando TipoArma es Rafaga.
    [FormerlySerializedAs("balasRafaga")]
    [SerializeField, Min(1)] private int proyectilesPorRafaga = 3;

    // Pausa entre cada raycast de la rafaga.
    [FormerlySerializedAs("tiempoEntreBalasRafaga")]
    [SerializeField, Min(0.01f)] private float tiempoEntreProyectilesRafaga = 0.12f;

    [Header("Referencias")]
    // Punto desde donde se calcula el raycast. Normalmente es la camara del jugador.
    [FormerlySerializedAs("cameraPrimeraPersona")]
    [SerializeField] private Transform origenRaycast;

    // Punto visual donde aparece el trail. Normalmente es la punta del arma.
    [FormerlySerializedAs("origenProyectil")]
    [SerializeField] private Transform origenVisual;

    // Efecto visual del disparo. Si esta vacio, se dibuja una linea amarilla de debug.
    [SerializeField] private TrailRenderer trailPrefab;

    // Efecto opcional en el punto de impacto.
    [SerializeField] private GameObject efectoImpactoPrefab;

    private bool puedeDisparar = true;
    private bool disparandoRafaga;

    private void Awake()
    {
        if (origenRaycast == null && Camera.main != null)
        {
            origenRaycast = Camera.main.transform;
        }

        if (origenVisual == null)
        {
            origenVisual = origenRaycast != null ? origenRaycast : transform;
        }
    }

    public void ProcesarEntrada(bool presionado)
    {
        if (!presionado || !puedeDisparar || disparandoRafaga)
        {
            return;
        }

        if (tipoArma == TipoArma.Rafaga)
        {
            StartCoroutine(DispararRafaga());
            return;
        }

        StartCoroutine(DispararUnaVez());
    }

    // Disparo normal: se ejecuta una vez y despues espera el cooldown.
    private IEnumerator DispararUnaVez()
    {
        puedeDisparar = false;
        DispararGrupoDeRaycasts();

        yield return new WaitForSeconds(tiempoEntreDisparos);
        puedeDisparar = true;
    }

    // Rafaga: dispara varios raycasts uno por uno, separados por una pausa corta.
    private IEnumerator DispararRafaga()
    {
        puedeDisparar = false;
        disparandoRafaga = true;

        int cantidad = Mathf.Max(1, proyectilesPorRafaga);
        for (int i = 0; i < cantidad; i++)
        {
            DispararGrupoDeRaycasts();

            if (i < cantidad - 1)
            {
                yield return new WaitForSeconds(tiempoEntreProyectilesRafaga);
            }
        }

        yield return new WaitForSeconds(tiempoEntreDisparos);
        disparandoRafaga = false;
        puedeDisparar = true;
    }

    // Decide cuantos raycasts salen en este disparo. Shotgun usa varios; los demas usan uno.
    private void DispararGrupoDeRaycasts()
    {
        int cantidad = tipoArma == TipoArma.Shotgun ? Mathf.Max(1, proyectilesPorDisparo) : 1;

        for (int i = 0; i < cantidad; i++)
        {
            DispararRaycast(CalcularDireccion());
        }
    }

    // Ejecuta un raycast, aplica dano si impacta algo con Salud y crea el efecto visual.
    private void DispararRaycast(Vector3 direccion)
    {
        if (origenRaycast == null)
        {
            Debug.LogWarning("El arma no tiene origen de raycast asignado.");
            return;
        }

        Vector3 inicioRaycast = origenRaycast.position;
        Vector3 inicioVisual = origenVisual != null ? origenVisual.position : inicioRaycast;
        Vector3 puntoFinal = inicioRaycast + direccion * alcance;

        if (Physics.Raycast(inicioRaycast, direccion, out RaycastHit hit, alcance, capasImpactables, QueryTriggerInteraction.Ignore))
        {
            puntoFinal = hit.point;
            AplicarDano(hit);
            CrearEfectoImpacto(hit);
        }

        CrearTrail(inicioVisual, puntoFinal);
    }

    // Calcula la direccion final del proyectil. La dispersion permite simular shotgun.
    private Vector3 CalcularDireccion()
    {
        Vector3 direccionBase = origenRaycast != null ? origenRaycast.forward : transform.forward;

        if (dispersionGrados <= 0f)
        {
            return direccionBase;
        }

        float desvioX = Random.Range(-dispersionGrados, dispersionGrados);
        float desvioY = Random.Range(-dispersionGrados, dispersionGrados);
        return Quaternion.Euler(desvioX, desvioY, 0f) * direccionBase;
    }

    // Usa la clase Salud existente en el proyecto para respetar la estructura actual.
    private void AplicarDano(RaycastHit hit)
    {
        Salud salud = hit.collider.GetComponentInParent<Salud>();
        if (salud != null)
        {
            salud.PerderSalud(danoPorProyectil);
        }
    }

    // Instancia un prefab opcional en el punto donde pego el raycast.
    private void CrearEfectoImpacto(RaycastHit hit)
    {
        if (efectoImpactoPrefab == null)
        {
            return;
        }

        Quaternion rotacion = Quaternion.LookRotation(hit.normal);
        Destroy(Instantiate(efectoImpactoPrefab, hit.point, rotacion), 2f);
    }

    // Crea el trail; si no hay prefab asignado, dibuja una linea temporal para pruebas.
    private void CrearTrail(Vector3 inicio, Vector3 fin)
    {
        if (trailPrefab == null)
        {
            Debug.DrawLine(inicio, fin, Color.yellow, 0.25f);
            return;
        }

        TrailRenderer trail = Instantiate(trailPrefab, inicio, Quaternion.identity);
        StartCoroutine(MoverTrail(trail, inicio, fin));
    }

    // Mueve el trail desde la punta del arma hasta el punto final del raycast.
    private IEnumerator MoverTrail(TrailRenderer trail, Vector3 inicio, Vector3 fin)
    {
        float duracion = Mathf.Max(0.01f, trail.time);
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            trail.transform.position = Vector3.Lerp(inicio, fin, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        trail.transform.position = fin;
        Destroy(trail.gameObject, trail.time);
    }

    private void OnValidate()
    {
        proyectilesPorDisparo = Mathf.Max(1, proyectilesPorDisparo);
        proyectilesPorRafaga = Mathf.Max(1, proyectilesPorRafaga);
        tiempoEntreDisparos = Mathf.Max(0.01f, tiempoEntreDisparos);
        tiempoEntreProyectilesRafaga = Mathf.Max(0.01f, tiempoEntreProyectilesRafaga);
        alcance = Mathf.Max(1f, alcance);
        danoPorProyectil = Mathf.Max(0f, danoPorProyectil);
    }
}
