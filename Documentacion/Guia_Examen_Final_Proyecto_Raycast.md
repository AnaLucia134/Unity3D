# Documentacion del Examen Final: Sistema de Armas con Raycast

## Proposito del documento

Este documento explica los cambios realizados en el proyecto final del curso de Desarrollo de Videojuegos.

La idea es que cualquier companero pueda redactar la documentacion y preparar el video con base en los cambios reales del proyecto, sin tener que revisar el codigo directamente ni inventar informacion.

## Resumen de lo implementado

Se agrego un sistema de armas para el juego 3D usando raycasts en Unity.

La funcionalidad principal se hizo con un solo script parametrizable llamado `Arma.cs`. Ese script permite configurar distintos comportamientos desde el Inspector sin crear un script diferente para cada arma.

Tambien se usa el script `ControlArma.cs`, que se encarga de recibir la entrada del jugador mediante el Input System y avisarle al arma cuando debe disparar.

Ademas, se agregaron mejoras visuales al escenario usando modelos 3D del paquete `SimpleNaturePack`, como arboles, rocas, pasto, suelo y otros objetos decorativos.

## Historial de cambios realizados

Esta seccion resume los cambios hechos en el proyecto segun los commits del repositorio.

Repositorio:

`https://github.com/AnaLucia134/Unity3D`

### Commit `b494b5c` - Ajustes iniciales de input, camara y controles

Mensaje del commit:

`Refactor input handling and improve logging in ControlArma and StarterAssetsInputs`

En este cambio se empezo a corregir la forma en que el jugador recibia entradas del Input System.

Cambios principales:

- Se modifico `StarterAssetsInputs.cs` para suscribirse manualmente a las acciones del `PlayerInput`.
- Se agrego lectura para las acciones `Move`, `Look`, `Jump` y `Sprint`.
- Se agregaron mensajes de debug para comprobar si el input estaba llegando correctamente.
- Se modifico `FirstPersonController.cs` para agregar mensajes de prueba durante el inicio y movimiento.
- Se agrego una validacion para evitar errores si `cinemachineCameraTarget` no estaba asignado.
- Se modifico `ControlArma.cs` para conectar la accion `Disparar` con el metodo `ProcesarEntrada` del arma.

Problema que buscaba resolver:

- El jugador podia tener problemas para moverse, saltar, mirar con la camara o disparar si el Input System no estaba enviando bien los eventos.
- Los mensajes de debug ayudaron a identificar si el error venia del input, de la camara o del arma.

Archivos relacionados:

- `Assets/StarterAssets/InputSystem/StarterAssetsInputs.cs`
- `Assets/StarterAssets/FirstPersonController/Scripts/FirstPersonController.cs`
- `Assets/StarterAssets/FirstPersonController/Scripts/Controlarma.cs`
- `Assets/StarterAssets/FirstPersonController/Scenes/Playground.unity`

### Commit `1305b5e` - Primera version completa del sistema de armas

Mensaje del commit:

`Enhance weapon system with configurable attributes and shooting mechanics`

En este cambio se amplio el script `Arma.cs` para convertirlo en un sistema de armas parametrizable.

Cambios principales:

- Se creo el enum `TipoArma`.
- Se agregaron comportamientos para arma estandar, escopeta, francotirador y rafaga.
- Se agregaron parametros configurables desde el Inspector.
- Se agrego dano por proyectil.
- Se agrego tiempo entre disparos.
- Se agrego alcance del raycast.
- Se agrego dispersion para simular armas como la escopeta.
- Se agrego cantidad de proyectiles por disparo.
- Se agrego cantidad de proyectiles por rafaga.
- Se agrego pausa entre proyectiles de rafaga.
- Se agrego uso de corrutinas para controlar tiempos.
- Se agrego `TrailRenderer` para representar visualmente el disparo.

Problema que resolvia:

- Antes el arma funcionaba como un disparo simple.
- Despues de este cambio, el mismo script podia configurarse para crear diferentes tipos de arma sin duplicar codigo.

