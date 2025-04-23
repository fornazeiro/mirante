using Mirante.Data.Repositories.Interfaces;
using Mirante.Models.DTOs;
using Mirante.Models.Entities;
using Mirante.Models.Enums;
using Mirante.Services.Interfaces;

namespace Mirante.Services.Entities
{
    public class TarefaSevice : ITarefaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TarefaSevice(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Tarefa> CreateTarefaAsync(TarefaDTO tarefaDto)
        {
            var novaTarefa = new Tarefa
            {
                Titulo = tarefaDto.Titulo,
                Descricao = tarefaDto.Descricao,
                Status = tarefaDto.Status,
                DataVencimento = tarefaDto.DataVencimento
            };

            await _unitOfWork.TarefaRepository.AddAsync(novaTarefa);
            await _unitOfWork.CommitAsync();
            return novaTarefa;
        }

        public async Task UpdateTarefaAsync(int id, TarefaDTO tarefaDto)
        {
            var tarefaExistente = await _unitOfWork.TarefaRepository.GetByIdAsync(id);
            
            

            if (!string.IsNullOrEmpty(tarefaDto.Titulo))
            {
                tarefaExistente.Titulo = tarefaDto.Titulo;
            }

            if (tarefaDto.Descricao is not null)
            {
                tarefaExistente.Descricao = tarefaDto.Descricao;
            }

            if (string.IsNullOrEmpty(tarefaDto.Status.ToString()))
            {
                tarefaExistente.Status = tarefaDto.Status;
            }

            if (tarefaDto.DataVencimento != DateTime.MinValue)
            {
                tarefaExistente.DataVencimento = tarefaDto.DataVencimento;
            }

            _unitOfWork.TarefaRepository.Update(tarefaExistente);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteTarefaAsync(int id)
        {
            var tarefaExistente = await _unitOfWork.TarefaRepository.GetByIdAsync(id);

            if (tarefaExistente != null)
            {
                _unitOfWork.TarefaRepository.Delete(tarefaExistente);
                await _unitOfWork.CommitAsync();
            }
        }

        public async Task<IEnumerable<Tarefa>> GetAllTarefasAsync(StatusTarefa? status, DateTime? dataVencimento)
        {
            var tarefas = await _unitOfWork.TarefaRepository.GetAllAsync();

            if (status.HasValue)
            {
                tarefas = tarefas.Where(t => t.Status == status.Value).ToList();
            }

            if (dataVencimento.HasValue)
            {
                tarefas = tarefas.Where(t => t.DataVencimento.Date == dataVencimento.Value.Date).ToList();
            }

            return tarefas;
        }

        public async Task<Tarefa> GetTarefaByIdAsync(int id)
        {
            return await _unitOfWork.TarefaRepository.GetByIdAsync(id);
        }
    }
}
