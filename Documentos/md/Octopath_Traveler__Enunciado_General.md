|     | Pontificia   | Universidad |            | Católica |             | de Chile    |          |        |
| --- | ------------ | ----------- | ---------- | -------- | ----------- | ----------- | -------- | ------ |
|     | Escuela      | de          | Ingeniería |          |             |             |          |        |
|     | Departamento |             | de Ciencia |          | de la       | Computación |          |        |
|     | IIC2113      | Diseño      | Detallado  |          | de Software |             |          |        |
|     |              |             | Proyecto:  |          | Octopath    |             | Traveler |        |
|     |              |             | Javiera    | Ignacia  |             | Pinto       | Santa    | María  |
|     |              |             | Matías     |          | Andrés      | Poblete     |          | Farías |
1. Introducción
Octopath Traveler es una serie de videojuegos del género JRPG desarrollados por Square Enix junto a
Acquire.
Su historia comienza el 2018, cuando se lanza el primer juego de la franquicia, titulado Octopath Traveler.
Este fue publicado primero para la consola Nintendo Switch y luego para Microsoft Windows, obteniendo
críticasmayormentepositivas,enlascualessedestacaprincipalmenteelestiloartísitcodeljuego,unamezcla
entre diseños 2D y fondos en 3D. Desde entonces se han publicados dos juegos más, Octopath Traveler II
en el 2023 y Octopath Traveler 0 en el 2025, además de una versión móvil de descarga gratuita, Octopath
| Traveler: Champions | of  | the | Continent. |     |     |     |     |     |
| ------------------- | --- | --- | ---------- | --- | --- | --- | --- | --- |
Lasagadestacaporsusistemadecombateporturnosysuestructuranarrativaúnica,dondecadapersonaje
cuenta con su propia historia, permitiendo al jugador explorar el mundo desde múltiples perspectivas. Su
distintivoestilovisual,conocidocomo“HD-2D”,combinapíxelesclásicosconefectosmodernosdeiluminación
y profundidad, consolidando a Octopath Traveler como una de las franquicias más reconocibles y aclamadas
| del género JRPG | contemporáneo. |        |     |            |             |     |             |          |
| --------------- | -------------- | ------ | --- | ---------- | ----------- | --- | ----------- | -------- |
|                 |                | Figura | 1:  | Personajes | principales |     | de Octopath | Traveler |
El objetivo de este proyecto es implementar una versión simplificada y modificada del sistema de combate
por turnos de Octopath Traveler. A grandes rasgos, el juego consiste en un enfrentamiento entre tu equipo y
| bestias enemigas, | donde | se busca | atacar | sus | debilidades | para | ganar | turnos. |
| ----------------- | ----- | -------- | ------ | --- | ----------- | ---- | ----- | ------- |
1

2. Setup
En el juego, el equipo del jugador, formado por viajeros, se enfrentará en combate a un equipo enemigo,
formado por bestias. Cada uno de los equipos se formará de la siguiente manera:
Equipo del jugador:
Debe conformarse de 1 a 4 viajeros.
No pueden haber viajeros repetidos
Cada viajero puede tener un máximo de 8 habilidades activas, sin embargo, este también podría no
tener habilidades asignadas.
Cada viajero tiene un máximo de 4 habilidades pasivas, sin embargo, este también podría no tener
ninguna habilidad asignada.
Unviajeronopuedetenerhabilidadesrepetidas.PorejemplosiCyrustienelahabilidadFireball,no
puede tener un segundo Fireball en el listado de habilidades.
Equipo enemigo:
Debe estar conformado de 1 a 5 bestias
Las bestias tienen una única habilidad.
No pueden haber bestias repetidas en el equipo enemigo.
Luegodearmarlosequipos,lasunidadesseposicionaránsobreuntableroendondesedesarrollaráelcombate.
El jugador cuenta con 4 posiciones, donde se ubicarán sus viajeros, utilizando los espacios de izquierda a
derecha. Por otro lado, el enemigo cuenta con 5 posiciones en el tablero, en donde se ubicarán las bestias,
llenando las posiciones de izquierda a derecha.
4 7 5 4 9
80HP 4890HP 4569HP 3887HP 4875HP
ChubbyCait Heavenwing Monarch DemonGoat Dreadwolf
1BP 1BP 1BP 1BP
2757HP 2706HP 2892HP 341HP
470SP 504SP 332SP 67SP
Ophilia Primrose Therion Olberic
Figura2:Batallaentreunequipode4viajeroscontra5bestias.Elequipodelasbestiasenlapartesuperior
del tablero y los viajeros en la inferior, junto con la ronda actual y la siguiente, de izquierda a derecha, en
la parte superior de la figura
2

Si el jugador posee menos de 4 viajeros o el enemigo posee menos de 5 bestias, se considerará que ciertos
| espacios | del tablero | quedan vacíos. |     |     |     |     |     |     |     |     |     |
| -------- | ----------- | -------------- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
3. Unidades
Antes de presentar el flujo del juego, se ahondará en los distintos atributos que puede poseer una unidad. Si
bien, tanto viajeros como bestias poseen atributos en común, existen diferencias que serán fundamentales al
| momento | de determinar | el flujo del | juego. |     |     |     |     |     |     |     |     |
| ------- | ------------- | ------------ | ------ | --- | --- | --- | --- | --- | --- | --- | --- |
Viajeros
Los viajeros se caracterizan por tener un conjunto de habilidades activas y habilidades de apoyo, las cuales
el jugador podrá utilizar durante el combate. El viajero posee los siguientes atributos:
|             |             |          | Nombre:       | Es el                                            | nombre   | de la unidad       | y                | su identificador |             | único.     |      |
| ----------- | ----------- | -------- | ------------- | ------------------------------------------------ | -------- | ------------------ | ---------------- | ---------------- | ----------- | ---------- | ---- |
|             |             |          | Stats:        | Son una                                          | serie de | números            | que determinarán |                  | qué         | tan buena  | es   |
|             |             |          | la unidad     | en distintos                                     |          | roles.             |                  |                  |             |            |      |
|             |             |          | Armas:        | Una lista                                        | con      | las distintas      | armas            | que puede        | tener       | un viajero |      |
|             |             |          | para realizar | un                                               | ataque   | básico.            |                  |                  |             |            |      |
|             |             |          | Habilidades   | Activas:                                         |          | Son habilidades    |                  | que los          | viajeros    | pueden     | usar |
|             |             |          | para atacar   | o provocar                                       |          | efectos especiales |                  | en el juego.     |             |            |      |
|             |             |          | Habilidades   | Pasivas:Sonhabilidadesquelosviajerosposeendefor- |          |                    |                  |                  |             |            |      |
|             |             |          | ma pasiva,    | provocando                                       |          | efectos sobre      | sí               | mismo o          | sobre otras | unidades.  |      |
| (a) H’aanit | de Octopath | Traveler |               |                                                  |          |                    |                  |                  |             |            |      |
Bestias
Por otro lado, las bestías poseen una única habilidad que usarán en combate, además de tener debilidades y
Shields.
|     |     |     | Nombre:      | Es el        | nombre       | de la unidad | y                | su identificador |        | único.      |     |
| --- | --- | --- | ------------ | ------------ | ------------ | ------------ | ---------------- | ---------------- | ------ | ----------- | --- |
|     |     |     | Stats:       | Son una      | serie de     | números      | que determinarán |                  | qué    | tan buena   | es  |
|     |     |     | la unidad    | en distintos |              | roles.       |                  |                  |        |             |     |
|     |     |     | Habilidad:   | Es           | la habilidad | que          | la bestia        | puede            | usar   | para atacar | o   |
|     |     |     | provocar     | efectos      | especiales   | en el        | juego            |                  |        |             |     |
|     |     |     | Debilidades: | Determina    |              | si la bestia | será             | débil            | contra | algún tipo  | de  |
|     |     |     | ataque,      | recibiendo   | daño         | extra.       |                  |                  |        |             |     |
Shields:Esunnúmeroqueindicalacantidaddevecesqueunabestia
|     |     |     | puede recibir | un  | ataque | al que es | débil. | Al llegar | a 0, | la bestia | entra |
| --- | --- | --- | ------------- | --- | ------ | --------- | ------ | --------- | ---- | --------- | ----- |
(a)DeepOnedeOctopathTraveler en un estado de debilidad llamado Breaking Point (ver sección 4.5
|     |     |     | para más | detalles). |     |     |     |     |     |     |     |
| --- | --- | --- | -------- | ---------- | --- | --- | --- | --- | --- | --- | --- |
3.1. Stats
Los stats son parte importante del flujo del juego, determinando el resultado de ciertas acciones. Estos son
| personales | para cada | unidad y presentan | diferencias | entre | viajeros | y bestias: |     |     |     |     |     |
| ---------- | --------- | ------------------ | ----------- | ----- | -------- | ---------- | --- | --- | --- | --- | --- |
3

