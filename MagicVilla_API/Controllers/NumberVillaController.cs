using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Abstractions;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumberVillaController : ControllerBase
    {
        private readonly ILogger<NumberVillaController> _logger;

        private readonly IVillaRepository _villaRepo;
        private readonly INumberVillaRepository _numberRepo;

        private readonly IMapper _mapper;

        protected APIResponse _response;

        //inyectamos las dependencias
        public NumberVillaController(ILogger<NumberVillaController> logger, IVillaRepository villaRepo, INumberVillaRepository numberRepo, IMapper mapper)
        {
            // inicializamos las dependencias
            _logger = logger;
            _villaRepo = villaRepo; 
            _numberRepo = numberRepo;
            _mapper = mapper;
            _response = new();
        }

        //GET de todas las villas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<APIResponse>> GetNumberVillas()
        {
            try
            {
                _logger.LogInformation("Obtener numeros de  villas");

                IEnumerable<NumberVilla> numberVillaList = await _numberRepo.GetAll();

                //con el automapper queremos retornar un objeto de tipo VillaDto, y es obtenido de villaList
                _response.Result = _mapper.Map<IEnumerable<NumberVillaDto>>(numberVillaList);
                _response.statusCode = HttpStatusCode.OK;

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response; 
            
        }

        //GET by ID
        [HttpGet("id", Name = "GetNumberVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetNumberVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al ingresar el id: " + id);
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var numberVilla = await _numberRepo.Get(v => v.VillaNo == id);

                if (numberVilla == null)
                {
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<NumberVillaDto>(numberVilla);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages  = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        //POST
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //               recibe parametros del Body para poder crear una villa con esas respectivas caracteristicas
        public async Task<ActionResult<APIResponse>> CreateNumberVilla([FromBody] NumberVillaCreateDto createDto)
        {
            try
            {
                //validacion de ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //Validacion personalizada
                if (await _numberRepo.Get(v => v.VillaNo == createDto.VillaNo) !=null)
                {//                     Nombre de la validacion         Mensaje de error
                    ModelState.AddModelError("NombreExiste", "El numero de Villa ya existe!");
                    return BadRequest(ModelState);
                }

                if(await _villaRepo.Get(v => v.Id == createDto.VillaId) == null)
                {
                    ModelState.AddModelError("ClaveForanea", "El ID de la Villa no existe!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                NumberVilla modelo = _mapper.Map<NumberVilla>(createDto);

                //metodo para añadir todos los nuevos datos a la BD
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;
                await _numberRepo.Create(modelo);
                _response.Result = modelo;
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetNumberVilla", new { id = modelo.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        //DELETE
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        // las interfaces no nos permiten poner un tipo. No agregamos APIResponse como tipo pero si podemos usar nuestra class response
        public async Task<IActionResult> DeleteNumberVilla(int id)
        {
            try
            {
                if (id  == 0)
                {
                    _response.isSuccess = false;
                    _response.statusCode =  HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                //buscamos la villa por su respectivo ID en el store
                var numberVilla = await _numberRepo.Get(v => v.VillaNo == id);

                if (numberVilla == null)
                {
                    _response.isSuccess = false;
                    _response.statusCode =  HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _numberRepo.Delete(numberVilla);

                //cuando trabajamos con DELETE debemos devolver un NoContent, ya que el modelo no existirá en el registro.
                _response.statusCode =  HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.isSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return BadRequest(_response);

        }

        //PUT

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] NumberVillaUpdateDto updateDto)
        {
            if(updateDto == null || id != updateDto.VillaNo)
            {
                _response.isSuccess = false;
                _response.statusCode =  HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if(await _villaRepo.Get(v => v.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("ClaveForanea", "El Id de la Villa no existe");
                return BadRequest(ModelState);
            }

            NumberVilla modelo = _mapper.Map<NumberVilla>(updateDto);

            await _numberRepo.Refresh(modelo);

            _response.statusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}
