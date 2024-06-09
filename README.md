# Tugas Besar 3 IF2211 Strategi Algoritma
> Aplikasi Biometric oleh kelompok S1T4j4111

## Daftar Konten
* [Informasi Umum](#informasi-umum)
* [Deskripsi Singkat](#deskripsi-singkat)
* [Kebutuhan](#kebutuhan)
* [Setup dan Penggunaan](#setup-dan-penggunaan)
* [Setup Database](#setup-database)
* [Kreator](#kreator)

## Informasi Umum
Pemanfaatan Pattern Matching dalam Membangun Sistem Deteksi Individu Berbasis Biometrik Melalui Citra Sidik Jari

## Deskripsi Singkat
### Boyer Moore
Algoritma Boyer-Moore (BM) merupakan algoritma pencocokan teks yang didasarkan pada dua teknik, yaitu teknik _looking-glass_ dan teknik _character-jump_. Teknik _looking-glass_ adalah teknik pencocokan yang pengecekan karakternya dimulai dari indeks terakhir pola. Algoritma ini memanfaatkan array _last-occurence_ yang menyimpan kemunculan terakhir suatu karakter.
### Knuth Morris Pratt
Algoritma Knuth-Morris-Pratt (KMP) adalah algoritma pencarian kata pada sebuah teks dengan pengecekan yang dilakukan dari kiri ke kanan, tetapi perpindahan posisi dilakukan dengan menggunakan informasi dari karakter yang telah dicocokan sebelumya. Pada algoritma ini, saat ditemukan karakter yang berbeda saat pencocokan antara kata dengan teks, akan dilakukan pemrosesan untuk menentukan banyak pergeseran kata berdasarkan prefiks dan sufiks pada kata tersebut dan disimpan di dalam array Border Function.
### Regular Expression
Regular Expression adalah salah satu bentuk string matching. String matching yang dilakukan adalah pencarian teks dengan pola tertentu. Jadi, selama teks yang dicocokkan memiliki pola yang sama dengan pola regex, teks tersebut akan dianggap valid.
Regular Expression mencocokkan suatu teks dengan pola tertentu. Regular expression digunakan untuk mencocokkan suatu teks dengan bahasa alaynya. Teks asli akan diubah menjadi bentuk regex bahasa alaynya. Kemudian, regex tersebut akan dicocokkan dengan bahasa alaynya.
## Kebutuhan
1. .Net SDK x64  [version 8.0](#https://dotnet.microsoft.com/en-us/download)

## Setup dan Penggunaan
1. Clone repository dengan salah satu dari perintah berikut
    ```
    $ git clone https://github.com/julianchandras/Tubes3_S1T4j4111.git
    $ git clone git@github.com:julianchandras/Tubes3_S1T4j4111.git
    ```
2. Pindah ke dalam directory Biometric dengan perintah `cd src/Biometric`
3. Unduh semua _dependencies_ dengan perintah `dotnet restore`
4. Jika ingin meng-_compile_ program jalankan perintah `dotnet build`
5. Untuk menjalankan program, jalankan perintah `dotnet run`. Pastikan bahwa database sudah tersedia (dengan perintah di bawah)
6. Masukkan gambar yang ingin dibandingkan dengan database
7. Pilih salah satu dari tombol BM atau KMP untuk memilih algoritma yang akan digunakan untuk melakukan perbandingan
8. Aplikasi akan menampilkan gambar beserta biodata dari pengguna yang terdapat di basis data yang memiliki kemiripan dengan masukkan

## Setup Database
1. Buatlah file .env pada root directory dengan struktur isi sebagai berikut (sesuaikan)
    ```
    DB_SERVER=localhost
    DB_USER=root
    DB_PASS=xxxx
    DB_DATABASE=tubes3_stima
    ```
2. Pastikan Python dan MariaDB sudah terinstall pada perangkat
3. Unduh library yang diperlukan dengan perintah berikut
	```
	$ pip install faker
	$ pip install python-dotenv
	$ pip install mysql-connector-python
	```
4. Buat databse dengan perintah berikut pada terminal di dalam directory database
    ```
    mysql -u <DB_USER> -p
    CREATE DATABASE <DB_DATABASE>
    exit
    mysql -u <DB_USER> -p <DB_DATABASE> < tubes3_stima24.sql
    ```
5. Jalankan perintah `python seed.py` pada terminal di dalam directory database

## Kreator
| NIM | Nama | Kontak |
|-----|------| ------ |
| 10023485 | Lidya Rahmatul Fitri | [Lidyarf24](#https://github.com/Lidyarf24) |
| 13522033 | Bryan Cornelius Lauwrence | [BryanLauw](#https://github.com/bryanlauw) |
| 13522062 | Salsabiila | [salsbiila](#https://github.com/salsbiila) |
| 13522090 | Julian Chandra Sutadi| [julianchandras](#https://github.com/julianchandras) |