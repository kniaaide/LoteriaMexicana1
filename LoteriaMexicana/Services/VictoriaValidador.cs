using LoteriaMexicana.Domain;
using LoteriaMexicana.Domain.Enums;

namespace LoteriaMexicana.Services;

public static class VictoriaValidador
{
    public static bool EsVictoria(
        Tabla tabla,
        IReadOnlySet<int> marcadas,
        FormatoGanador formato,
        List<(int Fila, int Columna)>? patronCustom = null)
    {
        var idx = IndicesMarcados(tabla, marcadas);
        int f = tabla.Filas, c = tabla.Columnas;

        return formato switch
        {
            FormatoGanador.FilaCompleta => TieneLineaH(idx, f, c),
            FormatoGanador.ColumnaCompleta => TieneLineaV(idx, f, c),
            FormatoGanador.DiagonalCompleta => TieneDiagonal(idx, f, c),
            FormatoGanador.EsquinasCompletas => TieneEsquinas(idx, f, c),
            FormatoGanador.TablaLlena => idx.Count == tabla.TotalCasillas,
            FormatoGanador.ElleArriba => TieneElle(idx, f, c, arriba: true, espejo: false),
            FormatoGanador.ElleAbajo => TieneElle(idx, f, c, arriba: false, espejo: false),
            FormatoGanador.ElleEspejo => TieneElle(idx, f, c, arriba: true, espejo: true),
            FormatoGanador.CruzCentral => TieneCruzCentral(idx, f, c),
            FormatoGanador.FormaTee => TieneFormaTee(idx, f, c),
            FormatoGanador.MarcoCompleto => TieneMarco(idx, f, c),
            FormatoGanador.MarcoInterior => TieneMarcoInterior(idx, f, c),
            FormatoGanador.DosDiagonales => TieneDosDiagonales(idx, f, c),
            FormatoGanador.PrimeraCarta => idx.Count >= 1,
            FormatoGanador.TresFilas => ContarFilasCompletas(idx, f, c) >= 3,
            FormatoGanador.TresColumnas => ContarColumnasCompletas(idx, f, c) >= 3,
            FormatoGanador.TodasLasFormas => EsVictoriaEnCualquierForma(idx, f, c, tabla),
            _ => false
        };
    }

    // ── TodasLasFormas: gana con cualquiera de los formatos básicos ───────────
    private static bool EsVictoriaEnCualquierForma(HashSet<int> idx, int filas, int cols, Tabla tabla)
    {
        return TieneLineaH(idx, filas, cols)
            || TieneLineaV(idx, filas, cols)
            || TieneDiagonal(idx, filas, cols)
            || TieneEsquinas(idx, filas, cols)
            || idx.Count == tabla.TotalCasillas
            || TieneElle(idx, filas, cols, true, false)
            || TieneElle(idx, filas, cols, false, false)
            || TieneElle(idx, filas, cols, true, true)
            || TieneCruzCentral(idx, filas, cols)
            || TieneFormaTee(idx, filas, cols)
            || TieneMarco(idx, filas, cols)
            || TieneDosDiagonales(idx, filas, cols)
            || ContarFilasCompletas(idx, filas, cols) >= 3
            || ContarColumnasCompletas(idx, filas, cols) >= 3;
    }

    // =========================================================================
    // DETECCIÓN DE TRAMPA
    // =========================================================================
    public static IEnumerable<int> DetectarTrampa(
        Tabla tabla,
        IReadOnlySet<int> marcadas,
        IEnumerable<int> cantadasNumeros)
    {
        var setCantadas = cantadasNumeros.ToHashSet();
        var invalidas = new List<int>();

        for (int f = 0; f < tabla.Filas; f++)
            for (int c = 0; c < tabla.Columnas; c++)
            {
                var carta = tabla.Casillas[f, c];
                if (carta != null && marcadas.Contains(carta.Numero))
                    if (!setCantadas.Contains(carta.Numero))
                        invalidas.Add(carta.Numero);
            }

        return invalidas;
    }

    // =========================================================================
    // HELPERS: índices marcados
    // =========================================================================
    private static HashSet<int> IndicesMarcados(Tabla tabla, IReadOnlySet<int> marcadas)
    {
        var inds = new HashSet<int>();
        for (int f = 0; f < tabla.Filas; f++)
            for (int c = 0; c < tabla.Columnas; c++)
            {
                var carta = tabla.Casillas[f, c];
                if (carta != null && marcadas.Contains(carta.Numero))
                    inds.Add(Tabla.IndiceDe(f, c, tabla.Columnas));
            }
        return inds;
    }

    // =========================================================================
    // FORMAS
    // =========================================================================
    private static bool TieneLineaH(HashSet<int> idx, int filas, int cols)
    {
        for (int f = 0; f < filas; f++)
            if (Enumerable.Range(0, cols).Select(c => Tabla.IndiceDe(f, c, cols)).All(idx.Contains))
                return true;
        return false;
    }