| Stats de | Viajeros |     |            |     |          |        |           |        |       |            |         |     |
| -------- | -------- | --- | ---------- | --- | -------- | ------ | --------- | ------ | ----- | ---------- | ------- | --- |
|          |          |     | HP máximo: |     | Vida     | máxima | que       | puede  | tener | la unidad. | También | re- |
|          |          |     | presenta   | la  | vida con | la que | la unidad | inicia | el    | combate.   |         |     |
HP actual:Vidaactualdelaunidad.Siestevalorllegaa0,launidad
muere.Estevalorseencuentraentre0yHPmáximo.Porsimplicidad
|     |     |     | a este                                     | stat  | se le suele | llamar | simplemente |        | HP.      |        |                |        |
| --- | --- | --- | ------------------------------------------ | ----- | ----------- | ------ | ----------- | ------ | -------- | ------ | -------------- | ------ |
|     |     |     | SP máximo:DeterminalacantidadmáximadeSkill |       |             |        |             |        |          |        | Points quepue- |        |
|     |     |     | de tener                                   | una   | unidad.     |        |             |        |          |        |                |        |
|     |     |     | SP actual:                                 |       | Determina   | la     | cantidad    | actual | de Skill | Points | que posee      | la     |
|     |     |     | unidad.                                    | Estos | pueden      | ser    | gastados    | para   | que la   | unidad | utilice        | una de |
sushabilidadesactivas.Estevalorseencuentraentre0ySPmáximo.
|     |     |     | Por simplicidad |      | a este    | stat | se le suele    | llamar | simplemente |          | SP.      |         |
| --- | --- | --- | --------------- | ---- | --------- | ---- | -------------- | ------ | ----------- | -------- | -------- | ------- |
|     |     |     | Phys            | Atk: | Determina | la   | potencia       | de los | ataques     | físicos. |          |         |
|     |     |     | Phys            | Def: | Determina | que  | tan resistente |        | es la       | unidad   | a golpes | físicos |
del rival.
|             |             |          | Elem     | Atk:      | Determina | la           | potencia       | de los | ataques     | elementales. |          |      |
| ----------- | ----------- | -------- | -------- | --------- | --------- | ------------ | -------------- | ------ | ----------- | ------------ | -------- | ---- |
| (a) Therion | de Octopath | Traveler |          |           |           |              |                |        |             |              |          |      |
|             |             |          | Elem     | Def:      | Determina | que          | tan resistente |        | es          | la unidad    | a golpes | ele- |
|             |             |          | mentales | del       | rival.    |              |                |        |             |              |          |      |
|             |             |          | Speed:   | Determina |           | la prioridad | durante        |        | los turnos. |              |          |      |
| Stats de    | Bestias     |          |          |           |           |              |                |        |             |              |          |      |
Por otro lado las bestias se diferencian en que no poseen valores asociados a SP:
|     |     |     | HP máximo: |     | Vida     | máxima | que       | puede  | tener | la unidad. | También | re- |
| --- | --- | --- | ---------- | --- | -------- | ------ | --------- | ------ | ----- | ---------- | ------- | --- |
|     |     |     | presenta   | la  | vida con | la que | la unidad | inicia | el    | combate.   |         |     |
HP actual:Vidaactualdelaunidad.Siestevalorllegaa0,launidad
muere.Estevalorseencuentraentre0yHPmáximo.Porsimplicidad
|     |     |     | a este | stat | se le suele | llamar | HP.            |        |         |          |          |         |
| --- | --- | --- | ------ | ---- | ----------- | ------ | -------------- | ------ | ------- | -------- | -------- | ------- |
|     |     |     | Phys   | Atk: | Determina   | la     | potencia       | de los | ataques | físicos. |          |         |
|     |     |     | Phys   | Def: | Determina   | que    | tan resistente |        | es la   | unidad   | a golpes | físicos |
del rival.
|              |           |          | Elem     | Atk: | Determina | la  | potencia       | de los | ataques | elementales. |          |      |
| ------------ | --------- | -------- | -------- | ---- | --------- | --- | -------------- | ------ | ------- | ------------ | -------- | ---- |
|              |           |          | Elem     | Def: | Determina | que | tan resistente |        | es      | la unidad    | a golpes | ele- |
| (a) Devourer | of dreams | de Octo- | mentales | del  | rival.    |     |                |        |         |              |          |      |
path Traveler
|             |     |       | Speed: | Determina |     | la prioridad | durante |     | los turnos. |     |     |     |
| ----------- | --- | ----- | ------ | --------- | --- | ------------ | ------- | --- | ----------- | --- | --- | --- |
| 4. Sistemas | de  | juego |        |           |     |              |         |     |             |     |     |     |
Una vez conformado los equipos, el jugador deberá tomar decisiones sobre sus viajeros para intentar vencer
al equipo enemigo de bestias. Por su parte, el equipo de bestias será controlado de forma automática.
Antes de ahondar en las mecánicas del combate, el jugador debe comprender elementos esenciales del juego:
los ataques, el sistema de turnos, las acciones que puede tomar cada unidad, el boosting y el breaking
point.
4.1. Turnos
OctopathTraveler esunjuegoporturnos,dondeelordenenquelasunidadesatacanpuedevariardependiendo
de qué decisiones se tomen. La idea es lograr maximizar los turnos del jugador y minimizar los turnos del
4

| enemigo, | para así | realizar | más | ataques | y disminuir |     | el daño recibido. |     |
| -------- | -------- | -------- | --- | ------- | ----------- | --- | ----------------- | --- |
El juego se desarrolla en rondas. En cada ronda, todas las unidades —viajeros y bestias— tienen la opor-
tunidad de actuar una vez. Cada una de esas acciones corresponde a un turno. Ciertas habilidades podrían
otorgarle al viajero la oportunidad de atacar de nuevo, teniendo más de un turno por ronda. Cuando todas
| las unidades | han | jugado | su turno, | la  | ronda | termina | y comienza | la siguiente. |
| ------------ | --- | ------ | --------- | --- | ----- | ------- | ---------- | ------------- |
De base, el orden de los turnos dentro de cada ronda, se decide por la velocidad de las unidades en combate,
tanto viajeros como bestias, con las cuales se arma una cola de turnos en donde las unidades con mayor
Speedjugaránsuturnoprimero.Sinembargo,elusodeciertashabilidades,laexplotacióndelasdebilidades
y el breaking point pueden alterar el orden de la cola de turnos. Más adelante se ahondará en esto.
| 4.2. | Tipos | de Ataque |     | y Debilidades |     |     |     |     |
| ---- | ----- | --------- | --- | ------------- | --- | --- | --- | --- |
En el juego existen distintos tipos de ataques, los cuales pueden resultar útiles en diversas situaciones. Los
| tipos de | ataque | se pueden | clasificar |     | en dos | categorías: |     |     |
| -------- | ------ | --------- | ---------- | --- | ------ | ----------- | --- | --- |
Ataques Físicos: Aquellos que determinan su daño en base a los stats Phys Atk y Phys Def.
Losataquefísicoscorrespondenalostipos: Sword, Spear, Axe, Dagger, Bow y Stave
Ataque Elementales: Aquellos que determinan su daño en base a los stats Elem Atk y Elem Def.
Los ataques elementales corresponden a los tipos: Fire, Ice, Lightning, Wind, Light y
Dark.
Cadabestiadelequipoenemigotienealmenosunadebilidadaalgúntipodeataque,yaseafísicooelemental.
Atacar a una bestia con su debilidad hará que esta reciba una mayor cantidad de daño y puede cambiar el
flujo del combate.
En casos muy especiales pueden existir ataques físicos o elementales que no tengan tipo, así como ataques
| que no sean | ni físicos |     | ni elementales. |     |     |     |     |     |
| ----------- | ---------- | --- | --------------- | --- | --- | --- | --- | --- |
4.3. Acciones
Las unidades podrán realizar diversas acciones cuando sea su turno de actuar, las que pueden tener diversos
| efectos en | el juego. |     |     |     |     |     |     |     |
| ---------- | --------- | --- | --- | --- | --- | --- | --- | --- |
Enestepuntohayunadiferenciaentreviajerosybestias.Losviajerospuedenescogerentredistintasacciones,
| a diferencia | de las | bestias | que | actúan | de forma |     | automática. |     |
| ------------ | ------ | ------- | --- | ------ | -------- | --- | ----------- | --- |
Viajeros
Cada vez que llega el turno de un viajero, el jugador deberá seleccionar entre las siguientes opciones:
Ataque Básico: Selecciona una de sus armas y la utiliza para atacar a un enemigo. Todas las armas
son del tipo físico, por lo tanto, la acción de ataque básico siempre significa un ataque físico.
UsarHabilidadActiva:Seleccionaunadelashabilidadesactivasqueposeeelviajero.Lashabilidades
| pueden | ser | de daño | físico | o elemental, |     | o bien, | tener efectos | sobre el combate. |
| ------ | --- | ------- | ------ | ------------ | --- | ------- | ------------- | ----------------- |
Defender: El viajero gasta su turno para aumentar su resistencia. Por el resto de la ronda, el viajero
recibirá 50% del daño que recibiría normalmente y obtendrá prioridad de turno en la siguiente ronda.
Huir: Huye del combate, acabando el juego y dando por ganadores a los enemigos.
5

