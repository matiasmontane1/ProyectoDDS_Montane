|     | Pontificia   | Universidad      | Católica | de Chile       |             |
| --- | ------------ | ---------------- | -------- | -------------- | ----------- |
|     | Escuela      | de Ingeniería    |          |                |             |
|     | Departamento | de Ciencia       | de       | la Computación |             |
|     | IIC2113      | Diseño Detallado | de       | Software       |             |
|     |              | Entrega          |          | 3: Octopath    | Traveler    |
|     |              | Javiera          | Ignacia  | Pinto          | Santa María |
|     |              | Matías           | Andrés   | Poblete        | Farías      |
Introducción
Paraestaentrega,deberásimplementarhabilidadesquetienenmásdeunhit,habilidadesqueotorganbuffs
y debuffsalasstatsdelosviajerosylasbestiasyhabilidadesdivinas.Porotraparte,deberásimplementar
| el uso de Boost | Points | tanto en ataques | básicos | como en | las habilidades. |
| --------------- | ------ | ---------------- | ------- | ------- | ---------------- |
Test cases
| Para esta entrega, | se evaluarán | los siguientes |     | grupos de tests: |     |
| ------------------ | ------------ | -------------- | --- | ---------------- | --- |
E1-BasicCombat.
E1-InvalidTeams.
E1-RandomBasicCombat.
E2-BeastsSkills.
E2-DefendAndBreakingPoint.
E2-OffensiveSkills.
E2-HealingAndQueueSkills.
E2-BaseStatsPassives.
E2-Mix
E2-Random.
E3-BasicAttackBoosting.
E3-BasicAttackBoostingRandom.
E3-BasicPassives.
E3-BasicPassivesRandom.
E3-MultiHitOffensive.
E3-BasicSkillBoosting.
E3-AdvanceSkillBoosting.
E3-StatusEffects.
E3-BeastsSkills.
E3-DivineSkills.
1

E3-IntermediatePassives.
E3-Mix.
E3-Random.
| Habilidades |     | activas |     |     |     |     |     |     |     |
| ----------- | --- | ------- | --- | --- | --- | --- | --- | --- | --- |
Enestaentrega,deberásimplementarmáshabilidadesofensivasycurativas,ademásdehabilidadesdebuffs
| y debuffs, | habilidades |     | híbridas | y   | divinas. |     |     |     |     |
| ---------- | ----------- | --- | -------- | --- | -------- | --- | --- | --- | --- |
A continuación, listaremos las habilidades de esta entrega con el siguiente formato:
|             |     | [Type,Cost,Modifier,Target,Hits], |     |     |     |     | Name: Effect. | [Efecto | por BP] |
| ----------- | --- | --------------------------------- | --- | --- | --- | --- | ------------- | ------- | ------- |
| Habilidades |     | ofensivas                         |     |     |     |     |               |         |         |
[ Fire,22,1.6,Enemies,2], Fire Storm: Inflige daño de tipo Fire a todos los enemigos 2 veces.
| [Aumenta |     | el modificador |     | en  | un 90% | por cada BP] |     |     |     |
| -------- | --- | -------------- | --- | --- | ------ | ------------ | --- | --- | --- |
[ Ice,22,1.6,Enemies,2], Blizzard: Inflige daño de tipo Ice a todos los enemigos 2 veces. [Au-
| menta | el  | modificador |     | en un | 90% por | cada BP] |     |     |     |
| ----- | --- | ----------- | --- | ----- | ------- | -------- | --- | --- | --- |
[ Lightning,22,1.6,Enemies,2], Lightning Blast: Inflige daño de tipo Lightning a todos los
| enemigos |     | 2 veces. | [Aumenta |     | el modificador | en un | 90% por cada | BP] |     |
| -------- | --- | -------- | -------- | --- | -------------- | ----- | ------------ | --- | --- |
[ Dagger,6,1.6,Single,2], HP Thief: Inflige daño de tipo Dagger a un enemigo 2 veces, ade-
más, recupera la mitad del daño infligido como HP. [Aumenta el modificador en un 70% por cada
BP]
[ Spear,20,0.8,Single,7],ThousandSpears:Infligedañodetipo Spearalenemigoconmenor
| PhysDef |     | 7 veces. | [Aumenta |     | el modificador | en un | 50% por cada | BP] |     |
| ------- | --- | -------- | -------- | --- | -------------- | ----- | ------------ | --- | --- |
[ Dagger,6,1.6,Single,2],StealSP:Infligedañodetipo Daggeraunenemigo2veces,además,
gana un 5% del daño infligido como SP. [Aumenta el modificador en un 70% por cada BP]
[ Bow,8,0.8,Single,6], Rain of Arrows: Inflige daño de tipo Bow al enemigo con menor HP
| 6   | veces. | [Aumenta | el  | modificador | en  | un 50% por | cada BP] |     |     |
| --- | ------ | -------- | --- | ----------- | --- | ---------- | -------- | --- | --- |
[ Bow,24,0.8,Enemies,6], Arrowstorm: Inflige daño de tipo Bow a todos los enemigos 6 veces.
| [Aumenta |     | el modificador |     | en  | un 40% | por cada BP] |     |     |     |
| -------- | --- | -------------- | --- | --- | ------ | ------------ | --- | --- | --- |
[ Sword,35,2,Single,5],Guardian Liondog:Infligedañodetipo Swordalenemigoconmayor
| Speed | 5   | veces. | [Aumenta | el  | modificador | en un 80% | por cada BP] |     |     |
| ----- | --- | ------ | -------- | --- | ----------- | --------- | ------------ | --- | --- |
[ Fire,36,1.6,Enemies,3],IgnisArdere:Infligedañodetipo Fireatodoslosenemigos3veces.
| [Aumenta |     | el modificador |     | en  | un 90% | por cada BP] |     |     |     |
| -------- | --- | -------------- | --- | --- | ------ | ------------ | --- | --- | --- |
[ Ice,36,1.6,Enemies,3], Glacies Claudere: Inflige daño de tipo Ice a todos los enemigos 3
| veces. | [Aumenta |     | el modificador |     | en un | 90% por | cada BP] |     |     |
| ------ | -------- | --- | -------------- | --- | ----- | ------- | -------- | --- | --- |
[ Lightning,36,1.6,Enemies,3], Tonitrus Canere: Inflige daño de tipo Lightning a todos los
| enemigos |     | 3 veces. | [Aumenta |     | el modificador | en un | 90% por cada | BP] |     |
| -------- | --- | -------- | -------- | --- | -------------- | ----- | ------------ | --- | --- |
[ Wind,36,1.6,Enemies,3], Ventus Saltare: Inflige daño de tipo Wind a todos los enemigos 3
| veces. | [Aumenta |     | el modificador |     | en un | 90% por | cada BP] |     |     |
| ------ | -------- | --- | -------------- | --- | ----- | ------- | -------- | --- | --- |
[ Light,36,1.6,Enemies,3], Lux Congerere: Inflige daño de tipo Light a todos los enemigos
| 3   | veces. | [Aumenta | el  | modificador | en  | un 90% por | cada BP] |     |     |
| --- | ------ | -------- | --- | ----------- | --- | ---------- | -------- | --- | --- |
2

