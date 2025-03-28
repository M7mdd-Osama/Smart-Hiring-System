using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Repositories
{
	public interface IPostRepository : IGenericRepository<Post>
	{
		Task<Post> GetPostWithRelations(int id);
		void DeleteRelatedEntities(Post post);
	}

}