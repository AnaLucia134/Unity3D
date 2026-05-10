# Proyecto: Sistema de armas parametrizable con raycasts

## Caratula

- Curso:
- Nombre:
- Carne:
- Docente:
- Fecha:
- Proyecto: Mejoras visuales y sistema de armas para juego 3D

## Indice

1. Objetivo general
2. Objetivos especificos
3. Analisis basico del script de armas
4. Diseno del script de armas
5. Configuracion de armas en Unity
6. Mejoras visuales
7. Metacognicion
8. Anexos

## Objetivo general

Aplicar conocimientos de desarrollo de videojuegos, Unity y programacion orientada a objetos para mejorar el juego 3D y crear un sistema de armas configurable desde el editor usando raycasts.

## Objetivos especificos

1. Implementar mejoras visuales mediante modelos 3D para plataformas, estructuras u objetos decorativos.
2. Desarrollar un solo script parametrizable para crear diferentes armas.
3. Probar y presentar el comportamiento de las armas mediante un video corto.

## Analisis basico del script de armas

### Componentes de Unity a utilizar

- `PlayerInput`: detecta la accion `Disparar`.
- `Transform`: define el origen y direccion del raycast.
- `Physics.Raycast`: simula el proyectil de forma instantanea.
- `LayerMask`: limita que capas pueden ser impactadas.
- `TrailRenderer`: muestra visualmente la trayectoria del disparo.
- `Coroutine`: permite generar pausas, especialmente en el arma de rafaga.
- `Salud`: clase existente que recibe dano con `PerderSalud`.

### Entrada

- Click izquierdo o boton asignado a la accion `Disparar`.
- Parametros configurados desde el Inspector:
  - Tipo de arma.
  - Dano por proyectil.
  - Tiempo entre disparos.
  - Alcance.
  - Dispersion.
  - Proyectiles por disparo.
  - Proyectiles por rafaga.
  - Tiempo entre proyectiles de rafaga.
  - Capas impactables.

### Salida

- Raycast lanzado desde la camara o punto indicado.
- Dano aplicado al objeto impactado si contiene el componente `Salud`.
- Trail visual que representa el disparo.
- Efecto opcional en el punto de impacto.

### Procesos

1. `ControlArma` escucha la accion `Disparar` del `PlayerInput`.
2. `ControlArma` llama a `ProcesarEntrada` en el script `Arma`.
3. `Arma` valida si puede disparar usando su cooldown interno.
4. Si el tipo es `Rafaga`, inicia una corrutina que dispara varios raycasts espaciados.
5. Si el tipo es `Shotgun`, dispara varios raycasts al mismo tiempo con dispersion.
6. Si el tipo es `Sniper` o `Estandar`, dispara un solo raycast.
7. Si el raycast impacta un objetivo con `Salud`, aplica dano.

### Restricciones

- Todas las armas se configuran desde el mismo script `Arma`.
- No se usa ningun asset externo para resolver la funcionalidad de disparo.
- El sistema depende de que exista la accion `Disparar` en el asset de Input System.
- Para ver dano, el objetivo debe tener el componente `Salud`.

## Diseno del script de armas

El sistema se disena con una clase principal llamada `Arma`, encargada de ejecutar el comportamiento de disparo. Para mantener el proyecto escalable, los valores variables se exponen con `[SerializeField]`, de forma que el balance del juego se pueda modificar desde Unity sin cambiar codigo.

El tipo de arma se define por medio del enum `TipoArma`. Este enum no crea scripts separados, solo cambia la manera en que el mismo sistema interpreta los parametros:

- `Shotgun`: usa varios proyectiles por disparo y dispersion alta.
- `Sniper`: usa un proyectil, largo alcance, dano alto, baja dispersion y cooldown alto.
- `Rafaga`: usa una corrutina para disparar varios raycasts con una pausa entre cada uno.
- `Estandar`: funciona como disparo basico de prueba.

## Configuracion de armas en Unity

| Arma | TipoArma | Dano | Tiempo entre disparos | Alcance | Dispersion | Proyectiles por disparo | Proyectiles por rafaga | Tiempo entre rafaga |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Shotgun | Shotgun | 2 | 0.8 | 25 | 8 | 8 | 3 | 0.12 |
| Sniper | Sniper | 20 | 1.5 | 200 | 0 | 1 | 3 | 0.12 |
| Rafaga | Rafaga | 4 | 0.7 | 80 | 1 | 1 | 3 | 0.15 |

Para cambiar de arma en esta entrega no es obligatorio implementar un selector. Se puede modificar manualmente el campo `Tipo Arma` y los valores del componente `Arma` desde el Inspector.

## Mejoras visuales

Se deben integrar modelos 3D gratuitos para al menos uno de estos grupos:

- Terreno o plataformas.
- Edificios o estructuras.
- Objetos decorativos como cajas, contenedores, barriles, rocas o paredes.

Los modelos deben colocarse ordenadamente en una carpeta dentro de `Assets`, por ejemplo `Assets/Modelos` o `Assets/Decoracion`. Luego se agregan a la escena para mejorar la presentacion del nivel.

## Metacognicion

Durante el desarrollo se aprendio a separar la entrada del jugador del comportamiento del arma. Tambien se reforzo el uso de corrutinas para ejecutar acciones con pausas, como el disparo en rafaga. Lo mas interesante fue que diferentes armas pueden salir del mismo script si se identifican bien los parametros que cambian entre ellas.

Como mejora futura, se podria agregar cambio de armas en tiempo real, municion, recarga, sonidos, animaciones y una interfaz que muestre el arma activa.

## Anexos

### Commits

- Enlace commit 1:
- Enlace commit 2:
- Enlace commit 3:

### Video de presentacion

- Enlace del video:

Guion sugerido para el video:

1. Explicar que el sistema usa un solo script parametrizable.
2. Mostrar `ControlArma` y `Arma` en el Inspector.
3. Mostrar la configuracion de Shotgun, Sniper y Rafaga.
4. Probar cada arma contra objetos con `Salud`.
5. Mostrar brevemente las mejoras visuales agregadas al escenario.