Archivo relacionado:

- `Assets/StarterAssets/FirstPersonController/Scripts/Arma.cs`

### Commit `8bf0ad9` - Correccion de camara, salto e input del jugador

Mensaje del commit:

`Refactor FirstPersonController and StarterAssetsInputs for improved input handling and serialization`

Este cambio mejoro el control del jugador y corrigio problemas importantes de camara e input.

Cambios principales:

- Se agrego `using UnityEngine.Serialization;`.
- Se agregaron atributos `[FormerlySerializedAs]` en `FirstPersonController.cs`.
- Esto evita que Unity pierda valores ya configurados en el Inspector cuando se cambian nombres de variables.
- Se agrego busqueda automatica del `CinemachineCameraTarget` usando el tag `CinemachineTarget`.
- Se agrego validacion para mostrar error si el target de camara no existe.
- Se corrigio el calculo de rotacion de camara con mouse.
- Se agrego el metodo `IsCurrentDeviceMouse()` en `StarterAssetsInputs.cs`.
- Si el dispositivo actual es mouse, la camara no multiplica el movimiento por `Time.deltaTime`.

Problema que resolvia:

- La camara podia sentirse bloqueada o moverse mal al usar mouse.
- El target de Cinemachine podia quedar vacio y causar que la camara no respondiera.
- Los valores del salto, movimiento, gravedad y camara podian perderse si Unity detectaba cambios de nombre en las variables.
- El salto dependia de que el input se conservara y llegara correctamente al controlador.

Archivos relacionados:

- `Assets/StarterAssets/FirstPersonController/Scripts/FirstPersonController.cs`
- `Assets/StarterAssets/InputSystem/StarterAssetsInputs.cs`

### Commit `ec051d4` - Limpieza final del sistema de armas y documentacion

Mensaje del commit:

`Refactor Arma and ControlArma scripts for improved weapon handling and input processing; add documentation for weapon system`

Este cambio dejo el sistema de armas mas ordenado y corrigio la forma en que `ControlArma.cs` recibe el disparo.

Cambios principales en `Arma.cs`:

- Se reorganizo el sistema de armas.
- Se usaron nombres mas claros para los parametros.
- Se mantuvo un solo script parametrizable.
- Se agrego compatibilidad con nombres anteriores usando `[FormerlySerializedAs]`.
- Se agrego `OnValidate()` para evitar valores invalidos en el Inspector.
- Se separaron mejor las responsabilidades internas del disparo.
- Se agrego efecto de impacto opcional.
- Se agrego linea de debug amarilla si no hay `TrailRenderer` asignado.

Cambios principales en `ControlArma.cs`:

- Se agrego `[RequireComponent(typeof(PlayerInput))]`.
- Se agrego referencia configurable para el nombre de la accion de disparo.
- Se agrego busqueda automatica del arma en hijos si no esta asignada.
- Se agrego soporte para dos formas de evento:
  - `OnDisparar(InputAction.CallbackContext context)`
  - `OnDisparar(InputValue value)`
- Se evito duplicar disparos cuando el `PlayerInput` esta configurado con eventos de Unity.
- Se agrego validacion si no existe la accion `Disparar`.

Problema que resolvia:

- Unity podia lanzar un error tipo `MissingMethodException` si el evento `OnDisparar` no coincidia con la firma esperada.
- El arma podia no disparar si no estaba correctamente asignada.
- El input de disparo podia conectarse de forma duplicada dependiendo de la configuracion del `PlayerInput`.

Archivos relacionados:

- `Assets/StarterAssets/FirstPersonController/Scripts/Arma.cs`
- `Assets/StarterAssets/FirstPersonController/Scripts/Controlarma.cs`
- `Documentacion/TrabajoEscrito_ProyectoArmas.md`

### Cambios visuales no confirmados en commit

Despues de los commits anteriores tambien se trabajo en la parte visual del escenario.

Cambios realizados:

