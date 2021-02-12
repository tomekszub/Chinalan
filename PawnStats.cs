using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[System.Serializable]
public class PawnStats
{
    // koszt ruchu na danych terenach tablica[0]-baza [1]-trawa [2]-piasek [3]-snieg
    public float[] terrainTypeMovements = { 1, 0.5f, 1.5f, 1.5f };
    public PawnStats()
    {

    }
}

