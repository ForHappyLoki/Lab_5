using CourceWork.Data;
using CourceWork.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace CourceWork.Services
{
    public class UserService
    {
        private DatabaseContext db;
        public IMemoryCache cache;
        public UserService(DatabaseContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }
        public async Task<IEnumerable<Employee>> GetUsers()
        {
            return SortUsers(await db.Employees.ToListAsync());
        }
        public async Task AddUser(Employee user)
        {
            db.Employees.Add(user);
            int n = await db.SaveChangesAsync();
            if (n > 0)
            {
                cache.Set(user.EmployeeId, user, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }
        public async Task UpdateUser(Employee user)
        {
            // Получаем существующего пользователя из базы данных
            var existingUser = await db.Employees.FindAsync(user.EmployeeId);
            if (existingUser != null)
            {
                // Обновляем поля существующего пользователя
                existingUser.FullName = user.FullName;
                existingUser.HireDate = user.HireDate;
                existingUser.Position = user.Position;
                existingUser.Login = user.Login;
                existingUser.Password = user.Password;
                existingUser.Role = user.Role;

                // Сохраняем изменения
                await db.SaveChangesAsync();

                // Обновляем данные в кэше
                cache.Set($"user_{existingUser.EmployeeId}", existingUser, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }
        public async Task<Employee> GetUser(int id)
        {
            Employee user = null;
            if (!cache.TryGetValue(id, out user))
            {
                user = await db.Employees.FirstOrDefaultAsync(p => p.EmployeeId == id);
                if (user != null)
                {
                    cache.Set($"user_{user.EmployeeId}", user,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return user;
        }
        public async Task<IEnumerable<Employee>> GetUser(string name)
        {
            // Попытка получить сотрудников по префиксу из кэша
            if (cache.TryGetValue($"fullName_{name}", out IEnumerable<Employee> users))
            {
                return SortUsers(users);
            }

            // Если поиск по префиксу не дал результатов, ищем по полному имени
            users = await db.Employees
                .Where(u => u.FullName.Contains(name)) // Поиск по полному имени
                .ToListAsync();

            // Сохраняем результаты в кэш, используя ключ, основанный на полном имени
            if (users.Any())
            {
                cache.Set($"fullName_{name}", users, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }

            return users;
        }
        private IEnumerable<Employee> SortUsers(IEnumerable<Employee> users)
        {
            return users.OrderBy(u => u.FullName).ToList();
        }
        public async Task<IEnumerable<Employee>> GetEmployeesStartingWith(string prefix)
        {
            // Получаем всех сотрудников из базы данных, если кэш пуст
            var allEmployees = new List<Employee>();

            for (int i = 1; i <= await db.Employees.CountAsync(); i++)
            {
                if (cache.TryGetValue($"user_{i}", out Employee employee))
                {
                    allEmployees.Add(employee);
                }
                else
                {
                    // Если сотрудник не найден в кэше, запрашиваем из базы данных
                    employee = await db.Employees.FindAsync(i);
                    if (employee != null)
                    {
                        // Кэшируем сотрудника
                        cache.Set($"user_{i}", employee, new MemoryCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                        });
                        allEmployees.Add(employee);
                    }
                }
            }

            // Фильтрация сотрудников по имени
            return SortUsers(allEmployees.Where(e => e.FullName.Contains(prefix)).ToList());
        }
        [HttpGet]
        public async Task Delete(int employeeId)
        {
            // Найти сотрудника по ID в базе данных
            var employee = await db.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                var tvShowEmployees = db.TvshowEmployees.Where(te => te.EmployeeId == employeeId);
                if (tvShowEmployees.Count() > 0)
                {
                    db.TvshowEmployees.RemoveRange(tvShowEmployees); // Удаляем все найденные записи
                }
                // Удаляем сотрудника из базы данных
                db.Employees.Remove(employee);
                await db.SaveChangesAsync(); // Сохраняем изменения

                // Удаляем из кеша, используя тот же ключ
                cache.Remove($"user_{employee.EmployeeId}"); 
            }
        }
    }
}