[ Dark,36,1.6,Enemies,3], Tenebrae Operire: Inflige daño de tipo Dark a todos los enemigos
| 3 veces. | [Aumenta | el  | modificador | en un | 90% por | cada BP] |
| -------- | -------- | --- | ----------- | ----- | ------- | -------- |
Buff y debuff
[-,6,-,Ally,1], Sheltering Veil: Otorga a un aliado Increased Elemental Defense por 2
| rondas. | [Aumenta | la  | duración | en 2 rondas | por cada | BP] |
| ------- | -------- | --- | -------- | ----------- | -------- | --- |
[-,4,-,Ally,1],Abide:Otorgaaunaliado IncreasedPhysicalAttackpor2rondas.[Aumenta
| la duración | en  | 2 rondas | por cada | BP] |     |     |
| ----------- | --- | -------- | -------- | --- | --- | --- |
[-,4,-,User,1], Stout Wall: Otorga al usuario Increased Physical Defense por 3 rondas.
| [Aumenta | la duración |     | en 2 rondas | por | cada BP] |     |
| -------- | ----------- | --- | ----------- | --- | -------- | --- |
[-,4,-,Ally,1], Lion Dance: Otorga a un aliado Increased Physical Attack por 2 rondas.
| [Aumenta | la duración |     | en 2 rondas | por | cada BP] |     |
| -------- | ----------- | --- | ----------- | --- | -------- | --- |
[-,4,-,Ally,1],PeacockStrut:Otorgaaunaliado IncreasedElementalAttackpor2rondas.
| [Aumenta | la duración |     | en 2 rondas | por | cada BP] |     |
| -------- | ----------- | --- | ----------- | --- | -------- | --- |
[-,4,-,Ally,1], Mole Dance: Otorga a un aliado Increased Physical Defense por 2 rondas.
| [Aumenta | la duración |     | en 2 rondas | por | cada BP] |     |
| -------- | ----------- | --- | ----------- | --- | -------- | --- |
[-,4,-,Ally,1],Panther Dance:Otorgaaunaliado Increased Speedpor2rondas.[Aumenta
| la duración | en  | 2 rondas | por cada | BP] |     |     |
| ----------- | --- | -------- | -------- | --- | --- | --- |
[-,4,-,Single,1], Shackle Foe: Provoca Decreased Physical Attack a un enemigo por 2
| rondas. | [Aumenta | la  | duración | en 2 rondas | por cada | BP] |
| ------- | -------- | --- | -------- | ----------- | -------- | --- |
[-,4,-,Single,1], Armor Corrosive: Provoca Decreased Physical Defense a un enemigo
| por 2 rondas. |     | [Aumenta | la duración | en  | 2 rondas | por cada BP] |
| ------------- | --- | -------- | ----------- | --- | -------- | ------------ |
[-,25,-,Ally,1], Starsong: Por 2 rondas, le otorga a un aliado Increased Physical Defense,
Increased Elemental Defense y Increased Speed. [Aumenta la duración del efecto en 2
| rondas | por cada | BP] |     |     |     |     |
| ------ | -------- | --- | --- | --- | --- | --- |
Híbridas
[ Stave,20,2,Single,1],ElementalBreak:Infligedañodetipo Staveaunenemigoyleprovoca
Decreased Elemental Defense por 2 rondas. [Aumenta el modificador en un 100% por cada
BP]
Divinas
Estashabilidadessolopuedenutilizarsecuandosetienenalmenos3BoostPoints.Debidoaquesenecesitan
de BP para poder utilizarlas, estas habilidades no tienen un efecto adicional por BP.
[ Sword,30,7,Single,1], Brand’s Thunder: Inflige un poderoso ataque de tipo Sword en un
enemigo.
[ Bow,30,7,Enemies,1], Draefendi’s Rage: Inflige un poderoso ataque de tipo Bow en todos
los enemigos.
[ Dark,50,3,Enemies,1], Steorra’s Prophecy: Inflige daño de tipo Dark a todos los enemigos.
Aumenta el modificador en un 20% por cada BP que tenga el equipo. (Se consideran los BP de todas
3

las unidades, incluyendo las muertas. No entran en el cálculo los 3 BP que debe gastar la unidad para
| utilizar | la habilidad) |     |     |     |     |     |     |
| -------- | ------------- | --- | --- | --- | --- | --- | --- |
[ Fire Ice Lightning Wind Light Dark,50,1.5,Single,1],Balogar’s Blade:Infligedaño
por cada elemento a un enemigo. (Los golpes se realizan en el orden mencionado)
[ Sword Spear Dagger Axe Bow Stave,50,1.5,Enemies,1], Winnehild’s Battle Cry: In-
flige daño por cada arma a todos los enemigos. (Los golpes se realizan en el orden mencionado)
| Habilidades |     | pasivas |     |     |     |     |     |
| ----------- | --- | ------- | --- | --- | --- | --- | --- |
Para esta entrega deberás implementar habilidades pasivas que tienen por objetivo tanto User, como Ally
y Enemy.
|                 |         |               |     | [Target], |                 | Name: | Effect. |
| --------------- | ------- | ------------- | --- | --------- | --------------- | ----- | ------- |
| Las habilidades | pasivas | a implementar |     | son       | las siguientes: |       |         |
[User],VimandVigor:Alfinalizarcadaronda,elportadorrecuperaunacantidaddeHPequivalente
| al 10% | de su | HP máximo. |     |     |     |     |     |
| ------ | ----- | ---------- | --- | --- | --- | --- | --- |
[User], Second Wind: Al finalizar cada ronda, el portador recupera una cantidad de SP equivalente
| al 5% | de su | SP máximo. |     |     |     |     |     |
| ----- | ----- | ---------- | --- | --- | --- | --- | --- |
[User], Patience: Si luego de jugar todos los turnos de una ronda, el stat HP y el stat SP de la
unidad son par, entonces la unidad obtiene un turno adicional antes de finalizar la ronda. Uso único
| por | ronda. |     |     |     |     |     |     |
| --- | ------ | --- | --- | --- | --- | --- | --- |
[User], Persistence: Todos los cambios de aumento de stat que la unidad reciba durarán una ronda
extra.
[User], Hang Tough: Cuando la unidad está sobre el 10% de su HP máximo, cualquier ataque que
| podría  | reducir | su HP  | a 0 lo | dejará a   | 1 de HP. |                |     |
| ------- | ------- | ------ | ------ | ---------- | -------- | -------------- | --- |
| [User], | SP      | Saver: | Reduce | el consumo | de       | SP a la mitad. |     |
[Ally], The Show Goes On: Todo cambio de aumento de stat que la unidad otorgue durará una
| ronda | extra. |     |     |     |     |     |     |
| ----- | ------ | --- | --- | --- | --- | --- | --- |
[User], Encore: La primera vez que la unidad muere en combate, vuelve con el 25% de su HP
[User], Inspiration: Al realizar ataques básicos, la unidad restaurará SP igual al 1% del daño total
realizado.
[User], Heightened Healing: La unidad gana un 30% adicional de HP cada vez que es curado.
| [User], | Boost | Start: | Obtiene | 1 BP | adicional | al inicio | de la batalla. |
| ------- | ----- | ------ | ------- | ---- | --------- | --------- | -------------- |
[User], Divine Aura: Si al recibir un ataque, el HP del portador y el HP del atacante son par,
| entonces | anula | el daño | recibido. |     |     |     |     |
| -------- | ----- | ------- | --------- | --- | --- | --- | --- |
[User], Stat Swap: A la unidad se le intercambian los valores de las stats PhysAtk y ElemAtk.
| Habilidades    |     | de las        | bestias |         |     |                |              |
| -------------- | --- | ------------- | ------- | ------- | --- | -------------- | ------------ |
| Para los tests | de  | esta entrega, | habrán  | bestias | con | las siguientes | habilidades: |
[Phys,1.3,Single,2],Double Bite:RealizaunataquefísicoalviajeroconmenorPhysDef2veces.
[Elem,1.5,Enemies,2], Shadow Magic: Realiza un ataque elemental a todos los viajeros 2 veces.
4

