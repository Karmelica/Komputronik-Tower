# Komputronik Tower 🗼

**Komputronik Tower** to dynamiczna gra 2D stworzona w Unity, w której wcielasz się w postać wspinającą się na nieskończenie wysoką wieżę! Wykorzystuj fizykę odbić od ścian i precyzyjne skoki, aby dotrzeć jak najwyżej i ustanowić nowy rekord wysokości.

## 🎮 Opis gry

Gra polega na sterowaniu postacią, która musi wspinać się na górę wieży, odbijając się od ścian i wykorzystując dynamiczną fizykę ruchu. Im wyżej dotrzesz, tym więcej punktów zdobędziesz! System wynagradza szybkie i efektowne ruchy dodatkowymi punktami.

### ✨ Główne cechy:
- **Fizyka odbić**: Realistyczne odbijanie się od ścian z możliwością boostów
- **System punktacji**: Punkty za wysokość z mnożnikiem za prędkość i styl
- **System rekordów**: Zapisywanie najlepszych wyników z identyfikacją gracza
- **Proceduralna generacja**: Nieskończenie generowane segmenty wieży
- **Responsywne sterowanie**: Płynne sterowanie z użyciem Unity Input System

## 🎯 Sterowanie

| Akcja | Klawisz/Input |
|-------|---------------|
| Ruch w lewo/prawo | A/D lub strzałki ←/→ |
| Skok | Spacja |
| Wall Boost | Wciśnij kierunek w stronę ściany tuż po uderzeniu |

### 🔥 Zaawansowane techniki:
- **Wall Boost**: Wciśnij kierunek w stronę ściany w ciągu 0.2s po uderzeniu dla extra boost
- **Velocity Jump**: Szybszy ruch poziomy = wyższy skok
- **Speed Multiplier**: Wyższe prędkości zwiększają zdobywane punkty

## 🛠️ Wymagania techniczne

### Wymagania systemowe (dla graczy):
- **System operacyjny**: Windows 10/11, macOS 10.12+, Linux Ubuntu 16.04+
- **Pamięć RAM**: 4 GB RAM
- **Miejsce na dysku**: 200 MB
- **Karta graficzna**: DirectX 11 lub OpenGL 3.3

### Wymagania deweloperskie:
- **Unity**: 6000.1.10f1 lub nowszy
- **System operacyjny**: Windows 10/11, macOS 10.14+, lub Linux
- **Miejsce na dysku**: 2 GB (ze wszystkimi zasobami)

## 🚀 Instalacja i uruchomienie

### Dla graczy:
1. Pobierz najnowszą wersję z [Releases](../../releases)
2. Rozpakuj archiwum
3. Uruchom `Komputronik Tower.exe` (Windows) lub odpowiedni plik dla twojego systemu

### Dla deweloperów:

#### 1. Klonowanie repozytorium
```bash
git clone https://github.com/Karmelica/Komputronik-Tower.git
cd Komputronik-Tower
```

#### 2. Otwarcie w Unity
1. Uruchom Unity Hub
2. Kliknij "Add" i wybierz folder projektu
3. Upewnij się, że używasz Unity 6000.1.10f1 lub nowszego
4. Otwórz projekt

#### 3. Konfiguracja projektu
- Wszystkie dependencje są już skonfigurowane w `Packages/manifest.json`
- Unity automatycznie pobierze wymagane pakiety przy pierwszym otwarciu
- Domyślna scena: `Assets/Scenes/SampleScene.unity`

#### 4. Budowanie gry
1. File → Build Settings
2. Wybierz platformę docelową
3. Dodaj sceny: `Assets/Scenes/SampleScene.unity`
4. Kliknij "Build" i wybierz folder docelowy

## 🏗️ Architektura projektu

### 📁 Struktura folderów
```
Assets/
├── Scenes/           # Sceny Unity
├── Scripts/          # Skrypty C#
├── Materials/        # Materiały graficzne
├── Resources/        # Zasoby ładowane dynamicznie
└── Settings/         # Ustawienia projektu
```

### 🧩 Główne komponenty

#### Character.cs
Główny kontroler postaci obsługujący:
- ✅ System ruchu i skoków
- ✅ Fizyka odbić od ścian
- ✅ Wall boost mechanic
- ✅ Integracja z Unity Input System

#### SegmentGen.cs
Generator poziomów odpowiedzialny za:
- ✅ Proceduralne tworzenie segmentów wieży
- ✅ System poolingu obiektów
- ✅ Pseudolosową generację z seedem

#### Score.cs
System punktacji:
- ✅ Śledzenie najwyższej pozycji gracza
- ✅ Mnożnik punktów oparty na prędkości
- ✅ Integracja z systemem rekordów

#### HighScoreManager.cs
Zarządzanie rekordami:
- ✅ System logowania gracza (email)
- ✅ Lokalne przechowywanie rekordów
- ✅ UI logowania i wyników

#### PoolingManager.cs
Optymalizacja wydajności:
- ✅ Pool obiektów dla segmentów
- ✅ Redukcja garbage collection
- ✅ Efektywne zarządzanie pamięcią

## 🎨 Zasoby i materiały

Projekt wykorzystuje:
- **Universal Render Pipeline (URP)** - dla lepszej wydajności
- **Unity Input System** - nowoczesne sterowanie
- **TextMesh Pro** - wysokiej jakości renderowanie tekstu
- **Physics2D** - realistyczna fizyka w 2D

## 🐛 Znane problemy i rozwiązania

### Problem: Gra nie uruchamia się
**Rozwiązanie**: Upewnij się, że masz zainstalowane Visual C++ Redistributable

### Problem: Niestabilne FPS
**Rozwiązanie**: Sprawdź ustawienia Quality Settings w Unity

### Problem: Sterowanie nie działa
**Rozwiązanie**: Sprawdź czy Input System Package jest poprawnie zainstalowany

## 🤝 Wkład w projekt

Chcesz pomóc w rozwoju? Świetnie! 

### Jak przyczynić się do projektu:
1. **Fork** repozytorium
2. Stwórz nową gałąź dla swojej funkcji (`git checkout -b feature/nowa-funkcja`)
3. **Commit** swoje zmiany (`git commit -m 'Dodaj nową funkcję'`)
4. **Push** do gałęzi (`git push origin feature/nowa-funkcja`)
5. Otwórz **Pull Request**

### 📋 TODO Lista:
- [ ] Dodanie efektów dźwiękowych
- [ ] Implementacja systemu power-upów
- [ ] Graficzne ulepszenia (particle effects)
- [ ] Dodanie różnych typów przeszkód
- [ ] System skin'ów dla postaci
- [ ] Globalne tabele wyników (online)

## 📄 Licencja

Ten projekt jest udostępniony na licencji MIT - zobacz plik [LICENSE](LICENSE) aby uzyskać szczegóły.

## 📞 Kontakt

**Autor**: Karmelica  
**Repozytorium**: [https://github.com/Karmelica/Komputronik-Tower](https://github.com/Karmelica/Komputronik-Tower)

---

### 🏆 Rekordy społeczności
Masz świetny wynik? Podziel się nim w Issues z tagiem `high-score`!

**Miłej zabawy z wspinaczką na Komputronik Tower! 🎮✨**
