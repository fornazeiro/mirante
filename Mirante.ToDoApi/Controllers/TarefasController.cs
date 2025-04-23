using Microsoft.AspNetCore.Mvc;
using Mirante.Models.DTOs;
using Mirante.Models.Entities;
using Mirante.Models.Enums;
using Mirante.Services.Interfaces;

namespace Mirante.ToDoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        public TarefasController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarefa>>> GetTarefas([FromQuery] StatusTarefa? status, [FromQuery] DateTime? dataVencimento)
        {
            try
            {
                var tarefas = await _tarefaService.GetAllTarefasAsync(status, dataVencimento);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
           //return Ok(tarefas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tarefa>> GetTarefa(int id)
        {
            var tarefa = await _tarefaService.GetTarefaByIdAsync(id);

            if (tarefa == null)
            {
                return NotFound();
            }

            return Ok(tarefa);
        }

        [HttpPost]
        public async Task<ActionResult<Tarefa>> PostTarefa(TarefaDTO tarefaDto)
        {
            var novaTarefa = await _tarefaService.CreateTarefaAsync(tarefaDto);
            return CreatedAtAction(nameof(GetTarefa), new { id = novaTarefa.Id }, novaTarefa);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarefa(int id, TarefaDTO tarefaDto)
        {
            await _tarefaService.UpdateTarefaAsync(id, tarefaDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefa(int id)
        {
            await _tarefaService.DeleteTarefaAsync(id);
            return NoContent();
        }

    }
}
