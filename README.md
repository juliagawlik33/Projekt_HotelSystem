# HotelSystem

Aplikacja do zarządzania hotelem stworzona przez Dawida Pątko i Julię Gawlik

Przed uruchomieniem należy zaktualizować pakiety NuGet i ustawić connection string w 10 linijce pliku appsettings.json.

Aplikacja pozwala użytkownikowi na zarezerwowanie pokoju hotelowego w określonym terminie i pokoju a także na usunięcie rezerwacji.
Z perspektywy admina można dodawać pokoje, edytować je a także zarządzać poszczególnymi rezerwacjami.
Użyliśmy Entity Framework i Identity do stworzenia tej aplikacji. Ponadto dodany został także Swagger aby ładnie zobaczyć jak wygląda API pod adresem /swagger

Podzieliliśmy się rolami:
Dawid:
- Logika biznesowa rezerwacji
- Część użytkownika
- API

Julia:
- Identity i logowanie
- Część admina
- Część logiki biznesowej w przypadku administratora
- Poprawki na język polski

Testowi użytkownicy:
admin:
admin@admin.com
TestAdmin123!

user:
user@test.com
TestUser123!
