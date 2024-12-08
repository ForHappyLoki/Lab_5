using CourceWork.Data;
using CourceWork.Services;
using CourceWork.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CourceWork.Controllers
{
    public class EmployeeController(DatabaseContext databaseContext, EmployeeServices employeeServices) 
        : Controller
    {
        private readonly DatabaseContext _db = databaseContext;
        private readonly EmployeeServices _employeeServices = employeeServices;
        public async Task<IActionResult> Index(int employeeId)
        {
            EmployeeModel employeeModel = await _employeeServices.GetEmployeeModel(employeeId);
            return View(employeeModel);
        }
    }
}
