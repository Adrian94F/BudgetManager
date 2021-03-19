# Budget Manager

Aplikacja umożliwiająca zapisywanie codziennych wydatków i wyświetlanie statystyk.

![Wydatki miesięczne](doc/s1.png)

![Wykres wypalenia](doc/s2.png)

## TODO

- Lista wydatków z filtrowaniem wg daty (lub zakresu dat) i kategorii
  - Zmiana widoku zamiast otwierania okna
- Dostosowanie wyglądu do [ModernWpf](https://github.com/Kinnara/ModernWpf)
  - Widok listy wydatków, podsumowania, wypalenia, historii
  - Widok miesięcy (DataGrid)
  - Widok ustawień
    - Kategorie
    - Typowy początek miesiąca
    - Ścieżka zapisu danych
- Generowanie raportu
- Wydatki
  
  - ostrzeżenie w przypadku daty spoza okresu rozliczeniowego
- Porządkowanie danych
  - W przypadku daty spoza zakresu przenoszenie wydatków do właściwego okresu rozliczeniowego
    - Jeśli okresu brak, pytanie o zmianę daty lub usunięcie
  - Usuwanie nieużywanych kategorii
