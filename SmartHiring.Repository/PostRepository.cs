using Microsoft.EntityFrameworkCore;
using SmartHiring.Core.Entities;
using SmartHiring.Core.Repositories;
using SmartHiring.Repository.Data;

namespace SmartHiring.Repository
{
    public class PostRepository : GenericRepository<Post>, IPostRepo
    {
        private readonly SmartHiringDbContext _dbContext;

        public PostRepository(SmartHiringDbContext dbcontext) : base(dbcontext)
        {
            _dbContext = dbcontext;
        }

        public async Task<Post> GetPostWithRelations(int id)
        {
            return await _dbContext.Posts
                .Include(p => p.Company)
                .Include(p => p.HR)
                .Include(p => p.PostJobCategories)
                    .ThenInclude(pjc => pjc.JobCategory)
                .Include(p => p.PostJobTypes)
                    .ThenInclude(pjt => pjt.JobType)
                .Include(p => p.PostWorkplaces)
                    .ThenInclude(pwp => pwp.Workplace)
                .Include(p => p.PostSkills)
                    .ThenInclude(ps => ps.Skill)
                .Include(p => p.PostCareerLevels)
                    .ThenInclude(pcl => pcl.CareerLevel)
                .Include(p => p.Applications)
                .Include(p => p.CandidateLists)
                .Include(p => p.Interviews)
                .Include(p => p.SavedPosts)
                .Include(p => p.Notes) 
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public void DeleteRelatedEntities(Post post)
        {
            _dbContext.Notes.RemoveRange(post.Notes);
            _dbContext.SavedPosts.RemoveRange(post.SavedPosts);
            _dbContext.Interviews.RemoveRange(post.Interviews);
            _dbContext.Applications.RemoveRange(post.Applications);
            _dbContext.CandidateLists.RemoveRange(post.CandidateLists);
            _dbContext.PostSkills.RemoveRange(post.PostSkills);
            _dbContext.PostCareerLevels.RemoveRange(post.PostCareerLevels);
            _dbContext.PostJobCategories.RemoveRange(post.PostJobCategories);
            _dbContext.PostJobTypes.RemoveRange(post.PostJobTypes);
            _dbContext.PostWorkplaces.RemoveRange(post.PostWorkplaces);
        }
    }
}