Bestias
Cada vez que llega el turno de una bestia esta solo tendrá la opción de utilizar su habilidad. Las habilidades
| pueden ser | de  | daño físico | o elemental, |     | o bien, | tener | efectos sobre | el combate. |
| ---------- | --- | ----------- | ------------ | --- | ------- | ----- | ------------- | ----------- |
Notemos que dado que las bestias solo tienen la acción de utilizar su habilidad, en ningún momento se da
una elección sobre la acción que realizará una bestia. El efecto de la habilidad, así como a quien afectará se
| encuentra | descrito | en el | efecto | de la | habilidad. |     |     |     |
| --------- | -------- | ----- | ------ | ----- | ---------- | --- | --- | --- |
4.4. Boosting
Cada uno de los viajeros tendrá una cantidad de a los cuales llamaremos BP.
|     |     |     |     |     |     | Boost | Points |     |
| --- | --- | --- | --- | --- | --- | ----- | ------ | --- |
En el turno de cada viajero, el jugador puede escoger gastar BP para realizar su acción con boosting,
amplificando su efecto. El boosting tiene el siguiente efecto en cada acción del viajero:
Ataque Básico:SerealizaunataqueadicionalporcadaBPgastado.Porejemplosiunviajerorealiza
ataque básico con 2 BP, entonces acabará realizando su ataque normal y 2 ataques adicionales.
Usar Habilidad Activa: Potencia el efecto de la habilidad que se utiliza. El aumento de potencia
depende de cada habilidad y su efecto. Por ejemplo, habilidades activas que infligen daño, pasan a
| infligir     | una | mayor    | cantidad | de      | daño     | por cada | BP gastado |     |
| ------------ | --- | -------- | -------- | ------- | -------- | -------- | ---------- | --- |
| Las acciones | de  | Defender | y        | Huir no | permiten | utilizar | BP.        |     |
Cada viajero iniciará el combate con 1 BP y, al final de cada ronda, todos los viajeros vivos obtendrán 1
BP adicional, pudiendo llegar a un máximo de 5 BP. Al momento de realizar una acción con boosting, el
viajeropuedegastarhastaunmáximode3 BP,independientedesitienemás.Además,siunviajerorealizó
| una acción | con | boosting | en una | ronda, | no  | recibirá | 1 BP al final | de esta. |
| ---------- | --- | -------- | ------ | ------ | --- | -------- | ------------- | -------- |
4.5. Shields
Cada bestia tiene una cantidad de Shields asociados, este es un número que indica cuántas veces hay que
atacar a alguna de sus debilidades para que la bestia entre al estado Breaking Point.
Como se puede apreciar en la Figura 2, la bestia Chubby Cait posee 4 Shields. Con esto, es necesario
golpearlo4vecesconalgunadesusdebilidadesparaquesusShieldslleguena0yentrealestadoBreaking
Point. Si un ataque hace 0 de daño, entonces la bestia no perderá Shields por el ataque. Más adelante se
| mostrará | un ejemplo | de  | esto. |     |     |     |     |     |
| -------- | ---------- | --- | ----- | --- | --- | --- | --- | --- |
Una vez los Shields de la bestia llegan a cero, esta entrará en un Breaking Point durante la ronda actual
y la siguiente. En este estado, la bestia no es capaz de realizar ningún tipo de acción y además recibe daño
adicional de cualquier ataque que reciba. Al pasar las dos rondas, la bestia sale del estado de Breaking
Point, reinicia su cantidad de Shields y obtiene prioridad en el orden de turnos de la siguiente ronda.
| 4.6. Manejo |     | del | cola |     |     |     |     |     |
| ----------- | --- | --- | ---- | --- | --- | --- | --- | --- |
Dentro de una ronda, todas las unidades, tanto viajeros como bestias, se deben ordenar bajo ciertas reglas
| para así     | saber | cuándo es | su turno | de         | atacar. |              |            |     |
| ------------ | ----- | --------- | -------- | ---------- | ------- | ------------ | ---------- | --- |
| Para generar | la    | cola de   | turnos,  | se seguirá |         | el siguiente | algoritmo: |     |
1. Se posicionan primero en la cola las bestias que se recuperen del estado de Breaking Point. En caso
dequedosomásbestiasserecuperenenlamismaronda,estasseordenaránporsuvelocidad,esdecir,
quientengamayorSpeediráprimeroyquientengamenorSpeediráalfinal.Sisemantieneelempate,
se ordenarán por orden de tablero, la bestia que esté más a la izquierda irá primero y la que esté más
| a la | derecha | irá al | final. |     |     |     |     |     |
| ---- | ------- | ------ | ------ | --- | --- | --- | --- | --- |
6

2. Después, se posicionarán los viajeros que hayan utilizado la acción defender en la ronda anterior. Si
dos o más viajeros se encuentran en esta situación, se seguirán las mismas reglas recién explicadas.
Es decir, primero se intentará ordenar en base a su velocidad, y en caso de empate, se ordenarán por
| orden | de tablero. |     |     |     |     |
| ----- | ----------- | --- | --- | --- | --- |
3. Luego,seposicionaránlasunidadesque,porefectodealgunahabilidad,aumentaronsuprioridadenla
ronda. Si dos o más unidades se encuentran en esta situación, se priorizan a viajeros sobre las bestias.
Si el empate se mantiene, se seguirán las mismas reglas explicadas anteriormente. Es decir, primero se
intentará ordenar en base a su velocidad, y en caso de empate, se ordenarán por orden de tablero.
4. Unidadesquenoentrenenningunadelascategoríasanterioresseordenaránporsuvelocidad,esdecir,
primero irá la unidad con mayor Speed y al final la que tenga menor Speed. Si dos o más unidades
tiene el mismo valor en la stat, se le dará prioridad a los viajeros por sobre las bestias. Si dos o más
bestias,o,dosomásviajeros,tienenelmismovalordeSpeed,estasseordenaránporordendetablero.
5. Finalmente,seposicionaránlasunidadesque,porefectodealgunahabilidad,disminuyeronsuprioridad
en la ronda. Si dos o más unidades se encuentran en esta situación, se priorizan a viajeros sobre las
bestias. Si el empate se mantiene, se seguirán las mismas reglas explicadas anteriormente. Es decir,
primero se intentará ordenar en base a su velocidad, y en caso de empate, se ordenarán por orden de
tablero.
6. Cualquiercasodonde,unaunidadestépresenteendosomásdelascategoríasdescritas,mandalamás
prioritaria. Por ejemplo, si una bestia se recupera del estado de Breaking Point y además está bajo
el efecto de alguna habilidad que le baja su prioridad, entonces manda el primer punto, es decir, la
| bestia   | debería   | estar de | las primeras         | en la cola de | turnos.    |
| -------- | --------- | -------- | -------------------- | ------------- | ---------- |
| Con esto | en mente, | veamos   | un ejemplo mezclando | todos         | los casos. |
Supongamos una batalla de 4 viajeros y contra 5 bestias, tal como el de la figura 2, donde se tienen las
| siguientes  | stats de | speed:      |      |     |     |
| ----------- | -------- | ----------- | ---- | --- | --- |
| Ophilia,    | Speed:   | 291.        |      |     |     |
| Primrose,   |          | Speed: 392. |      |     |     |
| Therion,    | Speed:   | 445.        |      |     |     |
| Olberic,    | Speed:   | 253.        |      |     |     |
| Chubby      | Cait,    | Speed:      | 500. |     |     |
| Heavenwing, |          | Speed: 275. |      |     |     |
| Monarch,    |          | Speed: 152. |      |     |     |
| Demon       | Goat,    | Speed:      | 267. |     |     |
| Dreadwolf,  |          | Speed: 325. |      |     |     |
Dado esto, en un principio las unidades se deberían ordenar de la siguiente manera siguiendo únicamente su
velocidad:
| 1. Chubby | Cait. |     |     |     |     |
| --------- | ----- | --- | --- | --- | --- |
2. Therion.
3. Primrose.
4. Dreadwolf.
5. Ophilia.
7

6. Heavenwing.
| 7. Demon | Goat. |     |     |     |     |     |     |     |     |     |     |
| -------- | ----- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
8. Olberic.
9. Monarch.
| Entonces, | tendríamos | lo         | siguiente: |            |          |         |         |           |     |           |        |
| --------- | ---------- | ---------- | ---------- | ---------- | -------- | ------- | ------- | --------- | --- | --------- | ------ |
|           |            |            |            | 4          | 7        |         | 5       |           | 4   |           | 9      |
|           |            |            | 80HP       |            | 4890HP   | 4569HP  |         | 3887HP    |     |           | 4875HP |
|           |            | ChubbyCait |            | Heavenwing |          |         |         |           |     |           |        |
|           |            |            |            |            |          | Monarch |         | DemonGoat |     | Dreadwolf |        |
|           |            |            |            | 1BP        |          | 1BP     |         | 1BP       |     |           | 1BP    |
|           |            |            | 2757HP     |            |          | 2706HP  |         | 2892HP    |     |           | 341HP  |
|           |            |            | 470SP      |            |          | 504SP   |         | 332SP     |     |           | 67SP   |
|           |            |            | Ophilia    |            | Primrose |         | Therion |           |     | Olberic   |        |
Figura 7: Tablero al inicio de la ronda, ordenando a las unidades en base a su velocidad
Se puede ver al equipo enemigo en la parte superior del tablero y al equipo del jugador en la parte inferior.
En la esquina superior derecha de las bestias se encuentra su cantidad de shields, mientras que en los
viajeros se puede ver la cantidad de boost points. Por último, las colas de turnos de la ronda actual y la
siguiente se encuentran en la parte superior de la figura, la izquierda siendo la ronda actual y la derecha la
siguiente.
| Ahora, supongamos |     | los | siguientes | escenarios: |     |     |     |     |     |     |     |
| ----------------- | --- | --- | ---------- | ----------- | --- | --- | --- | --- | --- | --- | --- |
Ophilia posee y se le activa una habilidad pasiva que otorga prioridad en la cola de turnos.
| En  | la ronda | anterior, | Olberic | utilizó | Defender. |     |     |     |     |     |     |
| --- | -------- | --------- | ------- | ------- | --------- | --- | --- | --- | --- | --- | --- |
En la ronda anterior, Primrose utilizó una habilidad sobre Dreadwolf que tiene por efecto desprio-
| rizar | a la unidad | por | dos | rondas. |     |     |     |     |     |     |     |
| ----- | ----------- | --- | --- | ------- | --- | --- | --- | --- | --- | --- | --- |
En la ronda anterior, Therion utilizó una habilidad que, además de hacer daño, le otorga prioridad
| en      | la cola | de turnos. |           |           |     |          |     |        |     |     |     |
| ------- | ------- | ---------- | --------- | --------- | --- | -------- | --- | ------ | --- | --- | --- |
| Durante | la      | ronda      | anterior, | Dreadwolf |     | entró en |     | Point. |     |     |     |
Breaking
| Entonces, | la ronda | quedaría | de  | la siguiente | manera: |     |     |     |     |     |     |
| --------- | -------- | -------- | --- | ------------ | ------- | --- | --- | --- | --- | --- | --- |
8

