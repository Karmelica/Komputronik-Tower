# Changelog

Wszystkie istotne zmiany w projekcie Komputronik Tower będą dokumentowane w tym pliku.

Format oparty na [Keep a Changelog](https://keepachangelog.com/pl/1.0.0/),
a wersjonowanie zgodne z [Semantic Versioning](https://semver.org/lang/pl/).

## [Unreleased]

### Planowane funkcje
- Efekty dźwiękowe i muzyka
- System power-upów
- Efekty wizualne (particle effects)
- Różne typy przeszkód
- System skin'ów dla postaci
- Globalne tabele wyników (online)

## [1.0.0] - 2024-08-08

### Dodane
- **Podstawowa mechanika gry**: Wspinaczka po wieży z odbijaniem od ścian
- **System sterowania**: Płynne sterowanie z Unity Input System
  - Ruch w lewo/prawo (A/D, strzałki)
  - Skok (Spacja)
  - Wall Boost mechanic
- **System punktacji**: 
  - Punkty za wysokość z mnożnikiem prędkości
  - Śledzenie najwyższej pozycji gracza
- **System rekordów**:
  - Lokalne przechowywanie high scores
  - System logowania gracza (email)
  - Interfejs użytkownika dla rekordów
- **Proceduralna generacja**:
  - Nieskończenie generowane segmenty wieży
  - System seedów dla powtarzalnej generacji
- **Optymalizacja wydajności**:
  - Object pooling dla segmentów
  - Efektywne zarządzanie pamięcią
- **Fizyka i mechaniki**:
  - Realistyczne odbijanie od ścian
  - Wall boost z timingiem
  - Velocity-based jump boosting
  - Clamp prędkości dla stabilności

### Techniczne
- **Unity 6000.1.10f1**: Najnowsza wersja Unity
- **Universal Render Pipeline**: Dla lepszej wydajności
- **Input System Package**: Nowoczesne sterowanie
- **TextMesh Pro**: Wysokiej jakości renderowanie tekstu
- **Physics2D**: Kompletny system fizyki 2D

### Komponenty
- `Character.cs`: Główny kontroler postaci
- `SegmentGen.cs`: Generator poziomów
- `Score.cs`: System punktacji
- `HighScoreManager.cs`: Zarządzanie rekordami
- `PoolingManager.cs`: Optymalizacja wydajności
- `Platform.cs`: Logika platform
- `SegmentScript.cs`: Kontroler segmentów wieży

### Zasoby
- Prefaby: Player, Segment, Platform
- Materiały i textury
- Sceny: SampleScene (główna gra), Default
- Input Actions configuration
- URP settings i profile

---

## Legenda

- `Dodane` - nowe funkcje
- `Zmienione` - zmiany w istniejących funkcjach
- `Przestarzałe` - funkcje, które będą usunięte w przyszłości
- `Usunięte` - usunięte funkcje
- `Naprawione` - poprawki błędów
- `Bezpieczeństwo` - poprawki związane z bezpieczeństwem