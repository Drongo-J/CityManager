using AutoMapper;
using CityManagerApi.Data;
using CityManagerApi.Dtos;
using CityManagerApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityManagerApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private IAppRepository _appRepository;
        private IMapper _mapper;
        public CitiesController(IAppRepository appRepository, IMapper mapper)
        {
            _appRepository = appRepository;
            _mapper = mapper;
        }

        [HttpGet("{id?}")]
        public IActionResult GetCities(int id=3)
        {
            //var cities = _appRepository.GetCities(3)
            //    .Select(c => new CityForListDto
            //    {
            //         Description = c.Description,
            //          Id=c.Id,
            //           Name = c.Name,
            //           PhotoUrl=c.Photos.FirstOrDefault(p=>p.IsMain).Url
            //    });

            var cities = _appRepository.GetCities(id);
            var citiesToReturn = _mapper.Map<List<CityForListDto>>(cities);

            return Ok(citiesToReturn);
        }

        [HttpPost]
        [Route("add")]
        public IActionResult Add(CityDto dto)
        {
            var item=_mapper.Map<City>(dto);
            _appRepository.Add(item);
            _appRepository.SaveAll();
            return Ok(item);
        }

        [HttpGet("Detail")]
        public IActionResult GetCityById(int id)
        {
            var city=_appRepository.GetCityById(id);
            var cityToReturn = _mapper.Map<CityForDetailDto>(city);
            return Ok(cityToReturn);
        }

        [HttpGet("Photos/{cityId}")]
        public IActionResult GetPhotosByCityId(int cityId)
        {
            var photos=_appRepository.GetPhotosByCityId(cityId);
            return Ok(photos);
        }

        [HttpGet("SinglePhoto/{id}")]
        public IActionResult GetPhotoById(int id)
        {
            var photos = _appRepository.GetPhotoById(id);
            return Ok(photos);
        }
    }
}
