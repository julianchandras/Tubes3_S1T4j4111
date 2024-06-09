import json
import re
import os
from faker import Faker
import random
from datetime import datetime
import mysql.connector
from dotenv import load_dotenv
import subprocess

load_dotenv("../.env")

def make_name_alay(name):
    word = [
        "([aA4])?",    # A
        "[bB8]",       # B
        "[cC]",        # C
        "[dD]",        # D
        "([eE3])?",    # E
        "[fF]",        # F
        "[gG6]",       # G
        "[hH]",        # H
        "([iI1])?",    # I
        "[jJ]",        # J
        "[kK]",        # K
        "[lL]",        # L
        "([mM]|111)",  # M
        "([nN]|11)",   # N
        "([oO0])?",    # O
        "[pP]",        # P
        "[qQ]",        # Q
        "([rR]|12)",   # R
        "[sS5]",       # S
        "[tT]",        # T
        "([uU])?",     # U
        "[vV]",        # V
        "[wW]",        # W
        "[xX]",        # X
        "[yY]",        # Y
        "[zZ2]"        # Z
    ]

    alay_name = ""
    for char in name:
        idx = ord(char.upper()) - ord('A')
        if 0 <= idx < len(word):
            pattern = word[idx]
            matches = re.findall(pattern, pattern)
            if matches:
                alay_name += random.choice(matches)
        else:
            # If the character is not a letter, add it unchanged
            alay_name += char

    return alay_name

def load_biodata(persons):
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
        person["nama"] = make_name_alay(person["nama"])
        cursor.execute(add_person, person)

    connection.commit()
    cursor.close()
    connection.close()

def load_fingerprints(persons):
    db_config = {
        'user': os.getenv('DB_USER'),
        'password': os.getenv('DB_PASS'),
        'host': os.getenv('DB_SERVER'),
        'database': os.getenv('DB_DATABASE')
    }

    connection = mysql.connector.connect(**db_config)
    cursor = connection.cursor()

    files = os.listdir("../test/real/")
    sorted(files)

    j = 0
    for person in persons:
        for i in range(10):
            fingerprint_entry = {
                "berkas_citra" : files[j],
                "nama" : person["nama"]
            }
            add_fingerprint = ("INSERT INTO sidik_jari "
                               "(berkas_citra, nama) VALUES (%(berkas_citra)s, %(nama)s)")  
            cursor.execute(add_fingerprint, fingerprint_entry)
            j += 1

    connection.commit()
    cursor.close()
    connection.close

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
    delete_fingerprint = ("DELETE FROM sidik_jari")

    cursor.execute(delete_biodata)
    cursor.execute(delete_fingerprint)

    connection.commit()
    cursor.close()
    connection.close()

if __name__ == "__main__":
    reset()
    persons = seed_persons()
    load_fingerprints(persons)
    load_biodata(persons)