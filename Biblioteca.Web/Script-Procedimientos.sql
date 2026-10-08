USE BibliotecaDB;
GO

-- Añadir columna Anio si no existe (permite almacenar año de publicación)
IF COL_LENGTH('dbo.Libros', 'Anio') IS NULL
BEGIN
	ALTER TABLE dbo.Libros ADD Anio INT NULL;
END
GO

/* Procedimientos almacenados (CREATE OR ALTER) */
GO

-- Listar libros activos con nombre de autor (filtrado opcional por título)
CREATE OR ALTER PROCEDURE usp_Libros_ListarActivos
	@Titulo NVARCHAR(150) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SELECT L.LibroId AS Id,
		   L.Titulo,
		   L.AutorId,
		   A.Nombre AS AutorNombre,
		   L.Anio,
		   L.Ejemplares,
		   L.Activo
	FROM Libros L
	INNER JOIN Autores A ON L.AutorId = A.AutorId
	WHERE L.Activo = 1
	  AND (@Titulo IS NULL OR L.Titulo LIKE '%' + @Titulo + '%');
END
GO

-- Obtener libro por id
CREATE OR ALTER PROCEDURE usp_Libros_GetById
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	SELECT L.LibroId AS Id,
		   L.Titulo,
		   L.AutorId,
		   A.Nombre AS AutorNombre,
		   L.Anio,
		   L.Ejemplares,
		   L.Activo
	FROM Libros L
	INNER JOIN Autores A ON L.AutorId = A.AutorId
	WHERE L.LibroId = @Id;
END
GO

-- Insertar libro (devuelve NuevoId)
CREATE OR ALTER PROCEDURE usp_Libros_Insertar
	@Titulo NVARCHAR(150),
	@AutorId INT,
	@ISBN NVARCHAR(20) = '',
	@Anio INT = NULL,
	@Ejemplares INT = 0,
	@NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Libros (Titulo, ISBN, AutorId, Anio, Ejemplares)
	VALUES (@Titulo, @ISBN, @AutorId, @Anio, @Ejemplares);

	SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- Actualizar libro
CREATE OR ALTER PROCEDURE usp_Libros_Actualizar
	@Id INT,
	@Titulo NVARCHAR(150),
	@AutorId INT,
	@Anio INT = NULL,
	@Ejemplares INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE Libros
	SET Titulo = @Titulo,
		AutorId = @AutorId,
		Anio = @Anio,
		Ejemplares = @Ejemplares
	WHERE LibroId = @Id;
END
GO

-- Eliminación lógica
CREATE OR ALTER PROCEDURE usp_Libros_EliminarLogico
	@Id INT
AS
BEGIN
	SET NOCOUNT ON;
	UPDATE Libros SET Activo = 0 WHERE LibroId = @Id;
END
GO

-- Listar autores activos
CREATE OR ALTER PROCEDURE usp_Autores_ListarActivos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT AutorId AS Id, Nombre FROM Autores WHERE Activo = 1;
END
GO

-- Socios: listar activos
CREATE OR ALTER PROCEDURE usp_Socios_ListarActivos
AS
BEGIN
	SET NOCOUNT ON;
	SELECT SocioId AS Id, DNI, Nombre, Email, Activo FROM Socios WHERE Activo = 1;
END
GO

-- Insertar socio
CREATE OR ALTER PROCEDURE usp_Socios_Insertar
	@Nombre NVARCHAR(100),
	@DNI NVARCHAR(20),
	@Email NVARCHAR(100) = NULL,
	@NuevoId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	INSERT INTO Socios (DNI, Nombre, Email)
	VALUES (@DNI, @Nombre, @Email);
	SET @NuevoId = SCOPE_IDENTITY();
END
GO

-- Validar existencia de DNI (devuelve 1/0)
CREATE OR ALTER PROCEDURE usp_Socios_ExisteDNI
	@DNI NVARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT COUNT(1) FROM Socios WHERE DNI = @DNI;
END
GO

-- Reporte de préstamos por rango de fechas
CREATE OR ALTER PROCEDURE usp_Prestamos_ReportePorFechas
	@Desde DATETIME,
	@Hasta DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	SELECT P.PrestamoId AS PrestamoId,
		   S.Nombre AS SocioNombre,
		   L.Titulo AS LibroTitulo,
		   P.FechaPrestamo,
		   P.FechaLimite,
		   P.Estado
	FROM Prestamos P
	INNER JOIN Socios S ON P.SocioId = S.SocioId
	INNER JOIN DetallePrestamo DP ON P.PrestamoId = DP.PrestamoId
	INNER JOIN Libros L ON DP.LibroId = L.LibroId
	WHERE P.FechaPrestamo BETWEEN @Desde AND @Hasta
	ORDER BY P.FechaPrestamo DESC;
END
GO
