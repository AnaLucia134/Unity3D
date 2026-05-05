using System.Collections;
using UnityEngine;

/*
 * TipoArma - Enumeración que define los tipos de arma disponibles.
 *
 * Estandar:      Disparo único, sin comportamiento especial.
 * Escopeta:      Dispara múltiples proyectiles simultáneos con dispersión aleatoria.
 * Francotirador: Disparo único de alta precisión y largo alcance.
 * Rafaga:        Dispara varios proyectiles en secuencia con pausa entre cada uno.
 */
public enum TipoArma
{
    Estandar,
    Escopeta,
    Francotirador,
    Rafaga
}

/*
 * Arma - Sistema de armas parametrizable.
 *
 * Permite configurar diferentes tipos de armas desde el Inspector de Unity
 * modificando los valores de los campos serializados. Un mismo script
 * cubre todos los comportamientos: disparo estándar, escopeta, francotirador
 * y ráfaga, sin necesidad de crear scripts adicionales.
 *
 * Flujo de ejecución:
 * 1. ControlArma invoca ProcesarEntrada() al detectar la acción de disparo.
 * 2. ProcesarEntrada() selecciona la corrutina correspondiente según tipoArma.
 * 3. Disparar() o DispararRafaga() gestionan el cooldown y llaman a ProcesarRaycast().
 * 4. ProcesarRaycast() define cuántos proyectiles se lanzan y calcula su dirección.
 * 5. ProcesarDisparoIndividual() ejecuta el raycast, aplica daño y muestra el trail.
 */
public class Arma : MonoBehaviour
{
    /* ============================================================
     * Campos serializados - Configurables desde el Inspector
     * ============================================================ */

    [Header("Tipo de Arma")]
    [SerializeField] private TipoArma tipoArma = TipoArma.Estandar;

    [Header("Atributos Generales")]
    // Cantidad de daño que aplica cada proyectil al impactar un objeto con Salud
    [SerializeField] private float ataque = 5f;

    // Tiempo en segundos entre cada disparo (cooldown)
    [SerializeField] private float tiempoEntreDisparo = 0.5f;

    // Distancia máxima que recorre el raycast (alcance del arma)
    [SerializeField] private float rango = 100f;

    // Ángulo de dispersión en grados. 0 = precisión perfecta, >0 = desviación aleatoria
    [SerializeField] private float dispersion = 0f;

    // Capas que el raycast puede detectar como objetivos válidos
    [SerializeField] private LayerMask layerMask;

    [Header("Escopeta")]
    // Cantidad de proyectiles por disparo. Solo aplica cuando tipoArma = Escopeta
    [SerializeField] private int balasPorDisparo = 1;

    [Header("Rafaga")]
    // Cantidad de proyectiles por ráfaga. Solo aplica cuando tipoArma = Rafaga
    [SerializeField] private int balasRafaga = 3;

    // Tiempo en segundos entre cada proyectil de la ráfaga
    [SerializeField] private float tiempoEntreBalasRafaga = 0.1f;

    [Header("GameObjects")]
    [SerializeField] private Transform cameraPrimeraPersona;
    [SerializeField] private Transform origenProyectil;
    [SerializeField] private TrailRenderer trailPrefab;

    /* ============================================================
     * Estado interno
     * ============================================================ */

    // Indica si el arma está lista para disparar (false durante el cooldown)
    private bool puedeDisparar = true;

    // Indica si se está ejecutando una ráfaga (bloquea nuevos disparos)
    private bool enRafaga = false;

    /* ============================================================
     * Métodos públicos
     * ============================================================ */

    /*
     * ProcesarEntrada - Punto de entrada del sistema de armas.
     *
     * Parámetros:
     *   value - true si se presionó el botón de disparo.
     *
     * Lógica:
     *   - Si no puede disparar o está en ráfaga, ignora la entrada.
     *   - Si tipoArma es Rafaga, inicia la corrutina DispararRafaga().
     *   - Para los demás tipos, inicia la corrutina Disparar().
     */
    public void ProcesarEntrada(bool value)
    {
        if (!puedeDisparar || !value || enRafaga) return;

        switch (tipoArma)
        {
            case TipoArma.Rafaga:
                StartCoroutine(DispararRafaga());
                break;
            default:
                StartCoroutine(Disparar());
                break;
        }
    }

    /* ============================================================
     * Corrutinas de disparo
     * ============================================================ */

    /*
     * Disparar - Corrutina para disparo único (Estandar, Escopeta, Francotirador).
     *
     * Flujo:
     *   1. Bloquea nuevos disparos (puedeDisparar = false).
     *   2. Ejecuta ProcesarRaycast() según el tipo de arma.
     *   3. Espera tiempoEntreDisparo segundos (cooldown).
     *   4. Desbloquea nuevos disparos.
     */
    private IEnumerator Disparar()
    {
        puedeDisparar = false;
        ProcesarRaycast();
        yield return new WaitForSecondsRealtime(tiempoEntreDisparo);
        puedeDisparar = true;
    }

