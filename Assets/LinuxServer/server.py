#!/usr/bin/env python3
"""
Prosty serwer HTTP dla Unity WebGL
Obsługuje wymagane nagłówki CORS i MIME types
"""

import http.server
import socketserver
import os
import mimetypes

# Dodaj MIME types dla Unity WebGL
mimetypes.add_type('application/wasm', '.wasm')
mimetypes.add_type('application/javascript', '.js')
mimetypes.add_type('application/octet-stream', '.data')
mimetypes.add_type('application/octet-stream', '.symbols.json')

class UnityWebGLHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        # Dodaj nagłówki CORS
        self.send_header('Cross-Origin-Opener-Policy', 'same-origin')
        self.send_header('Cross-Origin-Embedder-Policy', 'require-corp')
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS')
        self.send_header('Access-Control-Allow-Headers', 'Content-Type')
        
        # Kompresja gzip dla plików Unity
        if self.path.endswith('.wasm.gz'):
            self.send_header('Content-Encoding', 'gzip')
            self.send_header('Content-Type', 'application/wasm')
        elif self.path.endswith('.js.gz'):
            self.send_header('Content-Encoding', 'gzip')
            self.send_header('Content-Type', 'application/javascript')
        elif self.path.endswith('.data.gz'):
            self.send_header('Content-Encoding', 'gzip')
            self.send_header('Content-Type', 'application/octet-stream')
        
        super().end_headers()

def run_server(port=8000):
    os.chdir(os.path.dirname(os.path.abspath(__file__)))
    
    with socketserver.TCPServer(("", port), UnityWebGLHandler) as httpd:
        print(f"Serwer Unity WebGL działa na porcie {port}")
        print(f"Otwórz: http://localhost:{port}")
        print("Naciśnij Ctrl+C aby zatrzymać serwer")
        
        try:
            httpd.serve_forever()
        except KeyboardInterrupt:
            print("\nSerwer zatrzymany")

if __name__ == "__main__":
    import sys
    port = int(sys.argv[1]) if len(sys.argv) > 1 else 8000
    run_server(port)
