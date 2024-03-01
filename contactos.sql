-- Crear la base de datos
CREATE DATABASE Agenda;
GO

-- Usar la base de datos creada
USE Agenda;
GO

-- Crear la tabla de contactos
CREATE TABLE Contactos (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellidos VARCHAR(50),
    Comentario VARCHAR(MAX)
);

-- Crear la tabla de teléfonos
CREATE TABLE Telefonos (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_Contacto INT,
    Telefono VARCHAR(15),
    FOREIGN KEY (ID_Contacto) REFERENCES Contactos(ID) ON DELETE CASCADE
);

-- Crear la tabla de emails
CREATE TABLE Correos (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ID_Contacto INT,
    Correo VARCHAR(100),
    FOREIGN KEY (ID_Contacto) REFERENCES Contactos(ID) ON DELETE CASCADE
);