- Se importo el paquete `SimpleNaturePack`.
- Se agregaron prefabs decorativos al escenario.
- Se pintaron prefabs del escenario para que no se vieran grises.
- Se igualo el color de los prefabs de suelo `Ground_01`, `Ground_02` y `Ground_03`.

Estos cambios visuales aparecen actualmente como archivos nuevos sin commit en el estado del repositorio.

## Scripts modificados o utilizados

### `Arma.cs`

Ubicacion:

`Assets/StarterAssets/FirstPersonController/Scripts/Arma.cs`

Este es el script principal del sistema de armas.

Su responsabilidad es:

- Definir el tipo de arma actual.
- Guardar los parametros configurables del arma.
- Procesar el disparo.
- Ejecutar raycasts.
- Aplicar dano si el raycast golpea un objetivo con componente `Salud`.
- Crear efectos visuales como trail o linea de prueba.
- Controlar el tiempo entre disparos.
- Ejecutar rafagas usando corrutinas.

### `ControlArma.cs`

Ubicacion:

`Assets/StarterAssets/FirstPersonController/Scripts/Controlarma.cs`

Este script conecta el Input System con el arma.

Su responsabilidad es:

- Buscar el componente `PlayerInput`.
- Buscar o recibir una referencia al script `Arma`.
- Escuchar la accion de disparo configurada.
- Llamar al metodo `ProcesarEntrada` del arma cuando el jugador dispara.

## Enum `TipoArma`

En `Arma.cs` se creo un enum llamado `TipoArma`.

```csharp
public enum TipoArma
{
    Estandar,
    Shotgun,
    Sniper,
    Rafaga
}
```

Este enum permite seleccionar desde el Inspector el comportamiento del arma.

Los valores disponibles son:

- `Estandar`: dispara un solo raycast normal.
- `Shotgun`: dispara varios raycasts al mismo tiempo con dispersion.
- `Sniper`: usa el mismo disparo normal, pero se configura con largo alcance, precision alta y mayor tiempo entre disparos.
- `Rafaga`: dispara varios raycasts uno tras otro usando una corrutina.

## Parametros configurables en el Inspector

El script usa `[SerializeField]` para permitir modificar variables desde Unity sin hacerlas publicas.

Esto ayuda a mantener encapsulamiento y buenas practicas de programacion orientada a objetos.

### Configuracion del arma

| Campo | Funcion |
| --- | --- |
| `tipoArma` | Define si el arma es Estandar, Shotgun, Sniper o Rafaga. |
| `danoPorProyectil` | Cantidad de dano que aplica cada raycast que impacta. |
| `tiempoEntreDisparos` | Tiempo de espera antes de permitir otro disparo. |
| `alcance` | Distancia maxima del raycast. |
| `dispersionGrados` | Variacion aleatoria en grados para desviar el disparo. |
| `capasImpactables` | Capas de Unity que el raycast puede detectar. |

### Configuracion de Shotgun

| Campo | Funcion |
| --- | --- |
| `proyectilesPorDisparo` | Cantidad de raycasts que salen en un disparo de Shotgun. |

Para Shotgun se recomienda usar:

- Alcance corto.
- Varios proyectiles.
- Dispersion mayor que 0.
- Tiempo entre disparos moderado.

### Configuracion de Rafaga

| Campo | Funcion |
| --- | --- |
| `proyectilesPorRafaga` | Cantidad de disparos que se ejecutan en una rafaga. |
| `tiempoEntreProyectilesRafaga` | Pausa entre cada proyectil de la rafaga. |

Para Rafaga se recomienda usar:

- Al menos 3 proyectiles por rafaga.
- Pausa corta entre proyectiles.
- Alcance medio.

### Referencias

| Campo | Funcion |
| --- | --- |
| `origenRaycast` | Punto desde donde se lanza el raycast. Normalmente es la camara del jugador. |
| `origenVisual` | Punto desde donde se muestra el trail visual. Normalmente es la punta del arma. |
| `trailPrefab` | Prefab visual del proyectil o bala. |
| `efectoImpactoPrefab` | Prefab opcional para mostrar un efecto donde impacta el raycast. |

