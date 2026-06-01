using CareSync.Common;
using CareSync.Data;
using CareSync.Dtos;
using CareSync.Models;
using Microsoft.EntityFrameworkCore;

namespace CareSync.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly EmedicineContext _context;

        public MedicineService(EmedicineContext context)
        {
            _context = context;
        }
        public async Task<ServiceResponse<Medicine>> AddMedicineAsync(AddMedicineDto model)
        {
            var response = new ServiceResponse<Medicine>();

            string? imagePath = null;

            // Upload image if provided
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images");

                // Create folder if it doesn't exist
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.ImageFile.FileName);

                string filePath = Path.Combine(
                    uploadsFolder,
                    fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imagePath = $"/images/{fileName}";
            }

            var medicine = new Medicine
            {
                Name = model.Name,
                Manufacturer = model.Manufacturer,
                UnitPrice = model.UnitPrice,
                Discount = model.Discount,
                Quantity = model.Quantity,
                ExpDate = model.ExpDate,
                ImageUrl = imagePath,
                Status = 1
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            response.Data = medicine;
            response.Message = "Medicine added successfully.";

            return response;
        }

        public async Task<ServiceResponse<List<Medicine>>> GetMedicinesAsync()
        {
            return new ServiceResponse<List<Medicine>>
            {
                Data = await _context.Medicines.ToListAsync()
            };
        }
    }
}
