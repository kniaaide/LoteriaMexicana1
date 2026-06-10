# Lotería Mexicana - Aplicación de Escritorio

Una aplicación completa en **C# .NET** para jugar Lotería Mexicana con soporte para múltiples jugadores, configuración personalizada, desempates y seguimiento de puntajes.

## Características Principales

### Mecánica del Juego
- **Tablas dinámicas**: Soporta tablas de 4x4 a 10x10
- **Cartas cantadas**: Sistema de historial con navegación hacia atrás y adelante
- **Validación en tiempo real**: Verifica cartas válidas vs inválidas
- **Detección de ganancias**: Múltiples formatos ganadores:
  - Línea horizontal
  - Línea vertical
  - Diagonales
  - Cruz (+ o X)
  - Cruzita (Plus)
  - Tabla llena

### Sistema de Desempate
- Cuando múltiples jugadores ganan simultáneamente
- Solo los ganadores pueden continuar en la siguiente ronda
- Fichas desactivadas para eliminados

### Gestión de Puntajes
- Seguimiento automático de victorias
- Rankings en tiempo real
- Resumen de múltiples rondas

### Configuración Personalizada
- Tamaño de tabla ajustable
- Permitir/prohibir cartas dobles
- Seleccionar formatos ganadores activos
- Crear tablas manualmente

### Interfaz Temática
- Diseño inspirado en la estética tradicional mexicana
- Colores: Rojo, verde, amarillo y tonos cálidos
- Fuentes elegantes (Georgia para títulos, Segoe UI para contenido)

## Estructura del Proyecto

```
LoteriaMexicana1/
├── Domain/
│   ├── Carta.cs                    # Modelo de cartas
│   ├── Tabla.cs                    # Grid de juego dinámico
│   ├── ConfiguracionJuego.cs       # Configuración personalizable
│   ├── HistorialCartas.cs          # Navegación de cartas cantadas
│   └── Enums/
│       └── FormatoGanador.cs       # Tipos de patrones ganadores
├── Services/
│   ├── ValidacionCartas.cs         # Verificación de cartas válidas
│   ├── ServicioDesempate.cs        # Lógica de desempates
│   └── GestorPuntajes.cs           # Seguimiento de puntajes
├── Forms/
│   ├── FormConfiguracion.cs        # Diálogo de configuración
│   ├── FormCrearTabla.cs           # Creador de tablas personalizado
│   └── (Otros formularios...)
├── LoteriaHub.cs                   # Orquestador central del juego
└── Program.cs                       # Punto de entrada
```

## Clases Principales

### LoteriaHub
Centro de control del juego que orquesta:
- Generación y validación de tablas
- Gestión del historial de cartas
- Verificación de ganancias
- Desempates y puntajes

**Métodos clave:**
```csharp
// Configuración
ActualizarConfiguracion(ConfiguracionJuego config)
GenerarTabla()
GenerarTablaPersonalizada(Tabla tabla)

// Cartas
CantarCarta(Carta carta)
ObtenerCartasCantadas()
RetrocederHistorial() / AvanzarHistorial()

// Validación
EsCartaValida(int numero)
ObtenerCartasValidas(HashSet<int> cartas)
ObtenerCartasInvalidas(HashSet<int> cartas)

// Desempate
RegistrarGanador(string nombre)
HayDesempate()
ActivarFichasParaDesempate(List<string> jugadores)

// Puntajes
ObtenerRanking()
ObtenerResumenPuntajes()
```

### ConfiguracionJuego
Define parámetros del juego:
- `TamañoTabla`: 4-10 (default: 5)
- `PermitirCartasDobles`: bool
- `FormatosActivos`: List<FormatoGanador>

### ValidacionCartas
Métodos estáticos para validar:
```csharp
EsValida(int numero, HashSet<int> cantadas)
ObtenerCartasValidas(HashSet<int> colocadas, HashSet<int> cantadas)
ObtenerCartasInvalidas(HashSet<int> colocadas, HashSet<int> cantadas)
```

### ServicioDesempate
Maneja múltiples ganadores:
```csharp
HayDesempate(List<string> ganadores) → bool
ObtenerMensajeDesempate(List<string> ganadores) → string
GenerarFichasActivas(List<string> ganadores, List<string> todos) → Dict
```

