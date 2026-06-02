using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;

namespace LoteriaMexicana.Services;

public static class VictoriaValidador
{
    public static bool EsVictoria(Tabla tabla, IReadOnlySet<int> marcadas, FormatoGanador formato)
    {
        var idx = IndicesMarcados(tabla, marcadas);
        return formato switch
        {
            FormatoGanador.LineaHorizontal => TieneLineaH(idx),
            FormatoGanador.LineaVertical   => TieneLineaV(idx),
            FormatoGanador.Diagonal        => TieneDiagonal(idx),
            FormatoGanador.Cruz            => TieneCruz(idx),
            FormatoGanador.Cruzita         => TieneCruzita(idx),
            FormatoGanador.TablaLlena      => idx.Count == Tabla.TotalCasillas,
            _                              => false
        };
    }

    public static IEnumerable<int> DetectarTrampa(IReadOnlySet<int> marcadas, IEnumerable<int> cantadasNumeros)
    {
        var set = cantadasNumeros.ToHashSet();
        return marcadas.Where(n => !set.Contains(n));
    }

    private static bool TieneLineaH(HashSet<int> idx)
    {
        for (int f = 0; f < Tabla.Filas; f++)
            if (Enumerable.Range(0, Tabla.Columnas).Select(c => Tabla.IndiceDe(f, c)).All(idx.Contains))
                return true;
        return false;
    }

    private static bool TieneLineaV(HashSet<int> idx)
    {
        for (int c = 0; c < Tabla.Columnas; c++)
            if (Enumerable.Range(0, Tabla.Filas).Select(f => Tabla.IndiceDe(f, c)).All(idx.Contains))
                return true;
        return false;
    }

    private static bool TieneDiagonal(HashSet<int> idx)
        => Enumerable.Range(0, Tabla.Filas).Select(i => Tabla.IndiceDe(i, i)).All(idx.Contains)
        || Enumerable.Range(0, Tabla.Filas).Select(i => Tabla.IndiceDe(i, Tabla.Columnas - 1 - i)).All(idx.Contains);

    private static bool TieneCruz(HashSet<int> idx)
    {
        int mid = Tabla.Filas / 2;
        return Enumerable.Range(0, Tabla.Columnas).Select(c => Tabla.IndiceDe(mid, c)).All(idx.Contains)
            && Enumerable.Range(0, Tabla.Filas).Select(f => Tabla.IndiceDe(f, mid)).All(idx.Contains);
    }

    private static bool TieneCruzita(HashSet<int> idx)
    {
        for (int f = 1; f < Tabla.Filas - 1; f++)
            for (int c = 1; c < Tabla.Columnas - 1; c++)
            {
                int[] plus = [Tabla.IndiceDe(f,c), Tabla.IndiceDe(f-1,c), Tabla.IndiceDe(f+1,c),
                               Tabla.IndiceDe(f,c-1), Tabla.IndiceDe(f,c+1)];
                if (plus.All(idx.Contains)) return true;
            }
        return false;
    }

    private static HashSet<int> IndicesMarcados(Tabla tabla, IReadOnlySet<int> marcadas)
    {
        var res = new HashSet<int>();
        for (int f = 0; f < Tabla.Filas; f++)
            for (int c = 0; c < Tabla.Columnas; c++)
                if (tabla.Casillas[f, c] != null && marcadas.Contains(tabla.Casillas[f, c].Numero))
                    res.Add(Tabla.IndiceDe(f, c));
        return res;
    }
}
