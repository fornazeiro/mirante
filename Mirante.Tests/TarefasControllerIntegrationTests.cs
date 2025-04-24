using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Mirante.Data;
using Mirante.Models.Entities;
using Mirante.Models.Enums;
using System.Net;
using System.Net.Http.Json;

namespace Mirante.Tests
{
    public class TarefasControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public TarefasControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remover a configuração do banco de dados em memória padrão
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                            typeof(DbContextOptions<ToDoDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Adicionar um novo banco de dados em memória para cada teste
                    services.AddDbContext<ToDoDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IntegrationTests");
                    });

                    // Criar um escopo para acessar o contexto do banco de dados
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ToDoDbContext>();

                    // Garantir que o banco de dados seja criado
                    db.Database.EnsureCreated();

                    // Adicionar dados de teste iniciais
                    db.Tarefas.AddRange(
                        new Tarefa { Id = 1, Titulo = "Tarefa Teste 1", Status = StatusTarefa.Pendente },
                        new Tarefa { Id = 2, Titulo = "Tarefa Teste 2", Status = StatusTarefa.Concluido, DataVencimento = DateTime.Now.AddDays(7) }
                    );
                    db.SaveChanges();
                });
            });
        }

        private HttpClient GetClient()
        {
            return _factory.CreateClient();
        }

        [Fact]
        public async Task GetTarefas_WithoutFilters_ReturnsOkAndAllTarefas()
        {
            // Arrange
            var client = GetClient();

            // Act
            var response = await client.GetAsync("/api/Tarefas");
            var tarefas = await response.Content.ReadFromJsonAsync<List<Tarefa>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tarefas.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetTarefas_FilterByStatus_ReturnsOkAndFilteredTarefas()
        {
            // Arrange
            var client = GetClient();

            // Act
            var response = await client.GetAsync("/api/Tarefas?status=Pendente");
            var tarefas = await response.Content.ReadFromJsonAsync<IEnumerable<Tarefa>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tarefas.Should().HaveCount(1);
            tarefas.All(t => t.Status == StatusTarefa.Pendente);
        }

        [Fact]
        public async Task GetTarefas_FilterByDataVencimento_ReturnsOkAndFilteredTarefas()
        {
            // Arrange
            var client = GetClient();
            var dataVencimento = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");

            // Act
            var response = await client.GetAsync($"/api/Tarefas?dataVencimento={dataVencimento}");
            var tarefas = await response.Content.ReadFromJsonAsync<IEnumerable<Tarefa>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tarefas.Should().HaveCount(1);
            tarefas.All(t => t.DataVencimento.Date == DateTime.Now.AddDays(7).Date);
        }

        [Fact]
        public async Task GetTarefa_ExistingId_ReturnsOkAndTarefa()
        {
            // Arrange
            var client = GetClient();

            // Act
            var response = await client.GetAsync("/api/Tarefas/1");
            var tarefa = await response.Content.ReadFromJsonAsync<Tarefa>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            tarefa.Should().NotBeNull();
            tarefa.Id.Should().Be(1);
            tarefa.Titulo.Should().Be("Tarefa Teste 1");
        }

        [Fact]
        public async Task GetTarefa_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var client = GetClient();

            // Act
            var response = await client.GetAsync("/api/Tarefas/99");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task PostTarefa_ValidTarefa_ReturnsCreatedAndTarefa()
        {
            // Arrange
            var client = GetClient();
            var novaTarefa = new Tarefa { Titulo = "Nova Tarefa Teste", Status = StatusTarefa.Pendente };

            // Act
            var response = await client.PostAsJsonAsync("/api/Tarefas", novaTarefa);
            var tarefaCriada = await response.Content.ReadFromJsonAsync<Tarefa>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            tarefaCriada.Should().NotBeNull();
            tarefaCriada.Titulo.Should().Be("Nova Tarefa Teste");
            response.Headers.Location?.Should().Be($"/api/Tarefas/{tarefaCriada.Id}");
        }

        [Fact]
        public async Task PutTarefa_ExistingIdAndValidTarefa_ReturnsNoContent()
        {
            // Arrange
            var client = GetClient();
            var tarefaAtualizada = new Tarefa { Id = 1, Titulo = "Tarefa Atualizada Teste", Status = StatusTarefa.EmAndamento };

            // Act
            var response = await client.PutAsJsonAsync("/api/Tarefas/1", tarefaAtualizada);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar se a tarefa foi realmente atualizada no banco de dados
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            var tarefaNoBanco = await dbContext.Tarefas.FindAsync(1);
            tarefaNoBanco.Should().NotBeNull();
            tarefaNoBanco.Titulo.Should().Be("Tarefa Atualizada Teste");
            tarefaNoBanco.Status.Should().Be(StatusTarefa.EmAndamento);
        }

        [Fact]
        public async Task PutTarefa_NonExistingId_ReturnsBadRequest()
        {
            // Arrange
            var client = GetClient();
            var tarefaAtualizada = new Tarefa { Id = 99, Titulo = "Tarefa Atualizada Teste", Status = StatusTarefa.EmAndamento };

            // Act
            var response = await client.PutAsJsonAsync("/api/Tarefas/99", tarefaAtualizada);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteTarefa_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var client = GetClient();

            // Act
            var response = await client.DeleteAsync("/api/Tarefas/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar se a tarefa foi realmente removida do banco de dados
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();
            var tarefaNoBanco = await dbContext.Tarefas.FindAsync(1);
            tarefaNoBanco.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTarefa_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var client = GetClient();

            // Act
            var response = await client.DeleteAsync("/api/Tarefas/99");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