[Phys,1.3,Single,3], Triple Slash: Realiza un ataque físico al viajero con mayor HP 3 veces.
[–,–,Single,1],Consume Armor:Otorga Decreased Physical Defensealviajeroconmayor
|     | PhysDef |     | durante | 2 rondas. |     |     |     |     |     |
| --- | ------- | --- | ------- | --------- | --- | --- | --- | --- | --- |
[Phys,1.4,Single,1],Flap:RealizaunataquefísicoalviajeroconmayorHP.Otorga Increased
|     | Speed | al  | usuario | durante | 2   | rondas. |     |     |     |
| --- | ----- | --- | ------- | ------- | --- | ------- | --- | --- | --- |
[–,–,Single,1], Acid Spray: Otorga Decreased Physical Defense y Decreased Ele-
|     | mental |     | Defense | al  | viajero | con mayor | HP durante | 2 rondas. |     |
| --- | ------ | --- | ------- | --- | ------- | --------- | ---------- | --------- | --- |
[Phys,1.4,Single,1], Gather Strength: Realiza un ataque físico al viajero con menor PhysDef.
|     | Otorga |     | Increased |     | Physical | Attack | al usuario | durante | 2 rondas. |
| --- | ------ | --- | --------- | --- | -------- | ------ | ---------- | ------- | --------- |
[-,-,Party,1], Augmented Magic: Otorga Increased Elemental Attack y Increased
|     | Elemental |     | Defense |     | a todas | las bestias | durante | 2 rondas. |     |
| --- | --------- | --- | ------- | --- | ------- | ----------- | ------- | --------- | --- |
[Elem,1.5,Enemies,1],Volcano:Realizaunataqueelementalatodoslosviajeros.Luego,otorga
|        | Decreased |     | Elemental |     | Defense |     | a todos los viajeros | durante | 2 rondas. |
| ------ | --------- | --- | --------- | --- | ------- | --- | -------------------- | ------- | --------- |
| Output |           | del | juego     |     |         |     |                      |         |           |
El formato de output del juego se mantiene igual al formato de las entregas anteriores.
| Ataque |     | básico |     | con boosting |     |     |     |     |     |
| ------ | --- | ------ | --- | ------------ | --- | --- | --- | --- | --- |
Recordemos que en el caso de los ataques básicos, el boosting permite repetir el ataque tantas veces como
puntos de Boost se utilizaron. En caso de que el jugador decida utilizar boosting, el mensaje final deberá
reflejar la situación. Para ello se debe indicar el daño de cada golpe. El siguiente ejemplo muestra el output
| esperado |     | en un | ataque | que | utilizo | boosting | con 2 BP: |     |     |
| -------- | --- | ----- | ------ | --- | ------- | -------- | --------- | --- | --- |
1 ----------------------------------------
| 2 Turno | de     | H’aanit |     |     |     |     |     |     |     |
| ------- | ------ | ------- | --- | --- | --- | --- | --- | --- | --- |
| 1:      | Ataque | básico  |     |     |     |     |     |     |     |
3
| 2:  | Usar | habilidad |     |     |     |     |     |     |     |
| --- | ---- | --------- | --- | --- | --- | --- | --- | --- | --- |
4
3: Defender
5
4: Huir
6
| 7 INPUT: |     | 1   |     |     |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
8 ----------------------------------------
| 9 Seleccione |     | un  | arma |     |     |     |     |     |     |
| ------------ | --- | --- | ---- | --- | --- | --- | --- | --- | --- |
10 1: Axe
11 2: Bow
12 3: Cancelar
| INPUT: |     | 1   |     |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- | --- | --- |
13
----------------------------------------
14
| Seleccione |     | un  | objetivo |     | para H’aanit |     |     |     |     |
| ---------- | --- | --- | -------- | --- | ------------ | --- | --- | --- | --- |
15
| 1:  | Meep | - HP:1016/1016 |     |     | Shields:2 |     |     |     |     |
| --- | ---- | -------------- | --- | --- | --------- | --- | --- | --- | --- |
16
17 2: Cancelar
| 18 INPUT: |     | 1   |     |     |     |     |     |     |     |
| --------- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
19 ----------------------------------------
| 20 Seleccione |     | cuantos |     | BP utilizar |     |     |     |     |     |
| ------------- | --- | ------- | --- | ----------- | --- | --- | --- | --- | --- |
| 21 INPUT:     |     | 2       |     |             |     |     |     |     |     |
----------------------------------------
22
| H’aanit |     | ataca |     |     |     |     |     |     |     |
| ------- | --- | ----- | --- | --- | --- | --- | --- | --- | --- |
23
| Meep | recibe |     | 373 de | daño | de tipo | Axe |     |     |     |
| ---- | ------ | --- | ------ | ---- | ------- | --- | --- | --- | --- |
24
| Meep | recibe |     | 373 de | daño | de tipo | Axe |     |     |     |
| ---- | ------ | --- | ------ | ---- | ------- | --- | --- | --- | --- |
25
| 26 Meep | recibe  |     | 373 de | daño   | de tipo | Axe |     |     |     |
| ------- | ------- | --- | ------ | ------ | ------- | --- | --- | --- | --- |
| 27 Meep | termina |     | con    | HP:189 |         |     |     |     |     |
5

Se puede dar la situación donde la unidad objetivo muera cuando aún quedan ataques por realizar. En estos
casos el programa deberá mostrar de igual forma cada uno de los ataques y el daño que hubiera realizado
de forma normal. Supongamos para este ejemplo que Meep tiene HP:500 y H’aanit realiza un ataque con 2
BP.
1 ----------------------------------------
| 2 H’aanit     | ataca  |                  |     |
| ------------- | ------ | ---------------- | --- |
| 3 Meep recibe | 373 de | daño de tipo Axe |     |
| Meep recibe   | 373 de | daño de tipo Axe |     |
4
| Meep recibe | 373 de | daño de tipo Axe |     |
| ----------- | ------ | ---------------- | --- |
5
| Meep termina | con HP:0 |     |     |
| ------------ | -------- | --- | --- |
6
Aunque en el segundo ataque Meep ya estaba muerto, se debe mostrar el tercer ataque de igual manera,
| donde H’aanit | perderá | 2 BP. |     |
| ------------- | ------- | ----- | --- |
En caso de ingresar un número mayor al BP del personaje, se deberá informar y volver a solicitar el INPUT.
Para el siguiente ejemplo supongamos que nos encontramos en la ronda 1, por lo que H’aanit posee 1 BP
----------------------------------------
1
| Seleccione | cuantos | BP utilizar |     |
| ---------- | ------- | ----------- | --- |
2
| INPUT: | 3   |     |     |
| ------ | --- | --- | --- |
3
4 ----------------------------------------
| 5 H’aanit | no tiene 3 | BP para utilizar |     |
| --------- | ---------- | ---------------- | --- |
6 ----------------------------------------
| 7 Seleccione | cuantos | BP utilizar |     |
| ------------ | ------- | ----------- | --- |
8 INPUT:
Se puede presentar el caso en donde una bestia entra en Breaking Point durante un ataque con múltiples
golpes. En este caso se deberá mostrar cada golpe, indicando el momento exacto en donde se produce el
| Breaking | Point y respetando | los daños | de cada golpe. |
| -------- | ------------------ | --------- | -------------- |
----------------------------------------
1
| 2 Turno de  | H’aanit   |     |     |
| ----------- | --------- | --- | --- |
| 3 1: Ataque | básico    |     |     |
| 4 2: Usar   | habilidad |     |     |
5 3: Defender
6 4: Huir
| INPUT: | 1   |     |     |
| ------ | --- | --- | --- |
7
----------------------------------------
8
| Seleccione | un arma |     |     |
| ---------- | ------- | --- | --- |
9
1: Axe
10
11 2: Bow
12 3: Cancelar
| 13 INPUT: | 2   |     |     |
| --------- | --- | --- | --- |
14 ----------------------------------------
| 15 Seleccione | un objetivo    | para H’aanit |     |
| ------------- | -------------- | ------------ | --- |
| 1: Meep       | - HP:1016/1016 | Shields:2    |     |
16
2: Cancelar
17
| INPUT: | 1   |     |     |
| ------ | --- | --- | --- |
18
----------------------------------------
19
| 20 Seleccione | cuantos | BP utilizar |     |
| ------------- | ------- | ----------- | --- |
| 21 INPUT:     | 2       |             |     |
22 ----------------------------------------
| 23 H’aanit     | ataca       |                  |               |
| -------------- | ----------- | ---------------- | ------------- |
| 24 Meep recibe | 108 de      | daño de tipo Bow | con debilidad |
| 25 Meep recibe | 108 de      | daño de tipo Bow | con debilidad |
| Meep entra     | en Breaking | Point            |               |
26
| Meep recibe | 144 de | daño de tipo Bow | con debilidad |
| ----------- | ------ | ---------------- | ------------- |
27
| Meep termina | con HP:656 |     |     |
| ------------ | ---------- | --- | --- |
28
6

