using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Repositories
{
	public interface IPostRepo : IGenericRepo<Post>
	{
		Task<Post> GetPostWithRelations(int id);
		void DeleteRelatedEntities(Post post);
	}

}