## Flujo general del disparo

El disparo funciona de esta manera:

1. El jugador presiona la accion de disparo.
2. `ControlArma.cs` recibe la entrada del Input System.
3. `ControlArma.cs` llama a `arma.ProcesarEntrada(presionado)`.
4. `Arma.cs` revisa si el arma puede disparar.
5. Si el arma esta en modo `Rafaga`, inicia una corrutina.
6. Si el arma no es rafaga, dispara una vez.
7. El script calcula cuantos raycasts debe lanzar.
8. Cada raycast revisa si golpea un objeto valido.
9. Si golpea un objeto con componente `Salud`, se aplica dano.
10. Se crea el efecto visual del disparo.
11. El arma espera el tiempo configurado antes de poder disparar otra vez.

## Funcionamiento de `ControlArma.cs`

El script `ControlArma.cs` usa `PlayerInput` para conectarse con el sistema de controles.

Tiene estos campos principales:

```csharp
[SerializeField] private Arma arma;
[SerializeField] private string nombreAccionDisparo = "Disparar";
```

`arma` es la referencia al script `Arma`.

`nombreAccionDisparo` indica el nombre de la accion configurada en el Input System.

Cuando el jugador dispara, se ejecuta alguno de estos metodos:

```csharp
public void OnDisparar(InputAction.CallbackContext context)
public void OnDisparar(InputValue value)
```

Se incluyeron ambos metodos para evitar errores cuando Unity intenta llamar el evento de disparo con diferentes tipos de parametro.

Esto tambien ayuda a evitar errores como:

`MissingMethodException: Method ControlArma.OnDisparar not found`

## Funcionamiento de `Arma.cs`

### Metodo `ProcesarEntrada`

Este metodo recibe si el boton de disparo fue presionado.

Si no se presiono, si el arma esta en cooldown o si ya esta disparando una rafaga, no hace nada.

Si el tipo de arma es `Rafaga`, inicia la corrutina `DispararRafaga`.

Si no es rafaga, inicia la corrutina `DispararUnaVez`.

### Metodo `DispararUnaVez`

Este metodo se usa para armas normales, sniper y shotgun.

Primero bloquea el disparo con `puedeDisparar = false`.

Luego llama a `DispararGrupoDeRaycasts`.

Despues espera `tiempoEntreDisparos` usando:

```csharp
yield return new WaitForSeconds(tiempoEntreDisparos);
```

Finalmente vuelve a permitir el disparo.

### Metodo `DispararRafaga`

Este metodo se usa para el arma tipo `Rafaga`.

Dispara varios proyectiles uno por uno.

Entre cada disparo espera el tiempo configurado en `tiempoEntreProyectilesRafaga`.

Esto cumple el requisito de que los proyectiles de rafaga no salgan todos al mismo tiempo.

### Metodo `DispararGrupoDeRaycasts`

Este metodo decide cuantos raycasts se van a lanzar.

Si el arma es `Shotgun`, usa el valor `proyectilesPorDisparo`.

Si el arma no es `Shotgun`, lanza solamente un raycast.

### Metodo `DispararRaycast`

Este metodo ejecuta el raycast real.

Usa:

```csharp
Physics.Raycast(inicioRaycast, direccion, out RaycastHit hit, alcance, capasImpactables, QueryTriggerInteraction.Ignore)
```

Si el raycast impacta algo:

- Guarda el punto de impacto.
- Intenta aplicar dano.
- Crea un efecto de impacto si existe un prefab asignado.

Si no impacta, el disparo llega hasta la distancia maxima definida por `alcance`.

### Metodo `CalcularDireccion`

Este metodo calcula hacia donde va el disparo.

Si `dispersionGrados` es 0, el disparo va recto hacia adelante.

Si `dispersionGrados` es mayor que 0, se agrega una desviacion aleatoria usando `Random.Range`.

Esto permite crear la dispersion del Shotgun.

### Metodo `AplicarDano`

Este metodo busca si el objeto impactado tiene un componente `Salud`.

