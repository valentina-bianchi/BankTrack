--CREO BASE DE DATOS
create database BankTrackDB
GO

USE BankTrackDB;

go 
create table Usuario(
IdUsuario int primary key identity,
Username varchar(50),
Correo varchar(50),
Clave varchar(200),
Restablecer bit,
Confirmado bit,
Token varchar(200)
);

--CREO TABLAS CON ENTIDADES
go
create table Clientes(
id_cliente int not null identity primary key,
nombre_cliente nvarchar(60) not null,
apellido_cliente nvarchar(60) not null,
direccion_cliente nvarchar(60) not null,
dni_cliente bigint not null unique
);

GO
create table CuentasBancarias(
id_cuenta int not null identity primary key,
numero_cuenta bigint not null unique,
id_cliente int not null foreign key references Clientes(id_cliente),
saldo_objetivo decimal(25,5) not null,
saldo_actual decimal(25,5) not null,
);
go
create table Tipos_Transacciones(
id_tipo_transaccion int not null identity primary key,
nombre_tipo nvarchar(60) not null,
aumenta bit not null
);

go
create table Categorias(
id_categoria int not null identity primary key,
nombre_categoria nvarchar(60) not null unique,
icono_categoria nvarchar(60) not null,
descripcion_categoria nvarchar(150) not null,
id_tipo_transaccion int not null foreign key references Tipos_Transacciones(id_tipo_transaccion),
);
------------------------------------
go
create table Transacciones(
id_transaccion int not null identity primary key,
id_categoria int not null foreign key references Categorias(id_categoria),
fecha datetime not null,
monto decimal(25,5) not null,
id_cuenta int not null foreign key references CuentasBancarias(id_cuenta)
);


go 
INSERT INTO Tipos_Transacciones(nombre_tipo, aumenta)
values ('Ingreso', 1),('Egreso', 0)


-- stored procedures  
-- CLIENTES
go 
CREATE PROCEDURE SP_AGREGARCLIENTE
@nombre_cliente nvarchar(60),
@apellido_cliente nvarchar(60),
@direccion_cliente nvarchar(60),
@dni_cliente bigint 
AS
BEGIN 
INSERT INTO Clientes(nombre_cliente, apellido_cliente, direccion_cliente, dni_cliente)
VALUES (@nombre_cliente, @apellido_cliente, @direccion_cliente, @dni_cliente)
END

GO
CREATE PROCEDURE SP_MODIFICARCLIENTE 
@id_cliente int,
@nombre_cliente nvarchar(60),
@apellido_cliente nvarchar(60),
@direccion_cliente nvarchar(60),
@dni_cliente bigint
AS BEGIN
	UPDATE Clientes SET nombre_cliente = @nombre_cliente, apellido_cliente =  @apellido_cliente, direccion_cliente = @direccion_cliente WHERE id_cliente = @id_cliente
END

GO
CREATE PROCEDURE SP_ELIMINARCLIENTE @id_cliente nvarchar(25)
AS BEGIN
	DELETE FROM Clientes WHERE id_cliente = @id_cliente
END

-- CUENTA BANCARIA
go
CREATE PROCEDURE SP_RECUPERARCUENTAS
AS
BEGIN
    SELECT 
        C.id_cuenta, 
        C.numero_cuenta, 
        C.saldo_actual, 
        C.saldo_objetivo, 
        CL.id_cliente AS IdCliente, 
        CL.dni_cliente AS DNICLIENTE 
    FROM 
        CuentasBancarias C 
        INNER JOIN Clientes CL ON CL.id_cliente = C.id_cliente;
END

go
CREATE PROCEDURE SP_AGREGARCUENTA
@numero_cuenta bigint,
@dni_cliente bigint,
@saldo_objetivo decimal(25,5),
@saldo_actual decimal(25,5)
AS
BEGIN
    INSERT INTO CuentasBancarias(numero_cuenta, id_cliente, saldo_objetivo, saldo_actual)
    VALUES (@numero_cuenta,(Select id_cliente from Clientes where dni_cliente = @dni_cliente), @saldo_objetivo, @saldo_actual)
END

go
 CREATE PROCEDURE SP_ELIMINARCUENTA
    @id_cuenta INT
AS
BEGIN
    DELETE FROM CuentasBancarias
    WHERE id_cuenta = @id_cuenta
END

go
CREATE PROCEDURE SP_MODIFICARCUENTA
@id_cuenta INT,
@numero_cuenta bigint,
@dni_cliente bigint,
@saldo_objetivo decimal(10,5),
@saldo_actual decimal(10,5)
AS
BEGIN
    UPDATE CuentasBancarias
    SET numero_cuenta = @numero_cuenta,
        id_cliente = (Select id_cliente from Clientes where dni_cliente = @dni_cliente),
        saldo_objetivo = @saldo_objetivo,
        saldo_actual = @saldo_actual
    WHERE id_cuenta = @id_cuenta
