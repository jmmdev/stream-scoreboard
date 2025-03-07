<h1><a href="https://github.com/jmmdev/stream-scoreboard/blob/main/README.md"><img src="https://img.shields.io/badge/lang-en-blue"></a></h1>
<div class="markdown prose w-full break-words dark:prose-invert dark">
   <img alt="logo-500" width=300 height=300 src="https://user-images.githubusercontent.com/100143610/193881093-a45bb6d2-acd9-439b-996d-f64ee961fddb.png"></p>
   <p><strong>Stream Scoreboard</strong> es una aplicación de escritorio que funciona como un marcador general para OBS y juegos de lucha, con algunas funciones adicionales: la posibilidad de introducir atajos de teclado directamente en OBS, buscar y conectar con torneos de start.gg, y facilitar la búsqueda de información sobre torneos y partidas de las brackets.</p>
   <h3>Pantalla Principal</h3>
   <p>Primero, echemos un vistazo a la pantalla principal:</p>
   <p><img alt="1" src="https://user-images.githubusercontent.com/100143610/195660544-c4aee492-dd1e-487f-82a9-4f3db9eec4af.png"></p>
   <p>Aquí puedes optar por usar la interfaz manual, donde toda la información, como los apodos de los jugadores, las rondas o los puntajes, se introduce manualmente.</p>
   <p><img alt="2" src="https://user-images.githubusercontent.com/100143610/195660574-677f3f8f-95ad-487c-8a81-e3958a58699f.png"></p>
   <p>¿Has notado las pestañas en la parte superior? Hablaremos de ellas más adelante. Por ahora, centrémonos en la conexión con start.gg. Para hacerlo, primero necesitas un token de autenticación, que puedes generar siguiendo esta guía: <a rel="noopener" target="_new" style="--streaming-animation-state: var(--batch-play-state-1); --animation-rate: var(--batch-play-rate-1);" href="https://developer.start.gg/docs/authentication"><span style="--animation-count: 14; --streaming-animation-state: var(--batch-play-state-2);">Documentación</span><span style="--animation-count: 15; --streaming-animation-state: var(--batch-play-state-2);"> de</span><span style="--animation-count: 16; --streaming-animation-state: var(--batch-play-state-2);"> autenticación</span><span style="--animation-count: 17; --streaming-animation-state: var(--batch-play-state-2);"> de</span><span style="--animation-count: 18; --streaming-animation-state: var(--batch-play-state-2);"> start</span><span style="--animation-count: 19; --streaming-animation-state: var(--batch-play-state-2);">.gg</span></a>.</p>
   <p>Una vez que tengas el token, configúralo en la pantalla de ajustes. Aquí también puedes establecer el directorio de salida para los archivos de texto de OBS, así como el tema y el idioma de la aplicación.</p>
   <p><img alt="3" src="https://user-images.githubusercontent.com/100143610/195660616-433c63d9-8991-4eee-baec-ec87400e3a72.png"></p>
   <h3>Búsqueda y Selección de Torneos</h3>
   <p>Después de buscar un torneo por su nombre, puedes seleccionarlo de una lista. El torneo también se guardará en la lista de "torneos recientes" para acceder más rápidamente en el futuro.</p>
   <p><img alt="4" src="https://user-images.githubusercontent.com/100143610/195664056-b154434d-c6f1-41ab-9dc1-d94cf5cc6b2c.png"></p>
   <p>Si un torneo ya ha finalizado, no estará disponible para seleccionarlo, y la aplicación mostrará una alerta (como ocurre con Genesis 8, por ejemplo).</p>
   <p><img alt="4" src="https://user-images.githubusercontent.com/100143610/194766065-10c68471-adf9-4b73-988c-0d3e18cf95ce.png"></p>
   <h3>Gestión de Solicitudes a la API</h3>
   <p>Para minimizar errores, las solicitudes a la API de start.gg se realizan en pequeñas partes. A medida que seleccionas valores, se desbloquean más campos hasta que puedes acceder a los sets. Una vez que selecciones un set, la aplicación completará automáticamente la información como si se hubiera introducido manualmente.</p>
   <p><img alt="5" src="https://user-images.githubusercontent.com/100143610/195884917-2b975614-e55b-4aa8-a328-cbf842f3866c.png"></p>
   <p><img alt="6" src="https://user-images.githubusercontent.com/100143610/195884933-46c44725-61de-4cab-81d5-ce59f9a0b899.png"></p>
   <p><strong>Nota:</strong> Si un set está completo o es inválido (por ejemplo, porque falta un jugador que sigue jugando en otra partida o porque la bracket no se ha actualizado), no estará disponible para seleccionarlo. Para solucionarlo, usa el botón junto a la selección de sets. Al pulsarlo, se actualizará la lista de sets disponibles.</p>
   <h3>Pestañas Adicionales</h3>
   <h4>Pestaña Extra</h4>
   <p>La pestaña Extra permite introducir los nombres de los comentaristas y otra información o mensajes adicionales para mostrar en pantalla.</p>
   <p><img alt="7" src="https://user-images.githubusercontent.com/100143610/195661050-54ed18fe-3b4a-4c06-acd0-0534530b1b0f.png"></p>
   <h4>Pestaña OBS</h4>
   <p>La pestaña OBS te permite añadir, editar y eliminar atajos de teclado (usando secuencias de teclas) que se pueden introducir directamente en OBS. Esto elimina la necesidad de memorizarlos todos. Esta función requiere el plugin WebSocket de OBS, que ya está incluido en la versión 28.0.0+ del programa.</p>
   <p><img alt="8" src="https://user-images.githubusercontent.com/100143610/195661084-9585f9ec-8c66-421f-a9ee-a97b95e042f8.png"></p>
   <p><img alt="9" src="https://user-images.githubusercontent.com/100143610/195661095-3b77cf46-ec34-47f3-af0b-795f93660859.png"></p>
</div>
