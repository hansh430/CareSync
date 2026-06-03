using CareSync.Dtos;
using CareSync.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _medicineService;

        public MedicineController(IMedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMedicine([FromForm] AddMedicineDto model)
        {
            var result = await _medicineService.AddMedicineAsync(model);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMedicines()
        {
            var result = await _medicineService.GetMedicinesAsync();

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, UpdateMedicineDto model)
        {
            var result = await _medicineService
                .UpdateMedicineAsync(id, model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicine(int id)
        {
            var result = await _medicineService
                .DeleteMedicineAsync(id);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