Vemos que Meep inicia con 2 Shields, por tanto, luego del segundo golpe con debilidad se muestra inme-
diatamente el mensaje de Breaking Point, mientras que el tercer golpe ya muestra el daño considerando el
| multiplicador |     | por Breaking |     | Point. |        |     |     |
| ------------- | --- | ------------ | --- | ------ | ------ | --- | --- |
| Habilidades   |     | con          | más | de     | un hit |     |     |
De la misma manera que los ataques básicos con Boost Points se anuncian tantas veces como ataques
se realicen, para las habilidades con más de un hit se deberá anunciar tantas veces como hits se realicen,
| adicionalmente |     | se debe | anunciar |     | el tipo de ataque | que se está | efectuando: |
| -------------- | --- | ------- | -------- | --- | ----------------- | ----------- | ----------- |
----------------------------------------
1
| Turno | de Cyrus |     |     |     |     |     |     |
| ----- | -------- | --- | --- | --- | --- | --- | --- |
2
| 1: Ataque | básico |     |     |     |     |     |     |
| --------- | ------ | --- | --- | --- | --- | --- | --- |
3
| 4 2: Usar | habilidad |     |     |     |     |     |     |
| --------- | --------- | --- | --- | --- | --- | --- | --- |
5 3: Defender
6 4: Huir
| 7 INPUT: | 2   |     |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- |
8 ----------------------------------------
| 9 Seleccione |       | una habilidad |     | para | Cyrus |     |     |
| ------------ | ----- | ------------- | --- | ---- | ----- | --- | --- |
| 1: Fire      | Storm |               |     |      |       |     |     |
10
2: Cancelar
11
| INPUT: | 1   |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- |
12
----------------------------------------
13
| 14 Seleccione |     | cuantos | BP  | utilizar |     |     |     |
| ------------- | --- | ------- | --- | -------- | --- | --- | --- |
| 15 INPUT:     | 0   |         |     |          |     |     |     |
16 ----------------------------------------
| 17 Cyrus       | usa Fire | Storm  |      |         |      |     |     |
| -------------- | -------- | ------ | ---- | ------- | ---- | --- | --- |
| 18 Meep recibe |          | 472 de | daño | de tipo | Fire |     |     |
| Meep recibe    |          | 472 de | daño | de tipo | Fire |     |     |
19
| Shaggy | Meep | recibe | 502 | de daño | de tipo Fire |     |     |
| ------ | ---- | ------ | --- | ------- | ------------ | --- | --- |
20
| Shaggy | Meep | recibe | 502 | de daño | de tipo Fire |     |     |
| ------ | ---- | ------ | --- | ------- | ------------ | --- | --- |
21
| War Wolf | recibe | 463 | de  | daño | de tipo Fire |     |     |
| -------- | ------ | --- | --- | ---- | ------------ | --- | --- |
22
| 23 War Wolf     | recibe  | 463     | de          | daño  | de tipo Fire |     |     |
| --------------- | ------- | ------- | ----------- | ----- | ------------ | --- | --- |
| 24 Meep termina |         | con     | HP:72       |       |              |     |     |
| 25 Shaggy       | Meep    | termina | con         | HP:32 |              |     |     |
| 26 War Wolf     | termina |         | con HP:2622 |       |              |     |     |
| Restauración    |         | de      | HP y        | SP    |              |     |     |
La habilidad HP Thief realiza dos hits, para luego restaurará una cantidad de HP proporcional al daño
realizó. Para ellos se deben mostrar los dos hits de forma normal y luego mostrar la cantidad de HP
| restaurada | por        | el total | de  | los dos | hits. |     |     |
| ---------- | ---------- | -------- | --- | ------- | ----- | --- | --- |
| Turno      | de Therion |          |     |         |       |     |     |
1
| 1: Ataque | básico |     |     |     |     |     |     |
| --------- | ------ | --- | --- | --- | --- | --- | --- |
2
| 2: Usar | habilidad |     |     |     |     |     |     |
| ------- | --------- | --- | --- | --- | --- | --- | --- |
3
3: Defender
4
5 4: Huir
| 6 INPUT: | 2   |     |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- |
7 ----------------------------------------
| 8 Seleccione |       | una habilidad |     | para | Therion |     |     |
| ------------ | ----- | ------------- | --- | ---- | ------- | --- | --- |
| 9 1: HP      | Thief |               |     |      |         |     |     |
10 2: Cancelar
| INPUT: | 1   |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- |
11
----------------------------------------
12
| Seleccione |     | un objetivo |     | para | Therion |     |     |
| ---------- | --- | ----------- | --- | ---- | ------- | --- | --- |
13
| 1: Meep | - HP:898/1016 |     |     | Shields:2 |     |     |     |
| ------- | ------------- | --- | --- | --------- | --- | --- | --- |
14
15 2: Cancelar
| 16 INPUT: | 1   |     |     |     |     |     |     |
| --------- | --- | --- | --- | --- | --- | --- | --- |
17 ----------------------------------------
| 18 Seleccione |     | cuantos | BP  | utilizar |     |     |     |
| ------------- | --- | ------- | --- | -------- | --- | --- | --- |
7

19 INPUT: 0
20 ----------------------------------------
21 Therion usa HP Thief
22 Meep recibe 172 de daño de tipo Dagger
23 Meep recibe 172 de daño de tipo Dagger
24 Therion recupera 172 de vida
25 Meep termina con HP:554
26 Therion termina con HP:4093
Por otro lado, la habilidad Steal SP posee un efecto similar, pero restaurando SP, el output debe seguir la
misma lógica, mostrando ambos hits, y la cantidad restaurada de SP.
1 Seleccione una habilidad para Therion
2 1: Steal SP
3 2: Night Ode
4 3: Cancelar
5 INPUT: 1
6 ----------------------------------------
7 Seleccione un objetivo para Therion
8 1: Mossy Meep - HP:772/1060 Shields:3
9 2: Cancelar
10 INPUT: 1
11 ----------------------------------------
12 Seleccione cuantos BP utilizar
13 INPUT: 0
14 ----------------------------------------
15 Therion usa Steal SP
16 Mossy Meep recibe 144 de daño de tipo Dagger
17 Mossy Meep recibe 144 de daño de tipo Dagger
18 Therion recupera 14 SP
19 Mossy Meep termina con HP:484
Habilidades de buff, debuff y estados
Toda habilidad que tenga un efecto con duración por rondas, se deberá anunciar la cantidad de rondas que
dura el efecto:
1 ----------------------------------------
2 Turno de Primrose
3 1: Ataque básico
4 2: Usar habilidad
5 3: Defender
6 4: Huir
7 INPUT: 2
8 ----------------------------------------
9 Seleccione una habilidad para Primrose
10 1: Lion Dance
11 2: Cancelar
12 INPUT: 1
13 ----------------------------------------
14 Seleccione un objetivo para Primrose
15 1: Primrose - HP:4002/4002 SP:504/504 BP:2
16 2: Tressa - HP:4689/4891 SP:357/357 BP:2
17 3: Cancelar
18 INPUT: 2
19 ----------------------------------------
20 Seleccione cuantos BP utilizar
21 INPUT: 0
22 ----------------------------------------
23 Primrose usa Lion Dance
24 Tressa tendrá Increased Physical Attack durante 2 rondas
8

