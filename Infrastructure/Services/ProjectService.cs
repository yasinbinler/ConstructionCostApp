using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;

        public ProjectService(IRepository<Project> projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
            => await _projectRepository.GetAllAsync();

        public async Task<Project?> GetByIdAsync(int id)
            => await _projectRepository.GetByIdAsync(id);

        public async Task<Project> CreateAsync(Project project)
        {
            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();
            return project;
        }

        public async Task UpdateAsync(Project project)
        {
            await _projectRepository.UpdateAsync(project);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _projectRepository.GetByIdAsync(id);
            if (entity != null)
            {
                await _projectRepository.DeleteAsync(entity);
                await _projectRepository.SaveChangesAsync();
            }
        }
    }
}
