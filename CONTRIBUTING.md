# Jak przyczynić się do rozwoju Komputronik Tower

Dziękujemy za zainteresowanie projektem! Każdy wkład jest cenny i pomoże uczynić grę jeszcze lepszą.

## 🚀 Jak zacząć

### 1. Przygotowanie środowiska
```bash
# Fork repozytorium na GitHubie
# Następnie sklonuj swój fork lokalnie
git clone https://github.com/TWÓJ_USERNAME/Komputronik-Tower.git
cd Komputronik-Tower

# Dodaj repozytorium główne jako upstream
git remote add upstream https://github.com/Karmelica/Komputronik-Tower.git
```

### 2. Otwórz projekt w Unity
- Unity 6000.1.10f1 lub nowszy
- Wszystkie zależności zostaną automatycznie pobrane

## 📋 Typy wkładu

### 🐛 Zgłaszanie błędów
Przed zgłoszeniem sprawdź, czy błąd nie został już zgłoszony w [Issues](../../issues).

**Szablon zgłoszenia błędu:**
```markdown
**Opis błędu**: Krótki opis problemu

**Kroki do reprodukcji:**
1. Idź do...
2. Kliknij na...
3. Przewiń do...
4. Zobacz błąd

**Oczekiwane zachowanie**: Opisz co powinno się stać

**Faktyczne zachowanie**: Opisz co się dzieje

**Environment:**
- OS: [np. Windows 10]
- Unity Version: [np. 6000.1.10f1]
- Build: [Development/Release]

**Zrzuty ekranu**: Jeśli applicable
```

### ✨ Propozycje funkcji
Opisz swoją propozycję szczegółowo:
- Jaki problem rozwiązuje?
- Jak widzisz implementację?
- Czy jest związana z istniejącymi funkcjami?

### 🔧 Kod
1. **Fork** projekt
2. **Utwórz branch** dla swojej funkcji:
   ```bash
   git checkout -b feature/nazwa-funkcji
   ```
3. **Implementuj** zmiany zgodnie z [guidelines](#-kod-guidelines)
4. **Testuj** dokładnie swoje zmiany
5. **Commit** z opisowymi wiadomościami:
   ```bash
   git commit -m "Dodaj system power-upów"
   ```
6. **Push** do swojego fork'a:
   ```bash
   git push origin feature/nazwa-funkcji
   ```
7. **Otwórz Pull Request**

## 📏 Code Guidelines

### Styl kodu C#
```csharp
// ✅ Dobrze - Unity style conventions
public class ExampleScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private bool _isMoving = false;
    
    private void Update()
    {
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        // Implementation
    }
}

// ❌ Źle
public class exampleScript : MonoBehaviour 
{
    public float MoveSpeed = 5f;
    private bool isMoving = false;
    
    void update() 
    {
        handleMovement();
    }
}
```

### Organizacja kodu
- **Publiczne zmienne SerializeField**: Na górze klasy
- **Private zmienne**: Po SerializeField
- **Unity lifecycle methods**: Awake, Start, Update, FixedUpdate
- **Public methods**: Po lifecycle
- **Private methods**: Na końcu
- **Komentarze**: Używaj do wyjaśnienia złożonej logiki

### Performance Best Practices
- Używaj `object pooling` dla często tworzonych obiektów
- Cache references do komponentów w `Awake()`
- Unikaj `GameObject.Find()` w Update loops
- Używaj `FixedUpdate()` dla fizyki, `Update()` dla UI/input

## 🎯 Priorytety rozwoju

### 🔥 High Priority
- **Efekty dźwiękowe**: Dźwięki skoków, odbić, UI
- **Particle effects**: Efekty wizualne dla skoków i odbić
- **Performance optimization**: Dalsze ulepszenia wydajności

### 📈 Medium Priority
- **Power-ups system**: Tymczasowe ulepszenia dla gracza
- **Different obstacles**: Nowe typy przeszkód i platform
- **Visual polish**: Lepsze animacje i grafika

### 💡 Low Priority
- **Online leaderboards**: Globalne tabele wyników
- **Character skins**: System customizacji postaci
- **Achievements system**: System osiągnięć

## 🧪 Testowanie

### Przed submitem sprawdź:
- [ ] Gra uruchamia się bez błędów
- [ ] Wszystkie nowe funkcje działają poprawnie
- [ ] Nie ma regression w istniejących funkcjach
- [ ] FPS pozostaje stabilne (60 FPS target)
- [ ] Brak memory leaks w Unity Profiler

### Test checklist dla nowych funkcji:
- [ ] Działa w Unity Editor
- [ ] Działa w build (Development)
- [ ] Działa w build (Release)
- [ ] Parametry są konfigurowalne przez Inspector
- [ ] Error handling jest implementowane

## 📝 Pull Request Guidelines

### Tytuł PR
- `Feature: Dodaj system power-upów`
- `Fix: Napraw wall bounce na wysokich prędkościach`
- `Docs: Aktualizuj README z informacjami o buildowaniu`

### Opis PR
```markdown
## Opis zmian
Krótki opis co zostało dodane/zmienione/naprawione

## Typ zmiany
- [ ] Bug fix (non-breaking change)
- [ ] New feature (non-breaking change)
- [ ] Breaking change (wymaga aktualizacji)
- [ ] Documentation update

## Jak zostało przetestowane?
- [ ] Unity Editor
- [ ] Development build
- [ ] Release build

## Checklist
- [ ] Kod followuje project style guidelines
- [ ] Self-review wykonany
- [ ] Komentarze dodane w trudnych sekcjach
- [ ] Dokumentacja zaktualizowana (jeśli potrzebne)
```

## 🏷️ Labels i Issues

### Używane labels:
- `bug`: Błędy do naprawienia
- `feature`: Nowe funkcje
- `enhancement`: Ulepszenia istniejących funkcji
- `documentation`: Aktualizacje dokumentacji
- `good first issue`: Dobre dla pierwszego wkładu
- `help wanted`: Potrzebna pomoc społeczności
- `high-score`: Zgłoszenia rekordów społeczności

## 🤝 Wsparcie społeczności

### Gdzie szukać pomocy:
- **Issues**: Techniczne problemy i pytania
- **Discussions**: Ogólne dyskusje o grze
- **Wiki**: Szczegółowa dokumentacja (gdy dostępne)

### Jak pomagać innym:
- Odpowiadaj na pytania w Issues
- Review Pull Requests
- Testuj nowe funkcje
- Dziel się feedback'iem

## 📜 Code of Conduct

### Nasze zobowiązanie
Chcemy stworzyć przyjazne środowisko dla wszystkich, niezależnie od:
- Doświadczenia w programowaniu
- Pochodzenia czy narodowości
- Wieku czy płci

### Oczekiwane zachowania
- Używaj przyjaznego i inkluzywnego języka
- Szanuj różne punkty widzenia
- Przyjmuj konstruktywną krytykę z gracją
- Pomagaj innym członkom społeczności

### Niedozwolone zachowania
- Język obraźliwy lub ataki personalne
- Trolling lub celowo prowokacyjne komentarze
- Harassment publiczny lub prywatny
- Publikowanie prywatnych informacji bez zgody

---

**Dziękujemy za wkład w rozwój Komputronik Tower! 🎮✨**

Masz pytania? Otwórz Issue z label'em `question` lub skontaktuj się z maintainerami.