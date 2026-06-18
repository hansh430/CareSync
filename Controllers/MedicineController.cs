using CareSync.Dtos;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMedicinesById(int id)
        {
            var result = await _medicineService.GetMedicineByIdAsync(id);
            if (!result.Success)
                return NotFound(result.Message);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, [FromForm] UpdateMedicineDto model)
        {
            var result = await _medicineService
                .UpdateMedicineAsync(id, model);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
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