    private static bool TieneLineaV(HashSet<int> idx, int filas, int cols)
    {
        for (int c = 0; c < cols; c++)
            if (Enumerable.Range(0, filas).Select(f => Tabla.IndiceDe(f, c, cols)).All(idx.Contains))
                return true;
        return false;
    }

    private static bool TieneDiagonal(HashSet<int> idx, int filas, int cols)
    {
        if (filas != cols) return false;
        return Enumerable.Range(0, filas).Select(i => Tabla.IndiceDe(i, i, cols)).All(idx.Contains)
            || Enumerable.Range(0, filas).Select(i => Tabla.IndiceDe(i, cols - 1 - i, cols)).All(idx.Contains);
    }

    private static bool TieneDosDiagonales(HashSet<int> idx, int filas, int cols)
    {
        if (filas != cols) return false;
        return Enumerable.Range(0, filas).Select(i => Tabla.IndiceDe(i, i, cols)).All(idx.Contains)
            && Enumerable.Range(0, filas).Select(i => Tabla.IndiceDe(i, cols - 1 - i, cols)).All(idx.Contains);
    }

    private static bool TieneEsquinas(HashSet<int> idx, int filas, int cols)
    {
        int[] esquinas =
        [
            Tabla.IndiceDe(0,        0,        cols),
            Tabla.IndiceDe(0,        cols - 1, cols),
            Tabla.IndiceDe(filas - 1, 0,        cols),
            Tabla.IndiceDe(filas - 1, cols - 1, cols),
        ];
        return esquinas.All(idx.Contains);
    }

    /// <summary>
    /// Elle (L):
    ///   arriba=true  espejo=false → esquina superior-izquierda  (fila 0 + columna 0)
    ///   arriba=false espejo=false → esquina inferior-derecha    (fila N + columna N)
    ///   arriba=true  espejo=true  → esquina superior-derecha    (fila 0 + columna N)
    /// </summary>
    private static bool TieneElle(HashSet<int> idx, int filas, int cols, bool arriba, bool espejo)
    {
        int fila = arriba ? 0 : filas - 1;
        int col = espejo ? cols - 1 : 0;

        var casillas = new List<int>();
        // toda la fila de la esquina
        for (int c = 0; c < cols; c++) casillas.Add(Tabla.IndiceDe(fila, c, cols));
        // toda la columna de la esquina (sin repetir la esquina)
        for (int f = 0; f < filas; f++)
            if (f != fila) casillas.Add(Tabla.IndiceDe(f, col, cols));

        return casillas.All(idx.Contains);
    }

    private static bool TieneCruzCentral(HashSet<int> idx, int filas, int cols)
    {
        if (filas < 3 || filas != cols) return false;
        int mid = filas / 2;
        return Enumerable.Range(0, cols).Select(c => Tabla.IndiceDe(mid, c, cols)).All(idx.Contains)
            && Enumerable.Range(0, filas).Select(f => Tabla.IndiceDe(f, mid, cols)).All(idx.Contains);
    }

    /// <summary>FormaTee: fila superior completa + columna central completa.</summary>
    private static bool TieneFormaTee(HashSet<int> idx, int filas, int cols)
    {
        int midCol = cols / 2;
        return Enumerable.Range(0, cols).Select(c => Tabla.IndiceDe(0, c, cols)).All(idx.Contains)
            && Enumerable.Range(0, filas).Select(f => Tabla.IndiceDe(f, midCol, cols)).All(idx.Contains);
    }

    private static bool TieneMarco(HashSet<int> idx, int filas, int cols)
    {
        for (int f = 0; f < filas; f++)
            for (int c = 0; c < cols; c++)
                if ((f == 0 || f == filas - 1 || c == 0 || c == cols - 1)
                    && !idx.Contains(Tabla.IndiceDe(f, c, cols)))
                    return false;
        return true;
    }

    /// <summary>Marco interior: solo aplica 5×5 (el anillo de índices 1..3).</summary>
    private static bool TieneMarcoInterior(HashSet<int> idx, int filas, int cols)
    {
        if (filas < 4 || cols < 4) return false;
        for (int f = 1; f < filas - 1; f++)
            for (int c = 1; c < cols - 1; c++)
                if ((f == 1 || f == filas - 2 || c == 1 || c == cols - 2)
                    && !idx.Contains(Tabla.IndiceDe(f, c, cols)))
                    return false;
        return true;
    }

    private static int ContarFilasCompletas(HashSet<int> idx, int filas, int cols)
    {
        int count = 0;
        for (int f = 0; f < filas; f++)
            if (Enumerable.Range(0, cols).Select(c => Tabla.IndiceDe(f, c, cols)).All(idx.Contains))
                count++;
        return count;
    }

    private static int ContarColumnasCompletas(HashSet<int> idx, int filas, int cols)
    {
        int count = 0;
        for (int c = 0; c < cols; c++)
            if (Enumerable.Range(0, filas).Select(f => Tabla.IndiceDe(f, c, cols)).All(idx.Contains))
                count++;
        return count;
    }
}