4 7 5 4 0
80HP 4890HP 4569HP 3887HP 3271HP
ChubbyCait Heavenwing Monarch DemonGoat Dreadwolf
0BP 0BP 0BP 2BP
2757HP 2706HP 2892HP 341HP
470SP 504SP 332SP 67SP
Ophilia Primrose Therion Olberic
Figura 8: Tablero al inicio de la ronda, ordenado tomando en consideración todos los escenarios
Se pueden observar varios casos en la figura 8:
1. Olberic se encuentra primero en la cola de la ronda actual, esto porque utilizó Defender en la ronda
anterior.
2. Le siguen Therion y Ophilia, ambos están bajo el efecto de alguna habilidad que prioriza su turno
en la cola, dado esto, se ordenaron entre ellos en base a su velocidad.
3. Luego, se encuentran todo el resto que no pertenecen a alguna categoría, por lo tanto, se les ordena
por velocidad.
4. Por último, Dreadwolf no se encuentra en la ronda actual porque está en estado de Breaking Point
desdelarondaanterior.Dadoesto,selepuedeverprimeroenlacoladelarondasiguienteaúncuando
está bajo el efecto de una habilidad que desprioriza, esto porque la categoría de recuperarse del estado
de Breaking Point manda sobre la categoría de efecto de alguna habilidad de despriorización.
5. Flujo de juego
Ahora que conocemos los elementos y los sistemas del juego, ahondaremos en el flujo general del programa.
En situaciones normales, el flujo del juego será el siguiente:
Comienza una ronda y se ordena la cola de turnos actual y siguiente con las reglas que se mencionó
anteriormente.
Siguiendo el orden definido, cada unidad actuará en su turno realizando alguna de las acciones dispo-
nibles.
Si en cualquier momento de la ronda una bestia entra en Breaking Point, esta pierde su turno en la
ronda actual, en caso de no haber actuado aún, y en la ronda siguiente.
9

Si en cualquier momento de la ronda, una unidad muere, esta permanecerá muerta en el tablero por el
| resto del | juego, en la | misma | posición | y no podrá actuar. |
| --------- | ------------ | ----- | -------- | ------------------ |
Si en cualquier momento de la ronda, una unidad actúa de tal forma que altera la cola de turnos de la
siguiente ronda, se debe editar la cola de turnos de la siguiente ronda para que refleje el cambio.
Si en cualquier momento, mueren todos los viajeros, el juego finaliza y el jugador pierde.
Si en cualquier momento, mueren todas las bestias, el juego finaliza y el jugador gana.
En el momento que ya no quedan turnos por jugar, se considera la ronda finalizada.
Una vez que se finalice una ronda, cada viajero vivo que no haya usado boosting obtendrá 1 BP y, a
continuación, se inicia una nueva ronda, la cual sucederá de la misma forma y bajo las mismas reglas.
| 5.1. Orden | de Turnos |     |     |     |
| ---------- | --------- | --- | --- | --- |
Como se mencionó en el listado anterior, el orden en que las unidades podrán actuar dependerá de cómo
se comparen sus stats. A continuación ahondaremos en cómo distintas interacciones pueden hacer que este
orden cambie.
Al inicio de la partida, el orden de los turnos se define según la stat Speed de forma descendiente, donde si
hay más de una unidad con el mismo valor, se priorizará a los viajeros por sobre las bestias y a las unidades
| que se encuentren | más a la | izquierda | en el | tablero. |
| ----------------- | -------- | --------- | ----- | -------- |
Veamos esto con ejemplo con unidades sin habilidades, supongamos una batalla de 4 viajeros contra 2
| enemigos, donde | se tienen    | las siguientes | stats | de Speed: |
| --------------- | ------------ | -------------- | ----- | --------- |
| Ophilia,        | Speed: 291.  |                |       |           |
| Tressa,         | Speed: 240.  |                |       |           |
| Therion,        | Speed: 445.  |                |       |           |
| H’aanit,        | Speed: 350.  |                |       |           |
| Mutant          | Mushroom,    | Speed:         | 131.  |           |
| Demon           | Deer, Speed: | 100.           |       |           |
Utilizando las reglas anteriormente mencionadas, el orden en el que atacan las unidades es: Therion,
| H’aanit, Ophilia, | Tressa, | Mutant | Mushroom | y Demon Deer. |
| ----------------- | ------- | ------ | -------- | ------------- |
10

|     |                |         | 3         | 4      |        |         |        |         |        |
| --- | -------------- | ------- | --------- | ------ | ------ | ------- | ------ | ------- | ------ |
|     |                | 2912HP  |           | 1440HP |        |         |        |         |        |
|     | MutantMushroom |         | DemonDeer |        |        |         |        |         |        |
|     |                |         | 1BP       |        | 1BP    |         | 1BP    |         | 1BP    |
|     |                |         | 1548HP    |        | 2000HP |         | 1376HP |         | 1548HP |
|     |                |         | 257SP     |        | 135SP  |         | 173SP  |         | 103SP  |
|     |                | Ophilia |           | Tressa |        | Therion |        | H’aanit |        |
Figura 9: Tablero al inicio del turno, con el orden de la cola de la ronda actual y la siguiente
Luegodequeunaunidadjueguesuturno,elordenavanzará.Esdecir,launidadqueacabadeactuarsaldráde
lacola,permitiendoqueselleveacaboelturnodelrestodepersonajes.Unavezlacoladeturnosestevacía,
sedaráporfinalizadalarondaycomenzaráunanueva.Tomandoelejemploanterior,siTherionrealizauna
acción, entonces el orden cambiará a H’aanit, Ophilia, Tressa, Mutant Mushroom y Demon Deer.
| Podemos | ver este cambio | ilustrado | en la | siguiente | figura: |     |     |     |     |
| ------- | --------------- | --------- | ----- | --------- | ------- | --- | --- | --- | --- |
11

|                | 3           |        | 4      |             |           |            |         |        |
| -------------- | ----------- | ------ | ------ | ----------- | --------- | ---------- | ------- | ------ |
|                | 2912HP      | 1232HP |        |             |           |            |         |        |
| MutantMushroom | DemonDeer   |        |        |             |           |            |         |        |
|                | 1BP         |        |        | 1BP         |           | 1BP        |         | 1BP    |
|                | 1548HP      |        | 2000HP |             |           | 1376HP     |         | 1548HP |
|                | 257SP       |        | 135SP  |             |           | 173SP      |         | 103SP  |
|                | Ophilia     |        | Tressa |             | Therion   |            | H’aanit |        |
| Figura         | 10: Tablero | luego  | de     | que Therion | realizara | una acción |         |        |
SupongamosahoraqueH’aanitensuturnoatacaaMutant MushroomybajasucontadordeShieldsa
0, provocándole un Breaking Point. Una bestia en estado de Breaking Point pierde su turno de la ronda
actual, si no ha actuado aún, y de la ronda siguiente. Podemos ver esto en la siguiente figura:
3
|                | 0         |        | 4      |     |         |        |         |        |
| -------------- | --------- | ------ | ------ | --- | ------- | ------ | ------- | ------ |
| 2561HP         |           | 1232HP |        |     |         |        |         |        |
| MutantMushroom | DemonDeer |        |        |     |         |        |         |        |
|                | 1BP       |        |        | 1BP |         | 1BP    |         | 1BP    |
|                | 1548HP    |        | 2000HP |     |         | 1376HP |         | 1548HP |
|                | 257SP     |        | 135SP  |     |         | 173SP  |         | 103SP  |
|                | Ophilia   |        | Tressa |     | Therion |        | H’aanit |        |
Figura 11: Tablero luego de que H’aanit llevase a Mutant Mushroom a estado de Breaking Point
12

Pasadasdosrondas,laactualylasiguiente,Mutant MushroomsaledelestadodeBreaking Point,tiene
prioridad en el orden de turnos y recupera sus Shields, el resto de las unidades se ordenan bajo las reglas
| anteriormente | mencionadas. |                | Podemos | verlo     | en     | la siguiente | figura: |         |       |         |        |
| ------------- | ------------ | -------------- | ------- | --------- | ------ | ------------ | ------- | ------- | ----- | ------- | ------ |
|               |              |                | 3       |           |        | 4            |         |         |       |         |        |
|               |              |                | 2561HP  | 1232HP    |        |              |         |         |       |         |        |
|               |              | MutantMushroom |         | DemonDeer |        |              |         |         |       |         |        |
|               |              |                | 3BP     |           |        | 3BP          |         |         | 3BP   |         | 3BP    |
|               |              |                | 1412HP  |           |        | 1860HP       |         | 1157HP  |       |         | 1498HP |
|               |              |                | 257SP   |           |        | 135SP        |         |         | 173SP |         | 103SP  |
|               |              |                | Ophilia |           | Tressa |              |         | Therion |       | H’aanit |        |
Figura 12: Tablero luego de que Mutant Mushroom saliese del estado de Breaking Point
Como se puede observar en la figura 12, los viajeros poseen 3 BP, esto porque pasaron 2 rondas desde el
inicio del juego. Al final de cada ronda, se le suma 1 BP a todos los viajeros, a menos de que los utilicen.
Supongamos que Therion en su turno utiliza 2 BP contra Mutant Mushroom, llevándolo de nuevo al
| estado de | Breaking | Point, | el tablero | quedaría |     | de la siguiente |     | forma: |     |     |     |
| --------- | -------- | ------ | ---------- | -------- | --- | --------------- | --- | ------ | --- | --- | --- |
13

