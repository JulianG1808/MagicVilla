﻿using AutoMapper;
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
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;

        private readonly IVillaRepository _villaRepo;

        private readonly IMapper _mapper;

        protected APIResponse _response;

        //inyectamos las dependencias
        public VillaController(ILogger<VillaController> logger, IVillaRepository villaRepo, IMapper mapper)
        {
            // inicializamos las dependencias
            _logger = logger;
            _villaRepo = villaRepo; 
            _mapper = mapper;
            _response = new();
        }

        //GET de todas las villas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obtener todas las villas");

                IEnumerable<Villa> villaList = await _villaRepo.GetAll();

                //con el automapper queremos retornar un objeto de tipo VillaDto, y es obtenido de villaList
                _response.Result = _mapper.Map<IEnumerable<VillaDto>>(villaList);
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
        [HttpGet("id", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetVilla(int id)
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

                //var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);

                var villa = await _villaRepo.Get(v => v.Id == id);

                if (villa == null)
                {
                    _response.isSuccess = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = _mapper.Map<VillaDto>(villa);
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
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                //validacion de ModelState
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                //Validacion personalizada
                if (await _villaRepo.Get(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) !=null)
                {//                     Nombre de la validacion         Mensaje de error
                    ModelState.AddModelError("NombreExiste", "la Villa con ese nombre ya existe");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }

                Villa modelo = _mapper.Map<Villa>(createDto);

                //metodo para añadir todos los nuevos datos a la BD
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;
                await _villaRepo.Create(modelo);
                _response.Result = modelo;
                _response.statusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response);
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
        public async Task<IActionResult> DeleteVilla(int id)
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
                var villa = await _villaRepo.Get(v => v.Id == id);

                if (villa == null)
                {
                    _response.isSuccess = false;
                    _response.statusCode =  HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _villaRepo.Delete(villa);

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
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if(updateDto == null || id != updateDto.Id)
            {
                _response.isSuccess = false;
                _response.statusCode =  HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            Villa modelo = _mapper.Map<Villa>(updateDto);

            await _villaRepo.Refresh(modelo);

            _response.statusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }

        //PATCH
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //var villa = VillaStore.VillaList.FirstOrDefault(v => v.Id == id);

            var villa = await _villaRepo.Get(v => v.Id == id, tracked:false);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            if (villa == null) return BadRequest();

            patchDto.ApplyTo(villaDto, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

            await _villaRepo.Refresh(modelo);

            _response.statusCode = HttpStatusCode.NoContent;
            return Ok(_response);
        }
    }
}
