using UnityEngine;

public class EstadoAtacar : EstadoBase
{
    private EnemigoME enemigoME;
    private Salud saludObjetivo;

    private float tiempoEntreAtaques = 1f;
    private float tiempoUltimoAtaque = 0f;

    public EstadoAtacar(EnemigoME maquinaEstados) : base(maquinaEstados)
    {
        enemigoME = (EnemigoME)maquinaEstados;
    }

    public override void Entrar()
    {
        base.Entrar();
        Debug.Log("Entró a Estado Atacar");

        enemigoME.NavMeshAgent.ResetPath();

        if (enemigoME.TransformObjetivo != null)
        {
            saludObjetivo = enemigoME.TransformObjetivo.GetComponent<Salud>();
        }

        if (saludObjetivo == null)
        {
            Debug.LogWarning("El objetivo no tiene el script Salud.");
        }
    }

    public override void UpdateLogica()
    {
        base.UpdateLogica();

        if (enemigoME.TransformObjetivo == null)
        {
            return;
        }

        VoltearAVerObjetivo();

        if (!enemigoME.RevisarRangoAtaque())
        {
            enemigoME.CambiarEstado(typeof(EstadoPerseguir));
            return;
        }

        Atacar();
    }

    private void Atacar()
    {
        if (saludObjetivo == null)
        {
            return;
        }

        if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            Debug.Log("Atacar");

            saludObjetivo.PerderSalud(enemigoME.DanioAtaque);

            tiempoUltimoAtaque = Time.time;
        }
    }

    private void VoltearAVerObjetivo()
    {
        Vector3 direccion = (enemigoME.TransformObjetivo.position - enemigoME.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direccion.x, 0, direccion.z));

        enemigoME.transform.rotation = Quaternion.Slerp(
            enemigoME.transform.rotation,
            lookRotation,
            Time.deltaTime * enemigoME.VelVoltearAVer
        );
    }
}