|                | 0         | 4      |       |         |       |         |       |
| -------------- | --------- | ------ | ----- | ------- | ----- | ------- | ----- |
| 2391HP         | 1232HP    |        |       |         |       |         |       |
| MutantMushroom | DemonDeer |        |       |         |       |         |       |
|                | 3BP       |        | 3BP   |         | 1BP   |         | 3BP   |
| 1412HP         |           | 1860HP |       | 1157HP  |       | 1238HP  |       |
|                | 257SP     |        | 135SP |         | 173SP |         | 103SP |
| Ophilia        |           | Tressa |       | Therion |       | H’aanit |       |
Figura 13: Tablero luego de que Therion llevase a Mutant Mushroom a estado de Breaking Point
utilizando 2 BP
Unavezterminadaestaronda,selesumaráatodos1 BPmenosaTherion,ilustradoenlasiguientefigura:
|                | 0         | 4      |       |         |       |         |       |
| -------------- | --------- | ------ | ----- | ------- | ----- | ------- | ----- |
| 1994HP         | 982HP     |        |       |         |       |         |       |
| MutantMushroom | DemonDeer |        |       |         |       |         |       |
|                | 4BP       |        | 4BP   |         | 1BP   |         | 4BP   |
| 1285HP         |           | 1860HP |       | 1157HP  |       | 1238HP  |       |
|                | 257SP     |        | 135SP |         | 173SP |         | 103SP |
| Ophilia        |           | Tressa |       | Therion |       | H’aanit |       |
Figura 14: Tablero al inicio de la siguiente ronda, donde todos sumaron 1 BP menos Therion
14

Si en esta ronda Tressa usa la acción Defender, además de otorgarle una reducción del 50% del daño hasta
elfinaldelaronda,tendráprioridaddeturnoenlasiguienteronda.Almismotiempo,Mutant Mushroom
saldrá del estado de Breaking Point. Como se mencionó en las reglas anteriormente, tiene prioridad una
bestia saliendo del estado de Breaking Point y luego un viajero que defiende. Se ilustra en la siguiente
figura:
|     |                |         | 3     |           | 4      |         |       |         |       |
| --- | -------------- | ------- | ----- | --------- | ------ | ------- | ----- | ------- | ----- |
|     | 1582HP         |         |       | 857HP     |        |         |       |         |       |
|     | MutantMushroom |         |       | DemonDeer |        |         |       |         |       |
|     |                |         | 5BP   |           | 5BP    |         | 2BP   |         | 5BP   |
|     |                | 1285HP  |       |           | 1830HP | 1157HP  |       | 1238HP  |       |
|     |                |         | 257SP |           | 135SP  |         | 173SP |         | 103SP |
|     |                | Ophilia |       |           | Tressa | Therion |       | H’aanit |       |
Figura 15: Tablero al inicio de la siguiente ronda, con Mutant Mushroom de primera priodad, luego
| Tressa y | después el | resto, siguiendo |     | las reglas |     |     |     |     |     |
| -------- | ---------- | ---------------- | --- | ---------- | --- | --- | --- | --- | --- |
6. Combate
Ahora se ahondará en detalle los aspectos más específicos del combate, desde el cálculo del daño hasta el
| manejo de    | decimales. |      |     |     |     |     |     |     |     |
| ------------ | ---------- | ---- | --- | --- | --- | --- | --- | --- | --- |
| 6.1. Cálculo | del        | daño |     |     |     |     |     |     |     |
El daño que realizará una unidad será calculado en base a sus stats y qué tipo de ataque realice. Este daño
| es un número | que se        | le resta   | al HP  | de la unidad                  | que es atacada. |     |            |     |     |
| ------------ | ------------- | ---------- | ------ | ----------------------------- | --------------- | --- | ---------- | --- | --- |
| El cálculo   | del daño base | sería      | con la | siguiente                     | fórmula:        |     |            |     |     |
|              |               | Daño=[stat |        | ofensiva]×[Modificador]−[stat |                 |     | defensiva] |     |     |
La variable [Modificador] tomará el valor que corresponda según la potencia del ataque. Ataques básicos
tienenunmodificadorde1.3,mientrasquecadahabilidadtendrásupropiomodificador.Veamosunejemplo
de esto:
Se tiene un combate donde la viajera Tressa atacará a la bestia enemiga Meep.
15

|     |     |     | Nombre: | Tressa |     |     |     |
| --- | --- | --- | ------- | ------ | --- | --- | --- |
Nombre: Meep
|     |     |     | Max | HP: 275 |     |     |     |
| --- | --- | --- | --- | ------- | --- | --- | --- |
Max HP: 212
|     |     |     | Max | SP: 50 |     |     |     |
| --- | --- | --- | --- | ------ | --- | --- | --- |
Phys Atk: 106
|     |     |     | Phys | Atk: 88 |     |     |     |
| --- | --- | --- | ---- | ------- | --- | --- | --- |
Phys Def: 20
|     |     |     | Phys | Def: 80 |     |     |     |
| --- | --- | --- | ---- | ------- | --- | --- | --- |
Elem Atk: 106
|     |     |     | Elem | Atk: 88 |     |     |     |
| --- | --- | --- | ---- | ------- | --- | --- | --- |
Elem Def: 17
|     |     |     | Elem | Def: 80 |     |     |     |
| --- | --- | --- | ---- | ------- | --- | --- | --- |
Speed: 75
Speed: 72
Si Tressa ataca a Meep con un ataque básico de tipo Spear, el cálculo del daño sería el siguiente:
|     |     | Daño | =   | [stat ofensiva]×[Modificador]−[stat |     | defensiva] |     |
| --- | --- | ---- | --- | ----------------------------------- | --- | ---------- | --- |
|     |     |      | =   | [Phys Atk]×[Modificador]−[Phys      |     | Def]       |     |
= 88×1.3−20
= 94,4
≈ 94
Con esto, Tressa ejecutaría 94 de daño a Meep, dejándolo con 212−94=118 de HP
Como se mencionó anteriormente, un ataque físico utiliza las stats Phys Atk del atacante y Phys Def
del enemigo, mientras que un ataque elemental utiliza las stats Elem Atk del atacante y Elem Def del
enemigo. Por esta razón el daño se calculó con la Phys Atk de Tressa y la Phys Def de Meep.
| 6.2. Daño | con | debilidades |     |     |     |     |     |
| --------- | --- | ----------- | --- | --- | --- | --- | --- |
Alcálculodedañoanteriorseledebeañadirunmultiplicadorencasodequeelobjetivopresentedebilidadal
tipodeataquerecibidooseencuentreenestadodeBreaking Point.Serdébilauntipodeataquesignificará
recibirun50%dedañoadicional,mientrasqueestarenestadoBreaking Pointrepresentatambiénun50%
de daño adicional al recibir ataques de cualquier tipo. Ambos modificadores son acumulables, de modo que
el daño final a recibir a partir del daño calculado en el punto anterior se describe de la siguiente forma:
|     |     |     |     | Estado Normal |     |     | Breaking Point |
| --- | --- | --- | --- | ------------- | --- | --- | -------------- |
Ataque Normal Daño Final = Daño Base Daño Final = Daño Base × 1.5
Ataque con Debilidad Daño Final = Daño Base × 1.5 Daño Final = Daño Base × 2
|          |            | Figura 17: | Comparación | de daños | con debilidades | y Breaking | Point |
| -------- | ---------- | ---------- | ----------- | -------- | --------------- | ---------- | ----- |
| Volvamos | al ejemplo | de antes:  |             |          |                 |            |       |
Supongamos ahora que Tressa ataca a Meep con un ataque básico de tipo Bow, tipo al que Meep es
| débil. El | cálculo | del daño sería | el siguiente: |     |     |         |     |
| --------- | ------- | -------------- | ------------- | --- | --- | ------- | --- |
|           |         | (cid:0)        |               |     |     | (cid:1) |     |
Daño = [stat ofensiva]×[Modificador]−[stat defensiva] ×[Multiplicador]
|     |     | (cid:0) |                                |            |      | (cid:1)          |     |
| --- | --- | ------- | ------------------------------ | ---------- | ---- | ---------------- | --- |
|     |     | =       | [Phys Atk]×[Modificador]−[Phys |            | Def] | ×[Multiplicador] |     |
|     |     | (cid:0) |                                | (cid:1)    |      |                  |     |
|     |     | =       | 88 × 1.3                       | − 20 × 1.5 |      |                  |     |
|     |     | = 94.4  | × 1.5                          |            |      |                  |     |
|     |     | = 141.6 |                                |            |      |                  |     |
|     |     | ≈ 141   |                                |            |      |                  |     |
Con esto, Tressa ejecutaría 141 de daño a Meep, dejándolo con 212−141=71 de HP.
El daño realizado siempre debe ser mayor o igual a 0, por lo tanto, en cualquier caso donde se dé un cálculo
| menor a | 0, entonces | la unidad | realizará | 0 de daño. |     |     |     |
| ------- | ----------- | --------- | --------- | ---------- | --- | --- | --- |
16