    /*
     * DispararRafaga - Corrutina para disparo en ráfaga (tipo Rafaga).
     *
     * Flujo:
     *   1. Bloquea puedeDisparar y enRafaga.
     *   2. Itera balasRafaga veces, ejecutando ProcesarRaycast() en cada iteración.
     *   3. Entre cada proyectil espera tiempoEntreBalasRafaga segundos
     *      (excepto después del último).
     *   4. Al terminar la ráfaga, espera tiempoEntreDisparo segundos (cooldown).
     *   5. Desbloquea ambos flags.
     */
    private IEnumerator DispararRafaga()
    {
        enRafaga = true;
        puedeDisparar = false;

        for (int i = 0; i < balasRafaga; i++)
        {
            ProcesarRaycast();
            if (i < balasRafaga - 1)
            {
                yield return new WaitForSecondsRealtime(tiempoEntreBalasRafaga);
            }
        }

        yield return new WaitForSecondsRealtime(tiempoEntreDisparo);
        puedeDisparar = true;
        enRafaga = false;
    }

    /* ============================================================
     * Procesamiento de raycast
     * ============================================================ */

    /*
     * ProcesarRaycast - Define cuántos proyectiles se disparan.
     *
     * Si tipoArma es Escopeta, dispara balasPorDisparo proyectiles.
     * Para los demás tipos, dispara un solo proyectil.
     * Cada proyectil obtiene su propia dirección con CalcularDireccion()
     * y se procesa con ProcesarDisparoIndividual().
     */
    private void ProcesarRaycast()
    {
        int cantidadBalas = tipoArma == TipoArma.Escopeta ? balasPorDisparo : 1;

        for (int i = 0; i < cantidadBalas; i++)
        {
            Vector3 direccion = CalcularDireccion();
            ProcesarDisparoIndividual(direccion);
        }
    }

    /*
     * ProcesarDisparoIndividual - Ejecuta un raycast individual.
     *
     * Parámetros:
     *   direccion - Vector dirección normalizado del raycast.
     *
     * Si el raycast impacta un objeto dentro del rango:
     *   - Muestra el trail hasta el punto de impacto.
     *   - Si el objeto tiene componente Salud, le aplica daño.
     * Si no impacta nada:
     *   - Muestra el trail hasta el punto máximo de alcance.
     */
    private void ProcesarDisparoIndividual(Vector3 direccion)
    {
        Vector3 origen = cameraPrimeraPersona.position;

        if (Physics.Raycast(origen, direccion, out RaycastHit hit, rango, layerMask))
        {
            TrailRenderer trail = Instantiate(trailPrefab, origenProyectil.position, Quaternion.identity);
            StartCoroutine(MoverTrail(trail, origenProyectil.position, hit.point));

            if (hit.transform.TryGetComponent<Salud>(out Salud saludObjetivo))
            {
                saludObjetivo.PerderSalud(ataque);
            }
        }
        else
        {
            TrailRenderer trail = Instantiate(trailPrefab, origenProyectil.position, Quaternion.identity);
            Vector3 puntoFinal = origen + direccion * rango;
            StartCoroutine(MoverTrail(trail, origenProyectil.position, puntoFinal));
        }
    }

    /*
     * CalcularDireccion - Calcula la dirección del proyectil con dispersión.
     *
     * Retorna:
     *   Vector3 - Dirección normalizada del proyectil.
     *
     * Funcionamiento:
     *   1. Toma cameraPrimeraPersona.forward como dirección base.
     *   2. Convierte dispersion de grados a radianes.
     *   3. Genera un desvío aleatorio en cada eje (X, Y, Z) dentro del
     *      rango [-dispersionRad, +dispersionRad].
     *   4. Suma el desvío a la dirección base.
     *   5. Normaliza el vector resultante.
     *
     *   dispersion = 0 → precisión perfecta (francotirador).
     *   dispersion > 0 → cada bala se desvía aleatoriamente (escopeta).
     */
    private Vector3 CalcularDireccion()
    {
        Vector3 direccion = cameraPrimeraPersona.forward;
        float dispersionRad = dispersion * Mathf.Deg2Rad;

        float desvioX = Random.Range(-dispersionRad, dispersionRad);
        float desvioY = Random.Range(-dispersionRad, dispersionRad);
        float desvioZ = Random.Range(-dispersionRad, dispersionRad);

        Vector3 direccionDesviada = new Vector3(
            direccion.x + desvioX,
            direccion.y + desvioY,
            direccion.z + desvioZ
        );

        return direccionDesviada.normalized;
    }

    /* ============================================================
     * Efecto visual del proyectil
     * ============================================================ */

    /*
     * MoverTrail - Anima el trail del proyectil desde el origen hasta el impacto.
     *
     * Parámetros:
     *   trail       - Instancia del TrailRenderer a mover.
     *   puntoInicio - Posición inicial del trail (boca del arma).
     *   puntoFinal  - Posición final del trail (punto de impacto o alcance máximo).
     *
     * Utiliza Vector3.Lerp para interpolar la posición del trail.
     * Al finalizar, destruye el objeto después del tiempo de vida del trail.
     */
    private IEnumerator MoverTrail(TrailRenderer trail, Vector3 puntoInicio, Vector3 puntoFinal)
    {
        float t = 0f;

        while (t < 1)
        {
            trail.transform.position = Vector3.Lerp(puntoInicio, puntoFinal, t);
            t += Time.deltaTime / trail.time;
            yield return null;
        }

        trail.transform.position = puntoFinal;
        Destroy(trail.gameObject, trail.time);
    }
}