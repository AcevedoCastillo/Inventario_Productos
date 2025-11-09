# Sistema de Inventario y Ventas

Sistema completo de gestión de inventario y ventas desarrollado en .NET 8 con arquitectura API + MVC, SQL Server y procedimientos almacenados.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=c-sharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?style=flat&logo=microsoft-sql-server)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat&logo=bootstrap)

---

## 📑 Índice

- [Características](#-características)
- [Tecnologías](#-tecnologías)
- [Requisitos Previos](#-requisitos-previos)

- [Usuarios de Prueba](#-usuarios-de-prueba)


---

## ✨ Características

### 🔐 Autenticación y Roles
- Sistema de login con contraseñas encriptadas (SHA256)
- Dos roles: **Administrador** y **Operador**
- Control de acceso basado en roles
- Gestión de sesiones con timeout

### 📦 Gestión de Productos
- CRUD completo de productos
- Búsqueda por código en tiempo real
- Control de stock automático
- Validación de códigos únicos
- Eliminación lógica con validación de ventas asociadas

### 💰 Registro de Ventas
- Interfaz intuitiva estilo punto de venta
- Búsqueda rápida de productos con Enter
- Cálculo automático de IVA (13%)
- Validación de stock en tiempo real
- Tabla de detalle con totales dinámicos
- Confirmación visual con modal de éxito
- Operadores solo pueden registrar ventas

### 📊 Reportes
- Reporte de ventas por período personalizado
- Exportación profesional a PDF (QuestPDF)
- Exportación a Excel con formato (EPPlus)
- Totales y subtotales calculados
- Diseño corporativo rojo y blanco
- Filtros por fecha con validaciones

### 🎨 Diseño
- Interfaz moderna con Bootstrap 5
- Tema corporativo en rojo (#dc3545) y blanco
- Diseño responsive para móviles y tablets
- Notificaciones Toast con auto-cierre
- Animaciones y transiciones suaves
- Iconos Font Awesome 6.4

---

### Proyectos

1. **SistemaVentas.Core** (Class Library)
   - Entidades del dominio
   - DTOs para transferencia de datos
   - Interfaces de repositorios y servicios

2. **SistemaVentas.API** (Web API - .NET 8)
   - Controllers RESTful
   - Services con lógica de negocio
   - Repositories con acceso a datos
   - DbContext de Entity Framework
   - Generación de reportes PDF/Excel

3. **SistemaVentas.Web** (MVC - .NET 8)
   - Controllers MVC
   - Views Razor con Bootstrap
   - ViewModels
   - JavaScript para interactividad
   - Consumo de API con HttpClient

---

## 🛠️ Tecnologías

### Backend
- **.NET 8.0** - Framework principal
- **C# 12** - Lenguaje de programación
- **Entity Framework Core 8** - ORM para consultas
- **ADO.NET** - Ejecución directa de procedimientos almacenados
- **SQL Server 2019+** - Base de datos relacional

### Frontend
- **ASP.NET Core MVC** - Patrón arquitectónico

- **Bootstrap 5.3** - Framework CSS responsive
- **jQuery 3.6** - Manipulación DOM y AJAX
- **JavaScript ES6+** - Lógica del cliente

### Librerías de Reportes
- **QuestPDF 2024.10** - Generación de documentos PDF
- **EPPlus 7.0** - Generación de hojas de cálculo Excel
- **Newtonsoft.Json 13.0** - Serialización y deserialización JSON

### Seguridad
- **SHA256** - Encriptación de contraseñas
- **Parámetros SQL** - Prevención de inyección SQL
- **CORS** - Control de acceso entre orígenes
- **Validación en cliente y servidor** - Seguridad en capas

---

## 📋 Requisitos Previos

Antes de instalar el sistema, asegúrate de tener instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (versión 8.0 o superior)
- [SQL Server 2019+](https://www.microsoft.com/sql-server/sql-server-downloads) o SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (Community, Professional o Enterprise) 
  - O [Visual Studio Code](https://code.visualstudio.com/) con extensión C#
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) (Recomendado para gestión de BD)
- [Git](https://git-scm.com/) (Para clonar el repositorio)

---
### Configurar Connection Strings

#### **SistemaVentas.API/appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SistemaVentas;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
## 👥 Usuarios de Prueba

El sistema viene con usuarios preconfigurados para pruebas:

| Usuario | Contraseña | Rol | Permisos |
|---------|-----------|-----|----------|
| `admin` | `admin123` | Administrador | • Acceso completo<br>• Gestión de productos<br>• Registro de ventas<br>• Reportes<br>• Dashboard |
| `operador1` | `operador123` | Operador | • Solo registro de ventas<br>• No puede ver productos<br>• No puede ver reportes |

