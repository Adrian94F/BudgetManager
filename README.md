# Budget Manager

Aplikacja umożliwiająca zapisywanie codziennych wydatków i wyświetlanie statystyk.

![Wydatki miesięczne](doc/s1.png)

![Wykres wypalenia](doc/s1.png)

## TODO

- Dostosowanie wyglądu do [ModernWpf](https://github.com/Kinnara/ModernWpf)
- Generowanie raportu
- Wydatki
  
  - ostrzeżenie w przypadku daty spoza okresu rozliczeniowego
- Porządkowanie danych
  - W przypadku daty spoza zakresu przenoszenie wydatków do właściwego okresu rozliczeniowego
    - Jeśli okresu brak, pytanie o zmianę daty lub usunięcie
  - Usuwanie nieużywanych kategorii
- Ustawienia
  - Skróty klawiszowe
    - Szybkie dodawanie do predefiniowanych kategorii (Ctrl + 1-9)
