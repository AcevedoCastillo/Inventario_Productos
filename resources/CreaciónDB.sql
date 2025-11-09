CREATE DATABASE SistemaVentas;
GO

USE SistemaVentas;
GO


CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    NombreUsuario VARCHAR(50) NOT NULL UNIQUE,
    Contrasena VARBINARY(256) NOT NULL,  -- Contraseña encriptada
    NombreCompleto VARCHAR(100) NOT NULL,
    Rol VARCHAR(20) NOT NULL CHECK (Rol IN ('Administrador', 'Operador')),
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Productos (
    IdPro INT PRIMARY KEY IDENTITY(1,1),
    Codigo VARCHAR(20) NOT NULL UNIQUE,
    Producto VARCHAR(100) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL CHECK (Precio >= 0),
    Stock INT NOT NULL DEFAULT 0,
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE()
);
GO


CREATE TABLE Ventas (
    IdVenta INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Vendedor VARCHAR(100) NOT NULL,
    SubTotal DECIMAL(10,2) NOT NULL,
    TotalIVA DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    IdUsuario INT NOT NULL,
    CONSTRAINT FK_Ventas_Usuarios FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario)
);
GO

CREATE TABLE DetalleVentas (
    IdDe INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    IdVenta INT NOT NULL,
    IdPro INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    Precio DECIMAL(10,2) NOT NULL,
    IVA DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_DetalleVentas_Ventas FOREIGN KEY (IdVenta) REFERENCES Ventas(IdVenta),
    CONSTRAINT FK_DetalleVentas_Productos FOREIGN KEY (IdPro) REFERENCES Productos(IdPro)
);
GO

CREATE INDEX IX_DetalleVentas_IdVenta ON DetalleVentas(IdVenta);
CREATE INDEX IX_DetalleVentas_IdPro ON DetalleVentas(IdPro);
CREATE INDEX IX_Ventas_Fecha ON Ventas(Fecha);
CREATE INDEX IX_Productos_Codigo ON Productos(Codigo);
GO

-- =============================================
-- Contraseña: "admin123" y "operador123"
-- =============================================

INSERT INTO Usuarios (NombreUsuario, Contrasena, NombreCompleto, Rol)
VALUES 
('admin', HASHBYTES('SHA2_256', 'admin123'), 'Administrador Principal', 'Administrador'),
('operador1', HASHBYTES('SHA2_256', 'operador123'), 'Juan Pérez', 'Operador');
GO


INSERT INTO Productos (Codigo, Producto, Precio, Stock)
VALUES 
('001', 'Lapicero BIC color azul', 0.75, 100),
('063', 'Cuaderno cuadriculado', 3.50, 50),
('010', 'Caja de colores facela', 4.10, 30),
('015', 'Borrador blanco', 0.50, 200),
('020', 'Regla de 30cm', 1.25, 75);
GO


-- Validar Login de Usuario
CREATE PROCEDURE SP_ValidarUsuario
    @NombreUsuario VARCHAR(50),
    @Contrasena VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ContrasenaHash VARBINARY(256);
    SET @ContrasenaHash = HASHBYTES('SHA2_256', @Contrasena);
    
    SELECT 
        IdUsuario,
        NombreUsuario,
        NombreCompleto,
        Rol,
        Activo
    FROM Usuarios
    WHERE NombreUsuario = @NombreUsuario 
        AND Contrasena = @ContrasenaHash
        AND Activo = 1;
END;
GO

-- Listar Usuarios Activos
CREATE PROCEDURE SP_ListarUsuarios
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdUsuario,
        NombreUsuario,
        NombreCompleto,
        Rol,
        Activo,
        FechaCreacion
    FROM Usuarios
    WHERE Activo = 1
    ORDER BY NombreCompleto;
END;
GO

-- Crear Usuario
CREATE PROCEDURE SP_CrearUsuario
    @NombreUsuario VARCHAR(50),
    @Contrasena VARCHAR(100),
    @NombreCompleto VARCHAR(100),
    @Rol VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @ContrasenaHash VARBINARY(256);
        SET @ContrasenaHash = HASHBYTES('SHA2_256', @Contrasena);
        
        INSERT INTO Usuarios (NombreUsuario, Contrasena, NombreCompleto, Rol)
        VALUES (@NombreUsuario, @ContrasenaHash, @NombreCompleto, @Rol);
        
        SELECT SCOPE_IDENTITY() AS IdUsuario;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Listar Todos los Productos
CREATE PROCEDURE SP_ListarProductos
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPro,
        Codigo,
        Producto,
        Precio,
        Stock,
        Activo,
        FechaCreacion
    FROM Productos
    WHERE Activo = 1
    ORDER BY Producto;
END;
GO

-- Buscar Producto por ID
CREATE PROCEDURE SP_ObtenerProductoPorId
    @IdPro INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPro,
        Codigo,
        Producto,
        Precio,
        Stock,
        Activo,
        FechaCreacion
    FROM Productos
    WHERE IdPro = @IdPro AND Activo = 1;
END;
GO

