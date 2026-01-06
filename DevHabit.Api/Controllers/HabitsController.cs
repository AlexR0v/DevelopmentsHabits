using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Common;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("habits")]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetHabits()
    {
        List<HabitDto> habits = await dbContext.Habits.Select(h => h.ToDto()).ToListAsync();
        int totalCount = await dbContext.Habits.CountAsync();
        var result = new PaginationResult<HabitDto>
        {
            Items = habits,
            TotalCount = totalCount
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHabitByIdAsync(string id)
    {
        Habit? habit = await dbContext.Habits
            .Where(h => h.Id == id)
            .FirstOrDefaultAsync();
        if (habit is null)
        {
            return NotFound();
        }

        return Ok(habit.ToDto());
    }

    [HttpPost]
    public async Task<IActionResult> CreateHabitAsync(CreateHabitDto createHabitDto)
    {
        Habit habit = createHabitDto.ToEntity();
        dbContext.Habits.Add(habit);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetHabits), new { id = habit.Id }, habit.ToDto());
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        habit.UpdateFromDto(updateHabitDto);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchHabit(string id, JsonPatchDocument<HabitDto> patchDocument)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }
        HabitDto habitDto = habit.ToDto();
        patchDocument.ApplyTo(habitDto, ModelState);
        if (!TryValidateModel(habitDto))
        {
            return ValidationProblem(ModelState);
        }
        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description;
        habit.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        dbContext.Habits.Remove(habit);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