| 6.3. | Manejo | de  | decimales |     |     |     |     |
| ---- | ------ | --- | --------- | --- | --- | --- | --- |
Para facilitar el testeo automático del juego, evitaremos usar números decimales al momento de realizar
cálculos. En particular, los distintos cálculos del juego producen resultados decimales, los cuales deben ser
truncados al entero más cercano por debajo. El valor que será truncado será el resultado final del cálculo
| del daño | luego | de haberle | aplicado | cualquier |     | tipo de modificador | de daño. |
| -------- | ----- | ---------- | -------- | --------- | --- | ------------------- | -------- |
Porejemplo,supongamosqueunpersonajeatacaaunabestiaconunataquebásicoenestadonormal,donde
X es la stat ofensiva, M es el modificador e Y es la stat defensiva, entonces el resultado será:
|     |     |     |     |     | Daño | = ⌊X×M | −Y⌋ |
| --- | --- | --- | --- | --- | ---- | ------ | --- |
Ahora,supongamosqueelpersonajerealizaunataquebásicocondebilidadaunenemigo,dondeRrepresenta
| el modificador |     | de ruptura, | el resultado |     | será: |            |         |
| -------------- | --- | ----------- | ------------ | --- | ----- | ---------- | ------- |
|                |     |             |              |     |       | (cid:0)    | (cid:1) |
|                |     |             |              |     | Daño  | = ⌊ X×M −Y | ×R⌋     |
En resumen, se truncará el valor final luego de aplicar todos los modificadores correspondientes.
7. Habilidades
EnelOctopathTraveler,existendostiposdehabilidades:habilidadesactivasypasivas.Lasprimerassonuna
acción que se ejecuta durante el turno del viajero, mientras que las segundas se activarán automáticamente
sin injerencia del jugador siempre que se cumplan ciertas condiciones. Adicionalmente, se encuentran las
habilidades que utilizarán las bestias, que funcionan de manera similar que las habilidades activas.
Independiente del tipo de habilidad, estas se pueden describir como un conjunto de efectos con un objetivo.
A continuación describiremos los posibles tipos de objetivos y efectos, para después definir los tipos de
| habilidades | junto | a ejemplos | de  | estas. |     |     |     |
| ----------- | ----- | ---------- | --- | ------ | --- | --- | --- |
7.1. Objetivos
Existen distintos tipos de objetivos que varían según la habilidad. Estos pueden dar al jugador una opción a
elegir entre ciertos personajes o bien tener un objetivo definido. Para este proyecto contaremos con 6 formas
| de seleccionar |     | objetivos | para una | habilidad. |     |     |     |
| -------------- | --- | --------- | -------- | ---------- | --- | --- | --- |
Single: Selecciona un único personaje entre los enemigos. Para el caso de las bestias, selecciona un
| único | personaje | entre | los | viajeros. |     |     |     |
| ----- | --------- | ----- | --- | --------- | --- | --- | --- |
Enemies:Afectaatodoslospersonajesenemigos.Paraelcasodelasbestias,atacaatodoslosviajeros.
| User: | Afecta | al personaje |     | que utilizó | la  | habilidad. |     |
| ----- | ------ | ------------ | --- | ----------- | --- | ---------- | --- |
Ally: Selecciona un personaje entre los aliados, pudiendo seleccionar a la misma unidad que utilizó la
habilidad. Estas habilidades se pueden seleccionar solo sobre unidades vivas, con la única excepción
| siendo | las | habilidades | que | tienen | el efecto | de revivir. |     |
| ------ | --- | ----------- | --- | ------ | --------- | ----------- | --- |
Party: Afecta a todos los personajes aliados, incluyendo al personaje que utilizó la habilidad.
| Any: | Puede | seleccionar | tanto | a   | un aliado, | a sí mismo | o a una bestia. |
| ---- | ----- | ----------- | ----- | --- | ---------- | ---------- | --------------- |
7.2. Efectos
Todahabilidadtienealmenosunefecto.Paraelcasodelashabilidadesactivasydebestias,estosseproducen
por utilizar la habilidad como la acción del turno, mientras que para las habilidades pasivas, los efectos se
activarán si se cumplen las condiciones de la habilidad. Se tienen las siguientes categorías de efectos:
17

| 7.2.1. | Aumento | de  | stat | durante | el combate |     |
| ------ | ------- | --- | ---- | ------- | ---------- | --- |
Este efecto agrega un valor a la stat base de la unidad que porta la habilidad, este efecto dura todo el juego.
7.2.2. Daño
Este efecto genera daño de tipo físico o elemental, dependiendo de la descripción de la habilidad, hacia
la unidad que se dirige el ataque. Los tipos de daño pueden ser: Sword, Spear, Axe, Dagger,
Bow, Stave, Fire, Ice, Lightning, Wind, Light y Dark, y estos ataques aportan como
| un golpe | para la | mecánica | de  |     | Point. |     |
| -------- | ------- | -------- | --- | --- | ------ | --- |
Breaking
El cálculo del daño se realiza de la misma manera que los ataques básicos, donde el modificador se obtiene
| de la descripción |     | de la | habilidad. |     |     |     |
| ----------------- | --- | ----- | ---------- | --- | --- | --- |
7.2.3. Recuperación
Este efecto restaura HP o SP a la unidad que se dirige la habilidad, esta unidad necesariamente debe estar
viva.
La cantidad de HP o SP restaurada sería, en general, calculada según la stat Elem Def de la unidad que
| utiliza la | habilidad.  | Se  | seguirá  | la siguiente | fórmula:        |            |
| ---------- | ----------- | --- | -------- | ------------ | --------------- | ---------- |
|            |             |     |          |              | (cid:0)         | (cid:1)    |
|            |             |     |          |              | Heal=⌊ Elem     | Def ×M⌋    |
| Donde M    | corresponde |     | al valor | Modificador  | de la habilidad | utilizada. |
7.2.4. Revivir
Este efecto devuelve a la vida a un viajero caído. Al revivir la unidad siempre se le dará 1 HP.
Cuando una unidad revive, esta va a poder actuar a partir de la siguiente ronda, no la ronda en la que lo
reviven.
7.2.5. Estados
Este tipo de efectos se aplican por cierta cantidad de tiempo, medido en rondas o acciones, de tal forma que
se pueden stackear los efectos del mismo tipo. Si una unidad posee un estado y se le vuelve aplicar el mismo
estado, simplemente se extenderá la duración de su efecto, en ningún caso se alterará o potenciará el efecto
del estado.
La duración de los estados empiezan a contar desde que se aplica el efecto, independiente de si la unidad
que lo recibe actuó o no. Por ejemplo, si una unidad le otorga un estado por 2 rondas a una bestia que ya
actuó, entonces al terminar la ronda actual, al efecto le quedará solo 1 ronda de duración. Lo mismo ocurre
| con cualquier | buff, | debuff, | ailment |     | o efecto sobre la cola | de turnos. |
| ------------- | ----- | ------- | ------- | --- | ---------------------- | ---------- |
Buffs
Este efecto modifica temporalmente las características de la unidad afectada, de tal forma que la beneficie
dealgunamanera.Estosenningúncasomodificanlosstatsbaseysuduracióndependerádecadahabilidad.
| Los tipos | de Buffs | son | los siguientes: |     |     |     |
| --------- | -------- | --- | --------------- | --- | --- | --- |
Increased Physical Attack: Al realizar un ataque físico, multiplica el daño a realizar por ×1,5.
Increased Elemental Attack: Al realizar un ataque elemental, multiplica el daño a realizar por
×1,5.
Increased Physical Defense: Al recibir un ataque físico, multiplica el daño recibido por ×2.
3
18