-- Buscar Producto por Código
CREATE PROCEDURE SP_BuscarProductoPorCodigo
    @Codigo VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        IdPro,
        Codigo,
        Producto,
        Precio,
        Stock,
        Activo,
        FechaCreacion
    FROM Productos
    WHERE Codigo = @Codigo AND Activo = 1;
END;
GO

-- Crear Producto
CREATE PROCEDURE SP_CrearProducto
    @Codigo VARCHAR(20),
    @Producto VARCHAR(100),
    @Precio DECIMAL(10,2),
    @Stock INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el código no exista
        IF EXISTS (SELECT 1 FROM Productos WHERE Codigo = @Codigo AND Activo = 1)
        BEGIN
            RAISERROR('El código de producto ya existe.', 16, 1);
            RETURN;
        END
        
        INSERT INTO Productos (Codigo, Producto, Precio, Stock)
        VALUES (@Codigo, @Producto, @Precio, @Stock);
        
        SELECT SCOPE_IDENTITY() AS IdPro;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Actualizar Producto
CREATE PROCEDURE SP_ActualizarProducto
    @IdPro INT,
    @Codigo VARCHAR(20),
    @Producto VARCHAR(100),
    @Precio DECIMAL(10,2),
    @Stock INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar que el código no exista en otro producto
        IF EXISTS (SELECT 1 FROM Productos WHERE Codigo = @Codigo AND IdPro != @IdPro AND Activo = 1)
        BEGIN
            RAISERROR('El código de producto ya existe en otro registro.', 16, 1);
            RETURN;
        END
        
        UPDATE Productos
        SET 
            Codigo = @Codigo,
            Producto = @Producto,
            Precio = @Precio,
            Stock = @Stock
        WHERE IdPro = @IdPro AND Activo = 1;
        
        SELECT @@ROWCOUNT AS FilasAfectadas;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Eliminar Producto (Lógico)
CREATE PROCEDURE SP_EliminarProducto
    @IdPro INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validar si el producto tiene ventas asociadas
        IF EXISTS (SELECT 1 FROM DetalleVentas WHERE IdPro = @IdPro)
        BEGIN
            RAISERROR('No se puede eliminar el producto porque tiene ventas asociadas.', 16, 1);
            RETURN;
        END
        
        UPDATE Productos
        SET Activo = 0
        WHERE IdPro = @IdPro;
        
        SELECT @@ROWCOUNT AS FilasAfectadas;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO


-- Crear Venta con Detalle (Transacción Completa)
CREATE PROCEDURE SP_CrearVenta
    @Vendedor VARCHAR(100),
    @SubTotal DECIMAL(10,2),
    @TotalIVA DECIMAL(10,2),
    @Total DECIMAL(10,2),
    @IdUsuario INT,
    @DetallesVenta NVARCHAR(MAX) -- JSON con los detalles
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @IdVenta INT;
        DECLARE @Fecha DATETIME = GETDATE();
        
        -- Insertar encabezado de venta
        INSERT INTO Ventas (Fecha, Vendedor, SubTotal, TotalIVA, Total, IdUsuario)
        VALUES (@Fecha, @Vendedor, @SubTotal, @TotalIVA, @Total, @IdUsuario);
        
        SET @IdVenta = SCOPE_IDENTITY();
        
        -- Insertar detalles desde JSON
        INSERT INTO DetalleVentas (Fecha, IdVenta, IdPro, Cantidad, Precio, IVA, Total)
        SELECT 
            @Fecha,
            @IdVenta,
            IdPro,
            Cantidad,
            Precio,
            IVA,
            Total
        FROM OPENJSON(@DetallesVenta)
        WITH (
            IdPro INT '$.IdPro',
            Cantidad INT '$.Cantidad',
            Precio DECIMAL(10,2) '$.Precio',
            IVA DECIMAL(10,2) '$.IVA',
            Total DECIMAL(10,2) '$.Total'
        );
        
        -- Actualizar stock de productos
        UPDATE P
        SET P.Stock = P.Stock - D.Cantidad
        FROM Productos P
        INNER JOIN (
            SELECT IdPro, Cantidad
            FROM OPENJSON(@DetallesVenta)
            WITH (
                IdPro INT '$.IdPro',
                Cantidad INT '$.Cantidad'
            )
        ) D ON P.IdPro = D.IdPro;
        
        COMMIT TRANSACTION;
        
        SELECT @IdVenta AS IdVenta;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- Listar Ventas
CREATE PROCEDURE SP_ListarVentas
    @FechaInicio DATETIME = NULL,
    @FechaFin DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        V.IdVenta,
        V.Fecha,
        V.Vendedor,
        V.SubTotal,
        V.TotalIVA,
        V.Total,
        U.NombreCompleto AS NombreUsuario
    FROM Ventas V
    INNER JOIN Usuarios U ON V.IdUsuario = U.IdUsuario
    WHERE (@FechaInicio IS NULL OR V.Fecha >= @FechaInicio)
        AND (@FechaFin IS NULL OR V.Fecha <= @FechaFin)
    ORDER BY V.Fecha DESC;
END;
GO

