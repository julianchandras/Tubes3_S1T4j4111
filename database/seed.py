import json
import os
from faker import Faker
import random
from datetime import datetime
import mysql.connector
from dotenv import load_dotenv
import subprocess

load_dotenv("../.env")

def load_data(persons):
    db_config = {
        'user': os.getenv('DB_USER'),
        'password': os.getenv('DB_PASS'),
        'host': os.getenv('DB_SERVER'),
        'database': os.getenv('DB_DATABASE')
    }

    connection = mysql.connector.connect(**db_config)
    cursor = connection.cursor()

    add_person = ("INSERT INTO Biodata "
                  "(NIK, nama, tempat_lahir, tanggal_lahir, jenis_kelamin, golongan_darah, alamat, agama, status_perkawinan, pekerjaan, kewarganegaraan) "
                  "VALUES (%(NIK)s, %(nama)s, %(tempat_lahir)s, %(tanggal_lahir)s, %(jenis_kelamin)s, %(golongan_darah)s, %(alamat)s, %(agama)s, %(status_perkawinan)s, %(pekerjaan)s, %(kewarganegaraan)s)")

    for person in persons:
        cursor.execute(add_person, person)

    connection.commit()
    cursor.close()
    connection.close()

fake = Faker('id_ID')  # Indonesian locale for relevant names and addresses

def generate_nik():
    # NIK format: AABBDDMMYYYYXXXX
    # AA: Province code
    # BB: City/Regency code
    # DDMMYYYY: Date of birth (DD and MM parts might be adjusted based on gender)
    # XXXX: Random sequence number
    province_code = random.randint(10, 99)
    city_code = random.randint(10, 99)
    birth_date = fake.date_of_birth(minimum_age=18, maximum_age=85)
    gender_code = 40 if birth_date.day > 31 else 0  # Simplistic gender-based adjustment
    day = birth_date.day + gender_code
    month = birth_date.month
    year = birth_date.year % 100
    sequence = random.randint(1000, 9999)
    
    return f"{province_code:02d}{city_code:02d}{day:02d}{month:02d}{year:02d}{sequence:04d}"


def generate_person():
    genders = ['Laki-laki', 'Perempuan']
    blood_types = ['A', 'B', 'AB', 'O']
    religions = ['Islam', 'Kristen', 'Katolik', 'Hindu', 'Buddha', 'Kong Hu Cu']
    marital_statuses = ['Belum Menikah', 'Menikah', 'Cerai']
    
    person = {
        "NIK": generate_nik(),
        "nama": fake.first_name() + " " + fake.last_name(),
        "tempat_lahir": fake.city(),
        "tanggal_lahir": fake.date_of_birth(minimum_age=18, maximum_age=85).isoformat(),
        "jenis_kelamin": random.choice(genders),
        "golongan_darah": random.choice(blood_types),
        "alamat": fake.address(),
        "agama": random.choice(religions),
        "status_perkawinan": random.choice(marital_statuses),
        "pekerjaan": fake.job(),
        "kewarganegaraan": "Indonesia"
    }
    
    return person

def seed_persons(n=600):
    persons = [generate_person() for _ in range(n)]
    return persons


def reset():
    db_config = {
        'user': os.getenv('DB_USER'),
        'password': os.getenv('DB_PASS'),
        'host': os.getenv('DB_SERVER'),
        'database': os.getenv('DB_DATABASE')
    }

    connection = mysql.connector.connect(**db_config)
    cursor = connection.cursor()

    delete_biodata = ("DELETE FROM Biodata")

    cursor.execute(delete_biodata)

    connection.commit()
    cursor.close()
    connection.close()

if __name__ == "__main__":
    reset()
    persons = seed_persons()
    load_data(persons)