### GestorPuntajes
Seguimiento de victorias:
```csharp
RegistrarVictoria(string nombre)
ObtenerRanking() → List<(string, int)>
ObtenerResumen() → string
```

### HistorialCartas
Navegación por cartas cantadas:
```csharp
AgregarCarta(Carta carta)
Retroceder() / Avanzar()
ObtenerTodas() → List<Carta>
PosicionActual, PuedoRetroceder, PuedoAvanzar
```

### Tabla
Grid dinámico de cartas:
```csharp
Tabla.GenerarAleatoria(List<Carta> todas, int tamaño, bool dobles)
Tabla.Vacia(int tamaño)
ObtenerIndice(int fila, int columna) → int
```

## Ejemplo de Uso

```csharp
// Crear el hub del juego
var cartas = CargarCartasDisponibles(); // Tu fuente de cartas
var hub = new LoteriaHub(cartas);

// Configurar el juego
var config = hub.ObtenerConfiguracion();
config.TamañoTabla = 6;
config.PermitirCartasDobles = true;
hub.ActualizarConfiguracion(config);

// Agregar jugadores
hub.AgregarJugador("Juan");
hub.AgregarJugador("María");

// Cantar cartas
var carta = cartas[0];
hub.CantarCarta(carta);

// Verificar cartas válidas
var colocadas = new HashSet<int> { 1, 5, 12 };
var validas = hub.ObtenerCartasValidas(colocadas);
var invalidas = hub.ObtenerCartasInvalidas(colocadas);

// Registrar ganador
hub.RegistrarGanador("Juan");

// Verificar desempate
if (hub.HayDesempate())
{
    var mensaje = hub.ObtenerMensajeDesempate();
    MessageBox.Show(mensaje);
    
    hub.ActivarFichasParaDesempate(new List<string> { "Juan", "María" });
}

// Obtener ranking
var ranking = hub.ObtenerRanking();
Console.WriteLine(hub.ObtenerResumenPuntajes());
```

## Tema de Colores

| Elemento | Color | RGB |
|----------|-------|-----|
| Fondo | Amarillo cálido | 254, 243, 210 |
| Superficie | Blanco roto | 255, 255, 240 |
| Rojo primario | Rojo mexicano | 206, 17, 38 |
| Verde | Verde oscuro | 0, 104, 56 |
| Amarillo | Dorado | 240, 185, 11 |
| Texto primario | Marrón oscuro | 40, 20, 10 |
| Texto secundario | Marrón claro | 120, 80, 40 |

## Flujo de Desempate

```
1. Múltiples jugadores ganan (mismo patrón, mismo momento)
   └─> 2. LoteriaHub.RegistrarGanador() para cada uno
   └─> 3. HayDesempate() retorna true
   └─> 4. Mostrar mensaje: "Desempate! Juan, María ganaron..."
   └─> 5. ActivarFichasParaDesempate(todos los jugadores)
   └─> 6. EstaJugadorActivo(nombre) → true solo para ganadores
   └─> 7. Continuar ronda solo con activos
   └─> 8. RegistrarRonda() limpia estado
```

## Dependencias

- **.NET 6.0+**
- **Windows Forms** (incluido en .NET)

## Compilación y Ejecución

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run
```

## Enumeraciones

### FormatoGanador
```csharp
public enum FormatoGanador
{
    LineaHorizontal,
    LineaVertical,
    Diagonal,
    Cruz,
    Cruzita,
    TablaLlena
}
```

## Validación de Entrada

- **Tamaño de tabla**: 4-10 (rango permitido)
- **Cartas dobles**: Verificadas en generación de tabla
- **Jugadores**: No duplicados en lista
- **Cartas colocadas**: Validadas contra cantadas

## Notas de Desarrollo

- `ServicioDesempate` puede extenderse para desempates por mejor tiempo
- `ValidacionCartas` es completamente estático (sin estado)
- `GestorPuntajes` puede integrase con base de datos
- Los formularios usan construcción manual de UI (sin Designer)

## Documentación Adicional

Cada clase contiene comentarios XML (`///`) con descripción de métodos públicos.

## Autor

Creado para Lotería Mexicana - Sistema de Juego Digital

---

**Versión**: 1.0.0
**Última actualización**: 2026-06-10
