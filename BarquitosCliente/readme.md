# IA - Barquitos
El juego consiste en dos fases principales: **Preparación** y **Ataque**. Durante la fase de **Preparación** los jugadores colocan las naves en el campo de batalla, con un tiempo máximo de 1 minuto. Después de esta fase los jugadores entran en la fase de **Ataque** en la que deben seleccionar posiciones objetivo enemigas para el ataque con un máximo de 30 segundos para tomar la decisión. La fase de **Ataque** se repite hasta que una flota es derrotada.

Para nuestra simulación hemos usado las siguientes naves:

- Portaaviones: 5 casillas
- Destructor: 4 casillas
- Submarino: 3 casillas
- Buque: 3 casillas
- Crusero: 2 casillas

## Preparación  
Para la fase de preparación nuestras IAs poseen tres factores que controlan la posición de las naves en el campo de batalla.  
* **Centralidad**: Probabilidad de que las naves intenten centrarse lo máximo posible.
* **Cercanía**: Probabilidad de que las naves intenten estar juntas.
* **Horizontalidad**: Probabilidad de que las naves estén en posición horizontal.

Estos factores son controlables desde el menu principal al seleccionar la opción de combatir contra la IA.

## Juego
Hay varias estrategias para resolver el problema de acabar con la flota enemiga y buscar las zonas más probables para el ataque. Buscando información en distintas fuentes acabamos decidiendo crear tres estrategias de ataque distintas variando en efectividad. Cada una la utilizará la IA en función de la dificultad que selecciones en el menú principal.

### Dificultad fácil
La IA en esta dificultad es la más sencilla de derrotar pues su comportamiento es bastante simple. En la dificultad fácil la IA eligirá celdas aleatorias del campo enemigo evitando repetir celdas atacadas previamente. De media esta estrategia toma 75 turnos.

### Dificultad intermedia
La IA en esta dificultad es bastante más difícil de derrotar comparada con la anterior. Una propiedad del tablero, que aprovechamos con esta estrategia, es que la nave más pequeña ocupa dos casillas y que solo pueden colocarse en horizontal o vertical.  
Gracias a esto podemos ahorrarnos la mitad de las posibilidades a la hora de buscar casillas con naves enemigas. Una forma fácil de hacerse a la idea es imaginar un tablero de damas o ajedrez.  
![tablero de damas](https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/10x10_checkered_board_transparent.svg/500px-10x10_checkered_board_transparent.svg.png)  
_Toda posible posición de naves tiene al menos una casilla blanca y negra ocupada._

La estrategia queda dividida en dos fases _buscar_ y _destruir_.  
Durante la fase de _buscar_ se atacarán aleatoriamente casillas de entre las 50 posiciones (casillas negras por ejemplo). Cuando una casilla acierta y da a una nave enemiga se pasa a la fase de _destruir_.  
Durante _destruir_ la IA atacará a las casillas verticales y horizontales con respecto a la nave enemiga que provocó el cambio de fase. Cuando se destruya la nave enemiga se volverá a la fase de _buscar_ hasta que se acaben todas las naves.

### Dificultad máxima
La IA en dificultad máxima utiliza una estrategia basada en probabilidades por lo tanto, si sabes como funciona, se puede aprovechar bastante de ésta debilidad.  
La estrategia fundamental se parece a la dificultad intermedia con dos fases de ataque pero ambas más refinadas. La fase de búsqueda simula todas las posiciones posibles de las naves enemigas con la información que conoce la IA hasta ese momento (donde hay naves que conoce, donde hay agua...). Y selecciona la casilla donde hay más posibles tableros que lleven a tener una barco en ella.  
La fase de destrucción ha sido optimizada para que la IA intente descubrir la orientación de la nave y priorice probar las casillas que siguen la orientación esperada.