Este último mensaje varía según el efecto. Para el caso de Buffs y Debuffs se debe mostrar un mensaje
| como | el del ejemplo |     | anterior. | Otros | buffs | podrían verse | de la siguiente | forma |
| ---- | -------------- | --- | --------- | ----- | ----- | ------------- | --------------- | ----- |
1 ----------------------------------------
| 2 Primrose | tendrá | Decreased |     | Elemental |     | Defense | durante 2 rondas |     |
| ---------- | ------ | --------- | --- | --------- | --- | ------- | ---------------- | --- |
3 ----------------------------------------
| 4 Olberic | tendrá | Increased |     | Physical | Attack | durante | 4 rondas |     |
| --------- | ------ | --------- | --- | -------- | ------ | ------- | -------- | --- |
5 ----------------------------------------
| H’aanit | tendrá | Increased |     | Speed | durante | 4 rondas |     |     |
| ------- | ------ | --------- | --- | ----- | ------- | -------- | --- | --- |
6
| Habilidades |     | divinas |     |     |     |     |     |     |
| ----------- | --- | ------- | --- | --- | --- | --- | --- | --- |
Dado que se requieren 3 Boost Points para utilizarlas, estas solo se mostrarán a la hora de seleccionar una
habilidad si el usuario tiene 3 o más BP y al momento de seleccionarse no se preguntará cuanto BP usar,
| usando | los 3 | BP de | forma | automática. |     |     |     |     |
| ------ | ----- | ----- | ----- | ----------- | --- | --- | --- | --- |
Silahabilidadtieneunefectodedañoocuracióninmediatoseanunciacomocualquierotrahabilidadofensiva
o de curación:
1 ----------------------------------------
| Turno | de Olberic |     |     |     |     |     |     |     |
| ----- | ---------- | --- | --- | --- | --- | --- | --- | --- |
2
| 1: Ataque | básico |     |     |     |     |     |     |     |
| --------- | ------ | --- | --- | --- | --- | --- | --- | --- |
3
| 2: Usar | habilidad |     |     |     |     |     |     |     |
| ------- | --------- | --- | --- | --- | --- | --- | --- | --- |
4
3: Defender
5
6 4: Huir
| 7 INPUT: | 2   |     |     |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- | --- |
8 ----------------------------------------
| 9 Seleccione  | una     | habilidad |     |     |     |     |     |     |
| ------------- | ------- | --------- | --- | --- | --- | --- | --- | --- |
| 10 1: Brand’s | Thunder |           |     |     |     |     |     |     |
11 2: Cancelar
| INPUT: | 1   |     |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- | --- |
12
----------------------------------------
13
| Seleccione | un  | objetivo |     |     |     |     |     |     |
| ---------- | --- | -------- | --- | --- | --- | --- | --- | --- |
14
| 1: Snow | Fox |     |     |     |     |     |     |     |
| ------- | --- | --- | --- | --- | --- | --- | --- | --- |
15
16 2: Cancelar
| 17 INPUT: | 1   |     |     |     |     |     |     |     |
| --------- | --- | --- | --- | --- | --- | --- | --- | --- |
18 ----------------------------------------
| 19 Olberic | usa         | Brand’s | Thunder |      |         |       |     |     |
| ---------- | ----------- | ------- | ------- | ---- | ------- | ----- | --- | --- |
| 20 Snow    | Fox recibe  | 2581    | de      | daño | de tipo | Sword |     |     |
| Snow       | Fox termina | con     | HP:19   |      |         |       |     |     |
21
| Habilidades |     | pasivas |     |     |     |     |     |     |
| ----------- | --- | ------- | --- | --- | --- | --- | --- | --- |
Patience
Estahabilidadpasivatienecomoefectootorgarunturnoadicionalalfinaldelarondasisecumplequetanto
el HP como el SP actual de la unidad son pares. En caso de cumplir la condición se deberá mostrar un
mensaje indicando que la unidad obtiene un turno adicional y luego se mostrará el turno nuevo de la forma
habitual:
1 ----------------------------------------
| 2 H’aanit | obtiene | un  | turno | adicional |     |     |     |     |
| --------- | ------- | --- | ----- | --------- | --- | --- | --- | --- |
----------------------------------------
3
| Equipo | del jugador |     |     |     |     |     |     |     |
| ------ | ----------- | --- | --- | --- | --- | --- | --- | --- |
4
| A-H’aanit | -   | HP:4226/4447 |     | SP:362/369 |     | BP:1 |     |     |
| --------- | --- | ------------ | --- | ---------- | --- | ---- | --- | --- |
5
| B-Z’aanta | -   | HP:3222/3222 |     | SP:371/371 |     | BP:0 |     |     |
| --------- | --- | ------------ | --- | ---------- | --- | ---- | --- | --- |
6
| 7 Equipo | del enemigo |              |     |           |     |     |     |     |
| -------- | ----------- | ------------ | --- | --------- | --- | --- | --- | --- |
| 8 A-War  | Wolf -      | HP:2730/3548 |     | Shields:2 |     |     |     |     |
9 ----------------------------------------
| 10 Turnos | de la | ronda |     |     |     |     |     |     |
| --------- | ----- | ----- | --- | --- | --- | --- | --- | --- |
9

1.H’aanit
11
12 ----------------------------------------
| 13 Turnos | de la siguiente |     | ronda |     |
| --------- | --------------- | --- | ----- | --- |
14 1.H’aanit
15 2.Z’aanta
| 16 3.War | Wolf |     |     |     |
| -------- | ---- | --- | --- | --- |
17 ----------------------------------------
| Turno | de H’aanit |     |     |     |
| ----- | ---------- | --- | --- | --- |
18
| 1: Ataque | básico |     |     |     |
| --------- | ------ | --- | --- | --- |
19
| 2: Usar | habilidad |     |     |     |
| ------- | --------- | --- | --- | --- |
20
3: Defender
21
22 4: Huir
23 INPUT:
En caso de que más de una unidad posee la habilidad Patience, estos utilizarán su turno según el orden
que tengan en el tablero, sin importar las reglas de ordenamiento de cola. Para anunciarlo se debe mostrar
el mensaje de turno extra para cada unidad que active la habilidad y la cola de turnos debe reflejar todos
| los turnos | extras: |     |     |     |
| ---------- | ------- | --- | --- | --- |
1 ----------------------------------------
| 2 Castti | obtiene un | turno | adicional |     |
| -------- | ---------- | ----- | --------- | --- |
3 ----------------------------------------
| Agnea | obtiene un turno |     | adicional |     |
| ----- | ---------------- | --- | --------- | --- |
4
----------------------------------------
5
| Equipo | del jugador |     |     |     |
| ------ | ----------- | --- | --- | --- |
6
| A-Partitio | - HP:4891/4891 |     | SP:314/314 | BP:2 |
| ---------- | -------------- | --- | ---------- | ---- |
7
| 8 B-Castti | - HP:5204/5336 |           | SP:426/426 | BP:0 |
| ---------- | -------------- | --------- | ---------- | ---- |
| 9 C-Agnea  | - HP:4002/4002 |           | SP:504/504 | BP:2 |
| 10 Equipo  | del enemigo    |           |            |      |
| 11 A-Meep  | - HP:556/1016  | Shields:2 |            |      |
12 ----------------------------------------
| 13 Turnos | de la ronda |     |     |     |
| --------- | ----------- | --- | --- | --- |
1.Castti
14
2.Agnea
15
----------------------------------------
16
| Turnos | de la siguiente |     | ronda |     |
| ------ | --------------- | --- | ----- | --- |
17
18 1.Agnea
19 2.Castti
20 3.Partitio
21 4.Meep
Notar que en el ejemplo Agnea tiene mayor velocidad que Castti, pero los turnos adicionales siguen única-
| mente | el orden del tablero. |     |     |     |
| ----- | --------------------- | --- | --- | --- |
Encore
Esta habilidad permite a una unidad revivir la primera vez que muere. Cuando esta habilidad se activa, se
debe indicar que la unidad ha revivido y luego mostrar su HP final de la forma habitual.
1 ----------------------------------------
| 2 Equipo     | del jugador    |     |            |      |
| ------------ | -------------- | --- | ---------- | ---- |
| 3 A-Primrose | - HP:324/4002  |     | SP:504/504 | BP:5 |
| 4 Equipo     | del enemigo    |     |            |      |
| 5 A-Mattias? | - HP:5780/5780 |     | Shields:4  |      |
----------------------------------------
6
| Turnos | de la ronda |     |     |     |
| ------ | ----------- | --- | --- | --- |
7
1.Mattias?
8
----------------------------------------
9
| Turnos | de la siguiente |     | ronda |     |
| ------ | --------------- | --- | ----- | --- |
10
11 1.Primrose
12 2.Mattias?
13 ----------------------------------------
10