Increased Elemental Defense: Al recibir un ataque elemental, multiplica el daño recibido por
×2.
3
Increased Increased Speed: Al ordenar la cola de turnos, la unidad se posicionará como si
|     | tuviera | ×1,5 de su | Speed | base. |     |     |
| --- | ------- | ---------- | ----- | ----- | --- | --- |
Debuffs
Este efecto modifica temporalmente las características de la unidad afectada, de tal forma que lo perjudique
de alguna manera. Al igual que los buffs, este efecto no modifica los stats base y su duración depende de
| cada | habilidad. | Los tipos | de  | Debuffs | son los siguientes: |     |
| ---- | ---------- | --------- | --- | ------- | ------------------- | --- |
Decreased Physical Attack: Al realizar un ataque físico, multiplica el daño a realizar por ×2.
3
Decreased Elemental Attack: Al realizar un ataque elemental, multiplica el daño a realizar por
×2.
3
Decreased Physical Defense: Al recibir un ataque físico, multiplica el daño recibido por ×1,5.
Decreased Elemental Defense: Al recibir un ataque elemental, multiplica el daño recibido por
×1,5.
Decreased Speed: Al ordenar la cola de turnos, la unidad se posicionará como si tuviera ×2 de
3
|     | su Speed | base. |     |     |     |     |
| --- | -------- | ----- | --- | --- | --- | --- |
Los estados de Buff y Debuff en ningún caso modifican los stats de las unidades, si no que modifican el
daño infligido/recibido o bien modifican la posición que tomarán en la cola de turnos.
Ailments
Esteefectootorgaunestadoqueperjudicaalaunidadquelorecibe.LosAilmentspermanecenenlaunidad
durante una cierta cantidad de rondas, mientras la unidad posea un Ailment tendrá un efecto pasivo sobre
| sí misma. | Los | tipos de | Ailments | son | los siguientes: |     |
| --------- | --- | -------- | -------- | --- | --------------- | --- |
Poison: Unidad recibe daño igual al 17% de su HP máximo luego de actuar en su turno, si la
unidad no tuvo turno en una ronda, entonces se aplica el daño al final de la ronda. Si la unidad tiene
más de un turno por ronda, recibirá daño después de cada uno de sus turnos.
|     | Silence: | Unidad | no  | puede | utilizar habilidades | activas. |
| --- | -------- | ------ | --- | ----- | -------------------- | -------- |
Unconscious: Unidad pierde su turno durante la ronda, de modo que no participa en ella a pesar
|     | de estar | viva. |     |     |     |     |
| --- | -------- | ----- | --- | --- | --- | --- |
Sleep: Unidad pierde el turno durante la ronda, de modo que no participa en ella a pesar de estar
viva. El estado se remueve por completo si la unidad recibe daño por un ataque.
Terror: Unidad no puede utilizar boost y tampoco ganará BP al final de la ronda.
| Priorización |     | en cola | de turnos |     |     |     |
| ------------ | --- | ------- | --------- | --- | --- | --- |
Este efecto le otorga a la unidad prioridad en la cola de turnos. Como se explicó en el algoritmo, una
unidad con prioridad en la cola de turnos se posiciona antes que el resto ignorando su velocidad. No existen
| habilidades     |     | que le otorguen | prioridad |        | a una bestia, | solo a viajeros. |
| --------------- | --- | --------------- | --------- | ------ | ------------- | ---------------- |
| Despriorización |     | en              | cola de   | turnos |               |                  |
Este efecto provoca que la unidad se posicione al final de la cola de turnos, ignorando la velocidad de esta.
No existen habilidades que desprioricen el turno de un viajero, solo a bestias.
Mejoras
19

Este efecto otorga mejoras a la unidad similar a los Buffs, sin embargo, la duración de estos no se miden
| por rondas, | si no | por | acciones o usos. | Los | tipos de mejoras | son: |
| ----------- | ----- | --- | ---------------- | --- | ---------------- | ---- |
CounterAttack: Refleja un ataque físico, infligiendo su daño a la unidad atacante.
ReflectiveVeil: Refleja un ataque elemental, infligiendo su daño a la unidad atacante.
Estos efectos no tienen duración por rondas, en cambio, la unidad no perderá el efecto hasta que lo utilice.
Porejemplo,siunaunidadtiene CounterAttackpor2acciones,estelotendrásiemprehastaquereciba
un ataque físico, en ese momento reflejará el daño que debería recibir y pasará a tener CounterAttack
por 1 acción.
Combinaciones
Notarqueunaunidadpuedetenermúltiplesestadosdeformasimultánea,aplicandocadaunodelosestados
para obtener los efectos finales. Por ejemplo, una unidad con Increased Physical Attack y con
| Decreased | Physical |     | Attack calculará | su  | daño de la | forma |
| --------- | -------- | --- | ---------------- | --- | ---------- | ----- |
2
|     |     |     |     | Daño | Final=Daño | Base×1,5× |
| --- | --- | --- | --- | ---- | ---------- | --------- |
3
7.2.6. Especiales
Enestacategoríaentranlosefectosquenocaendirectamenteenlascategoríasanteriores.Lostiposdeefectos
| especiales | son:        |              |              |            |                       |     |
| ---------- | ----------- | ------------ | ------------ | ---------- | --------------------- | --- |
| Modificar  |             | la selección | de objetivo. |            |                       |     |
| Aumentar   |             | la duración  | de Buffs     | y Debuffs. |                       |     |
| Otorgar    | un          | turno        | extra.       |            |                       |     |
| Realizar   |             | un ataque    | extra.       |            |                       |     |
| Alterar    | la          | cantidad     | de veces     | que se     | efectúa la habilidad. |     |
| Anular     | daño        | recibido.    |              |            |                       |     |
| Infligir   | más         | daño.        |              |            |                       |     |
| Entre      | otras.      |              |              |            |                       |     |
| 7.3.       | Habilidades |              | activas      |            |                       |     |
Formanpartedelasaccionesquepuedenrealizarlosviajeros.Estascorrespondenamovimientoscondistintos
efectos, que se aplican sobre distintos personajes, son más poderosas que un ataque básico, pero a cambio
se debe gastar SP correspondiente. No tener suficiente SP significará que no se puede utilizar la habilidad.
| Las características |     | que | definen una | habilidad | son las siguientes: |     |
| ------------------- | --- | --- | ----------- | --------- | ------------------- | --- |
20

|     |     |     |     |     | Nombre:       |     | Nombre                                           | de la habilidad, | es  | su identificador | único. |     |
| --- | --- | --- | --- | --- | ------------- | --- | ------------------------------------------------ | ---------------- | --- | ---------------- | ------ | --- |
|     |     |     |     |     | Costo         | de  | SP:CantidaddeSPqueelviajeroconsumiráparautilizar |                  |     |                  |        |     |
|     |     |     |     |     | la habilidad. |     |                                                  |                  |     |                  |        |     |
Tipo:Representaeltipodeataquequetienelahabilidad.Habilidades
|     |     |     |     |     | que no       | realizan | daño      | no poseen  | tipo.           |                  |          |          |
| --- | --- | --- | --- | --- | ------------ | -------- | --------- | ---------- | --------------- | ---------------- | -------- | -------- |
|     |     |     |     |     | Descripción: |          | Texto     | que define | el efecto       | de la habilidad. |          |          |
|     |     |     |     |     | Objetivo:    |          | Determina | sobre      | quien o quienes | se puede         | utilizar | la habi- |
lidad.
|     |     |     |     |     | Modificador: |              | Es  | un valor que | determina | la potencia | y efectividad | de  |
| --- | --- | --- | --- | --- | ------------ | ------------ | --- | ------------ | --------- | ----------- | ------------- | --- |
|     |     |     |     |     | algunas      | habilidades. |     |              |           |             |               |     |
Boost:Descripcióndelamejoraquerecibelahabilidadalserutilizada
|           |          |          |             |         | con BP       |         |     |           |     |     |     |     |
| --------- | -------- | -------- | ----------- | ------- | ------------ | ------- | --- | --------- | --- | --- | --- | --- |
| (a) Cyrus | de       | Octopath | Traveler    |         |              |         |     |           |     |     |     |     |
| Algunos   | ejemplos | de       | habilidades |         | activas son: |         |     |           |     |     |     |     |
| Fireball: |          | Inflige  | daño        | de tipo | Fire         | a todos | los | enemigos. |     |     |     |     |
Last Stand: Inflige daño de tipo Axe a todos los enemigos. Realiza 3% más de daño por cada 1%
| de      | HP  | que le | falte   | a la unidad | que usa        | la habilidad. |           |     |     |     |     |     |
| ------- | --- | ------ | ------- | ----------- | -------------- | ------------- | --------- | --- | --- | --- | --- | --- |
| Revive: |     | Revive | a todos | los         | aliados caídos |               | con 1 HP. |     |     |     |     |     |
Spearhead: Inflige daño de tipo Spear a un enemigo y posiciona al usuario primero en la cola de
| turnos | de  | la siguiente |     | ronda. |     |     |     |     |     |     |     |     |
| ------ | --- | ------------ | --- | ------ | --- | --- | --- | --- | --- | --- | --- | --- |
Lion Dance: Otorga Increased Physical Attack a un aliado durante 2 rondas.
| Reflective |     | Veil: | Otorga | a   | un aliado | ReflectiveVeil |     | por | 1 acción. |     |     |     |
| ---------- | --- | ----- | ------ | --- | --------- | -------------- | --- | --- | --------- | --- | --- | --- |
Steal SP: Realiza dos ataques del tipo Dagger a un enemigo y restaura SP equivalente al 5% del
| daño | realizado. |     |     |     |     |     |     |     |     |     |     |     |
| ---- | ---------- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
Incite: Durante 3 rondas, el usuario se vuelve el objetivo de las habilidades rivales que afectan a un
| único | personaje. |     |     |     |     |     |     |     |     |     |     |     |
| ----- | ---------- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
Comosepuedever,lashabilidadespuedentener1omásefectosdelosdescritosanteriormente.Siempreque
se tenga la cantidad necesaria de SP para ejecutar la habilidad, esta se podrá ocupar como una acción en el
turno.
| 7.3.1. | Boost | en  | Habilidades |     |     |     |     |     |     |     |     |     |
| ------ | ----- | --- | ----------- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
Este tipo de habilidades pueden verse beneficiadas al ser usadas con boosting points (BP), adquiriendo
un efecto más potente. La mejora que recibe una habilidad al ser utilizada con boosting dependerá de la
descripción de la propia habilidad. Toda habilidad tiene una descripción aparte que define cómo aportan los
BP al efecto.
| Algunas     | de las | mejoras    | comunes |         | por boosting | son | las siguientes: |     |     |     |     |     |
| ----------- | ------ | ---------- | ------- | ------- | ------------ | --- | --------------- | --- | --- | --- | --- | --- |
| Habilidades |        | con efecto |         | de daño |              |     |                 |     |     |     |     |     |
Habilidades que tienen como único efecto el daño a un enemigo, se ven beneficiadas aumentando su modifi-
cadorporcadaBPutilizado,locualimplicaunmayordaño.VeamosunejemploconlahabilidadFireball:
| Descripción: |     |     | Inflige | daño | de tipo | Fire | a todos | los enemigos |     |     |     |     |
| ------------ | --- | --- | ------- | ---- | ------- | ---- | ------- | ------------ | --- | --- | --- | --- |
| Modificador: |     |     | 1.5     |      |         |      |         |              |     |     |     |     |
21