-- Obtener Venta por ID con Detalle
CREATE PROCEDURE SP_ObtenerVentaPorId
    @IdVenta INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Encabezado
    SELECT 
        V.IdVenta,
        V.Fecha,
        V.Vendedor,
        V.SubTotal,
        V.TotalIVA,
        V.Total,
        V.IdUsuario,
        U.NombreCompleto AS NombreUsuario
    FROM Ventas V
    INNER JOIN Usuarios U ON V.IdUsuario = U.IdUsuario
    WHERE V.IdVenta = @IdVenta;
    
    -- Detalle
    SELECT 
        DV.IdDe,
        DV.Fecha,
        DV.IdVenta,
        DV.IdPro,
        P.Codigo,
        P.Producto,
        DV.Cantidad,
        DV.Precio,
        DV.IVA,
        DV.Total
    FROM DetalleVentas DV
    INNER JOIN Productos P ON DV.IdPro = P.IdPro
    WHERE DV.IdVenta = @IdVenta;
END;
GO

-- Obtener Detalle de Ventas por Fecha
CREATE PROCEDURE SP_ObtenerDetalleVentasPorFecha
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        V.IdVenta,
        V.Fecha,
        V.Vendedor,
        P.Codigo,
        P.Producto,
        DV.Cantidad,
        DV.Precio,
        DV.IVA,
        DV.Total,
        V.Total AS TotalVenta
    FROM DetalleVentas DV
    INNER JOIN Ventas V ON DV.IdVenta = V.IdVenta
    INNER JOIN Productos P ON DV.IdPro = P.IdPro
    WHERE V.Fecha >= @FechaInicio AND V.Fecha <= @FechaFin
    ORDER BY V.Fecha DESC, V.IdVenta;
END;
GO


-- Reporte de Ventas por Período
CREATE PROCEDURE SP_ReporteVentas
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        V.IdVenta AS 'NoVenta',
        CONVERT(VARCHAR(10), V.Fecha, 103) AS 'Fecha',
        V.Vendedor,
        COUNT(DV.IdDe) AS 'CantidadProductos',
        V.SubTotal,
        V.TotalIVA AS 'IVA',
        V.Total
    FROM Ventas V
    LEFT JOIN DetalleVentas DV ON V.IdVenta = DV.IdVenta
    WHERE V.Fecha >= @FechaInicio AND V.Fecha <= @FechaFin
    GROUP BY V.IdVenta, V.Fecha, V.Vendedor, V.SubTotal, V.TotalIVA, V.Total
    ORDER BY V.Fecha DESC;
END;
GO

-- Reporte Detallado de Ventas
CREATE PROCEDURE SP_ReporteDetalladoVentas
    @FechaInicio DATETIME,
    @FechaFin DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        V.IdVenta AS 'NoVenta',
        CONVERT(VARCHAR(10), V.Fecha, 103) AS 'Fecha',
        U.NombreCompleto AS Vendedor,
        P.Codigo,
        P.Producto,
        DV.Cantidad,
        DV.Precio AS 'PrecioUnitario',
        DV.IVA,
        DV.Total AS 'TotalProducto',
        V.Total AS 'TotalVenta'
    FROM DetalleVentas DV
    INNER JOIN Ventas V ON DV.IdVenta = V.IdVenta
    INNER JOIN Productos P ON DV.IdPro = P.IdPro
	INNER JOIN Usuarios U ON V.IdUsuario = U.IdUsuario
    WHERE V.Fecha >= @FechaInicio AND V.Fecha <= @FechaFin
    ORDER BY V.Fecha DESC, V.IdVenta, P.Producto;
END;
GO

-- Productos Más Vendidos
CREATE PROCEDURE SP_ProductosMasVendidos
    @FechaInicio DATETIME = NULL,
    @FechaFin DATETIME = NULL,
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@Top)
        P.Codigo,
        P.Producto,
        SUM(DV.Cantidad) AS 'Total Vendido',
        SUM(DV.Total) AS 'Ingresos',
        COUNT(DISTINCT DV.IdVenta) AS 'Número de Ventas'
    FROM DetalleVentas DV
    INNER JOIN Productos P ON DV.IdPro = P.IdPro
    INNER JOIN Ventas V ON DV.IdVenta = V.IdVenta
    WHERE (@FechaInicio IS NULL OR V.Fecha >= @FechaInicio)
        AND (@FechaFin IS NULL OR V.Fecha <= @FechaFin)
    GROUP BY P.Codigo, P.Producto
    ORDER BY SUM(DV.Cantidad) DESC;
END;
GO


CREATE PROCEDURE SP_VerificarStock
    @IdPro INT,
    @Cantidad INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @StockActual INT;
    
    SELECT @StockActual = Stock
    FROM Productos
    WHERE IdPro = @IdPro AND Activo = 1;
    
    IF @StockActual IS NULL
    BEGIN
        SELECT 0 AS Disponible, 'Producto no encontrado' AS Mensaje;
        RETURN;
    END
    
    IF @StockActual >= @Cantidad
        SELECT 1 AS Disponible, @StockActual AS StockActual, 'Stock disponible' AS Mensaje;
    ELSE
        SELECT 0 AS Disponible, @StockActual AS StockActual, 'Stock insuficiente' AS Mensaje;
END;
GO
