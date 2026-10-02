using DenounceBeasts.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DenounceBeasts.API.Controllers
{
    [ApiController]
    [Route("api/sectors")]
    public class SectorsController : ControllerBase
    {
        private static readonly List<Sector> _sectors = new List<Sector>
        {
            new Sector { Id = 1, Name = "Zona Colonial", MunicipalityId = 1, IsActive = true },
            new Sector { Id = 2, Name = "Gascue", MunicipalityId = 1, IsActive = true },
            new Sector { Id = 3, Name = "Cienfuegos", MunicipalityId = 2, IsActive = true }
        };

        [HttpGet] // GET: api/sectors
        public ActionResult<IEnumerable<Sector>> GetAll()
        {
            return Ok(_sectors);
        }

        [HttpGet("{id}")] // GET: api/sectors/5
        public ActionResult<Sector> GetById(int id)
        {
            var sector = _sectors.FirstOrDefault(s => s.Id == id);
            if (sector == null)
                return NotFound();
            return Ok(sector);
        }

        [HttpPost] // POST: api/sectors
        public ActionResult<Sector> Create(Sector sector)
        {
            if (string.IsNullOrWhiteSpace(sector.Name))
            {
                return BadRequest("Name of sector is required.");
            }
            if (sector.MunicipalityId <= 0)
            {
                return BadRequest("MunicipalityId must be provided and positive.");
            }
            // (Podríamos validar aquí que exista un Municipio con ese Id consultando la lista de municipios, 
            //  pero omitiremos esa comprobación en esta versión inicial.)

            int newId = _sectors.Any() ? _sectors.Max(s => s.Id) + 1 : 1;
            sector.Id = newId;
            sector.IsActive = true; // siempre creamos como activo
            _sectors.Add(sector);
            return CreatedAtAction(nameof(GetById), new { id = sector.Id }, sector);
        }

        [HttpPut("{id}")] // PUT: api/sectors/5
        public IActionResult Update(int id, Sector sector)
        {
            var existing = _sectors.FirstOrDefault(s => s.Id == id);
            if (existing == null)
                return NotFound();
            // Actualizar campos (excepto Id)
            existing.Name = sector.Name;
            existing.MunicipalityId = sector.MunicipalityId;
            existing.IsActive = sector.IsActive;
            return NoContent();
        }

        [HttpDelete("{id}")] // DELETE: api/sectors/5
        public IActionResult Delete(int id)
        {
            var existing = _sectors.FirstOrDefault(s => s.Id == id);
            if (existing == null)
                return NotFound();
            _sectors.Remove(existing);
            return NoContent();
        }
    }

}