Boost: Aumenta el modificador en un 90% del modificador base por cada BP gastado.
Si se utiliza Fireball con 2 BP entonces se calcularía el daño de forma normal, pero utilizando
Modificador = 1,5+(0,9×2)×1,5=1,5+1,8×1,5=4,2.
Habilidades con efecto de recuperación
Habilidades que se centrar en restaurar se ven beneficiadas aumentando su modificador por cada BP uti-
lizado, lo cual implica una mayor cantidad de HP o SP a restaurar. Veamos un ejemplo con la habilidad
Heal Wounds:
Descripción: Restaura HP a todas las unidades aliadas.
Modificador: 1.5
Boost: Aumenta el modificador en 0.5 por cada BP gastado.
SiseutilizaHeal Woundscon2BPentoncessecalcularíaalcuracióndeformanormal,peroutilizando
Modificador = 1.7 + 0.5 × 2 = 2.7
Habilidades de estado
HabilidadesqueotorganBuffs,DebuffsoAilments,sevenbeneficiadasaumentandoladuracióndelefecto
por cada BP utilizado. Veamos un ejemplo con la habilidad Lion Dance:
Descripción: Otorga Increased Physical Attack a un aliado por 2 rondas.
Boost: Aumenta la duración en 2 rondas por cada BP.
Si se utiliza Lion Dance con 3 BP, entonces la cantidad de rondas que durará el efecto sería =
2+(2×3)=8.
Similarmente, habilidades que otorgan Mejoras aumentarán la duración del efecto por cada BP utilizado
pero dirigido a cantidad de acciones y no rondas. Veamos un ejemplo con la habilidad Moon’s Reflexion:
Descripción: Otorga a un aliado CounterAttack por 1 acción.
Boost: Aumenta la duración en 1 acción por cada BP.
Si se utiliza Moon’s Reflexion con 2 BP, entonces la cantidad de acciones que durará el efecto sería
= 1+(1×2)=3.
7.3.2. Habilidades divinas
Estashabilidadessonhabilidadesactivasespeciales,yaquesolosepuedenutilizarcuandosetienenalmenos3
BP.Adiferenciadelrestodehabilidadesactivas,estasnotienenunefectopotenciadoporutilizarboosting,
porque se requiere del boosting para poder accionarlas.
Algunos ejemplos de habilidades divinas son:
Aelfric’sAuspices:Seleccionaaunaliado.Durante3rondaslashabilidadesdeesealiadoseactivarán
2 veces. No afecta a las habilidades divinas.
Alephan’s Enlightenment: Selecciona a un aliado. Durante 3 rondas sus habilidades con target
Enemies tendrán target Single realizando el doble de daño. No afecta a las habilidades divinas.
Steorra’s Prophecy: Inflige daño de tipo Dark a todos los enemigos. Aumenta el modificador en
un 20% por cada BP que tenga el equipo.
Winnehild’sBattleCry:Infligedañodelas6armas( Sword, Spear, Dagger, Axe, Bow
y Stave) a todos los enemigos.
22

Como se puede ver, las habilidades divinas se componen principalmente de efectos especiales, donde pueden
cambiarlaformadeejecucióndeestas,losobjetivosolaformadecalculareldaño.Sonhabilidadespoderosas
| que consumen |     | más SP | que | el resto. |     |     |     |     |     |     |     |     |     |
| ------------ | --- | ------ | --- | --------- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
Al igual que los efectos de estado, habilidades como Aelfric’s Auspices o Alephan’s Enlightenment
pueden stackearse. Por ejemplo, si una unidad ya tiene el efecto de Aelfric’s Auspices y otro viajero usa
Aelfric’s Auspicesenlamismaunidad,entonceselefectodurará6rondasenvezde3.Noobstante,como
dice la descripción de la habilidad, estas en ningún caso alterarán la ejecución de otra habilidad divina. Si
unaunidadestábajoelefectodeAelfric’s AuspicesyusaWinnehild’s Battle Cry,estanoseactivará
2 veces.
| 7.4. | Habilidades |     | pasivas |     |     |     |     |     |     |     |     |     |     |
| ---- | ----------- | --- | ------- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
Lashabilidadespasivassonhabilidadesqueotorgandistintosefectosenfavordelusuario,quepuedenafectar
tanto a la unidad, como a un aliado o un enemigo. Estas habilidades actúan de forma pasiva durante todo
| el combate     | y el     | jugador  | no            | puede   | elegir       | si se | usan o no.   |                  |     |              |                  |                    |     |
| -------------- | -------- | -------- | ------------- | ------- | ------------ | ----- | ------------ | ---------------- | --- | ------------ | ---------------- | ------------------ | --- |
| Cada habilidad |          | tendrá   | los siguiente |         | atributos:   |       |              |                  |     |              |                  |                    |     |
|                |          |          |               |         | Nombre:      |       | Nombre       | de la habilidad, |     | es su        | identificador    | único.             |     |
|                |          |          |               |         | Descripción: |       |              | Texto que define | el  | efecto       | de la habilidad. |                    |     |
|                |          |          |               |         | Objetivo:    |       | Determina    | cuáles           | son | las unidades |                  | que pueden recibir | el  |
|                |          |          |               |         | efecto       | de    | la habilidad |                  |     |              |                  |                    |     |
| (a) Primrose   | de       | Octopath | Traveler      |         |              |       |              |                  |     |              |                  |                    |     |
| Algunos        | ejemplos | de       | habilidades   | pasivas |              | son:  |              |                  |     |              |                  |                    |     |
Summon Strengh: Aumenta en 50 el stat Phys Atk de la unidad que porte la habilidad.
Patience: Si al final de la ronda, el stat HP y el stat SP de la unidad son par, entonces la unidad
| obtiene |     | un turno | adicional | antes | de  | finalizar | la  | ronda. |     |     |     |     |     |
| ------- | --- | -------- | --------- | ----- | --- | --------- | --- | ------ | --- | --- | --- | --- | --- |
The Show Goes On: Los buffs que la unidad otorgue a sus aliados, tendrán 1 ronda extra de
duración
Cover: El usuario recibe los ataque con target Single que se realicen sobre aliados con 30% de vida
o menos.
Incidental Attack: Al usar habilidades que no realicen daño sobre un enemigo, si el HP del usuario
| es  | impar, | realiza | un ataque |     | básico | contra | el objetivo | con su | primer | arma. |     |     |     |
| --- | ------ | ------- | --------- | --- | ------ | ------ | ----------- | ------ | ------ | ----- | --- | --- | --- |
Physical Prowess: El portador tendrá Increased Physical Attack y Increased Physical
| Defense |     | durante | toda | la partida. |     |     |     |     |     |     |     |     |     |
| ------- | --- | ------- | ---- | ----------- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
BP Eater: Las habilidades del usuario utilizadas con boost realizan un 50% de daño adicional.
Endure:Sielusuarioposeealgúnailment,tendrá IncreasedPhysicalDefensey Increased
| Elemental |     | Defense |     | por la | duración | del | ailment. |     |     |     |     |     |     |
| --------- | --- | ------- | --- | ------ | -------- | --- | -------- | --- | --- | --- | --- | --- | --- |
23

Los efectos de las habilidades pasivas se activarán siempre y cuando se cumpla la condición de la misma,
hay habilidades que no poseen condiciones como Summon Strengh y, por tanto, se activarán siempre. Un
viajero muerto no puede hacer uso de sus habilidades pasivas, por tanto, estas no se activan.
| 7.5. Habilidades |     | de las | bestias |     |
| ---------------- | --- | ------ | ------- | --- |
Lasbestias,adiferenciadelosviajeros,solotienenlaacciónUsar Habilidad,sinembargo,sushabilidades
sonsimilaresalashabilidadesactivasdeestos.Cadabestiaposeeunaúnicahabilidady,dadoquelasbestias
no poseen elección de objetivo, este se obtendrá de la definición de la habilidad.
| Algunos ejemplos | habilidades | de        | bestias son:      |                       |
| ---------------- | ----------- | --------- | ----------------- | --------------------- |
| Attack:          | Realiza un  | ataque    | físico al viajero | con mayor HP.         |
| Incinerate:      | Realiza     | un ataque | elemental         | a todos los viajeros. |
Double Bite: Realiza un ataque físico al viajero con menor Phys Def dos veces.
Acid Spray: Otorga Decreased Physical Defense y Decreased Elemental Defense al
| viajero | con mayor HP | durante | 2 rondas. |     |
| ------- | ------------ | ------- | --------- | --- |
Poison Strike: Realiza un ataque físico al viajero con mayor HP. Adicionalmente aplica Poison
| por 2 rondas. |     |     |     |     |
| ------------- | --- | --- | --- | --- |
Vortal Claw: Realiza un ataque que reduce a la mitad el HP de todos los viajeros.
Aligualquelashabilidadesactivas,estaspuedentener1omásefectosqueseactivansimplementealutilizar
la habilidad.
24