| Mattias? |     | usa | Black | Gale |     |     |     |
| -------- | --- | --- | ----- | ---- | --- | --- | --- |
14
| 15 Primrose |     | recibe  | 613 | de daño | elemental |     |     |
| ----------- | --- | ------- | --- | ------- | --------- | --- | --- |
| 16 Primrose |     | revive  |     |         |           |     |     |
| 17 Primrose |     | termina | con | HP:1000 |           |     |     |
Encasoquelaunidadrecibamásdeungolpe,sedebeanunciarquelaunidadreviveluegoderecibirelgolpe
| letal, | si  | aún quedan |     | golpes pendientes |     | los recibirá de forma | normal |
| ------ | --- | ---------- | --- | ----------------- | --- | --------------------- | ------ |
1 ----------------------------------------
| 2 Equipo     |     | del jugador |                |     |            |      |     |
| ------------ | --- | ----------- | -------------- | --- | ---------- | ---- | --- |
| 3 A-Primrose |     | -           | HP:324/4002    |     | SP:504/504 | BP:5 |     |
| 4 Equipo     |     | del enemigo |                |     |            |      |     |
| 5 A-Forest   |     | Fox         | - HP:1380/1380 |     | Shields:2  |      |     |
----------------------------------------
6
| Turnos |     | de la | ronda |     |     |     |     |
| ------ | --- | ----- | ----- | --- | --- | --- | --- |
7
| 1.Forest |     | Fox |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- |
8
----------------------------------------
9
| 10 Turnos |     | de la | siguiente | ronda |     |     |     |
| --------- | --- | ----- | --------- | ----- | --- | --- | --- |
11 1.Primrose
| 12 2.Forest |     | Fox |     |     |     |     |     |
| ----------- | --- | --- | --- | --- | --- | --- | --- |
13 ----------------------------------------
| 14 Forest |     | Fox usa | Double | Bite    |        |     |     |
| --------- | --- | ------- | ------ | ------- | ------ | --- | --- |
| Primrose  |     | recibe  | 398    | de daño | físico |     |     |
15
| Primrose |     | revive |     |     |     |     |     |
| -------- | --- | ------ | --- | --- | --- | --- | --- |
16
| Primrose |     | recibe | 398 | de daño | físico |     |     |
| -------- | --- | ------ | --- | ------- | ------ | --- | --- |
17
| Primrose |     | termina | con | HP:602 |     |     |     |
| -------- | --- | ------- | --- | ------ | --- | --- | --- |
18
| Cálculos |     |     | del | combate |     |     |     |
| -------- | --- | --- | --- | ------- | --- | --- | --- |
Tal como indica el enunciado general del proyecto, los cálculos de daño pueden generar números decimales.
Cuandoelloocurre,hayquetruncarelnúmeroasuenteromásbajo.EstosepuederealizarenC#utilizando
la función Math.Floor(...). Luego el resultado puede ser convertido a entero con Convert.ToInt32(...).
Rúbrica
Esta entrega tiene puntaje por funcionalidad y por limpieza de código. El puntaje por funcionalidad es en
baseadescuentos.Esdecir,separtecon6puntosysedescuentaenbasealporcentajedetestsquenopasen
| de  | cada | batería     | de tests. | Los          | descuentos | son:             |                     |
| --- | ---- | ----------- | --------- | ------------ | ---------- | ---------------- | ------------------- |
|     | [    | -3.0 puntos |           | ] Porcentaje | de test    | cases no pasados | en E1-BasicCombat.  |
|     | [    | -0.7 puntos |           | ] Porcentaje | de test    | cases no pasados | en E1-InvalidTeams. |
[ -2.3 puntos ] Porcentaje de test cases no pasados en E1-RandomBasicCombat
|     | [   | -1.0 puntos |     | ] Porcentaje | de test | cases no pasados | en E2-BeastsSkills. |
| --- | --- | ----------- | --- | ------------ | ------- | ---------------- | ------------------- |
[ -1.0 puntos ] Porcentaje de test cases no pasados en E2-DefendAndBreakingPoint.
[ -1.0 puntos ] Porcentaje de test cases no pasados en E2-OffensiveSkills.
[ -1.0 puntos ] Porcentaje de test cases no pasados en E2-HealingAndQueueSkills.
[ -1.0 puntos ] Porcentaje de test cases no pasados en E2-BaseStatsPassives.
|     | [   | -0.4 puntos |     | ] Porcentaje | de test | cases no pasados | en E2-Mix.    |
| --- | --- | ----------- | --- | ------------ | ------- | ---------------- | ------------- |
|     | [   | -0.4 puntos |     | ] Porcentaje | de test | cases no pasados | en E2-Random. |
[ -0.1 puntos ] Porcentaje de test cases no pasados en E3-BasicAttackBoosting.
11

[ -0.05 puntos ] Porcentaje de test cases no pasados en E3-BasicAttackBoostingRandom.
| [ -0.1 | puntos | ] Porcentaje |     | de test | cases | no pasados | en  | E3-BasicPassives. |
| ------ | ------ | ------------ | --- | ------- | ----- | ---------- | --- | ----------------- |
[ -0.05 puntos ] Porcentaje de test cases no pasados en E3-BasicPassivesRandom.
[ -0.15 puntos ] Porcentaje de test cases no pasados en E3-MultiHitOffensive.
[ -0.15 puntos ] Porcentaje de test cases no pasados en E3-BasicSkillBoosting.
[ -0.5 puntos ] Porcentaje de test cases no pasados en E3-AdvanceSkillBoosting.
| [ -0.5 | puntos | ] Porcentaje |     | de test | cases | no pasados | en  | E3-StatusEffects. |
| ------ | ------ | ------------ | --- | ------- | ----- | ---------- | --- | ----------------- |
| [ -0.5 | puntos | ] Porcentaje |     | de test | cases | no pasados | en  | E3-BeastsSkills.  |
| [ -0.5 | puntos | ] Porcentaje |     | de test | cases | no pasados | en  | E3-DivineSkills.  |
[ -0.5 puntos ] Porcentaje de test cases no pasados en E3-IntermediatePassives.
| [ -0.1 | puntos | ] Porcentaje |       | de test      | cases   | no pasados | en        | E3-Mix.    |
| ------ | ------ | ------------ | ----- | ------------ | ------- | ---------- | --------- | ---------- |
| [ -0.1 | puntos | ] Porcentaje |       | de test      | cases   | no pasados | en        | E3-Random. |
| [ -2.5 | puntos | ] La         | vista | con interfaz | gráfica | no         | funciona. |            |
Aligualqueelpuntajeporfuncionalidad,elpuntajeporlimpiezadecódigotambiénesenbaseadescuentos.
| Los descuentos | máximos |      | por capítulo |                | son los | siguientes: |             |       |
| -------------- | ------- | ---- | ------------ | -------------- | ------- | ----------- | ----------- | ----- |
| [ -1.0         | puntos  | ] No | sigue        | los principios |         | del cap.    | 2 de clean  | code. |
| [ -2.0         | puntos  | ] No | sigue        | los principios |         | del cap.    | 3 de clean  | code. |
| [ -2.0         | puntos  | ] No | sigue        | los principios |         | del cap.    | 6 de clean  | code. |
| [ -1.0         | puntos  | ] No | sigue        | los principios |         | del cap.    | 7 de clean  | code. |
| [ -1.0         | puntos  | ] No | sigue        | los principios |         | del cap.    | 8 de clean  | code. |
| [ -2.0         | puntos  | ] No | sigue        | los principios |         | del cap.    | 10 de clean | code. |
| [ -1.0         | puntos  | ] No | implementa   |                | MVC.    |             |             |       |
Adicionalmente, el puntaje por limpieza de código tendrá una bonificación, la cual será proporcional el
desempeño en ciertos capítulos. Las bonificaciones máximas por capítulo son las siguientes:
| [ +0.25 | puntos | ]       | Sigue | los principios | del | cap.   | 4 de clean | code. |
| ------- | ------ | ------- | ----- | -------------- | --- | ------ | ---------- | ----- |
| [ +0.5  | puntos | ] Sigue | los   | principios     | del | cap. 5 | de clean   | code. |
Finalmente, tu nota final será igual al promedio geométrico entre el puntaje por funcionalidad y el puntaje
√
por limpieza de código (más el punto base), donde el promedio geométrico entre x e y es igual a xy. En
| caso de | que x o y | sean negativos, |     | tu nota | será | un 1,0. |     |     |
| ------- | --------- | --------------- | --- | ------- | ---- | ------- | --- | --- |
Por ejemplo, si tienes 3 puntos por funcionalidad y 5 puntos por limpieza de código entonces tu nota será
√
3·5+1 = 4,9. Pero si tienes 6 puntos en funcionalidad y solo 1 punto en limpieza de código entonces tu
√
| nota será | 6·1+1=3,5. |     |     |     |     |     |     |     |
| --------- | ---------- | --- | --- | --- | --- | --- | --- | --- |
Importante: Sólo está permitido modificar el archivo Tests.cs del proyecto Octopath-Traveler.Tests
para agregar los test cases nuevos. Ninguna otra forma de modificación sobre este proyecto está permitida.
Hacerlo puede conllevar una penalización que dependerá de la gravedad de la situación.
12

