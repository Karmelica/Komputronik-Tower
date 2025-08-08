# Przewodnik dla deweloperów - Komputronik Tower

Ten dokument zawiera szczegółowe informacje techniczne dla deweloperów pracujących nad projektem Komputronik Tower.

## 🏗️ Architektura systemu

### Wzorce projektowe

#### Singleton Pattern
- **HighScoreManager**: Zapewnia globalny dostęp do systemu rekordów
- **PoolingManager**: Zarządza poolem obiektów w całej grze

#### Object Pooling Pattern
- **SegmentScript**: Wszystkie segmenty wieży są poolowane
- **Optymalizacja**: Redukuje garbage collection i poprawia wydajność

#### Observer Pattern (pośrednio)
- **Input System**: Character reaguje na wydarzenia input
- **Collision System**: Różne komponenty reagują na kolizje

### Przepływ danych

```
Input System → Character → Physics → Score → HighScoreManager
              ↓
           SegmentGen → PoolingManager → SegmentScript
```

## 🎮 Systemy gry

### System ruchu (Character.cs)

```csharp
// Główne zmienne ruchu
[SerializeField] public float jumpForce = 5f;
[SerializeField] private float moveSpeed = 20f;

// Wall bounce system
[SerializeField] float bounceX = 5f;
[SerializeField] float bounceY = 0.5f;
```

**Kluczowe metody:**
- `OnMove()`: Obsługuje input ruchu i wall boost
- `OnJump()`: System skoków z velocity boost
- `OnCollisionEnter2D()`: Wall bounce mechanic
- `GiveVelocityBounce()`: Fizyka odbić

### System generacji poziomów (SegmentGen.cs)

```csharp
public float segmentHeight = 24f; // Wysokość między segmentami
private System.Random rng; // Pseudolosowa generacja
```

**Algorytm generacji:**
1. Trigger wykrywa opuszczenie segmentu przez gracza
2. Nowy segment jest tworzony na pozycji: `poprzedni + (0, segmentHeight, 0)`
3. Wykorzystuje Object Pooling dla wydajności
4. Inicjalizuje segment z pseudolosowymi parametrami

### System punktacji (Score.cs)

```csharp
// Wzór na punkty: (wysokość - poprzednia_wysokość) * mnożnik
multiplier = _playerRb2D.linearVelocity.magnitude * highestYPosition / 1000f;
multiplier = Mathf.Clamp(multiplier, 0.1f, 100f);
score += (player.transform.position.y - highestYPosition) * multiplier;
```

**Czynniki wpływające na punkty:**
- Wysokość (podstawa)
- Prędkość gracza (mnożnik)
- Aktualna pozycja Y (wpływ na mnożnik)

## 🔧 Konfiguracja Unity

### Warstwy (Layers)
- `Ground`: Platforma startowa i powierzchnie do lądowania
- `Wall`: Ściany do odbijania się
- `Player`: Warstwa gracza

### Tagi (Tags)
- `Player`: Gracz
- `Wall`: Ściany do odbijania
- `Segment`: Segmenty wieży do recycling

### Physics2D Settings
- **Gravity**: `Y = -9.81` (realistyczna grawitacja)
- **Bounce Threshold**: Dostosowany dla smooth bouncing
- **Collision Detection**: Continuous dla precyzyjnych kolizji

### Input System Configuration

```csharp
// Actions zdefiniowane w InputSystem_Actions.inputactions
- Move: A/D, Left/Right arrows
- Jump: Space, Xbox A
- Look: Mouse (nieużywane)
- Attack: Left Click (nieużywane)
```

## 🏗️ Struktura kodu

### Wzórce nazewnictwa

#### Zmienne
```csharp
// Publiczne SerializeField (dla Inspector)
[SerializeField] private float moveSpeed = 20f;

// Prywatne zmienne
private float _moveInput;
private bool _isGrounded = true;

// Stałe
private const string CURRENT_PLAYER_KEY = "CurrentPlayer";
```

