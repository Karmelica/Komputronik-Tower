# Unity WebGL - Instrukcja hostowania na serwerze Linux Arch

## Podsumowanie systemu wyników

Twoja gra już ma zaimplementowany system przechowywania wyników:

### Funkcjonalności:
- ✅ Logowanie gracza przez email i opcjonalne imię
- ✅ Przechowywanie najlepszych wyników dla każdego gracza
- ✅ System rankingu (top 10 graczy)
- ✅ Automatyczne zapisywanie w localStorage przeglądarki
- ✅ Walidacja emaili
- ✅ Wyświetlanie pozycji gracza w rankingu

### Jak to działa:
1. Gracz loguje się podając email (wymagane) i imię (opcjonalne)
2. Dane są zapisywane w localStorage przeglądarki za pomocą Unity PlayerPrefs
3. Każdy nowy rekord jest automatycznie zapisywany
4. Ranking jest aktualizowany po każdym nowym rekordzie
5. Dane pozostają na urządzeniu gracza między sesjami

## Hostowanie na serwerze Linux Arch

### Opcja 1: Prosty serwer Python (do testów)

1. Skopiuj pliki z `Builds/Web/` na serwer
2. Uruchom prosty serwer:
```bash
cd /ścieżka/do/plików/gry
python3 server.py 8000
```

### Opcja 2: Profesjonalne hostowanie z Nginx (zalecane)

#### Przygotowanie plików na serwerze:

1. **Skopiuj pliki gry na serwer:**
```bash
# Na swoim komputerze, spakuj pliki
cd "C:\Users\Damian\Desktop\Uczelnia\Praktyki\Komputronik tower\Builds\Web"
tar -czf unity-game.tar.gz *

# Prześlij na serwer (przykład z scp)
scp unity-game.tar.gz user@twoj-serwer:/tmp/
```

2. **Na serwerze Arch Linux:**
```bash
# Wypakuj pliki
cd /tmp
tar -xzf unity-game.tar.gz

# Uruchom skrypt instalacyjny
chmod +x install-server.sh
./install-server.sh
```

#### Ręczna instalacja (krok po kroku):

```bash
# 1. Aktualizuj system
sudo pacman -Syu

# 2. Zainstaluj nginx
sudo pacman -S nginx

# 3. Utwórz katalog dla gry
sudo mkdir -p /var/www/unity-game
sudo chown -R $USER:$USER /var/www/unity-game

# 4. Skopiuj pliki gry
cp -r * /var/www/unity-game/

# 5. Skonfiguruj nginx
sudo cp nginx.conf /etc/nginx/sites-available/unity-game
sudo mkdir -p /etc/nginx/sites-enabled
sudo ln -sf /etc/nginx/sites-available/unity-game /etc/nginx/sites-enabled/

# 6. Testuj konfigurację
sudo nginx -t

# 7. Uruchom nginx
sudo systemctl enable nginx
sudo systemctl start nginx

# 8. Sprawdź status
sudo systemctl status nginx
```

### Konfiguracja portu i domeny

1. **Zmiana portu** (w nginx.conf):
```nginx
listen 8080;  # Zamiast 80
```

2. **Dodanie domeny** (w nginx.conf):
```nginx
server_name twoja-domena.com www.twoja-domena.com;
```

3. **SSL/HTTPS** (opcjonalne):
```bash
# Zainstaluj certbot
sudo pacman -S certbot certbot-nginx

# Uzyskaj certyfikat SSL
sudo certbot --nginx -d twoja-domena.com
```

### Testowanie połączenia

1. **Sprawdź czy serwer działa:**
```bash
curl -I http://localhost:80
curl -I http://twoj-ip:80
```

2. **Sprawdź logi:**
```bash
sudo tail -f /var/log/nginx/access.log
sudo tail -f /var/log/nginx/error.log
```

3. **Testuj w przeglądarce:**
- Otwórz `http://twoj-ip:80` lub `http://twoja-domena.com`
- Sprawdź konsolę deweloperską (F12) pod kątem błędów

### Rozwiązywanie problemów

#### Problem: Gra nie ładuje się
```bash
# Sprawdź uprawnienia plików
ls -la /var/www/unity-game/
sudo chown -R www-data:www-data /var/www/unity-game/
sudo chmod -R 755 /var/www/unity-game/
```

#### Problem: Błędy CORS
- Sprawdź czy nginx.conf zawiera poprawne nagłówki CORS
- Restart nginx: `sudo systemctl restart nginx`

#### Problem: Pliki .wasm nie ładują się
- Sprawdź czy nginx obsługuje MIME type dla .wasm
- Sprawdź czy pliki .gz mają poprawne nagłówki

### Monitorowanie

```bash
# Status serwera
sudo systemctl status nginx

# Wykorzystanie zasobów
htop

# Logi dostępu w czasie rzeczywistym
sudo tail -f /var/log/nginx/access.log

# Sprawdź połączenia
sudo netstat -tulpn | grep :80
```

### Aktualizacja gry

```bash
# 1. Zatrzymaj nginx (opcjonalne)
sudo systemctl stop nginx

# 2. Backup poprzedniej wersji
sudo cp -r /var/www/unity-game /var/www/unity-game-backup

# 3. Skopiuj nowe pliki
cp -r nowe-pliki/* /var/www/unity-game/

# 4. Uruchom nginx
sudo systemctl start nginx
```

## Uwagi dotyczące wydajności

1. **Kompresja**: Nginx automatycznie kompresuje pliki
2. **Cache**: Pliki .wasm, .js, .data są cache'owane na rok
3. **CDN**: Rozważ użycie CDN dla lepszej wydajności globalnej

## Bezpieczeństwo

1. **Firewall**: Otwórz tylko potrzebne porty (80, 443)
2. **Aktualizacje**: Regularnie aktualizuj system i nginx
3. **SSL**: Użyj HTTPS w produkcji
4. **Backup**: Regularnie twórz kopie zapasowe