```csharp
Salud salud = hit.collider.GetComponentInParent<Salud>();
```

Si encuentra el componente, llama:

```csharp
salud.PerderSalud(danoPorProyectil);
```

Esto respeta las clases existentes del proyecto.

### Metodo `CrearTrail`

Este metodo crea el efecto visual del disparo.

Si hay un `TrailRenderer` asignado, lo instancia y lo mueve desde el origen visual hasta el punto final.

Si no hay prefab asignado, usa:

```csharp
Debug.DrawLine(inicio, fin, Color.yellow, 0.25f);
```

Esto sirve como ayuda visual para pruebas.

### Metodo `OnValidate`

Este metodo se ejecuta en el editor cuando se cambian valores en el Inspector.

Su funcion es evitar valores invalidos.

Por ejemplo:

- Que la cantidad de proyectiles sea menor que 1.
- Que el tiempo entre disparos sea 0.
- Que el alcance sea menor que 1.
- Que el dano sea negativo.

## Configuracion sugerida para las armas

### Arma Estandar

| Parametro | Valor sugerido |
| --- | --- |
| Tipo Arma | Estandar |
| Dano Por Proyectil | 5 |
| Tiempo Entre Disparos | 0.5 |
| Alcance | 100 |
| Dispersion Grados | 0 |
| Proyectiles Por Disparo | 1 |

### Shotgun

| Parametro | Valor sugerido |
| --- | --- |
| Tipo Arma | Shotgun |
| Dano Por Proyectil | 3 a 5 |
| Tiempo Entre Disparos | 0.8 a 1 |
| Alcance | 25 a 40 |
| Dispersion Grados | 8 a 15 |
| Proyectiles Por Disparo | 6 |

### Sniper

| Parametro | Valor sugerido |
| --- | --- |
| Tipo Arma | Sniper |
| Dano Por Proyectil | 15 a 30 |
| Tiempo Entre Disparos | 1.2 a 2 |
| Alcance | 150 a 300 |
| Dispersion Grados | 0 |
| Proyectiles Por Disparo | 1 |

### Rafaga

| Parametro | Valor sugerido |
| --- | --- |
| Tipo Arma | Rafaga |
| Dano Por Proyectil | 4 a 6 |
| Tiempo Entre Disparos | 0.6 a 1 |
| Alcance | 80 a 120 |
| Dispersion Grados | 1 a 4 |
| Proyectiles Por Rafaga | 3 |
| Tiempo Entre Proyectiles Rafaga | 0.1 a 0.15 |

## Mejoras visuales realizadas

Se agregaron modelos 3D formales al escenario usando el paquete `SimpleNaturePack`.

Los elementos visuales pueden incluir:

- Arboles.
- Rocas.
- Pasto.
- Suelos.
- Arbustos.
- Flores.
- Troncos u objetos decorativos.

Tambien se cambiaron materiales de prefabs para que el escenario no se vea completamente gris.

Ejemplo de organizacion de colores:

- Arboles, arbustos, suelo y pasto: verde.
- Flores: rojo.
- Rocas: azul.
- Troncos, ramas u hongos: naranja.

Esta mejora cumple con el requisito de integrar modelos 3D formales para ambiente, terreno u objetos decorativos.

## Que explicar en el video

En el video se puede seguir este orden:

1. Presentar el objetivo del proyecto.
2. Mostrar el escenario 3D con las mejoras visuales.
3. Mostrar el objeto del jugador y el componente `ControlArma`.
4. Mostrar el objeto del arma y el componente `Arma`.
5. Explicar que todo se controla desde un solo script parametrizable.
6. Mostrar los campos `[SerializeField]` en el Inspector.
7. Configurar el arma como Shotgun y probarla.
8. Configurar el arma como Sniper y probarla.
9. Configurar el arma como Rafaga y probarla.
10. Explicar que la rafaga usa corrutina y `WaitForSeconds`.
11. Mostrar el trail o efecto visual del disparo.
12. Cerrar explicando que se usaron raycasts y programacion orientada a objetos.

