# ADR-01: Incorporacion de API REST con ASP.NET Core Web API

## Fecha
2026-06-19

## Estado
Aceptado

## Contexto
El proyecto CitasApp necesita exponer su logica de negocio (gestion de pacientes, medicos y citas) a clientes externos — aplicaciones moviles, frontends desacoplados u otros servicios. Hasta este punto, la unica interfaz de entrada era la aplicacion MVC (CitasApp.Web), que acopla la presentacion con la logica y no puede ser consumida por terceros de forma estandarizada.

## Decision
Se incorpora una API REST usando ASP.NET Core Web API como nuevo Adaptador de Entrada dentro de la Arquitectura Hexagonal existente. La API expone los mismos Casos de Uso (PacienteService, MedicoService, CitaService, CalculadoraService) que ya usa CitasApp.Web, sin duplicar logica de negocio. Swagger (Swashbuckle) se configura como herramienta de documentacion y prueba de endpoints.

## Alternativas consideradas

### GraphQL
Permite consultas flexibles y reduce el over-fetching. Se descarto porque agrega complejidad de implementacion (HotChocolate o similar) que no esta justificada para el alcance actual del proyecto. REST es suficiente para las operaciones CRUD requeridas.

### gRPC
Excelente rendimiento para comunicacion entre microservicios. Se descarto porque el cliente principal es un navegador web, y gRPC requiere un proxy adicional (grpc-web) para funcionar en ese contexto. Ademas, la curva de aprendizaje de Protocol Buffers no aporta valor en esta etapa.

### Minimal API (ASP.NET Core)
Mas ligero que Controllers. Se descarto en favor de Controllers porque la convencion de [ApiController] + [Route] genera documentacion Swagger mas clara y organizada para proyectos con multiples recursos.

## Consecuencias positivas
- Los endpoints quedan documentados automaticamente con Swagger, accesible en /swagger.
- La logica de negocio no se duplica: la API reutiliza los mismos Services que la vista MVC.
- El patron Ports & Adapters se mantiene limpio: la API es solo otro Adaptador de Entrada que llama a los mismos Puertos.
- Se puede cambiar el motor de persistencia (Json/Csv/Sqlite) sin tocar ningun controlador.

## Consecuencias negativas / riesgos
- Se agrega un proyecto mas a la solucion, aumentando la superficie de mantenimiento.
- Sin autenticacion (JWT o similar), los endpoints son publicos — aceptable para el alcance academico actual.

## Endpoints implementados

| Metodo | Ruta | Descripcion |
|--------|------|-------------|
| GET | /api/Pacientes | Lista todos los pacientes |
| GET | /api/Pacientes/{id} | Obtiene un paciente por ID |
| GET | /api/Medicos | Lista todos los medicos |
| GET | /api/Medicos/{id} | Obtiene un medico por ID |
| GET | /api/Citas | Lista todas las citas |
| GET | /api/Citas/{id} | Obtiene una cita por ID |
| GET | /api/Citas/porpaciente/{pacienteId} | Citas de un paciente especifico |
| POST | /api/Citas | Agenda una nueva cita |
| PATCH | /api/Citas/{id}/confirmar | Confirma una cita existente |
| GET | /api/Calculadora/sumar | Suma dos numeros |
| GET | /api/Calculadora/restar | Resta dos numeros |
| GET | /api/Calculadora/multiplicar | Multiplica dos numeros |
| GET | /api/Calculadora/dividir | Divide dos numeros (valida division entre cero) |

## Suite de Pruebas Automatizadas (Actividad #37)
* **Clases probadas:** Se implementaron pruebas unitarias con xUnit utilizando el patrón Arrange-Act-Assert para las clases `Cita`, `Paciente` y `Medico`.
* **Justificación:** Se seleccionaron por ser los pilares fundamentales del modelo de dominio en la arquitectura hexagonal, garantizando la integridad de la lógica de negocio antes de integrarse con la API y la interfaz web.
