/*
 Ütközés
Egy játéktáblán a 0. időegységben L bábu van. Mindegyiket elindítjuk valamerre. Egy
időegység alatt mindegyik a neki megfelelő távolságra mozdul el, a tábla szélére érve
megállnak. Lehetséges, hogy előbb-utóbb két bábu összeütközik: ugyanarra a helyre lépnének
vagy átlépnének egymáson.
Készítsen programot, amely megadja, hogy K időegységen belül mikor ütközik legelőször
két bábu!
A tabla.be szöveges állomány első sorában a játéktábla sorai és oszlopai száma
(1≤N,M<100), a bábuk száma (1<L≤10) és az időtartam (1<K<100 000) van, egyetlen
szóközzel elválasztva. A következő L sor egy-egy bábu leírását tartalmazza: a kezdő helyét
(1≤S≤N, 1≤0,M) és mozgás irányát (X₁,€{F,L,J,B} – fel, le, jobbra, balra), egy-egy
szóközzel elválasztva.
A tabla.ki szöveges állomány egyetlen sorába az első ütközés időpontját kell írni! Ha
K időegységen belül nincs ütközés, akkor -1-et kell kiírni!

tabla.be:
7 10 3 100
4 3 J
2 6 F
4 8 B

tabla.ki:
3

Magyarázat: A bábuk helyzete időegységenként:
1: (4,4), (1,6), (4,7)
2: (4,5), (1,6), (4,6)
3: (4,6), (1,6), (4,5)

A 3. időegységre az 1-es és a 3-as bábu egymáson átlépett volna, azaz a 3.
időegységben ütköztek. 
 */



using Utkozes;

string path = "./tabla.be";
string[] data = File.ReadAllLines(path);

int rowCount = int.Parse(data[0].Split(' ')[0]);
int colCount = int.Parse(data[0].Split(' ')[1]);
bool[,] board = new bool[rowCount, colCount];
List<Pawn> pawns = GetPawns(data);
PopulateBoard(board, pawns);
int tick = CalculateTick(rowCount, colCount, int.Parse(data[0].Split(' ')[3]));

//itt ugye figyeljünk, hogy a count egytől kezdődik, mert amíg 0. index van, 0. kör nincs..
int count = 1;
int collisonTick = 0;

// kezdő helyzet kirajzolása na jó, valamilyen értelemben azért még is van 0. kör.. 
DrawBoard(board, 0, tick);

while (count <= tick)
{

    foreach (Pawn pawn in pawns)
    {
        if (pawn.CanMove)
        {
            Position nextPos = pawn.NexPosition();
            bool withinBoundary = WithinBoundary(nextPos, board);
            bool canMove = false;
            bool isTheNextTileOccupied = false;

            if (withinBoundary)
            {
                canMove = true;
                isTheNextTileOccupied = board[nextPos.X, nextPos.Y];
            }

            // megnézzük, hogy tud-e mozogni és a leendő helye üres-e. ha igen, akkor lép a bábú,
            // ha viszont nem, akkor megnézzük, hogy miért nem. ha már nem tud mozogni, akkor 
            // megadjuk, hogy ő már mozgásképtelen, hogy legközelebb ebbe az if-be ne lépjünk
            // be vele. ha pedig azért nem tudna mozogni, mert a leendő helye már foglalt,
            // akkor ütközne, tehát megvan az ütközés, amit vártunk. ilyenkor ezt
            // az értéket lementjük, a countot tick+1-re állítva pedig kilépünk a while ciklusból.
            // használhatnánk simán break-et is, és akkor a count értéke lenne az ütközés tickjének
            // száma, valamiért egyetemen nem szerették, hogy ha a diákok break-et használnak
            // de én azt mondom, hogy ha tudod mit csinálsz power to you.
            if (canMove && !isTheNextTileOccupied) 
            {
                // aktuális pozíció, ahol most vagyunk legyen false, mozgás után pedig 
                // az akkorra aktuális pozíciónk legyen true, így jelöljük ugye a térképen a mozgást
                board[pawn.Position.X, pawn.Position.Y] = false;    
                pawn.Move();
                board[pawn.Position.X, pawn.Position.Y] = true;
                // lépés kirajzolása
                DrawBoard(board, count, tick);
            }
            else if (!canMove) pawn.CanMove = false;
            else if (isTheNextTileOccupied && collisonTick == 0)
            {
                DrawBoard(board, count,tick, pawn.Position, nextPos);
                collisonTick = count;
                count = tick;
            }
        }        
    }
    count++;
}



Console.WriteLine(collisonTick > 0 ?
    $"Collision happened on tick {collisonTick}" : "No collision happened.");
