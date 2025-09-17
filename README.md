# CinemaBookingApplication
Оваа веб апликација е наменета за управување со процесот на резервирање билети за кино (курс: интегрирани системи). Корисниците можат да прегледуваат филмови и термини (проекции), да прават резервации и да ги следат своите резервации. Апликацијата интегрира надворешно API (TMDB) од кое се влечат реални податоци за филмови; податоците се трансформираат и потоа се внесуваат во нашата база.
Ентитети: Movie, Hall, Screening, Reservation, ApplicationUser (+ DTO: CreateReservationViewModel).

Релации:
1-* меѓу Movie и Screening,
1-* меѓу Hall и Screening,
1-* меѓу Screening и Reservation,
1-* меѓу ApplicationUser и Reservation

Пристап:
-Админ функционалности (CRUD за Movies/Halls/Screenings, TMDB import, преглед на сите резервации):
 логин: admin@cinema.local / Admin123!
-Обичен корисник: доволно е да се регистрира; гледа и управува само со своите резервации.

