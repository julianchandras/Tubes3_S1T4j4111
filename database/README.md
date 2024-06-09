Make sure to have the following packages: faker, python-dotenv, mysql-connector-python
```
pip install faker
pip install python-dotenv
pip install mysql-connector-python
```

Please lakuin ini dulu
```
mysql -u <DB_USER> -p <DB_DATABASE> < tubes3_stima24.sql
```
Ganti DB_USER dan DB_DATABASE sesuai nama yang ada pada .env

Lakukan seeding dengan menjalankan py seed.py pada directory database