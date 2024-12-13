# Smart Home Energy Management API

The Smart Home Energy Management API is a backend service that manages houses, apartments, apartment complexes, and their associated devices. It provides endpoints to view, add, and control devices, as well as retrieve analytics on energy consumption. 

This application demonstrates backend best practices including:
- **Clean Architecture:** Clear separation of entities, DTOs, and controllers.
- **Data Validation:** Automatic validation with `DataAnnotations` and `[ApiController]`.
- **Documentation:** Comprehensive XML comments, Swagger annotations, and a well-structured API.
- **Scalability and Flexibility:** Ready for future enhancements like authentication, authorization, and frontend integration.

## Features

- **Houses, Apartments, and ApartmentComplexes:**  
  Manage various types of residential units and retrieve information about their devices.
  
- **Devices Management:**  
  Add new devices to houses or apartments, toggle devices on/off, and retrieve top-consuming devices.
  
- **Analytics:**  
  Get total energy consumption for a house or apartment complex, list devices currently turned on, and filter or search for specific houses or apartments.
  
- **Data Validation & Documentation:**  
  Input is validated automatically. Swagger UI and XML documentation help consumers understand the API.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQL Server (LocalDB, SQL Express, or full instance)

## Setup

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/fadezilla/smart-home-energy-management.git
   cd smart-home-energy-management/backend
2. **Restore Dependencies:**
    dotnet restore
3. **Database Setup: Update the connection string in appsettings.json if needed. Then apply migrations:**
    dotnet ef database update
4. **Run the Application:**
    dotnet run
    By default, the application will run on http://localhost:5000.

## API Documentation

**Swagger UI:**
Open http://localhost:5000/swagger to view all endpoints, their parameters, response codes, and model schemas.

**XML Comments and Swagger Annotations:**
Each endpoint includes XML comments and [SwaggerResponse] attributes, offering a rich documentation experience within the Swagger UI.

## Key Endpoints

**Houses**
1. GET /api/houses: List all houses with devices.
2. GET /api/houses/{id}: Get a single house by ID.
3. POST /api/houses/{id}/add-device: Add a device to a house.
4. GET /api/houses/{id}/top-devices: Get top 5 energy-consuming devices in a house.
5. GET /api/houses/search?name=HouseName: Search houses by name.
6. GET /api/houses/{id}/on-devices: List devices that are currently on in a specific house.
7. GET /api/houses/{id}/total-energy: Get total energy consumption for a house.

**Apartments**
1. GET /api/apartments: List all apartments with devices.
2. GET /api/apartments/{id}: Get a single apartment by ID.
3. POST /api/apartments/{id}/add-device: Add a device to an apartment.
4. GET /api/apartments/{id}/top-devices: Top 5 devices by energy usage in an apartment.
5. GET /api/apartments/{id}/on-devices: Devices currently on in an apartment.

**Apartment Complexes**
1. GET /api/apartmentcomplexes: List all complexes, their apartments, and devices.
2. GET /api/apartmentcomplexes/{id}: Get a single complex by ID.
3. GET /api/apartmentcomplexes/{id}/top-devices: Top 5 devices by consumption across the complex.
4. GET /api/apartmentcomplexes/{id}/on-devices: Devices that are on in the entire complex.
5. GET /api/apartmentcomplexes/{id}/total-energy: Total energy consumption for the complex.

**Devices**
1. GET /api/devices: List all devices.
2. GET /api/devices/{id}: Get a specific device.
3. POST /api/devices/{id}/toggle: Toggle a device on/off.
4. GET /api/devices/on: List all devices currently turned on.

## Data Validation
1. DTOs for input (e.g., AddDeviceDto) are validated automatically.
2. If validation fails, the API returns a 400 Bad Request with detailed error messages.

## License
This project is licensed under the MIT License.