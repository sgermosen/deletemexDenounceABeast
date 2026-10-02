using DenounceBeasts.API.Data;
using DenounceBeasts.API.Models.Dtos;
using DenounceBeasts.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DenounceBeasts.API.Controllers
{
    [ApiController]
    [Route("api/sectors")]
    public class SectorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context; // Contexto de BD para acceder a tables

        public SectorsController(ApplicationDbContext context)
        {
            _context = context;
            // Nota: No inicializamos lista estática, los datos están en la BD.
        }

        // GET /api/sectors
        [HttpGet]
        public ActionResult<List<SectorDto>> GetAll()
        {
            var sectors = _context.Sectors.ToList();  // Recupera los sectores (entidades) de la base de datos
                                                      // Mapeo manual de entidades Sector a DTOs SectorDTO
            var sectorDTOs = sectors.Select(s => new SectorDto
            {
                Id = s.Id,
                Name = s.Name,
                MunicipalityId = s.MunicipalityId
            }).ToList();

            //var sectotsresponse = new List<SectorDto>();

            //foreach (var sector in sectors) {
            //    var sectorDTO = new SectorDto
            //    {
            //        Id = sector.Id,
            //        Name = sector.Name,
            //        MunicipalityId = sector.MunicipalityId
            //    };
            //    sectotsresponse.Add(sectorDTO);
            //}



            return Ok(sectorDTOs);
        }

        [HttpGet("{municipalityId}/sectors")]
        public ActionResult<List<SectorDto>> GetSectorsByMunicipality(int municipalityId)
        {
            // Verificar si el municipio existe
            bool exists = _context.Municipalities.Any(m => m.Id == municipalityId);
            if (!exists)
                return NotFound();

            // Obtener los sectores que pertenecen al municipio dado
            var sectors = _context.Sectors
                                  .Where(s => s.MunicipalityId == municipalityId)
                                  .ToList();

            // Mapear cada sector a SectorDTO (reutilizamos el DTO básico de Sector)
            var sectorDTOs = sectors.Select(s => new SectorDto
            {
                Id = s.Id,
                Name = s.Name,
                MunicipalityId = s.MunicipalityId
            }).ToList();

            return Ok(sectorDTOs);
        }



        [HttpGet("with-municipality")]
        public ActionResult<List<SectorWithMunicipalityDTO>> GetSectorsWithMunicipality()
        {
            // Obtenemos los sectores incluyendo (join) la info de Municipio
            var sectors = _context.Sectors
                                  .Include(s => s.Municipality)  // requerimos los datos del municipio
                                  .ToList();

            // Mapeamos cada sector a SectorWithMunicipalityDTO
            var resultList = sectors.Select(s => new SectorWithMunicipalityDTO
            {
                Id = s.Id,
                Name = s.Name,
                MunicipalityId = s.Municipality.Id,
                MunicipalityName = s.Municipality.Name
            }).ToList();

            return Ok(resultList);
        }


        //[HttpGet("GetAllx")] 
        //public ActionResult<List<SectorDto>> GetAllxxx()
        //{
        //    return Ok();
        //}


        // GET /api/sectors/{id}
        [HttpGet("{id}")]
        public ActionResult<SectorDto> GetById(int id)
        {
            var sector = _context.Sectors.Find(id);
            if (sector == null)
                return NotFound();
            // Mapeo de la entidad encontrada a DTO
            var sectorDTO = new SectorDto
            {
                Id = sector.Id,
                Name = sector.Name,
                MunicipalityId = sector.MunicipalityId
            };
            return Ok(sectorDTO);
        }


        // POST: api/sectors
        [HttpPost]
        public ActionResult<Sector> Create([FromBody] CreateSectorDTO request)
        {
            // Validaciones básicas de negocio
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("El nombre del sector es obligatorio.");
            }
            // Validar FK: el Municipio referido debe existir
            var municipalityExists = _context.Municipalities
                                             .Any(m => m.Id == request.MunicipalityId);
            if (!municipalityExists)
            {
                return BadRequest($"No existe un Municipio con Id = {request.MunicipalityId}.");
            }

            var sector = new Sector
            {
                Name = request.Name,
                MunicipalityId = request.MunicipalityId,
                IsActive = true
            };
            // Preparar entidad Sector para guardar (la BD asignará el Id automáticamente)
            sector.Id = 0; // (Opcional: aseguramos que EF Core sepa que debe generar un nuevo Id)
            _context.Sectors.Add(sector);
            _context.SaveChanges(); // Ejecuta el INSERT en la BD

            // sector.Id ahora tiene el valor generado en la base de datos (Identity)

            // Devolver respuesta 201 Created con el recurso creado
            return Ok(              // Nombre de la acción a usar en el Location header
                new { sector.Id }                   // Cuerpo de la respuesta: sector creado
            );
        }

        // PUT: api/sectors/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Sector sector)
        {
            if (id != sector.Id)
            {
                // El Id de la URL no coincide con el Id del cuerpo (por seguridad, para evitar incoherencias)
                return BadRequest("El ID de la URL no coincide con el ID del cuerpo de la solicitud.");
            }
            if (string.IsNullOrWhiteSpace(sector.Name))
            {
                return BadRequest("El nombre del sector no puede estar vacío.");
            }
            // Verificar existencia del sector a actualizar
            var existingSector = _context.Sectors.Find(id);
            if (existingSector == null)
            {
                return NotFound();
            }
            // Si se desea, verificar existencia del nuevo MunicipalityId (en caso de que se permita cambiarlo)
            var municipalityExists = _context.Municipalities.Any(m => m.Id == sector.MunicipalityId);
            if (!municipalityExists)
            {
                return BadRequest($"No existe un Municipio con Id = {sector.MunicipalityId}.");
            }

            // Actualizar campos del sector existente con los del objeto recibido
            existingSector.Name = sector.Name;
            existingSector.MunicipalityId = sector.MunicipalityId;
            existingSector.IsActive = sector.IsActive; // si la entidad tiene más campos, actualizarlos también

            //_context.Entry(existingSector).State = EntityState.Modified; // Marcar como modificado
          
            _context.Update(existingSector); // Alternativa: _context.Sectors.Update(existingSector);

            _context.SaveChanges(); // Aplicar cambios en la base de datos (UPDATE)

            return NoContent(); // 204 indicando que la actualización fue exitosa sin contenido extra
        }

        // DELETE: api/sectors/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var sector = _context.Sectors.Find(id);
            if (sector == null)
            {
                return NotFound();
            }

            _context.Sectors.Remove(sector);
            _context.SaveChanges(); // Ejecuta el DELETE en la BD

            return NoContent(); // 204, se eliminó correctamente
        }
    }


}
