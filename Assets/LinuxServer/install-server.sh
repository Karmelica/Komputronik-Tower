#!/bin/bash
# Skrypt instalacyjny dla hostowania Unity WebGL na Arch Linux

echo "=== Instalacja serwera Unity WebGL na Arch Linux ==="

# Aktualizacja systemu
echo "Aktualizacja systemu..."
sudo pacman -Syu --noconfirm

# Instalacja nginx
echo "Instalacja nginx..."
sudo pacman -S nginx --noconfirm

# Instalacja python (jeśli nie ma)
echo "Instalacja python..."
sudo pacman -S python --noconfirm

# Tworzenie katalogu dla gry
echo "Tworzenie katalogu dla gry..."
sudo mkdir -p /var/www/unity-game
sudo chown -R $USER:$USER /var/www/unity-game

# Kopiowanie plików gry (zakładając że są w bieżącym katalogu)
echo "Kopiowanie plików gry..."
if [ -f "index.html" ]; then
    cp -r * /var/www/unity-game/
    echo "Pliki skopiowane do /var/www/unity-game/"
else
    echo "UWAGA: Nie znaleziono plików gry w bieżącym katalogu"
    echo "Skopiuj ręcznie pliki z Build/ do /var/www/unity-game/"
fi

# Konfiguracja nginx
echo "Konfiguracja nginx..."
if [ -f "nginx.conf" ]; then
    sudo cp nginx.conf /etc/nginx/sites-available/unity-game
    sudo mkdir -p /etc/nginx/sites-enabled
    sudo ln -sf /etc/nginx/sites-available/unity-game /etc/nginx/sites-enabled/
    
    # Test konfiguracji nginx
    sudo nginx -t
    if [ $? -eq 0 ]; then
        echo "Konfiguracja nginx OK"
    else
        echo "BŁĄD w konfiguracji nginx"
        exit 1
    fi
else
    echo "Nie znaleziono pliku nginx.conf - używam domyślnej konfiguracji"
fi

# Uruchomienie i włączenie nginx
echo "Uruchamianie nginx..."
sudo systemctl enable nginx
sudo systemctl start nginx

# Sprawdzenie statusu
sudo systemctl status nginx --no-pager

# Konfiguracja firewall (jeśli ufw jest zainstalowany)
if command -v ufw &> /dev/null; then
    echo "Konfiguracja firewall..."
    sudo ufw allow 'Nginx Full'
fi

echo ""
echo "=== Instalacja zakończona! ==="
echo ""
echo "Twoja gra powinna być dostępna pod adresem:"
echo "http://$(curl -s ifconfig.me):80"
echo "lub http://localhost:80 (lokalnie)"
echo ""
echo "Pliki gry znajdują się w: /var/www/unity-game/"
echo ""
echo "Aby zarządzać nginx:"
echo "  sudo systemctl start nginx    # uruchom"
echo "  sudo systemctl stop nginx     # zatrzymaj"
echo "  sudo systemctl restart nginx  # restart"
echo "  sudo systemctl status nginx   # sprawdź status"
echo ""
echo "Logi nginx:"
echo "  sudo tail -f /var/log/nginx/access.log"
echo "  sudo tail -f /var/log/nginx/error.log"
