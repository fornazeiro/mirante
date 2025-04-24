using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Mirante.Data;
using Mirante.Data.Repositories.Entities;
using Mirante.Models.Entities;
using Mirante.Models.Enums;

namespace Mirante.Tests
{
    public class TarefaRepositoryTests
    {
        private ToDoDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ToDoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ToDoDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTarefa_WhenTarefaExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new Repository<Tarefa>(context);
            var tarefa = new Tarefa { Id = 1, Titulo = "Tarefa Teste" };
            await context.Tarefas.AddAsync(tarefa);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Titulo.Should().Be("Tarefa Teste");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTarefas()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new Repository<Tarefa>(context);
            await context.Tarefas.AddRangeAsync(
                new Tarefa { Titulo = "Tarefa 1" },
                new Tarefa { Titulo = "Tarefa 2" }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task AddAsync_AddsNewTarefa()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new Repository<Tarefa>(context);
            var tarefa = new Tarefa { Titulo = "Nova Tarefa", Descricao = "Descricao da nova tarefa", Status = StatusTarefa.Pendente, DataVencimento = DateTime.Now };

            // Act
            await repository.AddAsync(tarefa);
            await context.SaveChangesAsync();

            // Assert
            var savedTarefa = await context.Tarefas.FirstOrDefaultAsync(t => t.Titulo == "Nova Tarefa");
            savedTarefa.Should().NotBeNull();
        }

        [Fact]
        public async Task Update_UpdatesExistingTarefa()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new Repository<Tarefa>(context);
            var tarefa = new Tarefa { Id = 1, Titulo = "Tarefa Original", Descricao = "Descricao Original" };
            context.Tarefas.Add(tarefa);
            await context.SaveChangesAsync();

            // Act
            tarefa.Descricao = "Descricao Atualizada";
            repository.Update(tarefa);
            await context.SaveChangesAsync();

            // Assert
            var updatedTarefa = await context.Tarefas.FindAsync(1);
            updatedTarefa.Should().NotBeNull();
            updatedTarefa.Descricao.Should().Be("Descricao Atualizada");
        }

        [Fact]
        public async Task Delete_DeletesExistingTarefa()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new Repository<Tarefa>(context);
            var tarefa = new Tarefa { Id = 1, Titulo = "Tarefa a Deletar" };
            context.Tarefas.Add(tarefa);
            await context.SaveChangesAsync();

            // Act
            repository.Delete(tarefa);
            await context.SaveChangesAsync();

            // Assert
            var deletedTarefa = await context.Tarefas.FindAsync(1);
            deletedTarefa.Should().BeNull();
        }

        [Fact]
        public async Task GetByStatusAsync_ReturnsTarefasWithGivenStatus()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new Repository<Tarefa>(context);
            await context.Tarefas.AddRangeAsync(
                new Tarefa { Titulo = "Tarefa Pendente 1", Status = StatusTarefa.Pendente },
                new Tarefa { Titulo = "Tarefa Em Andamento 1", Status = StatusTarefa.EmAndamento },
                new Tarefa { Titulo = "Tarefa Pendente 2", Status = StatusTarefa.Pendente }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.All(t => t.Status == StatusTarefa.Pendente);
        }

        [Fact]
        public async Task GetByVencimentoAsync_ReturnsTarefasWithGivenVencimento()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new Repository<Tarefa>(context);
            var dataVencimento = new DateTime(2025, 05, 01);
            await context.Tarefas.AddRangeAsync(
                new Tarefa { Titulo = "Tarefa Vencimento 1", DataVencimento = dataVencimento },
                new Tarefa { Titulo = "Tarefa Outro Vencimento", DataVencimento = new DateTime(2025, 05, 05) },
                new Tarefa { Titulo = "Tarefa Vencimento 2", DataVencimento = dataVencimento }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.All(t => t.DataVencimento.Date == dataVencimento.Date);
        }
    }
}