Bonus
Estaentregacontaráconunbonusquepuedepermitirquetupuntajedefuncionalidadseamayora6puntos.
| Para obtener | este | puntaje | debes | implementar | las siguientes | habilidades: |
| ------------ | ---- | ------- | ----- | ----------- | -------------- | ------------ |
Curación
[–,30,–,Party,1], Ethereal Healing: Por 2 rondas, cada viajero recuperará un 10% de su vida
máxima luego de jugar un turno. [Aumenta la duración en 1 ronda por cada BP]
Divinas
[-,30,-,Ally,1], Aelfric’s Auspices: Selecciona a un aliado. Durante 3 rondas las habilidades de
| ese | aliado se | activarán | 2 veces. | No  | afecta a las habilidades | divinas. |
| --- | --------- | --------- | -------- | --- | ------------------------ | -------- |
[-,30,-,Ally,1], Sealticge’s Seduction: Selecciona a un aliado. Durante 3 rondas sus habilidades
| con         | target Single | tendrán |     | target |     |     |
| ----------- | ------------- | ------- | --- | ------ | --- | --- |
| Habilidades | pasivas       |         |     |        |     |     |
[User], Saving Grace: Otorga a la unidad la habilidad de ser curado por sobre el HP máximo.
[Enemy],Second Serving:SilastatHPespar,launidadrealizaráunataqueextraensuturno.Solo
| aplica | para ataques |     | básicos. | .   |     |     |
| ------ | ------------ | --- | -------- | --- | --- | --- |
[User], Elemental Edge: El usuario tendrá Increased Elemental Attack y Increased
| Elemental |     | Defense | durante | todo | el juego. |     |
| --------- | --- | ------- | ------- | ---- | --------- | --- |
[User], Physical Prowess: El usuario tendrá Increased Physical Attack y Increased
| Physical | Defense |     | durante | todo | el juego. |     |
| -------- | ------- | --- | ------- | ---- | --------- | --- |
[Enemies], Intimidation: Si al inicio del combate, el viajero que se encuentra en la primera posición
deltableroposeeHPySPpares,entoncestodoslosenemigosiniciaráncon Decreased Elemental
Defense y Decreased Physical Defense durante 2 rondas. El efecto no es acumulable en caso
| de  | que más | de una | unidad | posea la | habilidad. |     |
| --- | ------- | ------ | ------ | -------- | ---------- | --- |
Existen2gruposdetestsquepruebanqueestashabilidadesfuncionen.Lostestsysubonuscorrespondiente
| en el puntaje | de funcionalidad |              | son: |         |               |                    |
| ------------- | ---------------- | ------------ | ---- | ------- | ------------- | ------------------ |
| [ +0.6        | puntos           | ] Porcentaje |      | de test | cases pasados | en E3-Bonus.       |
| [ +0.4        | puntos           | ] Porcentaje |      | de test | cases pasados | en E3-BonusRandom. |
Ten en cuenta que el código que escribas para implementar las habilidades del bonus será considerado para
determinar tu puntaje por limpieza de código y podría generar descuentos en esa categoría.
| Output | del | juego | (Bonus) |     |     |     |
| ------ | --- | ----- | ------- | --- | --- | --- |
Algunas de las habilidades muestran mensajes propios, similar a los buff y debuff, describiendo brevemente
| el efecto   | que tendrán | durante | las | rondas | en la que esté | activo. |
| ----------- | ----------- | ------- | --- | ------ | -------------- | ------- |
| Ethereal    | Healing     |         |     |        |                |         |
| 1 Turno de  | Ophilia     |         |     |        |                |         |
| 2 1: Ataque | básico      |         |     |        |                |         |
| 2: Usar     | habilidad   |         |     |        |                |         |
3
13

3: Defender
4
5 4: Huir
| 6 INPUT: | 2   |     |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- |
7 ----------------------------------------
| 8 Seleccione  |     | una habilidad |     | para | Ophilia |     |     |
| ------------- | --- | ------------- | --- | ---- | ------- | --- | --- |
| 9 1: Ethereal |     | Healing       |     |      |         |     |     |
10 2: Cancelar
| INPUT: | 1   |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- |
11
----------------------------------------
12
| Seleccione |     | cuantos | BP utilizar |     |     |     |     |
| ---------- | --- | ------- | ----------- | --- | --- | --- | --- |
13
| INPUT: | 0   |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- |
14
15 ----------------------------------------
| 16 Ophilia  | usa         | Ethereal     | Healing |     |            |          |     |
| ----------- | ----------- | ------------ | ------- | --- | ---------- | -------- | --- |
| 17 Olberic  | tendrá      | restauración |         | de  | HP durante | 2 rondas |     |
| 18 Ophilia  | tendrá      | restauración |         | de  | HP durante | 2 rondas |     |
| Aelfric’s   | Auspices    |              |         |     |            |          |     |
| 1 Turno     | de Primrose |              |         |     |            |          |     |
| 2 1: Ataque |             | básico       |         |     |            |          |     |
| 3 2: Usar   | habilidad   |              |         |     |            |          |     |
4 3: Defender
5 4: Huir
| 6 INPUT: | 2   |     |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- |
----------------------------------------
7
| Seleccione |     | una habilidad |     | para | Primrose |     |     |
| ---------- | --- | ------------- | --- | ---- | -------- | --- | --- |
8
| 1: Aelfric’s |     | Auspices |     |     |     |     |     |
| ------------ | --- | -------- | --- | --- | --- | --- | --- |
9
2: Cancelar
10
| 11 INPUT: | 1   |     |     |     |     |     |     |
| --------- | --- | --- | --- | --- | --- | --- | --- |
12 ----------------------------------------
| 13 Seleccione  |     | un objetivo    |     | para Primrose |     |      |     |
| -------------- | --- | -------------- | --- | ------------- | --- | ---- | --- |
| 14 1: Primrose |     | - HP:3426/4002 |     | SP:504/504    |     | BP:3 |     |
| 15 2: Ophilia  |     | - HP:3222/4002 |     | SP:462/470    |     | BP:3 |     |
3: Cancelar
16
| INPUT: | 2   |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- |
17
----------------------------------------
18
| Primrose | usa | Aelfric’s |     | Auspices |     |     |     |
| -------- | --- | --------- | --- | -------- | --- | --- | --- |
19
| 20 Ophilia  | activará    | sus       | habilidades |     | 2   | veces durante | 3 rondas |
| ----------- | ----------- | --------- | ----------- | --- | --- | ------------- | -------- |
| Sealticge’s |             | Seduction |             |     |     |               |          |
| Turno       | de Primrose |           |             |     |     |               |          |
1
| 1: Ataque |     | básico |     |     |     |     |     |
| --------- | --- | ------ | --- | --- | --- | --- | --- |
2
| 2: Usar | habilidad |     |     |     |     |     |     |
| ------- | --------- | --- | --- | --- | --- | --- | --- |
3
3: Defender
4
5 4: Huir
| 6 INPUT: | 2   |     |     |     |     |     |     |
| -------- | --- | --- | --- | --- | --- | --- | --- |
7 ----------------------------------------
| 8 Seleccione     |     | una habilidad |     | para | Primrose |     |     |
| ---------------- | --- | ------------- | --- | ---- | -------- | --- | --- |
| 9 1: Sealticge’s |     | Seduction     |     |      |          |     |     |
2: Cancelar
10
| INPUT: | 1   |     |     |     |     |     |     |
| ------ | --- | --- | --- | --- | --- | --- | --- |
11
----------------------------------------
12
| Seleccione |     | un objetivo |     | para Primrose |     |     |     |
| ---------- | --- | ----------- | --- | ------------- | --- | --- | --- |
13
| 14 1: Primrose |     | - HP:3185/4002 |     | SP:504/504 |     | BP:3 |     |
| -------------- | --- | -------------- | --- | ---------- | --- | ---- | --- |
| 15 2: Cyrus    | -   | HP:3075/3557   |     | SP:568/582 |     | BP:3 |     |
16 3: Cancelar
| 17 INPUT: | 2   |     |     |     |     |     |     |
| --------- | --- | --- | --- | --- | --- | --- | --- |
18 ----------------------------------------
| Primrose | usa | Sealticge’s |     | Seduction |     |     |     |
| -------- | --- | ----------- | --- | --------- | --- | --- | --- |
19
| Cyrus | modificará | sus | targets | durante |     | 3 rondas |     |
| ----- | ---------- | --- | ------- | ------- | --- | -------- | --- |
20
14

