using SmartHiring.Core.Entities;

namespace SmartHiring.Core.Specifications
{
    public class PostByIdSpec : BaseSpec<Post>
    {
        public PostByIdSpec(int postId)
            : base(p => p.Id == postId)
        {
        }
    }
}
