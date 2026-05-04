using UnityEngine;

public class EstadoPerseguir : EstadoBase
{
    private EnemigoME enemigoME;

    public EstadoPerseguir(EnemigoME maquinaEstados) : base(maquinaEstados)
    {
        enemigoME = (EnemigoME)maquinaEstados;
    }

    public override void Entrar()
    {
        base.Entrar();
        Debug.Log("Entró a Estado Perseguir");
    }

    public override void UpdateLogica()
    {
        base.UpdateLogica();

        if (enemigoME.TransformObjetivo == null)
        {
            return;
        }

        enemigoME.NavMeshAgent.SetDestination(enemigoME.TransformObjetivo.position);

        if (enemigoME.RevisarRangoAtaque())
        {
            enemigoME.CambiarEstado(typeof(EstadoAtacar));
            return;
        }

        if (enemigoME.DistanciaAlObjetivo > enemigoME.RangoDeteccion)
        {
            enemigoME.CambiarEstado(typeof(EstadoReposo));
            return;
        }
    }
}