#### Metody
```csharp
// Unity lifecycle
private void Awake() { }
private void Start() { }
private void Update() { }
private void FixedUpdate() { }

// Input handling
public void OnMove(InputAction.CallbackContext context) { }

// Collision handling
private void OnCollisionEnter2D(Collision2D other) { }
```

### Zarządzanie pamięcią

#### Object Pooling
```csharp
// Pobieranie z pool
var segment = PoolingManager.Instance.Get<SegmentScript>("Segment");

// Zwracanie do pool
PoolingManager.Instance.Return(segmentScript);
```

#### PlayerPrefs
```csharp
// Zapisywanie
PlayerPrefs.SetFloat($"HighScore_{playerEmail}", score);
PlayerPrefs.Save();

// Odczytywanie
float highScore = PlayerPrefs.GetFloat($"HighScore_{playerEmail}", 0f);
```

## 🧪 Debugowanie i testowanie

### Debug Tools

#### Gizmos (obecnie wyłączone)
```csharp
// W SegmentGen.cs - można odkomentować do debugowania
private void OnDrawGizmos()
{
    Gizmos.color = gizmoColor;
    Vector3 newSegmentPosition = previousSegment.transform.position + new Vector3(0, segmentHeight, 0);
    Gizmos.DrawWireCube(newSegmentPosition, gizmoDrawSize);
}
```

#### Console Logging
```csharp
Debug.LogError("No Rigidbody2D component found on the character.", this);
Debug.LogWarning("Email nie może być pusty!");
```

### Performance Profiling

**Kluczowe metryki do monitorowania:**
- **FPS**: Powinno być stabilne 60 FPS
- **Memory**: Object pooling powinien minimalizować GC
- **Physics**: Collision detection nie powinna powodować lag'ów

**Unity Profiler sekcje:**
- CPU Usage: Character.FixedUpdate(), SegmentGen
- Memory: PoolingManager effectiveness
- Rendering: URP pipeline performance

## 🔄 Workflow deweloperski

### Dodawanie nowych funkcji

1. **Planning**: Określ wpływ na istniejące systemy
2. **Implementation**: 
   - Dodaj nowe skrypty w `Assets/Scripts/`
   - Zachowaj istniejące wzorce nazewnictwa
   - Użyj SerializeField dla konfigurowalnych parametrów
3. **Testing**: Przetestuj w Unity Editor
4. **Integration**: Upewnij się, że nowe funkcje współgrają z:
   - Character movement system
   - Score system
   - Pooling system

### Code Style Guidelines

```csharp
// ✅ Dobrze
[SerializeField] private float jumpForce = 5f;
private bool _isGrounded = true;

// ❌ Źle
public float JumpForce = 5f;
private bool isGrounded = true;
```

### Performance Guidelines

1. **Używaj FixedUpdate() dla fizyki**
2. **Używaj Update() dla UI i input**
3. **Minimalizuj GameObject.Find() calls**
4. **Używaj object pooling dla często tworzonych obiektów**
5. **Cache component references w Awake()**

## 🐛 Częste problemy i rozwiązania

### Problem: Niestabilny wall bounce
**Przyczyna**: Collision detection timing
**Rozwiązanie**: Użyj `_preCollisionVelocity` i Continuous collision detection

### Problem: Memory leaks
**Przyczyna**: Nie zwracanie obiektów do pool
**Rozwiązanie**: Zawsze używaj `PoolingManager.Instance.Return()`

### Problem: Input lag
**Przyczyna**: Update() vs FixedUpdate() confusion
**Rozwiązanie**: Input handling w Update(), physics w FixedUpdate()

### Problem: Scoring inconsistency
**Przyczyna**: Floating point precision
**Rozwiązanie**: Użyj `ToString("F0")` dla display, zachowaj precision w logice

## 📚 Zasoby i dokumentacja

### Unity Documentation
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.7/manual/index.html)
- [Universal Render Pipeline](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
- [Physics2D](https://docs.unity3d.com/Manual/Physics2DReference.html)

### External Resources
- [Object Pooling Tutorial](https://unity.com/how-to/object-pooling-unity)
- [C# Coding Standards](https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity)

---

**Autor**: Karmelica  
**Ostatnia aktualizacja**: 2024-08-08