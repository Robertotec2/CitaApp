# CitasApp — Arquitectura Hexagonal (Puertos y Adaptadores)

Proyecto unificado a partir de las 4 ramas de `ArqSoft-S05-Enrique`
(`master`, `Api`, `Api-Calculadora`, `Hexagonal`), reescrito desde cero
siguiendo Puertos y Adaptadores, con frontend Cyberpunk / Modern Night.

## Estructura

```
CitasApp/
├── CitasApp.sln
├── CitasApp.Domain/            Modelos, Puertos (interfaces), Excepciones de negocio
├── CitasApp.Application/       Casos de uso (Services) — reglas de negocio
├── CitasApp.Infrastructure/    Adaptadores: Json, Csv, Sqlite (intercambiables)
├── CitasApp.Api/                Adaptador de entrada: REST API + Swagger
└── CitasApp.Web/                Adaptador de entrada: MVC con UI Cyberpunk
```

Regla de dependencias (hexagonal, de afuera hacia adentro):

```
Api / Web  →  Application  →  Domain
Api / Web  →  Infrastructure → Domain
```

`Domain` no depende de nada. `Infrastructure` no depende de `Application` ni
de `Api`/`Web`. Cambiar el motor de persistencia nunca obliga a tocar
Domain, Application, ni los controladores.

## Cambiar el adaptador de persistencia (Json / Csv / Sqlite)

En `appsettings.json` de **CitasApp.Api** o **CitasApp.Web**:

```json
"Persistencia": {
  "Proveedor": "Json"   // o "Csv" / "Sqlite"
}
```

No se recompila nada mas: `DependencyInjection.AddCitasInfrastructure`
(en `CitasApp.Infrastructure`) lee ese valor y conecta el Adaptador correcto
a los Puertos del Dominio.

## Como compilar y ejecutar

### Opcion A — Visual Studio / Rider / IntelliJ con plugin .NET

1. Abre `CitasApp.sln`.
2. Click derecho en `CitasApp.Api` (o `CitasApp.Web`) → **Set as Startup Project**.
3. F5 / Run.
4. La API expone Swagger en `/swagger`. El Web corre en `/` con la UI Cyberpunk.

### Opcion B — Linea de comandos

```bash
cd CitasApp.Api
dotnet restore
dotnet build
dotnet run
# abre https://localhost:7080/swagger
```

```bash
cd CitasApp.Web
dotnet restore
dotnet build
dotnet run
# abre https://localhost:7090/
```

Ambos proyectos pueden correr al mismo tiempo (puertos distintos) porque
comparten Domain/Application/Infrastructure pero tienen su propia carpeta
`data/` y su propio `appsettings.json`.

## Manejo de excepciones

- `EntidadNoEncontradaException` → HTTP 404 (Api) / `NotFound()` o redirect con mensaje (Web)
- `OperacionInvalidaException` → HTTP 400 (Api) / mensaje de validacion en el formulario (Web)
- `PersistenciaException` (Infrastructure) → HTTP 500 generico, detalle solo en logs
- En la Api, todo pasa por `Middleware/ExceptionHandlingMiddleware.cs`.
- En el Web, los controladores atrapan las excepciones de Dominio puntualmente
  y el resto cae en `app.UseExceptionHandler("/Home/Error")`.

## Modulo Calculadora

Heredado de la rama `Api-Calculadora`. Vive como puerto de Dominio
(`ICalculadoraService`) implementado en Application (`CalculadoraService`),
expuesto tanto en la API (`/api/calculadora/...`) como en una vista del Web
(`/Calculadora`). Dividir entre cero lanza `OperacionInvalidaException` en
vez de la excepcion generica `DivideByZeroException`, para mantener
consistencia con el resto de las reglas de negocio.
