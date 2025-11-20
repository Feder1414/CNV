# Evaluación del impacto de la comunicación no verbal en VR en la ejecución de tareas

## Tecnolologías usadas:
Las tecnologías utilizadas para el desarrollo del proyecto son las siguientes:
- Unity (Versión 6000.2.2f1)
- XR Interaction Toolkit (para interacción en VR con manos/controladores)

## Pre-requisitos:
- Unity instalado (Versión 6000.2.2f1)

## ¿De qué se trata? 

El proyecto busca medir la eficacia y la importancia de la comunicación no verbal (CNV) en la ejecución de tareas específicas en entornos de realidad virtual. Para esto, el proyecto desarrollado se ambienta en el interior de un submarino que sufrió un accidente. 
El usuario debe tomar el rol de un tripulante, que debe seguir una serie de instrucciones brindadas por un agente virtual (el capitán herido) para reparar todos los sistemas de la nave que sufireron daños. Hay dos escenarios diseñados: En el primero, el usuario recibe instrucciones con animaciones que transmiten información no verbal;
en el segundo, únicamente se reciben las instrucciones mediante audio y texto sin animaciones. La idea es medir las diferencias entre las ejecuciones de las tareas en ambas condiciones, para concluir si las señales no verbales impactan significativamente en la comprensión, traduciéndose en menos errores y tiempos más rapidos.

Específicamente, los sistemas a reparar con sus tareas son los siguientes:

- **Sistema de presión:** El usuario debe interactuar con una manivela y un botón para restaurar la presión.
- **Sistema eléctrico:** El usuario debe subir y bajar unas palancas en cierto orden y tiempo específicos.
- **Sistema navegación:** El usuario debe ajustar unas brújulas hacia una orientación específica, guiados por pistas auditivas.
- **Reiniciar la consola principal:** Conectado cables y accionando palancas y botones en una secuencia específica.

## Instrucciones de descarga para desarrollo
1. Instalar Unity HUB.
2. Una vez abierto Unity HUB en el panel izquierdo seleccionar la pestaña de instalaciones. Posteriormente, hacer click en instalaciones, instalar editor e instalar la versión recomendada de unity (Versión 6000.2.2f1).
3. Clonar el repositorio.
4. En Unity HUB, en la pestaña de proyectos, hacer click en  `añadir > añadir proyecto de disco` y seleccionar el directorio del repositorio clonado.

## Instrucciones de uso del ejecutable
En el repositorio, descargar y extraer de los `release` el ejecutable build. Hacer doble click en el ejecutabkle `My project.exe` con un dispostivo VR conectado al computador (no standalone). Una vez abierto el proyecto, se observa una escena en unity completamente vacía. Deste el TECLADO del computador, oprmir la tecla `upArrow` (la flecha hacia arriba) para seleccionar el escenario con gestos.
Presionar la tecla `downArrow` (la flecha hacia abajo) para seleccionar el escenario sin gestos. Adicionalmente, en cualquier momento se puede oprimir la tecla `RePag` para registrar en formato `.csv` los tiempos y errores totales de la tarea en la que actualmente se encuentra el usuario. En cualquier caso, estos tiempos y errores se registran automáticamente
cada vez que se completa una tarea de forma exitosa.