## Que decir sobre programacion orientada a objetos

El proyecto aplica programacion orientada a objetos porque:

- Se separaron responsabilidades entre scripts.
- `ControlArma.cs` se encarga de la entrada.
- `Arma.cs` se encarga de la logica del arma.
- Se usan campos privados con `[SerializeField]` para encapsular datos.
- Se usa un enum para representar el tipo de arma.
- Se reutiliza la clase `Salud` existente para aplicar dano.
- El sistema es parametrizable y escalable.

## Analisis basico del sistema

### Entrada

- Accion de disparo del jugador.
- Tipo de arma seleccionado.
- Dano.
- Alcance.
- Tiempo entre disparos.
- Dispersion.
- Cantidad de proyectiles.
- Capas impactables.
- Referencias visuales como origen y trail.

### Salida

- Raycasts disparados.
- Dano aplicado a objetos con `Salud`.
- Trail visual del proyectil.
- Linea de debug si no hay trail.
- Efecto visual de impacto si esta configurado.
- Comportamiento diferente segun el tipo de arma.

### Procesos

- Leer la entrada del jugador.
- Validar si el arma puede disparar.
- Seleccionar comportamiento segun `TipoArma`.
- Calcular direccion del disparo.
- Aplicar dispersion cuando corresponda.
- Ejecutar raycasts.
- Aplicar dano.
- Crear efectos visuales.
- Controlar cooldown y pausas con corrutinas.

### Restricciones

- Se usa un solo script para crear diferentes armas.
- No se usan assets que ya hagan la logica de armas.
- Los disparos se hacen con raycasts.
- La configuracion se realiza desde el Inspector.
- El Sniper no necesita zoom.
- No es obligatorio cambiar de arma durante la partida.

## Checklist para redactar la documentacion

Esta lista es para que el equipo de documentacion sepa que temas debe mencionar. No es necesario que revisen el codigo directamente.

- [ ] Mencionar que el proyecto usa Unity y C#.
- [ ] Mencionar que se trabajo sobre el juego 3D del curso.
- [ ] Explicar que se implemento un sistema de armas con raycast.
- [ ] Explicar que las armas se configuran desde un solo script parametrizable.
- [ ] Mencionar que `ControlArma.cs` recibe la entrada del jugador.
- [ ] Mencionar que `Arma.cs` contiene la logica de disparo.
- [ ] Explicar que el disparo se conecta con el Input System.
- [ ] Explicar que el raycast sale desde la camara del jugador.
- [ ] Explicar que el efecto visual sale desde un punto de referencia del arma.
- [ ] Mencionar que se usa `TrailRenderer` o una linea visual de prueba.
- [ ] Explicar que Shotgun dispara multiples raycasts con dispersion.
- [ ] Explicar que Sniper tiene largo alcance, precision alta y mayor tiempo entre disparos.
- [ ] Explicar que Rafaga dispara varios proyectiles uno por uno.
- [ ] Mencionar que Rafaga usa corrutinas y pausas con `WaitForSeconds`.
- [ ] Explicar que los parametros se modifican desde el Inspector.
- [ ] Mencionar que se corrigieron problemas de input, salto y camara.
- [ ] Mencionar que la camara se ajusto para funcionar correctamente con mouse.
- [ ] Mencionar que se agrego busqueda automatica del target de Cinemachine.
- [ ] Mencionar que se agregaron mejoras visuales con modelos 3D.
- [ ] Mencionar que el escenario incluye elementos decorativos como arboles, rocas, pasto y suelo.

## Nota importante para la documentacion

La documentacion debe describir solamente las funciones que realmente se trabajaron en el proyecto.

Puntos que no se deben afirmar:

- No decir que existe cambio automatico de armas durante la partida.
- No decir que el Sniper tiene zoom.
- No decir que se usan proyectiles fisicos.
- No decir que los assets hacen la logica de las armas.

Lo correcto es explicar que el disparo se calcula instantaneamente con `Physics.Raycast` y que el trail sirve como representacion visual del disparo.
