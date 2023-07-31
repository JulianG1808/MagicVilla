using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public ActionResult<IEnumerable<VillaDto>> GetVillas()
        {
            return Ok(VillaStore.VillaList);
        }

        [HttpGet("id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<VillaDto> GetVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(villa);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaDto)
        {
            if (villaDto == null)
            {
                return BadRequest(villaDto);
            }

            if(villaDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            villaDto.Id = VillaStore.VillaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            VillaStore.VillaList.Add(villaDto);

            return Ok(villaDto);
        }
    }
}
