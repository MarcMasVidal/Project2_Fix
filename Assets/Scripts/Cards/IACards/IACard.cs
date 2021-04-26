using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class IACard : MonoBehaviour
{
    [Serializable]
    public struct CardType
    {
        public Card card;
        public int priority;
    }

    public List<CardType> IADeck;//deck hecho por nosotros
    public List<Card> IAHand; //de 0 a 6 cartas q tendr� en la mano


    public Whiskas _whiskas;

    private int _whiskasCombinationAccumulate = 0;
    private int _cardStatsAccumulates = 0;
    private int _currentWhiskas => _whiskas.currentWhiskas;

    public LinkedList<string> qLinkedList = new LinkedList<string>();
    public List<List<int>> _combinations = new List<List<int>>(); //remover publicas
    public List<int> _valueCardStats = new List<int>(); //remover publicas

    //control de la visualizaci�n de las cartas de la  IA
    public Transform IAHandCanvas;
    public float scale = 1f;
    public GameObject prefabBackCard;
    private float maxCardsInHand = 6;
    public List<GameObject> cardsInHand = new List<GameObject>();
    private Transform cardInstance;
    //-----------------------------

    public Tilemap FloorTileMap;
    public Tilemap CollisionTileMap;
    private List<GameObject> mapList = new List<GameObject>();
    private void Start()
    {
        for (int i = 0; i < IAHand.Count; i++)
        {
            AddCardHand();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && IAHand.Count < maxCardsInHand)
            RandomCardChosen();
        if (Input.GetKeyDown(KeyCode.I))
        {
            RemoveCardHand();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            CombinationCard(UnitList());
            BestCombination();
            
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SpawnCard(0);
        }
    }

    private void RemoveCardHand()
    {
        Destroy(cardsInHand[cardsInHand.Count - 1]);
        cardsInHand.RemoveAt(cardsInHand.Count - 1);
    }
    private void AddCardHand()
    {
        cardInstance = Instantiate(prefabBackCard, IAHandCanvas.position, Quaternion.identity).transform;

        cardInstance.SetParent(IAHandCanvas);
        cardInstance.localScale = new Vector3(scale, scale, scale);//escalamos las cartas que se ven en la mano.
        cardsInHand.Add(cardInstance.gameObject);
    }
    /*
    SE ELIGEN (DOS) CARTAS ALEATORIAS
    DEL DECK (NO DE LA MANO).
    */
    private void RandomCardChosen()
    {
        int random1 = UnityEngine.Random.Range(0, IADeck.Count);
        int random2 = UnityEngine.Random.Range(0, IADeck.Count);

        while (random1 == random2)
        {
            random2 = UnityEngine.Random.Range(0, IADeck.Count);
        }
        IAHand.Add(ComproveHand(random1, random2));
        AddCardHand();
    }
    /*
    LA FUNCI�N REVISA SI LAS (DOS) CARTAS
    QUE ROBA A LA VEZ LA IA LAS TIENE O NO PARA
    NO REPETIR EN PRIMER LUGAR.

    SI UNA DE LAS (DOS) CARTAS NO LAS TIENE,
    COGER� LA QUE NO TIENE.

    EN CASO DE QUE SE REPITAN (AMBAS), 
    SELECCIONAR� LA CARTA CON MAYOR 
    PRIORIDAD
    */
    private Card ComproveHand(int random1, int random2)//comprueba que cartas tiene la IA en su mano.
    {
        bool _firstCardRepe = false;
        bool _secondCardRepe = false;
        //miramos en la mano cuales tiene.
        for (int i = 0; i < IAHand.Count; i++)
        {
            if (IAHand[i].name == IADeck[random1].card.name) //si el nombre es diferente =>  no la tiene| coge esta y no comprueba las otras.
            {
                // print("PRIMERA repe " + IADeck[random1].card.name);
                _firstCardRepe = true;
            }
            else if (IAHand[i].name == IADeck[random2].card.name) //si el nombre es diferente =>  no la tiene
            {
                // print("SEGUNDA repe " + IADeck[random2].card.name);
                _secondCardRepe = true;
            }
        }
        if (!_firstCardRepe)
            return IADeck[random1].card;
        else if (!_secondCardRepe)
            return IADeck[random2].card;
        else //sistema de prioridad.
        {
            if (IADeck[random1].priority < IADeck[random2].priority)
                return IADeck[random2].card;
            else
                return IADeck[random1].card;
        }
    }
    /*
    COMPRUEBA SI LA MANO DE LA IA TIENE
    ALG�N HECHIZO. SI ES AS� LO GUARDAR�
    PARA LUEGO COMPROBAR CUAL ES EL HECHIZO
    CON MAYOR PRIORIDAD PARA AS�, FACILITAR
    LA POSICI�N EN LA QUE SE ENCUENTRA EL 
    HECHIZO.
    */
    private bool IAHasSpells()
    {
        int maxPriority = 0;//prioridad mas alta
        int spellPriorityPos = 0; //pos 0 por default.
        int sCounter = 0;

        List<Spells> spellsList = new List<Spells>();
        //vamos a revisar que cartas de hechizo hay y cual es la mayor prioridad.
        for (int i = 0; i < IAHand.Count; i++)
        {
            if (IAHand[i] is Spells)
            {
                spellsList.Add(IAHand[i] as Spells);
                
                if (spellsList[sCounter].Priority > maxPriority)
                {
                    maxPriority = spellsList[sCounter].Priority;
                    spellPriorityPos = i;
                    sCounter++;
                }
            }
        }
        print(spellsList.Count);
        return spellsList.Count != 0;
    }

    /*
    ESTA FUNCI�N REALIZA LAS COMBINACIONES
    POSIBLES POR LA IA MEDIANTE EL MAN�(WHISKAS)
    DISPONIBLE.
    */
    private void CombinationCard(List<Card> list)
    {
        _combinations = new List<List<int>>();
        _valueCardStats = new List<int>();
        qLinkedList = new LinkedList<string>();

        // Create an empty queue of strings
        int lenghtHand = list.Count;
        // Enqueue the first binary number
        qLinkedList.AddLast("1");

        // This loops is like BFS of a tree
        // with 1 as root 0 as left child
        // and 1 as right child and so on
        float e = Mathf.Pow(2, lenghtHand) - 1;
        while (e-- > 0)
        {
            List<int> tempList = new List<int>();
            List<Unit> tempUnit = new List<Unit>();
            // print the front of queue
            string s1 = qLinkedList.First.Value;
            qLinkedList.RemoveFirst();
            int count = 0;
            _cardStatsAccumulates = 0;//e
            for (int i = s1.Length - 1; i >= 0; i--)
            {
                if (s1[i] == '1')
                {
                    _whiskasCombinationAccumulate += list[count].Whiskas;
                    tempUnit.Add(list[count] as Unit);
                    
                    _cardStatsAccumulates += tempUnit[tempUnit.Count - 1].Health + tempUnit[tempUnit.Count - 1].Power + tempUnit[tempUnit.Count - 1].Whiskas + 1;

                    tempList.Add(count);//a�adimos posici�n
                }

                print(list[count].name);
                count++;
            }
            print(s1 + ":posiciones |  mana total:     " + _whiskasCombinationAccumulate);

            if (_whiskasCombinationAccumulate <= _currentWhiskas)
            {
                print("a�adiendome");
                _combinations.Add(tempList);//a�adimos la combinaci�n con su posici�n
                _valueCardStats.Add(_cardStatsAccumulates); // su valor
            }

            _whiskasCombinationAccumulate = 0;

            // Store s1 before changing it
            string s2 = s1;

            // Append "0" to s1 and enqueue it
            qLinkedList.AddLast(s1 + "0");

            // Append "1" to s2 and enqueue it.
            // Note that s2 contains the previous front
            qLinkedList.AddLast(s2 + "1");
        }

    }

    private List<Card> UnitList()
    {
        List<Card> e = new List<Card>();
        for (int i = 0; i < IAHand.Count; i++)
        {
            if (IAHand[i] is Unit)
            {
                e.Add(IAHand[i]);
            }
        }
        return e;

    }
    /*
    ESTA FUNCI�N REALIZA LA COMBINACI�N
    QUE M�S PUNTOS DE STATS CONTIENE.

    STATS: SUMA DE VALORES.
    SUMA VIDA, WHISKAS Y PODER DE TODAS 
    LAS CARTAS DE LA COMBINACI�N, M�S LA CANTIDAD
    DE CARTAS QUE TIENE LA COMBINACI�N.

    (EJ: SI SON 3 CARTAS = +3)
    */
    private int BestCombination()//elige la carta con mayor stats independientemente del valor del mana (siempre < o = que current)
    {
        int max = 0;
        int maxPos = 0;

        for (int i = 0; i < _valueCardStats.Count; i++)
        {
            if (_valueCardStats[i] > max)
            {
                max = _valueCardStats[i];
                maxPos = i;

            }
        }
        return maxPos;
    }

    private void SpawnCard(int e)
    {
        List<Vector3> posSpawnTile = new List<Vector3>();

        Vector2 tilePos = new Vector2(UnityEngine.Random.Range(4, 6), UnityEngine.Random.Range(-3, 3));

        print("cy: " + FloorTileMap.cellSize.y + "cx: " + FloorTileMap.cellSize.x);



        GameObject theTile = Instantiate(IAHand[0].GetComponent<Unit>().sprite, tilePos, Quaternion.identity);
        theTile.transform.localPosition = new Vector3(tilePos.x + (FloorTileMap.cellSize.x / 2), tilePos.y + (FloorTileMap.cellSize.y / 2), 0);
        mapList.Add(theTile);


        print("hay en el mapa:  " + mapList.Count);

    }
}