END
-- TIPO TRANSACCION
go
CREATE PROCEDURE SP_RECUPERARTIPOSTRANSACCIONES
AS
BEGIN
    SELECT 
        T.id_tipo_transaccion, 
        T.nombre_tipo,
        T.aumenta
    FROM 
        Tipos_Transacciones T 
END
go
CREATE PROCEDURE SP_AGREGARTIPOTRANSACCION
    @nombre_tipo NVARCHAR(60),
    @aumenta BIT
AS
BEGIN
    INSERT INTO Tipos_Transacciones(nombre_tipo, aumenta)
    VALUES (@nombre_tipo, @aumenta)
END
go
CREATE PROCEDURE SP_ELIMINARTIPOTRANSACCION
    @id_tipo_transaccion INT
AS
BEGIN
    DELETE FROM Tipos_Transacciones
    WHERE id_tipo_transaccion = @id_tipo_transaccion
END
go
CREATE PROCEDURE SP_MODIFICARTIPOTRANSACCION
    @id_tipo_transaccion INT,
    @nombre_tipo NVARCHAR(60),
    @aumenta BIT
AS
BEGIN
    UPDATE Tipos_Transacciones
    SET nombre_tipo = @nombre_tipo,
        aumenta = @aumenta
    WHERE id_tipo_transaccion = @id_tipo_transaccion
END

--CATEGORIA
go
CREATE PROCEDURE SP_RECUPERARCATEGORIAS
AS
BEGIN
    SELECT 
        C.id_categoria, 
        C.nombre_categoria, 
        C.icono_categoria, 
        C.descripcion_categoria, 
        T.id_tipo_transaccion AS IdTipoTransaccion
    FROM 
        Categorias C 
        INNER JOIN Tipos_Transacciones T ON T.id_tipo_transaccion = C.id_tipo_transaccion;
END
go
CREATE PROCEDURE SP_AGREGARCATEGORIA
    @nombre_categoria NVARCHAR(60),
    @icono_categoria NVARCHAR(60),
    @descripcion_categoria NVARCHAR(150),
    @id_tipo_transaccion INT
AS
BEGIN
    INSERT INTO Categorias (nombre_categoria, icono_categoria, descripcion_categoria, id_tipo_transaccion)
    VALUES (@nombre_categoria, @icono_categoria, @descripcion_categoria, (Select id_tipo_transaccion from Tipos_Transacciones where id_tipo_transaccion = @id_tipo_transaccion))
END
go
CREATE PROCEDURE SP_ELIMINARCATEGORIA
    @id_categoria INT
AS
BEGIN
    DELETE FROM Categorias
    WHERE id_categoria = @id_categoria
END

go
CREATE PROCEDURE SP_MODIFICARCATEGORIA
    @id_categoria INT,
    @nombre_categoria NVARCHAR(60),
    @icono_categoria NVARCHAR(60),
    @descripcion_categoria NVARCHAR(150),
    @id_tipo_transaccion INT
AS
BEGIN
    UPDATE Categorias
    SET 
        nombre_categoria = @nombre_categoria,
        icono_categoria = @icono_categoria,
        descripcion_categoria = @descripcion_categoria,
        id_tipo_transaccion = (Select id_tipo_transaccion from Tipos_Transacciones where id_tipo_transaccion = @id_tipo_transaccion)
    WHERE id_categoria = @id_categoria
END
go
CREATE PROCEDURE ObtenerTotalesPorTipoTransaccion
    @IdCuenta INT
AS
BEGIN
    SELECT t.nombre_tipo AS TipoTransaccion, SUM(tr.monto) AS Total
    FROM Transacciones tr
    INNER JOIN Categorias c ON tr.id_categoria = c.id_categoria
    INNER JOIN Tipos_Transacciones t ON c.id_tipo_transaccion = t.id_tipo_transaccion
    WHERE tr.id_cuenta = @IdCuenta
    GROUP BY t.nombre_tipo;
END;
--TRANSACCION

go
CREATE PROCEDURE SP_AGREGARTRANSACCION
    @id_categoria INT,
    @fecha DATETIME,
    @monto DECIMAL(25,5),
    @id_cuenta INT
AS
BEGIN
    INSERT INTO Transacciones (id_categoria, fecha, monto, id_cuenta)
    VALUES ((Select id_categoria from Categorias where id_categoria = @id_categoria), @Fecha, @Monto, (Select id_cuenta from CuentasBancarias where id_cuenta = @id_cuenta))
END
