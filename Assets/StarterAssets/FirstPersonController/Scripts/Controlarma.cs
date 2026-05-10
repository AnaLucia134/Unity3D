using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ControlArma : MonoBehaviour
{
    [SerializeField] private Arma arma;
    [SerializeField] private string nombreAccionDisparo = "Disparar";

    private PlayerInput playerInput;
    private InputAction accionDisparo;
    private bool usaEventosDeUnity;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (arma == null)
        {
            arma = GetComponentInChildren<Arma>();
        }
    }

    private void OnEnable()
    {
        if (playerInput == null || playerInput.actions == null)
        {
            return;
        }

        usaEventosDeUnity = playerInput.notificationBehavior == PlayerNotifications.InvokeUnityEvents;
        if (usaEventosDeUnity)
        {
            return;
        }

        accionDisparo = playerInput.actions.FindAction(nombreAccionDisparo);
        if (accionDisparo == null)
        {
            Debug.LogError($"No se encontro la accion de disparo '{nombreAccionDisparo}'.");
            return;
        }

        accionDisparo.performed += ProcesarDisparo;
    }

    private void OnDisable()
    {
        if (accionDisparo != null)
        {
            accionDisparo.performed -= ProcesarDisparo;
        }
    }

    public void OnDisparar(InputAction.CallbackContext context)
    {
        ProcesarDisparo(context.performed);
    }

    public void OnDisparar(InputValue value)
    {
        ProcesarDisparo(value.isPressed);
    }

    private void ProcesarDisparo(InputAction.CallbackContext context)
    {
        ProcesarDisparo(context.performed);
    }

    private void ProcesarDisparo(bool presionado)
    {
        if (arma == null)
        {
            Debug.LogWarning("ControlArma no tiene un arma asignada.");
            return;
        }

        arma.ProcesarEntrada(presionado);
    }
}
