# IA - Barquitos
El juego consiste en dos fases principales: **Preparación** y **Ataque**. Durante la fase de **Preparación** los jugadores colocan las naves en el campo de batalla, con un tiempo máximo de 1 minuto. Después de esta fase los jugadores entran en la fase de **Ataque** en la que deben seleccionar posiciones objetivo enemigas para el ataque con un máximo de 30 segundos para tomar la decisión. La fase de **Ataque** se repite hasta que una flota es derrotada.

Para nuestra simulación hemos usado las siguientes naves:

- Portaaviones: 5 casillas
- Destructor: 4 casillas
- Submarino: 3 casillas
- Buque: 3 casillas
- Crusero: 2 casillas

## Preparación  
Para la fase de preparación nuestras IAs poseen tres factores que controlan la posición de sus naves aliadas en el campo de batalla.  
* **Centralidad**: Probabilidad se centren lo máximo posible.
* **Cercanía**: Probabilidad estén juntas.
* **Horizontalidad**: Probabilidad de que las naves estén en posición horizontal.

Estos factores son controlables desde el menu principal al seleccionar la opción de combatir contra la IA.

## Juego
Hay varias estrategias para resolver el problema de acabar con la flota enemiga y buscar las zonas más probables para el ataque. Buscando información en distintas fuentes acabamos decidiendo crear tres estrategias de ataque distintas variando en efectividad. Cada una la utilizará la IA en función de la dificultad que selecciones en el menú principal.

### Dificultad fácil
La IA en esta dificultad es la más sencilla de derrotar pues su comportamiento es bastante simple. En la dificultad fácil la IA eligirá celdas aleatorias del campo enemigo evitando repetir celdas atacadas previamente. De media esta estrategia toma 78 turnos en ganar.

### Dificultad intermedia
La IA en esta dificultad es bastante más difícil de derrotar comparada con la anterior. Una propiedad del tablero, que aprovechamos con esta estrategia, es que la nave más pequeña ocupa dos casillas y que solo pueden colocarse en horizontal o vertical.  
Gracias a esto podemos ahorrarnos la mitad de las posibilidades a la hora de buscar casillas con naves enemigas. Una forma fácil de hacerse a la idea es imaginar un tablero de damas o ajedrez. De media esta estrategia toma 65 turnos en ganar.  
![tablero de damas](http://wordaligned.org/images/chessboard-magick.png)  
_Toda posible posición de naves tiene al menos una casilla blanca y negra ocupada._

La estrategia queda dividida en dos fases _buscar_ y _destruir_.  
Durante la fase de _buscar_ se atacarán aleatoriamente casillas de entre las 50 posiciones (casillas negras por ejemplo). Cuando una casilla acierta y da a una nave enemiga se pasa a la fase de _destruir_.  

Durante _destruir_ la IA guarda una lista de las casillas potenciales. Que son las que se encuentran horizontales y verticales con respecto a las que acierta atacando. Ataca aleatoriamente a las casillas de la lista y la actualiza respectivamente hasta probarlas todas. 

### Dificultad máxima
La IA en dificultad máxima utiliza una estrategia basada en probabilidades por lo tanto, si sabes como funciona, se puede aprovechar bastante de esta debilidad.  
La estrategia fundamental se parece a la dificultad intermedia con dos fases de ataque pero ambas más refinadas. De media esta estrategia toma 45 turnos en ganar.  
La fase de búsqueda intenta colocar las naves enemigas con la información que conoce la IA hasta ese momento (donde hay naves que conoce, donde hay agua...). Durante esta simulación acumula las veces que se repiten las casillas en las posiciones válidas y prueba a atacar las casillas que acaban con mayores repeticiones.  
La fase de destrucción ha sido optimizada para que la IA intente descubrir la orientación de la nave y priorice probar las casillas que siguen la orientación esperada.

La forma de aprovecharse de la dificultad máxima es colocando las naves en los bordes, como siempre busca en las posiciones que más se repiten y los bordes no suelen repetirse es el último lugar en el que busca. La IA es buena contra colocaciones aleatorias de naves.
## Pruebas

### Ejemplo fácil

### Ejemplo intermedio

### Ejemplo difícil

### Ejemplo de truquito contra IA difícil

### Ejemplo colocación agrupados

### Ejemplo colocación aleatoria

### Comparación de resultados

## Recursos y bibliografía
- [Blog de datagenetics](https://datagenetics.com/blog/december32011/index.)
- [VSauce2 video](https://youtu.be/LbALFZoRrw8)