File.WriteAllText("tabla.ki", collisonTick.ToString());

Console.ReadLine();

//----------------------------------------------------------------------------------------------------------

int CalculateTick(int rowCount, int colCount, int tick)
{
    // nincs értelme annak, hogy a játékidő (tick) nagyobb legyen, mint a pályánkon maximálisan megtehető
    // lépések száma, szóval a ticket nyugodtan redukálhatjuk a sor-oszlop valamelyikének értékére. 
    // azéra, amelyik a nagyobb. Merugye ha valaki 0,0 pozícióban kezd (bal felső sarok)
    // és lefelé halad, mikor 5 sor van, 5 tick alatt megteszi ezt a távot, utána hiába van még mondjuk 95 tick, akkor 
    // már nem mozog, aki pedig mondjuk az 1,3 ponton kezd és ugyanúgy lefelé tart már az 5. tickben sem mozgott. 
    // hasonló okoskodás révén rájöhetünk, hogy az oszlopoknál is ugyanez a helyzet
    if (tick > rowCount || tick > colCount)
        tick = Math.Max(rowCount, colCount);
    return tick;
}

void PopulateBoard(bool[,] board, List<Pawn> pawns)
{

    // bábúkat a táblára helyezzük, feltételezve, hogy nincs egyből "ütközés", mivel hülyeség lenne,
    // ha két bábú kezdetben ugyanazon a helyen lenne. igazából nem nagy dolog hozzáadni egy ilyen
    // checket, ha akarod dobd hozzá, legalább gyakorolsz
    foreach (Pawn pawn in pawns)
    {
        board[pawn.Position.X, pawn.Position.Y] = true;
    }
}

void DrawBoard(bool[,] board, int currentTick, int maxTick, Position currentPos = null, Position collisionPos = null)
{
    Console.Clear();
    for (int i = 0; i < board.GetLength(0); i++)
    {
        for (int j = 0; j < board.GetLength(1); j++)
        {
            string value = "";
            if (currentPos is not null && i == currentPos.X && j == currentPos.Y)
                value = "-";
            else if (collisionPos is not null && i == collisionPos.X && j == collisionPos.Y)
                value = "x";
            else value = board[i, j] ? "1" : "-";
            Console.Write($"{value} ");
        }
        Console.WriteLine();
    }
    Console.WriteLine($"Current tick: {currentTick}\t Max tick: {maxTick}");
    Console.ReadKey();
}

bool WithinBoundary(Position position,bool[,] board)
{
    // alapból a bool-okat false értékkel szoktuk deklarálni és utána változtatjuk true-ra,
    // ha egy adott feltételnek megfelel, de lássuk be, hogy jelen esetben érthetőbb az az
    // olvasat, hogy lelépne a tábláról, mint az, hogy még benne van, ezért megfordul a 
    // csekkolás. Ennek simán az az oka, hogy így olvashatóbb és az többet ér. Ha akarjuk,
    // lehetne alapból false és minden feltételt tagadnánk az if-ben, majd ott állítanánk
    // true-ra, de az nehezebben olvasható kódot eredményez.
    bool isValid = true;

    if (position.X < 0 || position.X == board.GetLength(0)
        || position.Y < 0 || position.Y == board.GetLength(1))
        isValid = false;
    return isValid;
    
}

List<Pawn> GetPawns(string[] data)
{
    List<Pawn> pawns = new List<Pawn>();
    for (int i = 1; i < data.Length; i++)
    {
        string[] values = data[i].Split(' ');
        int x = int.Parse(values[0]);
        int y = int.Parse(values[1]);
        Position position = new Position() 
        { 
            X = x, 
            Y = y 
        };

        // ez a rövidebb verzió, mikor egy ilyen switchet alkalmazunk, ha kezdőbarátabb, 
        // érthetőbb módszert akarsz, akkor a Pawn osztályban lévő switchhez hasonlóan itt is meg lehet oldani, vagy
        // csak simán stringként adod be a konstruktorba és ott hívod rá a direction csekkolós metódust, csak mivel nem az osztály 
        // dolga, hogy a bekapott adatot magának elfogadható módra formálja, hanem nekünk a dolgunk, hogy alapból olyan
        // adatot adjunk be neki ezért nem csináltam úgy.
        Direction direction = values[2] switch
        {
            "F" => Direction.Fel,
            "L" => Direction.Le,
            "J" => Direction.Jobb,
            "B" => Direction.Bal
        };
        Pawn pawn = new Pawn(position, direction);
        pawns.Add(pawn);
    }
    return pawns;
}

