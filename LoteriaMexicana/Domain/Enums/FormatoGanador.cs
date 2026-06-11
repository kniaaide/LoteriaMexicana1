namespace LoteriaMexicana.Domain.Enums;

public enum FormatoGanador
{
    Ninguno,

    // Formas básicas
    FilaCompleta,          // Cualquier fila horizontal completa
    ColumnaCompleta,       // Cualquier columna vertical completa
    DiagonalCompleta,      // Cualquier diagonal (principal o secundaria)
    EsquinasCompletas,     // Las 4 esquinas marcadas
    TablaLlena,            // Toda la tabla marcada (lotería)

    // Formas en L / esquinas extendidas
    ElleArriba,            // L desde esquina superior izquierda
    ElleAbajo,             // L desde esquina inferior derecha
    ElleEspejo,            // L desde esquina superior derecha

    // Cruz / T
    CruzCentral,           // Fila y columna del centro se cruzan
    FormaTee,              // Fila superior + columna central

    // Marco / bordes
    MarcoCompleto,         // Todo el borde exterior marcado
    MarcoInterior,         // El borde interior (solo aplica 5×5)

    // Diagonales combinadas
    DosDiagonales,         // Ambas diagonales completas (X)

    // Especiales
    PrimeraCarta,          // Gana quien marque su primera carta cantada
    TresFilas,             // Tres filas completas cualesquiera
    TresColumnas,          // Tres columnas completas cualesquiera

    // Comodín
    TodasLasFormas,        // Se gana con CUALQUIERA de las formas anteriores
}