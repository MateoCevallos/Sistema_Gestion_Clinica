# Sistema Gestión Clínica

Sistema web para la gestión básica de una clínica.

Incluye los módulos:
- Pacientes
- Médicos
- Citas
- Reportes 

## Tecnologías

- Backend: ASP.NET Core Web API (.NET) + Entity Framework Core  
- Frontend: Angular (Standalone)  
- Base de datos: MySQL  

---

## Requisitos

- .NET SDK
- Node.js + npm
- Angular CLI
- MySQL

---

## Ejecución del proyecto

### Backend
1. Ubicarse en la terminal
2. Ejecutar:
   cd .\backend\SistemaGestionClinica.Api\
   dotnet run

## Frontend
1. Ubicarse en una nueva terminal
2. Ejecutar:
   cd .\frontend\sistema-gestion-clinica\
3. Si aparece error de paquetes (dado que no se incluye node_modules)
   Ejecutar:
   npm install
   ng serve -o