| Aelfric’s | Auspices |     |     |
| --------- | -------- | --- | --- |
La habilidad Aelfric’s Auspices otorga la capacidad de utilizar dos veces ciertas habilidades. Para esto el
output se verá como si la habilidad tuviera 2 veces su efecto. Vemos un ejemplo en donde Ophilia posee el
| efecto | de Aelfric’s Auspices. |     |     |
| ------ | ---------------------- | --- | --- |
1 ----------------------------------------
| 2 Turno     | de Ophilia |     |     |
| ----------- | ---------- | --- | --- |
| 3 1: Ataque | básico     |     |     |
| 4 2: Usar   | habilidad  |     |     |
3: Defender
5
4: Huir
6
| INPUT: | 2   |     |     |
| ------ | --- | --- | --- |
7
----------------------------------------
8
| 9 Seleccione | una habilidad | para Ophilia |     |
| ------------ | ------------- | ------------ | --- |
10 1: Fireball
11 2: Cancelar
| 12 INPUT: | 1   |     |     |
| --------- | --- | --- | --- |
13 ----------------------------------------
| 14 Seleccione | cuantos | BP utilizar |     |
| ------------- | ------- | ----------- | --- |
| INPUT:        | 0       |             |     |
15
----------------------------------------
16
| Ophilia | usa Fireball |     |     |
| ------- | ------------ | --- | --- |
17
| Meep | recibe 316 de | daño de tipo | Fire |
| ---- | ------------- | ------------ | ---- |
18
| 19 Meep   | recibe 316 de     | daño de tipo | Fire         |
| --------- | ----------------- | ------------ | ------------ |
| 20 Shaggy | Meep recibe       | 346 de daño  | de tipo Fire |
| 21 Shaggy | Meep recibe       | 346 de daño  | de tipo Fire |
| 22 Mossy  | Meep recibe 0     | de daño de   | tipo Fire    |
| 23 Mossy  | Meep recibe 0     | de daño de   | tipo Fire    |
| Meep      | termina con HP:68 |              |              |
24
| Shaggy | Meep termina | con HP:0 |     |
| ------ | ------------ | -------- | --- |
25
| Mossy | Meep termina | con HP:1060 |     |
| ----- | ------------ | ----------- | --- |
26
Esta habilidad, al permitir ejecutar dos veces una habilidad, solo genera todo el efecto de nuevo, es decir,
cualquier decisión que se haya tomado durante la primera ejecución de la habilidad, se mantendrá en la
segunda ejecución. Veamos un ejemplo con la habilidad Nightmare Chimera.
1 ----------------------------------------
| Turno | de Ochette |     |     |
| ----- | ---------- | --- | --- |
2
| 1: Ataque | básico |     |     |
| --------- | ------ | --- | --- |
3
| 2: Usar | habilidad |     |     |
| ------- | --------- | --- | --- |
4
3: Defender
5
6 4: Huir
| 7 INPUT: | 2   |     |     |
| -------- | --- | --- | --- |
8 ----------------------------------------
| 9 Seleccione    | una habilidad | para Ochette |     |
| --------------- | ------------- | ------------ | --- |
| 10 1: Nightmare | Chimera       |              |     |
2: Cancelar
11
| INPUT: | 1   |     |     |
| ------ | --- | --- | --- |
12
----------------------------------------
13
| Seleccione | un arma |     |     |
| ---------- | ------- | --- | --- |
14
1: Sword
15
16 2: Spear
17 3: Dagger
18 4: Axe
19 5: Bow
20 6: Stave
7: Cancelar
21
| INPUT: | 5   |     |     |
| ------ | --- | --- | --- |
22
----------------------------------------
23
| Seleccione | un objetivo | para Ochette |     |
| ---------- | ----------- | ------------ | --- |
24
| 25 1: Meep | - HP:1016/1016 | Shields:2 |     |
| ---------- | -------------- | --------- | --- |
26 2: Cancelar
| 27 INPUT: | 1   |     |     |
| --------- | --- | --- | --- |
15

----------------------------------------
28
| 29 Seleccione | cuantos | BP utilizar |     |     |
| ------------- | ------- | ----------- | --- | --- |
| 30 INPUT:     | 0       |             |     |     |
31 ----------------------------------------
| 32 Agnea       | usa Nightmare | Chimera      |                   |     |
| -------------- | ------------- | ------------ | ----------------- | --- |
| 33 Meep recibe | 566 de        | daño de tipo | Bow con debilidad |     |
| 34 Meep recibe | 566 de        | daño de tipo | Bow con debilidad |     |
| Meep termina   | con           | HP:0         |                   |     |
35
Para el caso de las habilidades con efecto de revivir, este no se puede ejecutar sobre una unidad viva, por
lo tanto, no se ejecutará dos veces ese efecto, sin embargo se podría repetir un efecto secundario. Veamos el
| caso con | la habilidad | Vivify la cual | revive a una unidad | y luego la cura. |
| -------- | ------------ | -------------- | ------------------- | ---------------- |
1 ----------------------------------------
| 2 Turno     | de Ophilia |     |     |     |
| ----------- | ---------- | --- | --- | --- |
| 3 1: Ataque | básico     |     |     |     |
| 4 2: Usar   | habilidad  |     |     |     |
5 3: Defender
4: Huir
6
| INPUT: | 2   |     |     |     |
| ------ | --- | --- | --- | --- |
7
----------------------------------------
8
| Seleccione | una habilidad | para Ophilia |     |     |
| ---------- | ------------- | ------------ | --- | --- |
9
10 1: Vivify
11 2: Cancelar
| 12 INPUT: | 1   |     |     |     |
| --------- | --- | --- | --- | --- |
13 ----------------------------------------
| 14 Seleccione | un objetivo | para Ophilia |      |     |
| ------------- | ----------- | ------------ | ---- | --- |
| 1: Primrose   | - HP:0/4002 | SP:474/504   | BP:2 |     |
15
2: Cancelar
16
| INPUT: | 1   |     |     |     |
| ------ | --- | --- | --- | --- |
17
----------------------------------------
18
| 19 Seleccione | cuantos | BP utilizar |     |     |
| ------------- | ------- | ----------- | --- | --- |
| 20 INPUT:     | 0       |             |     |     |
21 ----------------------------------------
| 22 Ophilia  | usa Vivify |             |     |     |
| ----------- | ---------- | ----------- | --- | --- |
| 23 Primrose | revive     |             |     |     |
| Primrose    | recupera   | 598 de vida |     |     |
24
| Primrose | recupera | 598 de vida |     |     |
| -------- | -------- | ----------- | --- | --- |
25
| Primrose | termina | con HP:1197 |     |     |
| -------- | ------- | ----------- | --- | --- |
26
Podemos ver que el efecto de revivir solo se aplica una vez, sin embargo la curación se efectua dos veces.
16