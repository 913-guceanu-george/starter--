using api.Data;
using api.DTOs;
using api.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public UserController(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        if (users == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<IList<UserDTO>>(users));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<UserDTO>(user));
    }
    [HttpPost]
    public async Task<IActionResult> PostUser(AddUserDTO addUserDTO)
    {
        var userExists = await _context.Users.AnyAsync(u => u.FirstName.ToLower() == addUserDTO.FirstName.ToLower() && u.LastName.ToLower() == addUserDTO.LastName.ToLower());
        if (userExists) return BadRequest();

        var newUser = _mapper.Map<User>(addUserDTO);

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Created(nameof(GetUserById), _mapper.Map<UserDTO>(newUser));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return BadRequest();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok();
    }
}